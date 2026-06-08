@model UserApp.Web.ViewModels.{{Name}}ViewModel

<h2>Create {{Name}}</h2>

<form asp-action="Create" method="post">

{{Inputs}}

    <button type="submit" class="btn btn-success">Save</button>
</form>
