using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace StableDiffusionDiscordBot
{
    public class DiscordAuthticateStateProvider : AuthenticationStateProvider
    {
        private class DiscordToken
        {
            [JsonPropertyName("access_token")]
            public string? AccessToken { get; set; }

            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonPropertyName("refresh_token")]
            public string? RefreshToken { get; set; }

            [JsonPropertyName("scope")]
            public string? Scope { get; set; }

            [JsonPropertyName("token_type")]
            public string? TokenType { get; set; }
        }

        private class DiscordUser
        {
            [JsonPropertyName("id")]
            public string? Id { get; set; }

            [JsonPropertyName("username")]
            public string? Username { get; set; }

            [JsonPropertyName("avatar")]
            public string? Avatar { get; set; }

            [JsonPropertyName("discriminator")]
            public int Discriminator { get; set; }

            [JsonPropertyName("public_flags")]
            public int PublicFlags { get; set; }

            [JsonPropertyName("flags")]
            public int Flags { get; set; }

            [JsonPropertyName("email")]
            public string? Email { get; set; }

            [JsonPropertyName("verified")]
            public bool Verified { get; set; }

            [JsonPropertyName("locale")]
            public string? Locale { get; set; }

            [JsonPropertyName("mfa_enabled")]
            public bool MfaEnabled { get; set; }
        }

        private readonly static AuthenticationState NoLoginAuthenticationState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        private readonly HttpClient httpClient;
        private readonly NavigationManager navigationManager;
        private readonly IConfiguration configuration;
        private string? code;
        private DiscordUser? user;

        public DiscordAuthticateStateProvider(HttpClient httpClient, NavigationManager navigationManager, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.navigationManager = navigationManager;
            this.configuration = configuration;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (string.IsNullOrEmpty(code))
            {
                return NoLoginAuthenticationState;
            }

            var tokenResponseMessage = await httpClient.PostAsync("https://discordapp.com/api/oauth2/token", new FormUrlEncodedContent(new Dictionary<string, string>() { { "client_id", configuration["DiscordClientId"]! }, { "client_secret", configuration["DiscordClientSecret"]! }, { "grant_type", "authorization_code" }, { "code", code! }, { "redirect_uri", navigationManager.BaseUri + "login" } }));

            var token = await tokenResponseMessage.Content.ReadFromJsonAsync<DiscordToken>();

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://discordapp.com/api/users/@me");

            httpRequestMessage.Headers.Add("Authorization", $"Bearer {token!.AccessToken}");

            var userResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            var user = await userResponseMessage.Content.ReadFromJsonAsync<DiscordUser>();

            if (string.IsNullOrWhiteSpace(user!.Id))
            {
                return NoLoginAuthenticationState;
            }

            this.user = user;

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user!.Id!) }, "apiauth")));
        }

        public void MarkUserAsAuthenticated(string code)
        {
            this.code = code;

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void MarkUserAsLoggedOut()
        {
            if (httpClient.DefaultRequestHeaders.Authorization != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = null;
            }

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public string GetLoginUserId()
        {
            return user?.Id!;
        }

        public string GetLoginUserEmail()
        {
            return user?.Email!;
        }
    }

    public interface IAuthService
    {
        bool Login(string code);

        void Logout();
    }

    public class DiscordAuthService : IAuthService
    {
        private readonly AuthenticationStateProvider authenticationStateProvider;

        public DiscordAuthService(AuthenticationStateProvider authenticationStateProvider)
        {
            this.authenticationStateProvider = authenticationStateProvider;
        }

        public bool Login(string code)
        {
            ((DiscordAuthticateStateProvider)authenticationStateProvider).MarkUserAsAuthenticated(code);

            return true;
        }

        public void Logout()
        {
            ((DiscordAuthticateStateProvider)authenticationStateProvider).MarkUserAsLoggedOut();
        }
    }
}
