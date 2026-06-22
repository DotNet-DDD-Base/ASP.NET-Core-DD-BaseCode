using System.Text;
using UserApp.Application.Common.DTOs;
using UserApp.Infrastructure.Services.CodeGeneration.Shared;

namespace UserApp.Infrastructure.Services.CodeGeneration;

public class TestGenerator
{
    private readonly PathProvider _paths;
    private readonly FileManager _files;
    private readonly TemplateEngine _templates;

    public TestGenerator(PathProvider paths, FileManager files, TemplateEngine templates)
    {
        _paths = paths;
        _files = files;
        _templates = templates;
    }

    public void Generate(string name, List<ModuleFieldDto> fields, bool hasImage)
    {
        var testRoot = Path.Combine(_paths.SolutionRoot, "tests", "UserApp.Tests");

        var domainFolder = Path.Combine(testRoot, "Domain", $"{name}s");
        var applicationFolder = Path.Combine(testRoot, "Application", $"{name}s");
        var viewModelFolder = Path.Combine(testRoot, "Web", "ViewModels");

        _files.EnsureDirectory(domainFolder);
        _files.EnsureDirectory(applicationFolder);
        _files.EnsureDirectory(viewModelFolder);

        // Entity test
        var entityTestContent = _templates.RenderFile(
            new[] { "Test", "Templates", "EntityTest.tpl" },
            new Dictionary<string, string>
            {
                ["Name"] = name,
                ["HasImageTest"] = hasImage
                    ? $@"
    [Fact]
    public void Implements_IHasMedia()
    {{
        var entity = new {name}();
        Assert.IsAssignableFrom<UserApp.Domain.Common.IHasMedia>(entity);
    }}
"
                    : ""
            });

        _files.WriteFile(Path.Combine(domainFolder, $"{name}Tests.cs"), entityTestContent);

        // Service test
        var serviceTestContent = _templates.RenderFile(
            new[] { "Test", "Templates", "ServiceTest.tpl" },
            new Dictionary<string, string>
            {
                ["Name"] = name
            });

        var serviceValidationTests = BuildServiceValidationTests(name, fields);
        if (!string.IsNullOrEmpty(serviceValidationTests))
        {
            var lastBrace = serviceTestContent.LastIndexOf("}");
            serviceTestContent = serviceTestContent.Insert(lastBrace, serviceValidationTests);
        }

        _files.WriteFile(Path.Combine(applicationFolder, $"{name}ServiceTests.cs"), serviceTestContent);

        // ViewModel test
        var viewModelTestContent = _templates.RenderFile(
            new[] { "Test", "Templates", "ViewModelTest.tpl" },
            new Dictionary<string, string>
            {
                ["Name"] = name,
                ["MediaUsing"] = hasImage ? "\nusing UserApp.Application.Media;" : "",
                ["DefaultStringAssertions"] = BuildDefaultStringAssertions(fields),
                ["ValidationTests"] = BuildValidationTests(name, fields, hasImage),
                ["PropertyInitializers"] = BuildPropertyInitializers(fields, hasImage),
                ["PropertyAssertions"] = BuildPropertyAssertions(fields, hasImage)
            });

        _files.WriteFile(Path.Combine(viewModelFolder, $"{name}ViewModelTests.cs"), viewModelTestContent);
    }

    private static string BuildDefaultStringAssertions(List<ModuleFieldDto> fields)
    {
        var sb = new StringBuilder();

        foreach (var field in fields.Where(f => f.Type == "string" && !f.IsRelation))
        {
            sb.AppendLine($"        Assert.Equal(string.Empty, vm.{field.Name});");
        }

        foreach (var field in fields.Where(f => f.IsRelation && !f.IsPivot))
        {
            sb.AppendLine($"        Assert.Equal(Guid.Empty, vm.{field.Name}Id);");
            sb.AppendLine($"        Assert.Empty(vm.{field.Name}Options);");
            sb.AppendLine($"        Assert.Equal(string.Empty, vm.{field.Name}Name);");
        }

        foreach (var field in fields.Where(f => f.IsPivot))
        {
            sb.AppendLine($"        Assert.Empty(vm.{field.Name}Options);");
            sb.AppendLine($"        Assert.Empty(vm.Selected{field.Name}Ids);");
            sb.AppendLine($"        Assert.Equal(string.Empty, vm.{field.Name}Display);");
        }

        foreach (var field in fields.Where(f => f.UseCommonTable))
        {
            sb.AppendLine($"        Assert.Empty(vm.{field.Name}Options);");
            sb.AppendLine($"        Assert.Equal(string.Empty, vm.{field.Name}Name);");
        }

        if (fields.Any(f => f.Type == "int" || f.Type == "decimal" || f.Type == "double" || f.Type == "float" || f.Type == "long"))
        {
            foreach (var field in fields.Where(f => f.Type is "int" or "decimal" or "double" or "float" or "long"))
            {
                sb.AppendLine($"        Assert.Equal(0, vm.{field.Name});");
            }
        }

        if (fields.Any(f => f.Type == "bool"))
        {
            foreach (var field in fields.Where(f => f.Type == "bool"))
            {
                sb.AppendLine($"        Assert.False(vm.{field.Name});");
            }
        }

        if (fields.Any(f => f.Type == "DateTime" || f.Type == "DateTime?"))
        {
            foreach (var field in fields.Where(f => f.Type == "DateTime" || f.Type == "DateTime?"))
            {
                sb.AppendLine($"        Assert.Equal(default, vm.{field.Name});");
            }
        }

        return sb.ToString();
    }

    private static string BuildValidationTests(string name, List<ModuleFieldDto> fields, bool hasImage)
    {
        var sb = new StringBuilder();

        foreach (var field in fields)
        {
            if (field.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                continue;

            if (field.IsRequired && field.Type == "string")
            {
                sb.AppendLine($@"
    [Fact]
    public void {field.Name}_Required_Validation()
    {{
        var vm = new {name}ViewModel();
        var context = new ValidationContext(vm) {{ MemberName = nameof({name}ViewModel.{field.Name}) }};
        var result = new List<ValidationResult>();
        var isValid = Validator.TryValidateProperty(vm.{field.Name}, context, result);

        Assert.False(isValid);
        Assert.Contains(result, r => r.ErrorMessage != null && r.ErrorMessage.Contains(""{field.Name}""));
    }}
");
            }

            if (IsStringType(field.Type) && HasStringLengthValidation(field))
            {
                var maxLength = field.MaxLength ?? ToLength(field.MaxValue) ?? field.Length ?? 500;

                sb.AppendLine($@"
    [Fact]
    public void {field.Name}_StringLength_Valid()
    {{
        var vm = new {name}ViewModel {{ {field.Name} = new string('x', {maxLength}) }};
        var context = new ValidationContext(vm) {{ MemberName = nameof({name}ViewModel.{field.Name}) }};
        var result = new List<ValidationResult>();
        var isValid = Validator.TryValidateProperty(vm.{field.Name}, context, result);

        Assert.True(isValid);
    }}

    [Fact]
    public void {field.Name}_StringLength_TooLong()
    {{
        var vm = new {name}ViewModel {{ {field.Name} = new string('x', {maxLength + 1}) }};
        var context = new ValidationContext(vm) {{ MemberName = nameof({name}ViewModel.{field.Name}) }};
        var result = new List<ValidationResult>();
        var isValid = Validator.TryValidateProperty(vm.{field.Name}, context, result);

        Assert.False(isValid);
    }}
");
            }

            if (IsNumericType(field.Type) && (field.MinValue.HasValue || field.MaxValue.HasValue))
            {
                var minValue = FormatDecimal(field.MinValue ?? 0);
                var maxValue = FormatDecimal(field.MaxValue ?? 999999999);

                sb.AppendLine($@"
    [Fact]
    public void {field.Name}_Range_Valid()
    {{
        var vm = new {name}ViewModel {{ {field.Name} = {minValue} }};
        var context = new ValidationContext(vm) {{ MemberName = nameof({name}ViewModel.{field.Name}) }};
        var result = new List<ValidationResult>();
        var isValid = Validator.TryValidateProperty(vm.{field.Name}, context, result);

        Assert.True(isValid);
    }}
");
            }

            if (field.IsRelation && !field.IsPivot && field.DeleteBehavior != "SetNull")
            {
                sb.AppendLine($@"
    [Fact]
    public void {field.Name}Id_Required_Validation()
    {{
        var vm = new {name}ViewModel();
        var context = new ValidationContext(vm) {{ MemberName = nameof({name}ViewModel.{field.Name}Id) }};
        var result = new List<ValidationResult>();
        var isValid = Validator.TryValidateProperty(vm.{field.Name}Id, context, result);

        Assert.False(isValid);
    }}
");
            }
        }

        if (hasImage)
        {
            sb.AppendLine($@"
    [Fact]
    public void ImageUrls_And_MediaList_AreEmpty_ByDefault()
    {{
        var vm = new {name}ViewModel();
        Assert.Empty(vm.ImageUrls);
        Assert.Empty(vm.MediaList);
    }}
");
        }

        return sb.ToString();
    }

    private static string BuildPropertyInitializers(List<ModuleFieldDto> fields, bool hasImage)
    {
        var sb = new StringBuilder();

        foreach (var field in fields)
        {
            if (field.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                continue;

            if (field.IsRelation && !field.IsPivot)
            {
                sb.AppendLine($@",
            {field.Name}Id = Guid.NewGuid()");
            }
            else if (field.Type == "string" || field.Type == "enum")
            {
                sb.AppendLine($@",
            {field.Name} = ""Test""");
            }
            else if (field.Type is "int" or "long")
            {
                sb.AppendLine($@",
            {field.Name} = 1");
            }
            else if (field.Type is "decimal" or "double" or "float")
            {
                sb.AppendLine($@",
            {field.Name} = 1.0{GetLiteralSuffix(field.Type)}");
            }
            else if (field.Type == "bool")
            {
                sb.AppendLine($@",
            {field.Name} = true");
            }
            else if (field.Type == "DateTime" || field.Type == "DateTime?")
            {
                sb.AppendLine($@",
            {field.Name} = new DateTime(2024, 1, 1)");
            }
        }

        if (hasImage)
        {
            sb.AppendLine($@",
            ImageUrls = new List<string> {{ ""img.jpg"" }}");
        }

        return sb.ToString();
    }

    private static string BuildPropertyAssertions(List<ModuleFieldDto> fields, bool hasImage)
    {
        var sb = new StringBuilder();

        foreach (var field in fields)
        {
            if (field.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                continue;

            if (field.IsRelation && !field.IsPivot)
            {
                sb.AppendLine($"        Assert.NotEqual(Guid.Empty, vm.{field.Name}Id);");
            }
            else if (field.Type == "string" || field.Type == "enum")
            {
                sb.AppendLine($"        Assert.Equal(\"Test\", vm.{field.Name});");
            }
            else if (field.Type is "int" or "long")
            {
                sb.AppendLine($"        Assert.Equal(1, vm.{field.Name});");
            }
            else if (field.Type is "decimal" or "double" or "float")
            {
                sb.AppendLine($"        Assert.Equal(1.0{GetLiteralSuffix(field.Type)}, vm.{field.Name});");
            }
            else if (field.Type == "bool")
            {
                sb.AppendLine($"        Assert.True(vm.{field.Name});");
            }
            else if (field.Type == "DateTime" || field.Type == "DateTime?")
            {
                sb.AppendLine($"        Assert.Equal(new DateTime(2024, 1, 1), vm.{field.Name});");
            }
        }

        if (hasImage)
        {
            sb.AppendLine($"        Assert.Single(vm.ImageUrls);");
        }

        return sb.ToString();
    }

    private static string BuildValidEntityInitializers(List<ModuleFieldDto> fields)
    {
        var props = new List<string>();
        foreach (var field in fields)
        {
            if (field.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                continue;

            if (IsStringType(field.Type))
            {
                var val = "TestValue";
                if (field.MinLength.HasValue && field.MinLength.Value > val.Length)
                    val = new string('x', field.MinLength.Value);
                if (field.MaxLength.HasValue && val.Length > field.MaxLength.Value)
                    val = val[..field.MaxLength.Value];
                props.Add($"{field.Name} = \"{val}\"");
            }
            else if (field.IsRelation && !field.IsPivot)
            {
                props.Add($"{field.Name}Id = Guid.NewGuid()");
            }
            else if (IsNumericType(field.Type))
            {
                var val = field.MinValue ?? 1;
                props.Add($"{field.Name} = {FormatDecimal(val)}{GetLiteralSuffix(field.Type)}");
            }
            else if (field.Type == "bool")
            {
                props.Add($"{field.Name} = true");
            }
            else if (field.Type == "DateTime" || field.Type == "DateTime?")
            {
                props.Add($"{field.Name} = new DateTime(2024, 1, 1)");
            }
        }

        if (props.Count == 0)
            return "";

        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine("        {");
        for (var i = 0; i < props.Count; i++)
        {
            var comma = i < props.Count - 1 ? "," : "";
            sb.AppendLine($"            {props[i]}{comma}");
        }
        sb.Append("        }");
        return sb.ToString();
    }

    private static string BuildServiceValidationTests(string name, List<ModuleFieldDto> fields)
    {
        var sb = new StringBuilder();

        foreach (var field in fields)
        {
            if (field.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                continue;

            if (field.IsRequired && IsStringType(field.Type))
            {
                sb.AppendLine($@"
    [Fact]
    public async Task AddAsync_Throws_When{field.Name}_IsEmpty()
    {{
        var entity = new {name}();
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AddAsync(entity));
    }}

    [Fact]
    public async Task UpdateAsync_Throws_When{field.Name}_IsEmpty()
    {{
        var entity = new {name}();
        await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateAsync(entity));
    }}
");
            }

            if (field.MinLength.HasValue && IsStringType(field.Type))
            {
                sb.AppendLine($@"
    [Fact]
    public async Task AddAsync_Throws_When{field.Name}_TooShort()
    {{
        var entity = new {name} {{ {field.Name} = new string('x', {field.MinLength.Value - 1}) }};
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AddAsync(entity));
    }}

    [Fact]
    public async Task UpdateAsync_Throws_When{field.Name}_TooShort()
    {{
        var entity = new {name} {{ {field.Name} = new string('x', {field.MinLength.Value - 1}) }};
        await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateAsync(entity));
    }}
");
            }

            if (field.MaxLength.HasValue && IsStringType(field.Type))
            {
                sb.AppendLine($@"
    [Fact]
    public async Task AddAsync_Throws_When{field.Name}_TooLong()
    {{
        var entity = new {name} {{ {field.Name} = new string('x', {field.MaxLength.Value + 1}) }};
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AddAsync(entity));
    }}

    [Fact]
    public async Task UpdateAsync_Throws_When{field.Name}_TooLong()
    {{
        var entity = new {name} {{ {field.Name} = new string('x', {field.MaxLength.Value + 1}) }};
        await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateAsync(entity));
    }}
");
            }

            if (field.MinValue.HasValue && IsNumericType(field.Type))
            {
                var minVal = FormatDecimal(field.MinValue.Value);
                var belowVal = FormatDecimal(field.MinValue.Value - 1);
                var suffix = GetLiteralSuffix(field.Type);
                sb.AppendLine($@"
    [Fact]
    public async Task AddAsync_Throws_When{field.Name}_BelowMin()
    {{
        var entity = new {name} {{ {field.Name} = {belowVal}{suffix} }};
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AddAsync(entity));
    }}

    [Fact]
    public async Task UpdateAsync_Throws_When{field.Name}_BelowMin()
    {{
        var entity = new {name} {{ {field.Name} = {belowVal}{suffix} }};
        await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateAsync(entity));
    }}
");
            }

            if (field.MaxValue.HasValue && IsNumericType(field.Type))
            {
                var maxVal = FormatDecimal(field.MaxValue.Value);
                var aboveVal = FormatDecimal(field.MaxValue.Value + 1);
                var suffix = GetLiteralSuffix(field.Type);
                sb.AppendLine($@"
    [Fact]
    public async Task AddAsync_Throws_When{field.Name}_AboveMax()
    {{
        var entity = new {name} {{ {field.Name} = {aboveVal}{suffix} }};
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AddAsync(entity));
    }}

    [Fact]
    public async Task UpdateAsync_Throws_When{field.Name}_AboveMax()
    {{
        var entity = new {name} {{ {field.Name} = {aboveVal}{suffix} }};
        await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateAsync(entity));
    }}
");
            }

            if (field.IsRelation && !field.IsPivot && field.DeleteBehavior != "SetNull")
            {
                sb.AppendLine($@"
    [Fact]
    public async Task AddAsync_Throws_When{field.Name}Id_Empty()
    {{
        var entity = new {name}();
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AddAsync(entity));
    }}

    [Fact]
    public async Task UpdateAsync_Throws_When{field.Name}Id_Empty()
    {{
        var entity = new {name}();
        await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateAsync(entity));
    }}
");
            }
        }

        return sb.ToString();
    }

    private static bool IsStringType(string type)
        => type.Equals("string", StringComparison.OrdinalIgnoreCase);

    private static bool IsNumericType(string type)
        => type.Equals("int", StringComparison.OrdinalIgnoreCase)
            || type.Equals("decimal", StringComparison.OrdinalIgnoreCase)
            || type.Equals("double", StringComparison.OrdinalIgnoreCase)
            || type.Equals("float", StringComparison.OrdinalIgnoreCase)
            || type.Equals("long", StringComparison.OrdinalIgnoreCase);

    private static bool HasStringLengthValidation(ModuleFieldDto field)
        => field.MinLength.HasValue
            || field.MaxLength.HasValue
            || field.Length.HasValue
            || field.MinValue.HasValue
            || field.MaxValue.HasValue;

    private static int? ToLength(decimal? value)
        => value.HasValue ? (int)value.Value : null;

    private static string FormatDecimal(decimal value)
        => value.ToString(System.Globalization.CultureInfo.InvariantCulture);

    private static string GetLiteralSuffix(string type)
        => type switch
        {
            "decimal" => "m",
            "double" => "d",
            "float" => "f",
            _ => ""
        };
}
