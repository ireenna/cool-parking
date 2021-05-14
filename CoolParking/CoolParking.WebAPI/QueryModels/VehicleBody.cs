using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoolParking.BL.Models;
using Newtonsoft.Json;

namespace CoolParking.WebAPI.QueryModels
{
    public class VehicleBody
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("vehicleType")]
        public int VehicleType { get; set; }
        [JsonProperty("balance")]
        public decimal Balance { get; set; }
    }
}
