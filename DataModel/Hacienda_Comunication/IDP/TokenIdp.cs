using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Hacienda_Comunication.IDP
{
    public class TokenIdp
    {
        public string access_token { set; get; }
        public int expires_in { set; get; }
        public int refresh_expires_in { set; get; }
        public string token_type { set; get; }
        public string id_token { set; get; }
        [JsonProperty("not-before-policy")]
        public int not_before_policy { set; get; }
        public string session_state { set; get; }

        private DateTime creation;
        public TokenIdp()
        {
            creation = DateTime.Now;
        }

        public bool isTokenValid
        {
            get
            {
                return DateTime.Now > creation.AddSeconds(300) ? false : true;
            }
        }
    }


    public class TokenRequest
    {
        public string client_id { set; get; }
        public string client_secret { set; get; }
        public string grant_type { set; get; }
        public string username { set; get; }
        public string password { set; get; }
        public string scope { set; get; }
    }
}
