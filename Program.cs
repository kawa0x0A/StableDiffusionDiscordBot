using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using StableDiffusionDiscordBot;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddRouting(options => { options.LowercaseUrls = true; });

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme;
}).AddCookie().AddDiscord(options =>
{
    options.ClientId = "1060468061611753492";
    options.ClientSecret = "RvzRzOlmHl-5ef58fslyGkBDKyxbzsgN";

    options.SaveTokens = true;
});

builder.Services.AddHttpContextAccessor();

Stripe.StripeConfiguration.ApiKey = "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";

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCookiePolicy(new CookiePolicyOptions() { MinimumSameSitePolicy = SameSiteMode.None });
app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
