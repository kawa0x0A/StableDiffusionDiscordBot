using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AspNet.Security.OAuth.Discord;

namespace StableDiffusionDiscordBot.Pages
{
    public class LogoutModel : PageModel
    {
        public async void OnGet()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/" });
        }
    }
}
