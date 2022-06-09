using Microsoft.AspNetCore.WebUtilities;
using System.Threading.Tasks;

namespace ZDC.Client.Pages;

public partial class LoginCallback
{
    protected override async Task OnInitializedAsync()
    {
        var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("code", out var authorizationCode))
        {
            SpinnerService.Show();
            await AuthService.Login(authorizationCode);
            SpinnerService.Hide();
            Navigation.NavigateTo("/", true);
        }
    }
}
