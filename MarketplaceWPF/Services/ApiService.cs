using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows;

namespace MarketplaceWPF.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7093/api/";

        public string Token { get; set; } = string.Empty;

        public ApiService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                ApplyAuthorizationHeader();

                var response = await _httpClient.GetAsync(endpoint);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(content);
                }

                MessageBox.Show($"Ошибка сервиса ({response.StatusCode}): {content}");
                return default;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}");
                return default;
            }
        }

        public async Task<T?> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                ApplyAuthorizationHeader();

                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    if (string.IsNullOrEmpty(responseContent) || responseContent == "\"\"")
                    {
                        return default;
                    }

                    if (typeof(T) == typeof(string))
                    {
                        return (T)(object)responseContent.Trim('"');
                    }

                    return JsonConvert.DeserializeObject<T>(responseContent);
                }

                MessageBox.Show($"Ошибка сервиса ({response.StatusCode}): {responseContent}");
                return default;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                return default;
            }
        }

        public async Task<bool> PutAsync(string endpoint, object? data = null)
        {
            try
            {
                ApplyAuthorizationHeader();

                HttpContent content = data != null
                    ? new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
                    : new StringContent(string.Empty, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(endpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка сервиса ({response.StatusCode}): {errorContent}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                ApplyAuthorizationHeader();

                var response = await _httpClient.DeleteAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка сервиса ({response.StatusCode}): {errorContent}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                return false;
            }
        }

        public async Task<string> UploadProductImageAsync(string filePath)
        {
            try
            {
                ApplyAuthorizationHeader();

                using var form = new MultipartFormDataContent();
                await using var fileStream = File.OpenRead(filePath);
                using var fileContent = new StreamContent(fileStream);

                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                form.Add(fileContent, "file", Path.GetFileName(filePath));

                var response = await _httpClient.PostAsync("Uploads/product-image", form);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Ошибка сервиса ({response.StatusCode}): {responseContent}");
                    return string.Empty;
                }

                dynamic? result = JsonConvert.DeserializeObject(responseContent);
                return result?.imageUrl ?? string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
                return string.Empty;
            }
        }

        private void ApplyAuthorizationHeader()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrEmpty(Token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Token);
            }
        }
    }
}
