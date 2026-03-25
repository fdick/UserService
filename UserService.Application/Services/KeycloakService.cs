using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using UserService.Core.Abstractions;
using UserService.Core.Models;

namespace UserService.Application.Services
{
    public partial class KeycloakService : IKeycloakService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public KeycloakService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            var accessToken = await GetAccessTokenAsync();

            var request = new HttpRequestMessage(HttpMethod.Get,
                $"{_configuration["Keycloak:BaseUrl"]}/admin/realms/main/users/{id.ToString()}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var user = await response.Content.ReadFromJsonAsync<User>();
            return user;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var accessToken = await GetAccessTokenAsync();

            var request = new HttpRequestMessage(HttpMethod.Get,
                $"{_configuration["Keycloak:BaseUrl"]}/admin/realms/main/users");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var users = await response.Content.ReadFromJsonAsync<List<User>>();
            return users;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var tokenEndpoint = $"{_configuration["Keycloak:BaseUrl"]}/realms/main/protocol/openid-connect/token";

            var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);

            var body = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "client_credentials"),
                new("client_id", _configuration["Keycloak:ClientId"]), 
                new("client_secret", _configuration["Keycloak:ClientSecret"])
            };

            request.Content = new FormUrlEncodedContent(body);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync <TokenResponse>();
            return content.access_token;
        }

        private record TokenResponse(string access_token);

    }
}
