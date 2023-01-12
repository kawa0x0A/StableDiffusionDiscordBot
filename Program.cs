using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using StableDiffusionDiscordBot;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddRouting(options => { options.LowercaseUrls = true; });

var azureServiceTokenProvider = new AzureServiceTokenProvider();

var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

builder.Configuration.AddAzureKeyVault("https://keycontainer-kawai321.vault.azure.net/", keyVaultClient, new DefaultKeyVaultSecretManager());

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
