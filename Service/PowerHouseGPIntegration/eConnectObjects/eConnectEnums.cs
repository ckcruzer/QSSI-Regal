namespace BSP.PowerHouse.DynamicsGP.Integration.eConnectObjects
{
    public enum GpSeries : int
    {
        All = 1,
        Financial = 2,
        Sales = 3,
        Purchasing = 4,
        Inventory = 5,
        Payroll = 6,
        Project = 7
    }
    public enum GpOrigin : int
    {
        TransactionEntry = 1,
        TransferOrComputerCheckOrCashReceipt = 2,
        ManualPayment = 3
    }
    public enum GpSopType : short
    {
        Quote = 1,
        Order = 2,
        Invoice = 3,
        Return = 4,
        BackOrder = 5,
        Fulfillment = 6
    }
    public enum GpReceiptType : short
    {
        Shipment = 1,
        ShipmentAndInvoice = 3
    }
    public enum GPIvTrxType : short
    {
        Adjustment = 1,
        Variance = 2,
        Transfer = 3
    }
    public enum GPIvAdjustmentType : short
    {
        Increase = 0,
        Decrease = 1
    }
}

