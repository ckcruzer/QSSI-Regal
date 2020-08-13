namespace BSP.DynamicsGP.PowerHouse.Models
{
    public class ShippingMethod
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string ShippingMethodDescription
        {
            get { return string.Format("{0}: {1}", this.Id.Trim(), this.Description.Trim()); }
        }
                
    }
}
