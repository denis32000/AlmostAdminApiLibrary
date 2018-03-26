using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AlmostAdmin.API
{
    public static class CryptoUtils
    {
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string HashSHA1(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);//ASCII
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private static string GetHexadecimalString(IEnumerable<byte> buffer)
        {
            return buffer.Select(b => b.ToString("x2")).Aggregate("", (total, cur) => total + cur);
        }
    }
    /*
    public interface IHttpClient : IDisposable
    {
        string ApiCode { get; set; }
        Task<T> GetAsync<T>(string route, QueryString queryString = null, Func<string, T> customDeserialization = null);
        Task<TResponse> PostAsync<TPost, TResponse>(string route, TPost postObject, Func<string, TResponse> customDeserialization = null, bool multiPartContent = false, string contentType = null);
    }

    public class BlockchainHttpClient : IHttpClient
    {
        private const string BASE_URI = "https://localhost:"; // TODO:
        private const int TIMEOUT_MS = 100000;
        private readonly HttpClient httpClient;
        public string ApiCode { get; set; }

        public BlockchainHttpClient(string apiCode = null, string uri = BASE_URI)
        {
            ApiCode = apiCode;
            httpClient = new HttpClient
            {
                BaseAddress = new Uri(uri),
                Timeout = TimeSpan.FromMilliseconds(BlockchainHttpClient.TIMEOUT_MS)
            };
        }

        public async Task<T> GetAsync<T>(string route, QueryString queryString = null, Func<string, T> customDeserialization = null)
        {
            if (route == null)
            {
                throw new ArgumentNullException(nameof(route));
            }

            if (ApiCode != null)
            {
                queryString?.Add("api_code", ApiCode);
            }

            if (queryString != null && queryString.Count > 0)
            {
                int queryStringIndex = route.IndexOf('?');
                if (queryStringIndex >= 0)
                {
                    //Append to querystring
                    string queryStringValue = queryStringIndex.ToString();
                    //replace questionmark with &
                    queryStringValue = "&" + queryStringValue.Substring(1);
                    route += queryStringValue;
                }
                else
                {
                    route += queryString.ToString();
                }
            }
            HttpResponseMessage response = await httpClient.GetAsync(route);
            string responseString = await ValidateResponse(response);
            var responseObject = customDeserialization == null
                ? JsonConvert.DeserializeObject<T>(responseString)
                : customDeserialization(responseString);
            return responseObject;
        }

        public async Task<TResponse> PostAsync<TPost, TResponse>(string route, TPost postObject, Func<string, TResponse> customDeserialization = null, bool multiPartContent = false, string contentType = "application/x-www-form-urlencoded")
        {
            if (route == null)
            {
                throw new ArgumentNullException(nameof(route));
            }
            if (ApiCode != null)
            {
                route += "?api_code=" + ApiCode;
            }
            string json = JsonConvert.SerializeObject(postObject);
            HttpContent httpContent;
            if (multiPartContent)
            {
                httpContent = new MultipartFormDataContent
                {
                    new StringContent(json, Encoding.UTF8, contentType)
                };
            }
            else
            {
                httpContent = new StringContent(json, Encoding.UTF8, contentType);
            }
            HttpResponseMessage response = await httpClient.PostAsync(route, httpContent);
            string responseString = await this.ValidateResponse(response);
            TResponse responseObject = JsonConvert.DeserializeObject<TResponse>(responseString);
            return responseObject;
        }

        private async Task<string> ValidateResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                if (responseString != null && responseString.StartsWith("{\"error\":"))
                {
                    JObject jObject = JObject.Parse(responseString);
                    string message = jObject["error"].ToObject<string>();
                    throw new ServerApiException(message, HttpStatusCode.BadRequest);
                }
                return responseString;
            }
            string responseContent = await response.Content.ReadAsStringAsync();
            if (string.Equals(responseContent, "Block Not Found"))
            {
                throw new ServerApiException("Block Not Found", HttpStatusCode.NotFound);
            }
            throw new ServerApiException(response.ReasonPhrase + ": " + responseContent, response.StatusCode);
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }
    }

    public class QueryString
    {
        private readonly IDictionary<string, string> _queryString;

        public QueryString()
        {
            _queryString = new Dictionary<string, string>();
        }

        public void Add(string key, string value)
        {
            if (_queryString.ContainsKey(key))
            {
                throw new ClientApiException($"Query string already has a value for {key}");
            }
            _queryString[key] = value;
        }

        public int Count => _queryString.Count;

        public void AddOrUpdate(string key, string value)
        {
            _queryString[key] = value;
        }

        public override string ToString()
        {
            return "?" + string.Join("&", _queryString.Select(kv => $"{kv.Key}={kv.Value}"));
        }
    }
    */
}
