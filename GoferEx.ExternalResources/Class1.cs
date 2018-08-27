using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GoferEx.ExternalResources
{
    public class Class1
    {
        private Uri _tokenUri;
        private string _clientId;
        private string _clientSecret;
        private Uri _redirectUri;
        private string _certUrl;

        public Class1(string tokenUri, string clientId, string clientSecret, string redirectUri, string certUrl)
        {
            _tokenUri = new Uri(tokenUri);
            _redirectUri = new Uri(redirectUri);
            _clientId = clientId;
            _clientSecret = clientSecret;
            _certUrl = certUrl;
        }
        //"https://www.googleapis.com/oauth2/v4/token"
        public TokenResponse ExchangeCodeForTokens(string code)
        {
            var cert = GetCert();
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"code", code},
                {"client_id", _clientId},
                {"client_secret", _clientSecret},
                {"redirect_uri", _redirectUri.ToString()},
                {"grant_type", "authorization_code"}
            });            
            string parameters = "code=" + code + "&client_id=" + _clientId +
                                "&client_secret=" + _clientSecret + "&redirect_uri="
                                + _redirectUri + "&grant_type=authorization_code";
            var htt = WebRequest.CreateHttp(_tokenUri.ToString());
            htt.ClientCertificates.Add(cert);
            htt.ContentType = "application/x-www-form-urlencoded";
            htt.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(parameters);
            htt.ContentType = "application/x-www-form-urlencoded";
            //htt.ContentLength = byteArray.Length;
            Stream postStream = htt.GetRequestStream();

            //Add the post data to the web request
            postStream.Write(byteArray, 0, byteArray.Length);
            postStream.Close();
            WebResponse response = htt.GetResponse();
            postStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(postStream);
            string responseFromServer = reader.ReadToEnd();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseFromServer);
            return tokenResponse;
        }

        private X509Certificate2 GetCert()
        {
            HttpWebRequest req = WebRequest.CreateHttp(_certUrl);
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            StreamReader reader = new StreamReader(res.GetResponseStream());
            string responseFromServer = reader.ReadToEnd();
            Dictionary<string, string> dict =
                JsonConvert.DeserializeObject<Dictionary<string, string>>(responseFromServer);
            X509Certificate2 cer = new X509Certificate2(Encoding.UTF8.GetBytes(dict.First().Value));            
            return cer;
        }
    }

    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresInSeconds { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("id_token")]
        public string IdToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonIgnore]
        public TimeSpan ExpiresIn => TimeSpan.FromSeconds(ExpiresInSeconds);
    }
}
