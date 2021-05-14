using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;

namespace CoolParking.BL.Models
{
    public static class Settings
    {
        public static readonly decimal InitialBalance = 0;
        public static readonly int Capacity = 10;
        public static readonly int PaymentTime = 5000;
        public static readonly int LogTime = 60000;
        public static readonly decimal KoefFine = 2.5m;
        public static readonly string logFilePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\Transactions.log";

        public static IReadOnlyDictionary<VehicleType, decimal> Tariff = new Dictionary<VehicleType, decimal>()
        {

            {VehicleType.PassengerCar, 2},
            {VehicleType.Truck, 5},
            {VehicleType.Bus, 3.5m},
            {VehicleType.Motorcycle, 1}
        };
    }
}
// TODO: implement class Settings.
//       Implementation details are up to you, they just have to meet the requirements of the home task.
