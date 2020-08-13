using BSP.PowerHouse.DynamicsGP.Integration.eConnectObjects;
using Microsoft.Dynamics.GP.eConnect.Serialization;
using System.IO;
using System.Xml.Serialization;

namespace BSP.PowerHouse.DynamicsGP.Integration.Extensions
{
    public static class eConnectExtensions
    {


        public static string SerializeObject(this eConnectType eConnect)
        {
            if (eConnect == null)
                return null;

            // Create an XML serializer object
            XmlSerializer serializer = new XmlSerializer(eConnect.GetType());

            // Serialize the eConnectType object to a string using the StringWriter.
            string serializedXML = string.Empty;
            using (StringWriter stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, eConnect);
                serializedXML = stringWriter.ToString();
                stringWriter.Close();
            }
            return serializedXML;
        }

        
    }
}
