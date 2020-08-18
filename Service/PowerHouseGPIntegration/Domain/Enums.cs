
namespace BSP.PowerHouse.DynamicsGP.Integration.Domain
{
    public enum BatchFrequency : int
    {
        Once = 1,
        Hourly = 2,
        Daily = 3,
        Weekly = 4,
        Monthly = 5
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




