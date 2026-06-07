namespace UserApp.Web.ViewModels.ModuleGenerator;

public class GenerateModuleViewModel
{
    public string ModuleName { get; set; } = string.Empty;

    public List<ModuleFieldViewModel> Fields { get; set; } = new();
}