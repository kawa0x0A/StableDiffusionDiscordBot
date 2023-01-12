using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StableDiffusionDiscordBot.Pages
{
    public class LoginModel : PageModel
    {
        public async void OnGet()
        {
            await HttpContext.ChallengeAsync(DiscordAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/" });
        }
    }
}
