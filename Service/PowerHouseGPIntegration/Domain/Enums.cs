
namespace BSP.PowerHouse.DynamicsGP.Integration.Domain
{
    public enum BatchFrequency : int
    {
        Once = 0,
        Hourly = 1,
        Daily = 2,
        Weekly = 3,
        Monthly = 4
    }
    public enum QuantityShortageOption : int
    {
        SellBalance = 1,
        OverrideShortage = 2,
        BackOrderAll = 3,
        BackOrderBalance = 4,
        CancelAll = 5,
        CancelBalance = 6
    }
    public enum PriceUnitOfMeasure : int
    {
        Selling = 1,
        Base = 2
    }
    public enum eConnectDocumentType
    {
        Customer,
        Item,
        SalesTransaction
    }
}




