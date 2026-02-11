using Newtonsoft.Json;
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

        public string Token { get; set; }

        public ApiService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        // GET запрос
        public async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                // КРИТИЧНО: Устанавливаем заголовок авторизации ПЕРЕД каждым запросом
                _httpClient.DefaultRequestHeaders.Authorization = null;
                if (!string.IsNullOrEmpty(Token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", Token);
                }

                var response = await _httpClient.GetAsync(endpoint);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(content);
                }
                else
                {
                    MessageBox.Show($"API Error ({response.StatusCode}): {content}");
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Network Error: {ex.Message}");
                return default(T);
            }
        }

        // POST запрос
        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                // Устанавливаем заголовок авторизации
                _httpClient.DefaultRequestHeaders.Authorization = null;
                if (!string.IsNullOrEmpty(Token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", Token);
                }

                var json = JsonConvert.SerializeObject(data);

              

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    if (string.IsNullOrEmpty(responseContent) || responseContent == "\"\"")
                    {
                        return default(T);
                    }
                    return JsonConvert.DeserializeObject<T>(responseContent);
                }
                else
                {
                    MessageBox.Show($"API Error ({response.StatusCode}): {responseContent}");
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return default(T);
            }
        }

        // PUT запрос
        public async Task<bool> PutAsync(string endpoint, object data = null)
        {
            try
            {
                // Устанавливаем заголовок авторизации
                _httpClient.DefaultRequestHeaders.Authorization = null;
                if (!string.IsNullOrEmpty(Token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", Token);
                }

                HttpContent content = null;
                if (data != null)
                {
                    var json = JsonConvert.SerializeObject(data);
                    content = new StringContent(json, Encoding.UTF8, "application/json");
                }
                else
                {
                    content = new StringContent("", Encoding.UTF8, "application/json");
                }

                var response = await _httpClient.PutAsync(endpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"API Error ({response.StatusCode}): {errorContent}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return false;
            }
        }

        // DELETE запрос
        public async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                // Устанавливаем заголовок авторизации
                _httpClient.DefaultRequestHeaders.Authorization = null;
                if (!string.IsNullOrEmpty(Token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", Token);
                }

                var response = await _httpClient.DeleteAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"API Error ({response.StatusCode}): {errorContent}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return false;
            }
        }
    }
}