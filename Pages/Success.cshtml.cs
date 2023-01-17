using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.OutputCaching;

namespace StableDiffusionDiscordBot.Pages
{
    public class SuccessModel : PageModel
    {
        [OutputCache(NoStore = true, Duration = 0)]
        public IActionResult OnGet()
        {
            return LocalRedirect("/");
        }
    }
}
