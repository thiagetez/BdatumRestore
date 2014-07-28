using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace BdatumRestore.Model
{
    /// <summary>
    /// Keys para fazer a autenticação
    /// </summary>
    class AuthenticationProperties
    {
        [JsonProperty(PropertyName = "node")]
        public string NodeKey { get; set; }
        [JsonProperty(PropertyName = "partner")]
        public string PartnerKey { get; set; }
    }
}
