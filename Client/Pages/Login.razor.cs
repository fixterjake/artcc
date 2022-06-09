namespace ZDC.Client.Pages;

public partial class Login
{
    protected override void OnInitialized()
    {
        Navigation.NavigateTo("api/Auth/login/redirect", true);
    }
}
