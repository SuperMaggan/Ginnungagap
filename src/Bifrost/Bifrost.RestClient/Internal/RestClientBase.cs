using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace Bifrost.RestClient.Internal
{
    public abstract class RestClientBase
    {
        protected readonly IAsgardRestClientSettings Settings;
        protected readonly TimeSpan Timeout;
        private readonly HttpClient _client;

        protected RestClientBase(IAsgardRestClientSettings settings)
        {
            Settings = settings;
            if (Settings.AsgardTimeoutMS == 0)
                Settings.AsgardTimeoutMS= 2000;
            Timeout = new TimeSpan(0, 0, 0, 0, Settings.AsgardTimeoutMS);

            _client = new HttpClient()
            {
                Timeout = Timeout
            };
            _client.DefaultRequestHeaders.Add("apikey", settings.AsgardClientApiKey);
        }



        protected T Get<T>(string url)
        {

            try
            {
                var result = _client.GetStringAsync(url).Result;
                return JsonConvert.DeserializeObject<T>(result);
            }
            catch (Exception e)
            {
                throw CreateException(e, url);
            }

        }


        protected void Post<T>(string url, T obj)
        {

            try
            {
                var body = new ByteArrayContent(ToByteArray(obj));
                body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var res = _client.PostAsync(url, body).Result;
                if (!res.IsSuccessStatusCode)
                    throw new RestException(res.ReasonPhrase, res.Content.ReadAsStringAsync().Result, res.StatusCode);
            }
            catch (Exception e)
            {
                throw CreateException(e, url);
            }

        }

        protected void Post(string url, Stream obj)
        {

            try
            {
                StreamContent content = new StreamContent(obj);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var res = _client.PostAsync(url, content).Result;
                if (!res.IsSuccessStatusCode)
                    throw new RestException(res.ReasonPhrase, res.Content.ReadAsStringAsync().Result, res.StatusCode);
            }
            catch (Exception e)
            {
                throw CreateException(e, url);
            }

        }


        protected TOut Post<TIn, TOut>(string url, TIn obj)
        {

            try
            {
                var body = new ByteArrayContent(ToByteArray(obj));
                body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var res = _client.PostAsync(url, body).Result;
                if (!res.IsSuccessStatusCode)
                    throw new RestException(res.ReasonPhrase, res.Content.ReadAsStringAsync().Result, res.StatusCode);
                return JsonConvert.DeserializeObject<TOut>(res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception e)
            {
                throw CreateException(e, url);
            }
        }


        protected T Post<T>(string url)
        {

            try
            {
                var res = _client.PostAsync(url, new StringContent("")).Result;
                if (!res.IsSuccessStatusCode)
                    throw new RestException(res.ReasonPhrase, res.Content.ReadAsStringAsync().Result, res.StatusCode);
                return JsonConvert.DeserializeObject<T>(res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception e)
            {
                throw CreateException(e, url);
            }

        }

        protected void Put<T>(string url, T obj)
        {

            try
            {

                var body = new ByteArrayContent(ToByteArray(obj));
                body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var res = _client.PutAsync(url, body).Result;
                if (!res.IsSuccessStatusCode)
                    throw new RestException(res.ReasonPhrase, res.Content.ReadAsStringAsync().Result, res.StatusCode);
            }
            catch (Exception e)
            {
                throw CreateException(e, url);
            }

        }

        protected T Put<T>(string url)
        {

            try
            {

                var res = _client.PutAsync(url, new StringContent("")).Result;
                if (!res.IsSuccessStatusCode)
                    throw new RestException(res.ReasonPhrase, res.Content.ReadAsStringAsync().Result, res.StatusCode);
                return JsonConvert.DeserializeObject<T>(res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception e)
            {
                throw CreateException(e, url);
            }

        }
        protected void Put(string url)
        {

            try
            {

                var body = new StringContent("");
                body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var res = _client.PutAsync(url, body).Result;
                if (!res.IsSuccessStatusCode)
                    throw new RestException(res.ReasonPhrase, res.Content.ReadAsStringAsync().Result, res.StatusCode);
            }
            catch (Exception e)
            {
                throw CreateException(e, url);
            }

        }


        protected TOut Put<TIn, TOut>(string url, TIn obj)
        {

            try
            {
                var body = new ByteArrayContent(ToByteArray(obj));
                body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var res = _client.PutAsync(url, body).Result;
                if (!res.IsSuccessStatusCode)
                    throw new RestException(res.ReasonPhrase, res.Content.ReadAsStringAsync().Result, res.StatusCode);
                return JsonConvert.DeserializeObject<TOut>(res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception e)
            {
                throw CreateException(e, url);
            }

        }


        protected void Delete(string url)
        {

            try
            {
                var res = _client.DeleteAsync(url).Result;
                if (!res.IsSuccessStatusCode)
                    throw new RestException(res.ReasonPhrase, res.Content.ReadAsStringAsync().Result, res.StatusCode);
                //return JsonConvert.DeserializeObject<T>(res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception e)
            {
                throw CreateException(e, url);
            }

        }


        protected Exception CreateException(Exception e, string url)
        {
            var baseE = GetInnermostException(e);
            return new Exception($"Error when requesting {url}: {baseE.Message}", baseE);

            //if (we.Response == null)
            //    return new RestException("Error communicating with the endpoint: " + url + "  ->" + we.Message, "", we);
            //var responseStream = we.Response.GetResponseStream();
            //if (responseStream == null)
            //    return new RestException("Error when communicating with the  endpoint " + url + ". It threw an error but gave no response", "", we.Status);

            //if ((we.Response as HttpWebResponse).StatusCode == HttpStatusCode.NotFound)
            //    return new RestException(string.Format("Requesting {0}\r\n{1}", url, we.Message), "", we.Status);
            //using (var reader = new StreamReader(responseStream))
            //{
            //    var responseHtml = reader.ReadToEnd();
            //    var first = responseHtml.IndexOf('\'');
            //    if (first < 1)
            //        throw new RestException("Failed to parse the exception! See HtmlResponse for more information: " + responseHtml, responseHtml, we.Status);
            //    var second = responseHtml.LastIndexOf('\'');
            //    if (second < 1)
            //        throw new RestException("Failed to parse the exception! See HtmlResponse for more information: " + responseHtml, responseHtml, we.Status);
            //    var exceptionMessage = responseHtml.Substring(first, second - first);
            //    throw new RestException("Endpoint threw an error: " + exceptionMessage, responseHtml, we.Status);
            //}
        }


        protected string QueryParameter(string name, params string[] values)
        {
            var queryString = values.Any()
                ? $"{name}={string.Join($"&{name}=", values)}"
                : "";
            return queryString;
        }

        private byte[] ToByteArray<T>(T obj)
        {
            var s = JsonConvert.SerializeObject(obj);
            return Encoding.UTF8.GetBytes(s);
        }


        private Exception GetInnermostException(Exception e)
        {
            if (e.InnerException == null)
                return e;
            return GetInnermostException(e.InnerException);
        }


    }
}