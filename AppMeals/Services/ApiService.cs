using AppMeals.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

        public async Task<(List<Category>? Categories, string? ErrorMessage)> GetCategories()
        {
            return await GetAsync<List<Category>>("api/Categories");
        }

        public async Task<(List<Product>? Products, string? ErrorMessage)> GetProducts(string productType, string categoryId)
        {
            string endpoint;

            if (string.IsNullOrWhiteSpace(categoryId))
            {
                endpoint = $"api/Products?productType={productType}";
            }
            else
            {
                endpoint = $"api/Products?productType={productType}&categoryId={categoryId}";
            }

            var (products, errorMessage) = await GetAsync<List<Product>>(endpoint);

            if (products != null)
            {
                var baseUrl = "https://q82b0738-44381.uks1.devtunnels.ms";

                foreach (var product in products)
                {
                    if (!string.IsNullOrEmpty(product.ImageUrl) && product.ImageUrl.StartsWith("/"))
                    {
                        product.ImageUrl = $"{baseUrl.TrimEnd('/')}/{product.ImageUrl.TrimStart('/')}";
                    }

                    _logger.LogInformation($"Nome: {product.Name}, Imagem: {product.ImageUrl}");
                }
            }

            return (products, errorMessage);
        }


        private async Task<(T? Data, string? ErrorMessage)>GetAsync<T>(string endpoint)
        {
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.GetAsync(AppConfig.BaseUrl + endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<T>(responseString, _serializerOptions);
                    return (data ?? Activator.CreateInstance<T>(), null);
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        string errorMessage = "Unauthorized";
                        _logger.LogWarning(errorMessage);
                        return (default, errorMessage);
                    }

                    string generalErrorMessage = $"Erro na requisição: {response.ReasonPhrase}";
                    _logger.LogError(generalErrorMessage);
                    return (default, generalErrorMessage);
                }
            }
            catch (HttpRequestException ex)
            {
                string errorMessage = $"HTTP request error: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
            catch (JsonException ex)
            {
                string errorMessage = $"JSON deserialization error: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Unexpected error: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }

        }

        private void AddAuthorizationHeader()
        {
            var token = Preferences.Get("accesstoken", string.Empty);

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<(Product? productDetail, string errorMessage)> GetProductDetail(int productId)
        {
            string endpoint = $"api/Products/{productId}";
            return await GetAsync<Product>(endpoint);
        }
    }
}
