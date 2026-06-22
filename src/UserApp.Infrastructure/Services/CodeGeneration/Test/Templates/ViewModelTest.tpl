using System.ComponentModel.DataAnnotations;
using Xunit;{{MediaUsing}}
using UserApp.Web.ViewModels;

namespace UserApp.Tests.Web.ViewModels;

public class {{Name}}ViewModelTests
{
    [Fact]
    public void DefaultValues_AreEmpty()
    {
        var vm = new {{Name}}ViewModel();

        Assert.Equal(Guid.Empty, vm.Id);
{{DefaultStringAssertions}}
    }

{{ValidationTests}}

    [Fact]
    public void Properties_CanBeSetAndGet()
    {
        var id = Guid.NewGuid();
        var vm = new {{Name}}ViewModel
        {
            Id = id{{PropertyInitializers}}
        };

        Assert.Equal(id, vm.Id);
{{PropertyAssertions}}
    }
}
