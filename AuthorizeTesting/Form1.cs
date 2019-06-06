using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AuthorizeTesting
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string Authority { get { return txtAuthority.Text; } }
        string ResourceUri { get { return txtResourceUri.Text; } }
        string ClientId { get { return txtClientId.Text; } }
        string RedirectUri { get { return txtRedirectUri.Text; } }

        private void btnAuthorize_Click(object sender, EventArgs e)
        {
            AuthenticationContext ac = new AuthenticationContext(Authority, false);
            var taskAcquireToken = ac.AcquireTokenAsync(ResourceUri, ClientId, new Uri(RedirectUri),null);
            taskAcquireToken.Start();
            taskAcquireToken.Wait();
            AuthenticationResult ar = taskAcquireToken.Result;
            string authHeader = ar.CreateAuthorizationHeader();
            UserInfo ui = ar.UserInfo;
            SendRequest(authHeader);
        }

        void SendRequest(string authHeader)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "");
            request.Headers.TryAddWithoutValidation("Authorization", authHeader);
            var taskSend = client.SendAsync(request);
            taskSend.Wait();
            HttpResponseMessage response = taskSend.Result;
            var taskRead = response.Content.ReadAsStringAsync();
            taskRead.Wait();
            string responseString = taskRead.Result;
        }
    }
}
