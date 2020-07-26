using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Exceptions;
using EnterpriseBot.ApiWrapper.Models.Other;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Utils
{
    public class ApiClient : IApiClient
    {
        private readonly Uri apiUri;
        private readonly HttpClient http;
        private readonly HttpMethod defaultHttpMethod = HttpMethod.Post;
        private readonly CurrentLocalization localization;

        public ApiClient(Uri apiServerUri, CurrentLocalization localization)
        {
            apiUri = apiServerUri;
            http = new HttpClient();
            this.localization = localization;
        }

        private JsonSerializerSettings JsonSerializerSettings
        {
            get
            {
                return new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy(processDictionaryKeys: true, overrideSpecifiedNames: false)
                    },
                    FloatParseHandling = FloatParseHandling.Decimal,
                    PreserveReferencesHandling = PreserveReferencesHandling.All,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                };
            }
        }

        #region public
        public async Task<T> Call<T>(ApiRequestInfo reqInfo, object objToSerialize)
        {
            var uri = ConstructUri(apiUri, reqInfo);

            var json = await Request(uri, objToSerialize);
            if (json == null)
            {
                return default;
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(json.ToString(), JsonSerializerSettings);
            }
            catch (JsonReaderException)
            {
                return (T)(Convert.ChangeType(json, typeof(T)));
            }
        }

        public async Task<object> Call(ApiRequestInfo reqInfo, object objToSerialize, Type typeToDeserialize)
        {
            var uri = ConstructUri(apiUri, reqInfo);

            var json = await Request(uri, objToSerialize);
            if (json == null)
            {
                return default;
            }

            try
            {
                return JsonConvert.DeserializeObject(json.ToString(), typeToDeserialize, JsonSerializerSettings);
            }
            catch (JsonReaderException)
            {
                return Convert.ChangeType(json, typeToDeserialize);
            }
        }

        public async Task<dynamic> Call(ApiRequestInfo reqInfo, object objToSerialize)
        {
            var uri = ConstructUri(apiUri, reqInfo);

            var json = await Request(uri, objToSerialize);
            if (json == null)
            {
                return default;
            }

            try
            {
                return JsonConvert.DeserializeObject(json.ToString(), JsonSerializerSettings);
            }
            catch (JsonReaderException)
            {
                return (dynamic)json;
            }
        }
        #endregion

        #region private
        private async Task<object> Request(Uri uri, object objToSerialize)
        {
            var result = await RequestBase(uri, objToSerialize);

            if (result.IsSuccess == false)
            {
                ThrowLocalizedError(result.LocalizedError, this.localization);
            }

            return result.JsonResult;
        }

        private async Task<Result> RequestBase(Uri uri, object objToSerialize)
        {
            string serialized = JsonConvert.SerializeObject(objToSerialize, JsonSerializerSettings);
            
            var request = ConstructJsonHttpRequest(serialized, defaultHttpMethod, uri);

            var response = await http.SendAsync(request);
            string responseResult = await response.Content.ReadAsStringAsync();

            Result result;
            try
            {
                result = JsonConvert.DeserializeObject<Result>(responseResult, JsonSerializerSettings);
            }
            catch (JsonReaderException)
            {
                ThrowLocalizedError(new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Critical,
                    EnglishMessage = $"API threw an exception: {responseResult}",
                    RussianMessage = $"API выбросило исключение: {responseResult}"
                }, localization);

                return null;
            }
            result.StatusCode = response.StatusCode;

            return result;
        }

        #region service methods
        private Uri ConstructUri(Uri uri, ApiRequestInfo reqInfo)
        {
            string relativeUri = Path.Combine(reqInfo.CategoryAreaName,
                                              reqInfo.CategorySubAreaName ?? string.Empty,
                                              reqInfo.CategoryName,
                                              reqInfo.MethodName);

            return new Uri(uri, relativeUri);
        }

        private HttpRequestMessage ConstructJsonHttpRequest(string contentJsonString, HttpMethod method, Uri requestUri)
        {
            var request = new HttpRequestMessage
            {
                Content = new StringContent(contentJsonString, Encoding.UTF8),
                Method = method,
                RequestUri = requestUri
            };
            //request.Headers.Add("Content-Type", "application/json; charset=utf-8");
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return request;
        }

        private void ThrowLocalizedError(LocalizedError localizedError, CurrentLocalization localization)
        {
            string message;
            switch (localization)
            {
                case CurrentLocalization.English:
                    message = localizedError.EnglishMessage;
                    break;

                case CurrentLocalization.Russian:
                    message = localizedError.RussianMessage;
                    break;

                default:
                    message = localizedError.EnglishMessage;
                    break;
            }

            switch (localizedError.ErrorSeverity)
            {
                case ErrorSeverity.Normal:
                    throw new ApiNormalException(message);

                case ErrorSeverity.Critical:
                    throw new ApiCriticalException(message);

                default:
                    throw new ApiException(message);
            }
        }
        #endregion
        #endregion
    }
}
