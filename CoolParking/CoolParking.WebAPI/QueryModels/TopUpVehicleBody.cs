using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CoolParking.WebAPI.QueryModels
{
    public class TopUpVehicleBody
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("sum")]
        public decimal Sum { get; set; }
    }
}
