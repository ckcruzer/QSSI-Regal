using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSP.DynamicsGP.PowerHouse.Extensions
{
    public static class DoubleExtension
    {
        public static double RoundUp(this double input, int places)
        {
            double multiplier = Math.Pow(10, Convert.ToDouble(places));
            return Math.Ceiling(input * multiplier) / multiplier;
        }
    }
}
