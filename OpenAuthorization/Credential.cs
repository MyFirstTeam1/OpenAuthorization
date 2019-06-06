using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAuthorization
{
    public class Credential
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string authURL { get; set; }

        public string RedirectUri { get; set; }

        public string Resource { get; set; }
    }
}
