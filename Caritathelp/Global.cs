using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tavis.UriTemplates;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Web;

namespace Caritathelp
{
    public class HttpHandler
    {
        public enum TypeRequest
        {
            POST,
            PUT,
            DELETE,
            GET
        }

        private static string token = null;
        private static string client_id = null;
        private static string uuid = null;
        private static HttpHandler http = null;
        static HttpClient httpClient = null;

        public static void resetHttp()
        {
            token = null;
            client_id = null;
            uuid = null;
            http = null;
            httpClient = null;
        }

        public static HttpHandler getHttp()
        {
            if (http == null)
            {
                http = new HttpHandler();
                httpClient = new HttpClient(new HttpClientHandler());
            }
            return http;
        }

        public async Task<Newtonsoft.Json.Linq.JObject> sendRequest(string url, List<KeyValuePair<string, string> > values, TypeRequest req)
        {
            Newtonsoft.Json.Linq.JObject err = new Newtonsoft.Json.Linq.JObject();
            string responseString;
            try
            {
                    Debug.WriteLine(req + " " + Global.API_IRL + url);
                HttpResponseMessage response = null;
                switch (req)
                {
                    case TypeRequest.POST:
                        response = await httpClient.PostAsync(Global.API_IRL + url, new FormUrlEncodedContent(values));
                        break;
                    case TypeRequest.PUT:
                        response = await httpClient.PutAsync(Global.API_IRL + url, new FormUrlEncodedContent(values));
                        break;
                    case TypeRequest.DELETE:
                        response = await httpClient.DeleteAsync(Global.API_IRL + url);
                        break;
                    case TypeRequest.GET:
                        response = await httpClient.GetAsync(Global.API_IRL + url);
                        break;
                }
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(url + responseString.ToString());
                Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                if (token == null)
                {
                    IEnumerable<string> headersValue;
                    HttpResponseHeaders responseHeadersCollection = response.Headers;
                    if (responseHeadersCollection.TryGetValues("access-token", out headersValue))
                    {
                        token = headersValue.First();
                    }
                    if (responseHeadersCollection.TryGetValues("client", out headersValue))
                    {
                        client_id = headersValue.First();
                    }
                    if (responseHeadersCollection.TryGetValues("uid", out headersValue))
                    {
                        uuid = headersValue.First();
                    }
                    httpClient.DefaultRequestHeaders.Add("access-token", token);
                    httpClient.DefaultRequestHeaders.Add("client", client_id);
                    httpClient.DefaultRequestHeaders.Add("uid", uuid);
                    return (Newtonsoft.Json.Linq.JObject)jObject["response"];
                }
                return jObject;
            }
            catch (HttpRequestException e)
            {
                err["status"] = 500;
                err["message"] = e.Message;
                Debug.WriteLine(e.Message);
            } catch (Exception e)
            {
                err["status"] = 500;
                err["message"] = e.Message;
                Debug.WriteLine(e.Message);
            }
            return err;
        }
    }

    class Global
    {
        public const string API_IRL = "http://staging.caritathelp.me:80";
        public const string WS_URL = "ws://ws.staging.caritathelp.me";
    }

    class Publication
    {
        public int id { get; set; }
        public int volunteer_id { get; set; }
        public string news_type { get; set; }


        public string title { get; set; }
        public string content { get; set; }
        public int group_id { get; set; }
        public string group_type { get; set; }
        public string group_name { get; set; }
        public string group_thumb_path { get; set; }


        public string created_at { get; set; }
        public string updated_at { get; set; }
        public int number_comments { get; set; }
        public bool as_group { get; set; }
        public string volunteer_name { get; set; }
        public string volunteer_thumb_path { get; set; }
    }

    class Comments
    {
        public int id { get; set; }
        public int new_id { get; set; }
        public int volunteer_id { get; set; }
        public string content { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string thumb_path { get; set; }
    }

    class commentInfos
    {
        public int id_news { get; set; }
        public int id_comments { get; set; }
        public int row_cmt { get; set; }
        public int row_news { get; set; }
    }

    class NewsInfos
    {
        public int id_news { get; set; }
        public int id_rows { get; set; }
    }

}
