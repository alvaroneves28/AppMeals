using AppMeals.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppMeals.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://q82b0738-44381.uks1.devtunnels.ms/";
        private readonly ILogger<ApiService> _logger;
        private readonly JsonSerializerOptions _serializerOptions;

        public ApiService(HttpClient httpClient,
                          ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ApiResponse<bool>> RegisterUser(string name, string email, string contact, string password)
        {
            try
            {
                var register = new Register()
                {
                    Name = name,
                    Email = email,
                    Contact = contact,
                    Password = password
                };

                var json = JsonSerializer.Serialize(register, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Users/Register", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"HTTP request error: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"HTTP request error: {response.StatusCode}"
                    };
                }

                return new ApiResponse<bool> { Success = true, Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception while registering user: {ex.Message}");
                return new ApiResponse<bool>
                {
                    ErrorMessage = "An error occurred while registering the user."
                };
            }
        }

        public async Task<ApiResponse<bool>> Login(string email, string password)
        {
            try
            {
                var login = new Login
                {
                    Email = email,
                    Password = password
                };

                var json = JsonSerializer.Serialize(login, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response;
                try
                {
                    response = await PostRequest("api/Users/login", content);
                }
                catch (HttpRequestException httpEx)
                {
                    _logger.LogError($"HTTP request error: {httpEx.Message}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = "Failed to contact the server."
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Unexpected error during POST: {ex.Message}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = "Unexpected error while sending request."
                    };
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"HTTP response error: {response.StatusCode}, Body: {error}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Authentication failed: {response.StatusCode}"
                    };
                }

                var jsonResult = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(jsonResult))
                {
                    _logger.LogError("Login response is empty.");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = "Empty server response."
                    };
                }

                _logger.LogInformation("Login response JSON: " + jsonResult);

                Token? result;
                try
                {
                    result = JsonSerializer.Deserialize<Token>(jsonResult, _serializerOptions);
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError($"Error deserializing JSON: {jsonEx.Message}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = "Failed to process server response."
                    };
                }

                if (result == null || string.IsNullOrWhiteSpace(result.AccessToken))
                {
                    _logger.LogError("Invalid or null token received.");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = "Invalid token received from server."
                    };
                }

                // Save preferences
                Preferences.Set("accesstoken", result.AccessToken);
                Preferences.Set("userid", (int)result.UserId!);
                Preferences.Set("username", result.UserName);

                _logger.LogInformation("Login successful.");
                return new ApiResponse<bool> { Success = true, Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"General exception during login: {ex.Message}");
                return new ApiResponse<bool>
                {
                    ErrorMessage = "An unexpected error occurred during login."
                };
            }
        }


        private async Task<HttpResponseMessage> PostRequest(string uri, HttpContent content)
        {
            var url = $"{_baseUrl.TrimEnd('/')}/{uri.TrimStart('/')}";
            try
            {
                var result = await _httpClient.PostAsync(url, content);
                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP request error to {url}: {ex.Message}");
                throw; 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error when sending POST to {url}: {ex.Message}");
                throw;
            }
        }

    }
}
