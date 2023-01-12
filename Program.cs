using AspNet.Security.OAuth.Discord;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using StableDiffusionDiscordBot;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddRouting(options => { options.LowercaseUrls = true; });

builder.Configuration.AddAzureKeyVault(new Uri("https://keycontainer-kawai321.vault.azure.net/"), new DefaultAzureCredential());

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme;
}).AddCookie().AddDiscord(options =>
{
    options.ClientId = builder.Configuration["DiscordClientId"]!;
    options.ClientSecret = builder.Configuration["DiscordSecret"]!;

    options.SaveTokens = true;
});

builder.Services.AddHttpContextAccessor();

Stripe.StripeConfiguration.ApiKey = builder.Configuration["StripeApiKey"]!;

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
