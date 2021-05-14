using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CoolParking.WebAPI.QueryModels
{
    public class TransactionInfoBody
    {
        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }
        [JsonProperty("sum")]
        public decimal Sum { get; set; }
        [JsonProperty("transactionDate")]
        public DateTime TransactionDate { get; set; }

        public override string ToString()
        {
            return $"{TransactionDate} : {Sum} money were withdrawn from vehicle with Id='{VehicleId}'.\n";
        }
    }
}
