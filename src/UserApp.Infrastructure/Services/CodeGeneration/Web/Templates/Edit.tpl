@model UserApp.Web.ViewModels.{{Name}}ViewModel

<h2>Edit {{Name}}</h2>

<form asp-action="Edit" method="post" enctype="multipart/form-data">
    @Html.AntiForgeryToken()
    <input type="hidden" asp-for="Id" />

{{Inputs}}

    <button type="submit" class="btn btn-primary">Update</button>
</form>
