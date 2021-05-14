using System;

namespace CoolParking.BL.Models
{
    public struct TransactionInfo
    {
        public string VehicleId { get; }
        public decimal Sum { get; }
        public DateTime TransactionDate { get; }
        

        public TransactionInfo(Vehicle vehicle, decimal sum)
        {
            TransactionDate = DateTime.Now;
            VehicleId = vehicle.Id;
            Sum = sum;
        }

        public override string ToString()
        {
            return $"{TransactionDate} : {Sum} money were withdrawn from vehicle with Id='{VehicleId}'.\n";
        }
    }

}
// TODO: implement struct TransactionInfo.
//       +Necessarily implement the Sum property (decimal) - is used in tests.
//       Other implementation details are up to you, they just have to meet the requirements of the homework.
