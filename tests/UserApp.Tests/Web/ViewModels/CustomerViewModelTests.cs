using System.ComponentModel.DataAnnotations;
using Xunit;
using UserApp.Application.Media;
using UserApp.Web.ViewModels;

namespace UserApp.Tests.Web.ViewModels;

public class CustomerViewModelTests
{
    [Fact]
    public void DefaultValues_AreEmpty()
    {
        var vm = new CustomerViewModel();

        Assert.Equal(Guid.Empty, vm.Id);
        Assert.Equal(string.Empty, vm.Name);
        Assert.Equal(0, vm.Phone);
        Assert.Equal(string.Empty, vm.Address);
        Assert.Empty(vm.ImageUrls);
        Assert.Empty(vm.MediaList);
    }

    [Fact]
    public void Name_StringLength_Valid()
    {
        var vm = new CustomerViewModel { Name = new string('x', 225) };
        var context = new ValidationContext(vm) { MemberName = nameof(CustomerViewModel.Name) };
        var result = new List<ValidationResult>();
        var isValid = Validator.TryValidateProperty(vm.Name, context, result);

        Assert.True(isValid);
    }

    [Fact]
    public void Name_StringLength_TooLong()
    {
        var vm = new CustomerViewModel { Name = new string('x', 226) };
        var context = new ValidationContext(vm) { MemberName = nameof(CustomerViewModel.Name) };
        var result = new List<ValidationResult>();
        var isValid = Validator.TryValidateProperty(vm.Name, context, result);

        Assert.False(isValid);
    }

    [Fact]
    public void Address_StringLength_Valid()
    {
        var vm = new CustomerViewModel { Address = new string('x', 300) };
        var context = new ValidationContext(vm) { MemberName = nameof(CustomerViewModel.Address) };
        var result = new List<ValidationResult>();
        var isValid = Validator.TryValidateProperty(vm.Address, context, result);

        Assert.True(isValid);
    }

    [Fact]
    public void Address_StringLength_TooLong()
    {
        var vm = new CustomerViewModel { Address = new string('x', 301) };
        var context = new ValidationContext(vm) { MemberName = nameof(CustomerViewModel.Address) };
        var result = new List<ValidationResult>();
        var isValid = Validator.TryValidateProperty(vm.Address, context, result);

        Assert.False(isValid);
    }

    [Fact]
    public void ImageUrls_And_MediaList_AreEmpty_ByDefault()
    {
        var vm = new CustomerViewModel();
        Assert.Empty(vm.ImageUrls);
        Assert.Empty(vm.MediaList);
    }

    [Fact]
    public void Properties_CanBeSetAndGet()
    {
        var id = Guid.NewGuid();
        var vm = new CustomerViewModel
        {
            Id = id,
            Name = "Test",
            Phone = 1,
            Address = "Test",
            ImageUrls = new List<string> { "img.jpg" }
        };

        Assert.Equal(id, vm.Id);
        Assert.Equal("Test", vm.Name);
        Assert.Equal(1, vm.Phone);
        Assert.Equal("Test", vm.Address);
        Assert.Single(vm.ImageUrls);
    }
}
