using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Reflection;

namespace Hms.WebApp.Services
{
    public class BaseService
    {
        #region Properties
        protected string HmsApiUrl { get; private set; } = "";
        protected string HmsApiVersion { get; private set; } = "";
        protected int HmsWebApiTimeoutMin { get; private set; }
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly IConfiguration _configuration;
        #endregion

        #region Constructor
        protected BaseService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            HmsApiUrl = _configuration["HmsApiUrl"] ?? "";
            HmsApiVersion = _configuration["HmsApiVersion"] ?? "";
            HmsWebApiTimeoutMin = _configuration.GetValue<int>("HmsWebApiTimeoutMin", 5);
        }
        #endregion

        #region Private Helper
        private void SetHttpClientProperties(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri(HmsApiUrl);
            httpClient.Timeout = TimeSpan.FromMinutes(HmsWebApiTimeoutMin);
            httpClient.DefaultRequestHeaders.Clear();
        }
        #endregion

        #region DoHttpPost
        public async Task<HttpResponseMessage> DoHttpPost(string url, dynamic model, string accessToken = "")
        {
            using (HttpClient httpClient = _httpClientFactory.CreateClient("HttpClient"))
            {
                SetHttpClientProperties(httpClient);

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                request.Content = new StringContent(JsonConvert.SerializeObject(model));
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                if (!string.IsNullOrEmpty(accessToken))
                {
                    request.Headers.Add("Authorization", $"Bearer {accessToken}");
                }

                var response = await httpClient.SendAsync(request);
                return response;
            }
        }
        #endregion

        #region DoHttpPostFromForm (File Upload)
        public async Task<HttpResponseMessage> DoHttpPostFromForm(string url, dynamic model, string accessToken = "")
        {
            var response = new HttpResponseMessage();

            using (HttpClient httpClient = _httpClientFactory.CreateClient("HttpClient"))
            {
                SetHttpClientProperties(httpClient);

                if (!string.IsNullOrEmpty(accessToken))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                }
                var formData = await ConvertToMultipartFormDataContent(model);

                response = await httpClient.PostAsync(url, formData);
            }

            return response;
        }

        private static async Task<MultipartFormDataContent> ConvertToMultipartFormDataContent(dynamic model)
        {
            var formData = new MultipartFormDataContent();

            await Task.Run(() =>
            {
                foreach (PropertyInfo property in model.GetType().GetProperties())
                {
                    var propertyName = property.Name;
                    var propertyValue = property.GetValue(model);

                    if (propertyValue is List<IFormFile> multiFile)
                    {
                        foreach (var file in multiFile)
                        {
                            var streamContent = new StreamContent(file.OpenReadStream());
                            streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                            {
                                Name = "Files",
                                FileName = file.FileName
                            };
                            formData.Add(streamContent);
                        }
                    }
                    else if (propertyValue is IFormFile singleFile)
                    {
                        var streamContent = new StreamContent(singleFile.OpenReadStream());
                        streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = "Files",
                            FileName = singleFile.FileName
                        };
                        formData.Add(streamContent);
                    }
                    else if (propertyValue is IEnumerable<string> stringList)
                    {
                        int i = 0;
                        foreach (var str in stringList)
                        {
                            formData.Add(new StringContent(str), propertyName + $"[{i}]");
                            i++;
                        }
                    }
                    else
                    {
                        if (propertyValue != null)
                        {
                            formData.Add(new StringContent(propertyValue.ToString()), propertyName);
                        }
                    }
                }
            });

            return formData;
        }
        #endregion

        #region DoHttpPut
        public async Task<HttpResponseMessage> DoHttpPut(string url, dynamic model, string accessToken = "")
        {
            using (HttpClient httpClient = _httpClientFactory.CreateClient("HttpClient"))
            {
                SetHttpClientProperties(httpClient);

                var request = new HttpRequestMessage(HttpMethod.Put, url);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = new StringContent(JsonConvert.SerializeObject(model));
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                if (!string.IsNullOrEmpty(accessToken))
                {
                    request.Headers.Add("Authorization", $"Bearer {accessToken}");
                }

                var response = await httpClient.SendAsync(request);
                return response;
            }
        }
        #endregion

        #region DoHttpDelete
        public async Task<HttpResponseMessage> DoHttpDelete(string url, dynamic model = null, string accessToken = "")
        {
            using (HttpClient httpClient = _httpClientFactory.CreateClient("HttpClient"))
            {
                SetHttpClientProperties(httpClient);

                var request = new HttpRequestMessage(HttpMethod.Delete, url);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (model != null)
                {
                    request.Content = new StringContent(JsonConvert.SerializeObject(model));
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }

                if (!string.IsNullOrEmpty(accessToken))
                {
                    request.Headers.Add("Authorization", $"Bearer {accessToken}");
                }

                var response = await httpClient.SendAsync(request);
                return response;
            }
        }
        #endregion

        #region DoHttpGet
        public async Task<HttpResponseMessage> DoHttpGet(string url, string accessToken = "")
        {
            using (HttpClient httpClient = _httpClientFactory.CreateClient("HttpClient"))
            {
                SetHttpClientProperties(httpClient);

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrEmpty(accessToken))
                {
                    request.Headers.Add("Authorization", $"Bearer {accessToken}");
                }

                var response = await httpClient.SendAsync(request);
                return response;
            }
        }
        #endregion
    }
}