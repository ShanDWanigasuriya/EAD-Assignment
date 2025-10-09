using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace EVWebApp.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _ctx;

        public ApiClient(HttpClient http, IHttpContextAccessor ctx)
        {
            _http = http;
            _ctx = ctx;
        }

        private void AttachBearer()
        {
            var token = _ctx.HttpContext?.Session.GetString("token");
            if (!string.IsNullOrEmpty(token))
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<T?> Get<T>(string url)
        {
            AttachBearer();
            var res = await _http.GetAsync(url);
            if (!res.IsSuccessStatusCode) return default;
            var json = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task<HttpResponseMessage> Post(string url, object body, bool auth = true)
        {
            if (auth) AttachBearer();
            var content = new StringContent(JsonConvert.SerializeObject(body), System.Text.Encoding.UTF8, "application/json");
            return await _http.PostAsync(url, content);
        }

        public async Task<HttpResponseMessage> Put(string url, object body)
        {
            AttachBearer();
            var content = new StringContent(JsonConvert.SerializeObject(body), System.Text.Encoding.UTF8, "application/json");
            return await _http.PutAsync(url, content);
        }

        public async Task<HttpResponseMessage> Patch(string url, object body)
        {
            AttachBearer();
            var content = new StringContent(JsonConvert.SerializeObject(body), System.Text.Encoding.UTF8, "application/json");
            var req = new HttpRequestMessage(new HttpMethod("PATCH"), url) { Content = content };
            return await _http.SendAsync(req);
        }

        public async Task<HttpResponseMessage> Delete(string url)
        {
            AttachBearer();
            return await _http.DeleteAsync(url);
        }
    }
}
