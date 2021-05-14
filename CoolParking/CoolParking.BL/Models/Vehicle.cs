using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CoolParking.BL.Models
{
    

public class Vehicle
{
    public string Id { get;}
    public static readonly Regex reg = new (@"^[A-Z]{2}-[0-9]{4}-[aA-Z]{2}$");
    public VehicleType VehicleType { get;}
    public decimal Balance { get; internal set; }

    

    public Vehicle(string id, VehicleType vehicleType, decimal balance)
    {
        if (reg.IsMatch(id) && balance >= 0 && Enum.IsDefined(typeof(VehicleType),vehicleType))
        {
            Id = id;
            VehicleType = vehicleType;
            Balance = balance;
            
        }
        else
        {
            throw new ArgumentException("Invalid body of the vehicle.");
        }

    }

    public static Vehicle CreateRandomVehicle()
    {
        var v = Enum.GetValues(typeof(VehicleType));
        var randomType = (VehicleType)v.GetValue(new Random().Next(v.Length));
        
        return new Vehicle(GenerateRandomRegistrationPlateNumber(), randomType, 0);
    }

    public static string GenerateRandomRegistrationPlateNumber()
    {
        Random rand = new Random();
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < 2; i++)
        {
            char randChar = (char)rand.Next('A','Z');
            sb.Append(randChar);
        }

        sb.Append("-");

        for (int i = 0; i < 4; i++)
        {
            int randNumb = rand.Next(0, 9);
            sb.Append(randNumb);
        }

        sb.Append("-");

        for (int i = 0; i < 2; i++)
        {
            char randChar = (char)rand.Next('A', 'Z');
            sb.Append(randChar);
        }
        // Checks if the Id is unique for the Parking
        foreach (var veh in Parking.GetInstance().Vehicles)
        {
            if (veh.Id == sb.ToString())
            {
                GenerateRandomRegistrationPlateNumber();
                break;
            }
        }

        return sb.ToString();
    }
}

}
// TODO: implement class Vehicle.
//       +Properties: Id (string), VehicleType (VehicleType), Balance (decimal).
//       +The format of the identifier is explained in the description of the home task.
//       +Id and VehicleType should not be able for changing.
//       +The Balance should be able to change only in the CoolParking.BL project.
//       +The type of constructor is shown in the tests and the constructor(?) should have a validation, which also is clear from the tests.
//       +Static method GenerateRandomRegistrationPlateNumber should return a randomly generated unique(?) identifier.
