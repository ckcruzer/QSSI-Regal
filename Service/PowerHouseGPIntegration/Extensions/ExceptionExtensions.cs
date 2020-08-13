using System;
using System.Text;

namespace BSP.PowerHouse.DynamicsGP.Integration.Extensions
{
    public static class ExceptionExtensions
    {
        public static string ToStringWithInnerException(
           this Exception ex)
        {
            StringBuilder description = new StringBuilder();
            description.AppendLine(ex.ToString());

            if (ex.InnerException != null)
            {
                description.Append("Inner exception ---> ");
                description.AppendLine(ex.InnerException.ToString());
            }

            description.Append("Stack trace ---> ");
            description.AppendLine(ex.StackTrace);

            return description.ToString();
        }
    }
}
