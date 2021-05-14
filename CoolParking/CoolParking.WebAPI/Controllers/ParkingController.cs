using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoolParking.BL.Interfaces;
using CoolParking.BL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoolParking.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingController : ControllerBase
    {
        private IParkingService parkingService;
        public ParkingController(IParkingService service)
        {
            parkingService = service;
        }


        //GET api/parking/balance
        [HttpGet("balance")]
        public ActionResult GetBalance() //<decimal>
        {
            try
            {
                return Ok(parkingService.GetBalance());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //GET api/parking/capacity
        [HttpGet("capacity")]
        public ActionResult GetCapacity() //<int>
        {
            try
            {
                return Ok(parkingService.GetCapacity());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //GET api/parking/freePlaces
        [HttpGet("freePlaces")]
        public ActionResult GetFreePlaces() //<int>
        {
            try
            {
                return Ok(parkingService.GetFreePlaces());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
