using System;
using System.Collections.Generic;
using System.IO;
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
    public class VehiclesController : ControllerBase
    {

        private IParkingService parkingService;
        public VehiclesController(IParkingService service)
        {
            parkingService = service;
        }


        //GET api/vehicles
        [HttpGet]
        public ActionResult Get() //<List<VehicleBody>>
        {
            return Ok(parkingService.GetVehicles());
        }


        //GET api/vehicles/id
        [HttpGet("{id}")]
        public ActionResult GetById([FromRoute] string id) //<VehicleBody>
        {
            try
            {
                return Ok(parkingService.FindVehicleById(id));
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e) //+InvalidDataException
            {
                return BadRequest(e.Message);
            }
            
        }


        //POST api/vehicles
        [HttpPost]
        public ActionResult Post([FromBody] VehicleBody vehicleBody) //<VehicleBody>
        {
            try
            {
                Vehicle vehicle = new Vehicle(vehicleBody.Id, (VehicleType)vehicleBody.VehicleType, vehicleBody.Balance);
                parkingService.AddVehicle(vehicle);
                return Created("Post", vehicleBody);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //DELETE api/vehicles/id (id - vehicle id of format “AA-0001-AA”)
        [HttpDelete("{id}")]
        public ActionResult DeleteById([FromRoute] string id)
        {
            try
            {
                parkingService.RemoveVehicle(id);
                return NoContent();
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e) //+InvalidOperationException
            {
                return BadRequest(e.Message);
            }
        }
    }
}
