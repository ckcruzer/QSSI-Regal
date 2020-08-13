using BSP.PowerHouse.DynamicsGP.Integration.Extensions;
using Microsoft.Dynamics.GP.eConnect.Serialization;
using System;

namespace BSP.PowerHouse.DynamicsGP.Integration.eConnectObjects
{
    public class SMTransactionBatch : IEConnectObject
    {
        #region property
        public string BatchNumber { get; set; }
        public string BatchSource { get; set; }
        public DateTime BatchDate { get; set; }
        public GpSeries GpSeries { get; set; }
        public GpOrigin GpOrigin { get; set; }
        public string BatchCheckBookId { get; set; }
        public string BatchComments { get; set; }
        #endregion
        public SMTransactionBatch()
        {
            BatchDate = DateTime.Today;
        }
        public SMTransactionBatch(string batchNumber, string batchSource, DateTime batchDate, GpSeries gpSeries, GpOrigin gpOrigin, string batchCheckBookId, string batchComments)
        {
            this.BatchNumber = batchNumber;
            this.BatchSource = batchSource;
            this.BatchDate = batchDate;
            this.GpSeries = gpSeries;
            this.GpOrigin = gpOrigin;
            this.BatchCheckBookId = batchCheckBookId;
            this.BatchComments = batchComments;
        }
        public virtual string GetXmlSerializedObject()
        {
            // Instantiate an eConnectType schema object
            eConnectType eConnect = new eConnectType();
            // Instantiate a schema object
            SMTransactionBatchType batchType = new SMTransactionBatchType();
            // Populate the schema
            batchType.taCreateUpdateBatchHeaderRcd = GetBatch();
            // Populate the eConnectType object with the schema objects
            eConnect.SMTransactionBatchType = new SMTransactionBatchType[] { batchType };

            return eConnect.SerializeObject();
        }
        public virtual taCreateUpdateBatchHeaderRcd GetBatch()
        {
            taCreateUpdateBatchHeaderRcd batch = new taCreateUpdateBatchHeaderRcd();
            batch.BACHNUMB = BatchNumber;
            if (BatchDate != null)
            {
                batch.GLPOSTDT = BatchDate.GpFormattedDate();
            }
            batch.BCHSOURC = BatchSource;
            batch.SERIES = (int)GpSeries;
            batch.ORIGIN = (int)GpOrigin;
            batch.CHEKBKID = BatchCheckBookId;
            batch.BCHCOMNT = BatchComments;

            return batch;
        }
    }
}
