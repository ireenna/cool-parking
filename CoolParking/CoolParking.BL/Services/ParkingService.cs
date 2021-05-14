using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using CoolParking.BL.Interfaces;
using CoolParking.BL.Models;

namespace CoolParking.BL.Services
{
    public class ParkingService : IParkingService
    {
        private Parking parking= Parking.GetInstance();
        readonly ITimerService withdrawTimer;
        readonly ITimerService logTimer;
        readonly ILogService logService;
        List<TransactionInfo> transactionInfo;

        public ParkingService()
        {
            
            {
                logService = new LogService(Settings.logFilePath);
                withdrawTimer = new TimerService(Settings.PaymentTime, WithdrawTimer_Elapsed);
                logTimer = new TimerService(Settings.LogTime, LogTimer_Elapsed);
                transactionInfo = new List<TransactionInfo>();
                new ParkingService(withdrawTimer, logTimer, logService);
            }
        }
        public ParkingService(ITimerService _withdrawTimer, ITimerService _logTimer, ILogService _logService)
        {
            
            
                transactionInfo = new List<TransactionInfo>();
                withdrawTimer = _withdrawTimer;
                logTimer = _logTimer;
                logService = _logService;
                withdrawTimer.Start();
                withdrawTimer.Elapsed += WithdrawTimer_Elapsed;
                logTimer.Start();
                logTimer.Elapsed += LogTimer_Elapsed;
            
        }
        public void TopUpParkingBalance(decimal sum)
        {
            parking.Balance += sum;
        }
        private void WithdrawTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (Vehicle vehicle in parking.Vehicles)
            {
                var withdrawnSum = Math.Round(CalculateWithdrawnFromVehicle(vehicle),5);
                
                if (vehicle.Balance > -900000000)
                    vehicle.Balance -= withdrawnSum;

                if (parking.Balance < 900000000)
                    TopUpParkingBalance(withdrawnSum);
                
                TransactionInfo transaction = new TransactionInfo(vehicle, withdrawnSum);
                transactionInfo.Add(transaction);
            }
            
        }
        
        private void LogTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string sb = "";
            transactionInfo.ToList().ForEach(item => sb += item.ToString());
            try
            {
                logService.Write(sb);
                transactionInfo.Clear();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }


        public decimal CalculateWithdrawnFromVehicle(Vehicle vehicle)
        {
            decimal withdrawnSum = Settings.Tariff[vehicle.VehicleType];
            if (vehicle.Balance >= withdrawnSum)
            {
                return withdrawnSum;
            }
            if (vehicle.Balance < withdrawnSum && vehicle.Balance >= 0)
            {
                return (withdrawnSum - vehicle.Balance) * Settings.KoefFine + vehicle.Balance;
            }
            return (Math.Abs(vehicle.Balance)) * Settings.KoefFine;
        }




        public decimal GetBalance()
        {
            return parking.Balance;
        }

        public int GetCapacity()
        {
            return parking.Vehicles.Capacity;
        }

        public int GetFreePlaces()
        {
            return GetCapacity() - parking.Vehicles.Count;
        }

        public ReadOnlyCollection<Vehicle> GetVehicles()
        {
            return parking.Vehicles.AsReadOnly();
        }

        public void AddVehicle(Vehicle vehicle)
        {
                if (GetFreePlaces() > 0)
                {

                    foreach (Vehicle veh in Parking.GetInstance().Vehicles)
                    {
                        if (vehicle.Id == veh.Id)
                        {
                            throw new ArgumentException("The vehicle with the same id already exists.");
                        }
                    }
                    parking.Vehicles.Add(vehicle);
                    
                }
                else
                {
                    throw new InvalidOperationException("There is no free place!");
                }
                Console.WriteLine("The vehicle with id = {0} has been added!!!", vehicle.Id);
        }

        public Vehicle FindVehicleById(string id)
        {
            if (!Vehicle.reg.IsMatch(id))
                throw new InvalidDataException("Invalid format of id.");

            foreach (Vehicle vehicle in Parking.GetInstance().Vehicles)
            {
                if (vehicle.Id == id)
                {
                    return vehicle;
                }
            }
            
            throw new ArgumentException("There are no such vehicles.");
        }

        public void RemoveVehicle(string vehicleId)
        {
            try
            {

                var vehicle = FindVehicleById(vehicleId);
                if (vehicle.Balance >= 0)
                {
                    Parking.GetInstance().Vehicles.Remove(vehicle);
                    Console.WriteLine("The vehicle {0} has been successfully deleted!", vehicle.Id);
                }
                else
                {
                    throw new InvalidOperationException("The vehicle can't be removed, because the balance is <0.");
                }
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException("{0}",e);
            }
            
            
        }

        public void TopUpVehicle(string vehicleId, decimal sum)
        {
            var vehicle = FindVehicleById(vehicleId);
            if (sum < 0)
            {
                throw new ArgumentException("The sum shouldn't be < 0");
            }
            
            vehicle.Balance += sum;
            Console.WriteLine("'{0}' was successfully topped up on ${1}.", vehicleId, sum);
        }

        public void Dispose()
        {
            logTimer.Dispose();
            withdrawTimer.Dispose();
            parking.Vehicles.Clear();
            parking.Balance = 0;
        }

        public TransactionInfo[] GetLastParkingTransactions()
        {
            return transactionInfo?.ToArray();
        }

        public string ReadFromLog()
        {
            return logService.Read();
        }
    }
}


// TODO: implement the ParkingService class from the IParkingService interface.
//       +For try to add a vehicle on full parking InvalidOperationException should be thrown.
//       +For try to remove vehicle with a negative balance (debt) InvalidOperationException should be thrown.
//       Other validation rules and constructor format went from tests.
//       Other implementation details are up to you, they just have to match the interface requirements
//       and tests, for example, in ParkingServiceTests you can find the necessary constructor format and validation rules.
