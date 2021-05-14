using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoolParking.BL.Interfaces;
using CoolParking.BL.Models;
using CoolParking.BL.Services;
using CoolParking.WebAPI.QueryModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CoolParking.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {

        private IParkingService parkingService;
        public TransactionsController(IParkingService service)
        {
            parkingService = service;
        }


        //GET api/transactions/last
        [HttpGet("last")]
        public ActionResult GetLast() //<TransactionInfo[]>
        {
            try
            {
                return Ok(parkingService.GetLastParkingTransactions());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //GET api/transactions/all (только транзакции с лог файла)
        [HttpGet("all")]
        public ActionResult GetAll() //<string>
        {
            Console.WriteLine(Settings.logFilePath);
            try
            {
                return Ok(parkingService.ReadFromLog());
            }
            catch (InvalidOperationException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        //PUT api/transactions/topUpVehicle
        [HttpPut("topUpVehicle")]
        public ActionResult TopUpVeh([FromBody] TopUpVehicleBody topUpVehicle) //<VehicleBody>
        {
            try
            {
                Vehicle vehicle = parkingService.FindVehicleById((topUpVehicle.Id));
                parkingService.TopUpVehicle(topUpVehicle.Id, topUpVehicle.Sum);
                vehicle = parkingService.FindVehicleById(topUpVehicle.Id);
                VehicleBody vb = new VehicleBody{VehicleType = (int)vehicle.VehicleType, Balance = vehicle.Balance, Id = vehicle.Id};
                return Ok(vb);
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
