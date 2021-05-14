using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CoolParking.BL.Models;
using CoolParking.BL.Services;
using CoolParking.BL.Tests;
using CoolParking.WebAPI.QueryModels;
using Newtonsoft.Json;
using Xunit;
using System.Linq;

namespace ParkingApp
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string localpath = $"https://localhost:44307/api/";
        static async Task Main()
        {

            async Task<bool> SelectVehicle()
            {
                if (await GetVehiclesCount() == 0)
                {
                    Console.WriteLine("There are no vehicles.");
                    return false;
                }
                Console.WriteLine("Select the vehicle: ");
                List<VehicleBody> list_veh = await GetVehicles();
                list_veh.ForEach(veh => Console.WriteLine("'{0}' {1} Balance: {2}", veh.Id, (VehicleType)veh.VehicleType, veh.Balance));
                return true;
            }
            
            do
            {
                try
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Parking app.");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("-----------------------------------------------");
                    Console.WriteLine(
                        "Select the action:" +
                        "\n1) Get parking balance.\n2) Get free places and capacity.\n3) Get the vehicles count." +
                        "\n4) Add the vehicle.\n5) Remove the vehicle.\n6) Show all the vehicles on the parking." +
                        "\n7) Top up the vehicle.\n8) Show transaction history for this period." +
                        "\n9) Show transaction history.\n10) Show earned money for this period.");
                    Console.WriteLine("-----------------------------------------------");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Select: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    int operation = Convert.ToInt32(Console.ReadLine());

                    switch (operation)
                    {
                        case 1:
                            Console.Write("Parking balance: $" + await GetBalance());
                            break;
                        case 2:
                            Console.WriteLine("Free places: " + await GetFreePlaces() + "\\" + await GetCapacity());
                            break;
                        case 3:
                            Console.WriteLine("Vehicles count: " + await GetVehiclesCount());
                            break;
                        case 4:
                        {
                            Console.WriteLine("Create a vehicle: 1) manually; 2) random.");
                            int creating = Convert.ToInt32(Console.ReadLine());
                            Vehicle vehicle;
                            switch (creating)
                            {
                                case 1:
                                {

                                    Console.Write("Write the Id (it should be such as XX-0000-XX format): ");
                                    string id = Console.ReadLine();
                                    Console.WriteLine(
                                        "Choose the type of vehicle: 1-PassengerCar, 2-Truck, 3-Bus, 4-Motorcycle.");
                                    int typeid = Convert.ToInt32(Console.ReadLine()) - 1;
                                    var v = Enum.GetValues(typeof(VehicleType));
                                    var vehicleType = (VehicleType) v.GetValue(typeid);
                                    Console.Write("Vehicle balance: ");
                                    decimal money = Convert.ToDecimal(Console.ReadLine());
                                    vehicle = new Vehicle(id, vehicleType, money);
                                    var addedVeh = await AddVehicle(vehicle);
                                    Console.WriteLine(
                                        $"The {addedVeh.VehicleType} (Id = '{addedVeh.Id}', balance = {addedVeh.Balance}) was added successfully!");

                                    break;
                                }
                                case 2:
                                {
                                    vehicle = Vehicle.CreateRandomVehicle();
                                    var addedVeh = await AddVehicle(vehicle);
                                    if (addedVeh != null)
                                        Console.WriteLine(
                                            $"The {addedVeh.VehicleType} (Id = '{addedVeh.Id}', balance = {addedVeh.Balance}) was added successfully!");
                                    break;
                                }
                                default:
                                    Console.WriteLine("There is no such operation. Try again.");
                                    break;

                            }

                            break;
                        }
                        case 5:
                        {
                            if (await SelectVehicle())
                            {
                                Console.Write("Vehicle Id: ");
                                string id = Console.ReadLine();
                                var sss = await GetVehicleById(id);
                                if (sss != null)
                                    await RemoveVehicle(id);
                                else
                                    Console.WriteLine("There is no such vehicle.");
                            }

                            break;
                        }
                        case 6:
                        {
                            await SelectVehicle();
                            break;
                        }
                        case 7:
                        {
                            if (await SelectVehicle())
                            {
                                Console.Write("Vehicle Id: ");
                                string id = Console.ReadLine();
                                Console.Write("Write the sum: ");
                                decimal money = Convert.ToDecimal(Console.ReadLine());
                                var t_veh = new TopUpVehicleBody {Id = id, Sum = money};
                                var veh = await TopUpVehicle(t_veh);
                                if (veh != null)
                                {
                                    Console.WriteLine("The vehicle was successfully topped up!");
                                }
                            }

                            break;
                        }
                        case 8:
                            TransactionInfoBody[] arr = await GetLastParkingTransactions();
                            if (arr.Length == 0)
                                Console.WriteLine("There is nothing to show.");

                            arr.ToList().ForEach(item => Console.Write(item.ToString()));
                            break;

                        case 9:
                            Console.WriteLine(await ReadFromLog());
                            break;

                        case 10:
                            decimal wholesum = 0;
                            var info = await GetLastParkingTransactions();

                            info.ToList().ForEach(item => wholesum += item.Sum);

                            Console.WriteLine("Earned money for this period: ${0:N2}", wholesum);
                            break;

                        default:
                            Console.WriteLine("There is no such operation.");
                            break;

                    }


                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("You wrote incorrect data. Please, try again.");
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (FormatException)
                {
                    Console.WriteLine("You wrote incorrect data. Try one more time.");
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Console.ReadKey();
            } while (true);
        }

        private static async Task<decimal> GetBalance()
        {
            HttpResponseMessage response = await client.GetAsync(localpath+"parking/balance");
            string balance = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<decimal>(balance);
        }
        private static async Task<int> GetFreePlaces()
        {
            HttpResponseMessage response = await client.GetAsync(localpath+ "parking/freePlaces");
            string freePl = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<int>(freePl);
        }

        private static async Task<int> GetCapacity()
        {
            HttpResponseMessage response = await client.GetAsync(localpath+"parking/capacity");
            string capacity = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<int>(capacity);
        }
        private static async Task<int> GetVehiclesCount()
        {
            return (await GetVehicles()).Count;
        }

        private static async Task<List<VehicleBody>> GetVehicles()
        {
            HttpResponseMessage response = await client.GetAsync(localpath + "vehicles");
            string vehs = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<VehicleBody>>(vehs);
        }
        private static async Task<Vehicle> AddVehicle(Vehicle vehicle)
        {
            var answer = await GetVehicleById(vehicle.Id);
            if (answer != null)
            {
                Console.WriteLine("There already exists vehicle with such Id.");
                return null;
            }

            var jsonVehicle = JsonConvert.SerializeObject(vehicle);
            var data = new StringContent(jsonVehicle, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(localpath+"vehicles", data);
            
            if (response.StatusCode == HttpStatusCode.Created)
                return JsonConvert.DeserializeObject<Vehicle>(await response.Content.ReadAsStringAsync());

            Console.WriteLine("There is no free place.");
            return null;

        }
        private static async Task RemoveVehicle(string id)
        {
            HttpResponseMessage response = await client.DeleteAsync(localpath+$"vehicles/{id}");
            var status = response.StatusCode;
            if (status == HttpStatusCode.NoContent)
            {
                Console.WriteLine("The vehicle was removed successfully.");
            }
            else if (status == HttpStatusCode.NotFound)
            {
                Console.WriteLine("There is no such vehicle.");
            }
            else if (status == HttpStatusCode.BadRequest)
            {
                Console.WriteLine("You can't delete vehicle with balance <0.");
            }

        }

        private static async Task<VehicleBody> TopUpVehicle(TopUpVehicleBody vehicle)
        {
            var answer = await GetVehicleById(vehicle.Id);
            if (answer == null)
            {
                Console.WriteLine("The vehicle with such id doesn't exist.");
                return null;
            }
            var jsonVehicle = JsonConvert.SerializeObject(vehicle);
            var data = new StringContent(jsonVehicle, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(localpath+"transactions/topUpVehicle", data);
            if(response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<VehicleBody>(await response.Content.ReadAsStringAsync());
            if (response.StatusCode == HttpStatusCode.NotFound)
                Console.WriteLine("The sum should be > 0.");

            if (response.StatusCode == HttpStatusCode.BadRequest)
                Console.WriteLine("Incorrect data. Please, try again.");
            
            return null;
        }

        private static async Task<VehicleBody> GetVehicleById(string id)
        {
            var response = await client.GetAsync(localpath+$"vehicles/{id}");
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new FormatException();
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            return JsonConvert.DeserializeObject<VehicleBody>(await response.Content.ReadAsStringAsync());


        }

        private static async Task<TransactionInfoBody[]> GetLastParkingTransactions()
        {
            HttpResponseMessage response = await client.GetAsync(localpath+"transactions/last");
            string trinfo = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TransactionInfoBody[]>(trinfo);
        }

        private static async Task<string> ReadFromLog()
        {
            HttpResponseMessage response = await client.GetAsync(localpath+"transactions/all");
            string trinfo = await response.Content.ReadAsStringAsync();
            return trinfo;
        }
    }

}
