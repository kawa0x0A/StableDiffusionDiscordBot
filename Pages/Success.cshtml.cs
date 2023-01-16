using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StableDiffusionDiscordBot.Pages
{
    public class SuccessModel : PageModel
    {
        public void OnGet()
        {
            Redirect("/");
        }
    }
}
