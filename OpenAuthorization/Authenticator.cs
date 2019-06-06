using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OpenAuthorization
{
    public class Authenticator
    {
        Credential credential;

        public async Task<AccessTokenInfo> GetAccessToken(string accessCode)
        {
            Task<HttpResponseMessage> task = GetAccessTokenResponse(credential, accessCode);
            HttpResponseMessage response = task.Result;
            string result = await response.Content.ReadAsStringAsync();
            //StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
            //string result = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<AccessTokenInfo>(result);
        }

        private async Task<HttpResponseMessage> GetAccessTokenResponse(Credential credential, string accessCode)
        {
            string reqUrl = credential.authURL + "/adfs/oauth2/token";

            string param = "client_id=" + credential.ClientId + "&redirect_uri=" + credential.RedirectUri + "&grant_type=authorization_code&code=" + accessCode;
            byte[] postData = Encoding.UTF8.GetBytes(param);
            HttpWebRequest request = HttpWebRequest.CreateHttp(reqUrl);
            request.Method = "post";
            request.Headers.Add("cache-control", "no-cache");
            request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            Stream reqStream = request.GetRequestStream();
            reqStream.Write(postData, 0, postData.Length);
            reqStream.Close();
            WebResponse resp = request.GetResponse();
            Stream stream = resp.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            string content = reader.ReadToEnd();
            reader.Close();
            stream.Close();

            Dictionary<string, string> paramMapping = new Dictionary<string, string>();
            paramMapping["client_id"] = credential.ClientId;
            paramMapping["redirect_uri"] = credential.RedirectUri;
            paramMapping["grant_type"] = "authorization_code";
            paramMapping["code"] = accessCode;
            HttpClient client = new HttpClient();
            HttpRequestMessage reqMsg = new HttpRequestMessage(HttpMethod.Post, reqUrl);
            reqMsg.Content = new FormUrlEncodedContent(paramMapping);

            HttpResponseMessage response = await client.SendAsync(reqMsg);
            return response;
        }

        //if you are not getting the access code, please debug here and verify your response.
        public string GetAccessCode()
        {
            string result = string.Empty;
            Task<HttpResponseMessage> task = GetAuthorizeResponse(credential);
            HttpResponseMessage response = task.Result;
            for (int i = 0; i < response.Headers.Count; i++)
            {
                if (response.Headers[i].ToString().Contains("auth/callback?"))
                {
                    string[] stringArray = response.Headers[i].ToString().Split('?');
                    foreach (string value in stringArray)
                    {
                        if (value.Contains("code"))
                            return value.Replace("code=", string.Empty);
                    }
                }
            }
            return string.Empty;
        }

        private async Task<HttpResponseMessage> GetAuthorizeResponse(Credential credential)
        {
            string reqUrl = credential.authURL + "/adfs/oauth2/authorize?response_type=code&client_id=" + credential.ClientId + "&resource=" + credential.Resource + "&redirect_uri=" + credential.RedirectUri;
            var client = new HttpClient();
            //var request = new HttpRequestMessage(HttpMethod.Get, reqUrl);

            //request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            //request.AddHeader("cache-control", "no-cache");
            //request.AddParameter("undefined", "UserName=" + credential.userName + "&Password=" + credential.password + "&AuthMethod=FormsAuthentication", ParameterType.RequestBody);
            return await client.GetAsync(reqUrl);
        }
    }
}
