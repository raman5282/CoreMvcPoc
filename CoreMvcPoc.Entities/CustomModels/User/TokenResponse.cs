using System;
using Newtonsoft.Json;
namespace CoreMvcPoc.Entities{

    public class TokenResponse
    {
        [JsonProperty("token")]
         public string Token { get; set; }    
         public int Role { get; set; }
    }

}