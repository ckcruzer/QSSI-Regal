namespace BSP.DynamicsGP.PowerHouse.Models
{
    public class Customer
    {
        public string CustomerNumber{ get; set; }
        public string CustomerName { get; set; }
        public decimal NoteIndex{ get; set; }
        public string Comment1{ get; set; }
        public string Comment2{ get; set; }
        public string PrimaryShipToAddressCode { get; set; }

        public short CustomerPriority { get; set; }

        //RIC 20210104: Added per Margie's request
        public string CustomerClass { get; set; }
    }
}
