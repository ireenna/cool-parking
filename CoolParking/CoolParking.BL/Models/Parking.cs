using System.Collections.Generic;

namespace CoolParking.BL.Models
{
    public class Parking
    {
        private static Parking instance;
        public decimal Balance { get; set; }

        public List<Vehicle> Vehicles;

        private Parking()
        {
            Balance = Settings.InitialBalance;
            Vehicles = new List<Vehicle>(Settings.Capacity);
        }

        public static Parking GetInstance()
        {
            if (instance == null)
                instance = new Parking();
            return instance;
        }

        
    }

}

// TODO: implement class Parking.
//       Implementation details are up to you, they just have to meet the requirements 
//       of the home task and be consistent with other classes and tests.
