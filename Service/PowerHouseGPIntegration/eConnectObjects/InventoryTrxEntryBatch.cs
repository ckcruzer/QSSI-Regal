using BSP.PowerHouse.DynamicsGP.Integration.Configuration;
using BSP.PowerHouse.DynamicsGP.Integration.Extensions;
using System;
using System.Globalization;

namespace BSP.PowerHouse.DynamicsGP.Integration.eConnectObjects
{
    public class InventoryTrxEntryBatch : SMTransactionBatch
    {
        private readonly DynamicsGpSettings _dynamicsGpSettings;
        public InventoryTrxEntryBatch(DynamicsGpSettings dynamicsGpSettings)
        {
            this._dynamicsGpSettings = dynamicsGpSettings;
            this.GpOrigin = GpOrigin.TransactionEntry;
            this.GpSeries = GpSeries.Inventory;
            this.BatchSource = "IV_Trxent";
            this.BatchCheckBookId = string.Empty;
            this.BatchComments = "Powerhouse Inventory Trx import";
            if (_dynamicsGpSettings != null)
            {
                DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
                string batchNumber = !string.IsNullOrWhiteSpace(_dynamicsGpSettings.ReceivingsTrxBatchId) ? _dynamicsGpSettings.InventoryTrxBatchId : "PHIV";
                if (!string.IsNullOrWhiteSpace(batchNumber) && _dynamicsGpSettings.BatchFrequency != Domain.BatchFrequency.Once)
                {
                    batchNumber += "-";
                }
                switch (_dynamicsGpSettings.BatchFrequency)
                {
                    case Domain.BatchFrequency.Once:
                        break;
                    case Domain.BatchFrequency.Hourly:
                        batchNumber += string.Format("{0:htt}", DateTime.Now);
                        break;
                    case Domain.BatchFrequency.Daily:
                        batchNumber += DateTime.Today.GpFormattedDate();
                        break;
                    case Domain.BatchFrequency.Weekly:
                        Calendar cal = dfi.Calendar;
                        batchNumber += string.Format("WK{0}", cal.GetWeekOfYear(DateTime.Today, dfi.CalendarWeekRule, dfi.FirstDayOfWeek));
                        break;
                    case Domain.BatchFrequency.Monthly:
                        batchNumber += DateTime.Today.ToString("MMM").ToUpper();
                        break;
                }
                this.BatchNumber = batchNumber.Truncate(15);
            }
            else
            {
                this.BatchNumber = DateTime.Today.GpFormattedDate();
            }
        }
    }
}
