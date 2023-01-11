using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration.UserSecrets;
using StableDiffusionDiscordBot;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<DiscordAuthticateStateProvider>();

builder.Services.AddScoped<AuthenticationStateProvider>(options => options.GetRequiredService<DiscordAuthticateStateProvider>());

builder.Services.AddScoped<IAuthService, DiscordAuthService>();

builder.Services.AddApiAuthorization();

builder.Services.AddOptions();

builder.Configuration.AddUserSecrets<Program>(true).Build();

Stripe.StripeConfiguration.ApiKey = builder.Configuration["StripeApiKey"];

await builder.Build().RunAsync();
