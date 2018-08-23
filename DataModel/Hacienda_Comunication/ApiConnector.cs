using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Hacienda_Comunication
{
    public class ApiRequest
    {
        public object Request { set; get; }
        public string Route { set; get; }
        public string Id { set; get; }
        public string Token { set; get; }
        public string mapObjet()
        {
            return JsonConvert.SerializeObject(Request);
        }       
    }


    public class ApiResponse
    {
        /// <summary>
        /// Retorna el tipo de respuesta obteida del servidor
        /// </summary>
        public HttpStatusCode Code { set; get; }
        public string Response { set; get; }
        /// <summary>
        /// Intenta convertir el resultado de la cconsulta a un objeto del tipo indicado
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T mapObjet<T>()
        {
            if (loadError)
                return Activator.CreateInstance<T>();

            return JsonConvert.DeserializeObject<T>(Response);
        }

        /// <summary>
        /// retorna un JObject apartir del string obtenido del servidor
        /// </summary>
        /// <returns></returns>
        public JObject GetJsonFromResponse()
        {
            return JObject.Parse(Response);
        }

        /// <summary>
        /// Indica si hay un error durante la transaccion
        /// </summary>
        public bool loadError { set; get; }
        public HttpResponseHeaders Headers { get; set; }
    }


    public class ApiClient
    {
        public int TimeOut { set; get; }
        string BaseURL;
        private HttpClient Client;

        public ApiClient(int TimeOut = 5, string BaseURL = null)
        {
            if (BaseURL == null)
            {
                this.BaseURL = ConfigurationManager.AppSettings.Get("UrlAPI");
            }
            else
            {
                this.BaseURL = BaseURL;
            }
            this.TimeOut = TimeOut;
            Client = null;
        }

        private Uri CreateUrlFromRequest(ApiRequest request)
        {
            if (request.Route == null)
            {
                return new Uri(BaseURL);
            }

            string Url = BaseURL + request.Route;
            if (request.Id != null)
            {
                Url += "/" + request.Id;
            }
            return new Uri(Url);
        }

       

       
        public ApiResponse POST(ApiRequest request)
        {

            try
            {
                string json = request.mapObjet();
                StringContent Content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage Response = null;
                using (Client = new HttpClient())
                {
                    if (request.Token != null)
                    {
                        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", request.Token);
                    }

                    Response = Client.PostAsync(CreateUrlFromRequest(request), Content).Result;
                }
                return ApiResponseFromHttpResponseMessage(Response);

            }
            catch (Exception ex)
            {
                return ResponseError(ex);
            }

        }



        public ApiResponse GET(ApiRequest request)
        {
            try
            {
                HttpResponseMessage Response = null;
         
                    using (Client = new HttpClient())
                    {
                        if (request.Token != null)
                        {
                            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", request.Token);
                        }

                        Response = Client.GetAsync(CreateUrlFromRequest(request)).Result;
                    }
                return ApiResponseFromHttpResponseMessage(Response);
            }
            catch (Exception ex)
            {
                return ResponseError(ex);
            }
        }

        private ApiResponse ApiResponseFromHttpResponseMessage(HttpResponseMessage Response)
        {
            return new ApiResponse()
            {
                loadError = false,
                Code = Response.StatusCode,
                Headers = Response.Headers,
                Response = Response.Content.ReadAsStringAsync().Result
            };
        }



        private ApiResponse ResponseError(Exception ex)
        {
            return new ApiResponse()
            {
                loadError = true,
                Code = HttpStatusCode.UnsupportedMediaType,
                Response = "Host Response Error " + ex
            };
        }


        public ApiResponse IDP_Post(ApiRequest request)
        {
            IDictionary<string, string> keyValueContent = ToKeyValue(request.Request);
            FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(keyValueContent);

            HttpResponseMessage Response = null;
            using (Client = new HttpClient())
            {
                if (request.Token != null)
                {
                    Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", request.Token);
                }

                Response = Client.PostAsync(CreateUrlFromRequest(request), formUrlEncodedContent).Result;
            }
            return ApiResponseFromHttpResponseMessage(Response);
        }


        private IDictionary<string, string> ToKeyValue(object metaToken)
        {
            if (metaToken == null)
            {
                return null;
            }

            JToken token = metaToken as JToken;
            if (token == null)
            {
                return ToKeyValue(JObject.FromObject(metaToken));
            }

            if (token.HasValues)
            {
                var contentData = new Dictionary<string, string>();
                foreach (var child in token.Children().ToList())
                {
                    var childContent = ToKeyValue(child);
                    if (childContent != null)
                    {
                        contentData = contentData.Concat(childContent)
                                                 .ToDictionary(k => k.Key, v => v.Value);
                    }
                }

                return contentData;
            }

            var jValue = token as JValue;
            if (jValue?.Value == null)
            {
                return null;
            }

            var value = jValue?.Type == JTokenType.Date ?
                            jValue?.ToString("o", CultureInfo.InvariantCulture) :
                            jValue?.ToString(CultureInfo.InvariantCulture);

            return new Dictionary<string, string> { { token.Path, value } };
        }

    }
}
