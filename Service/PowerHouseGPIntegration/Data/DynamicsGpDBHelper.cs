using BSP.PowerHouse.DynamicsGP.Integration.Configuration;
using BSP.PowerHouse.DynamicsGP.Integration.Domain;
using BSP.PowerHouse.DynamicsGP.Integration.Model;
using BSP.PowerHouse.DynamicsGP.Integration.PowerHouseWS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace BSP.PowerHouse.DynamicsGP.Integration.Data
{
    public static class DynamicsGpDB
    {
        #region SQL statements

        private static string SQL_GET_POWERHOUSE_SETTINGS = "zDP_BSP_Powerhouse_SETPSS_1";
        private static string SQL_GET_SOP_HEADER = "zDP_SOP10100SS_1";
        private static string SQL_GET_SOP_LINE = "zDP_SOP10200SS_1";
        private static string SQL_UPDATE_EDI = "BSP_SopUpdateEDIInsert";
        private static string SQL_UPDATE_EDI_ESI40500 = "BSP_SopUpdateEDIInsert_ESI40500";
        private static string SQL_UPDATE_ASN_MANAGER = "sp_UpdateASNManager";
        private static string SQL_FULFILLMENT_POST = "BSP_SopFulfillmentPost";
        private static string SQL_GET_CUSTOMER = "zDP_RM00101SS_1";
        private static string SQL_GET_CUSTOMER_BATCH = "zDP_BSP_Powerhouse_CustBaSS_1";

        #endregion

        #region SQL Parameters

        private static string PARM_CMPANYID = "@CMPANYID";
        private static string PARM_SOPNUMBE = "@SOPNUMBE";
        private static string PARM_SOPTYPE = "@SOPTYPE";
        private static string PARM_CMPNTSEQ = "@CMPNTSEQ";
        private static string PARM_LNITMSEQ = "@LNITMSEQ";

        private static string PARM_SOPNUMBE_ECONNECT = "@I_vSOPNUMBE";
        private static string PARM_SOPTYPE_ECONNECT = "@I_vSOPTYPE";
        private static string PARM_LNITMSEQ_ECONNECT = "@I_vLNITMSEQ";
        private static string PARM_VS_SEQUENCE_NUMBER = "@I_vVS_Sequence_Number";
        private static string PARM_VS_CARTON_ASNSTR = "@I_vVS_Carton_ASNSTR";
        private static string PARM_VS_PALLET_ASNSTR = "@I_vVS_Pallet_ASNSTR";
        private static string PARM_VS_PRODUCT_CODE = "@I_vVS_Product_Code";
        private static string PARM_VS_QTY_PACKEDASN = "@I_vVS_QTY_PackedASN";
        private static string PARM_VS_QTY_PACKEDTHISSLIP = "@I_vVS_QTY_PackedThisSlip";
        private static string PARM_VS_CARTON_WEIGHT = "@I_vVS_Carton_Weight";
        private static string PARM_VS_TOTAL_WEIGHT = "@I_vVS_Total_Weight";
        private static string PARM_VS_DATE_SHIPPED = "@I_vVS_Date_Shipped";
        private static string PARM_VS_SHIPMENT_NUMBER = "@I_vVS_Shipment_Number";
        private static string PARM_VS_SHIP_CARRIER_USED = "@I_vVS_Ship_Carrier_Used";
        private static string PARM_VS_PICKUP_NUMBER = "@I_vVS_Pickup_Number";
        private static string PARM_VS_TT_NUMBER = "@I_vVS_TT_Number";
        private static string PARM_VS_SEAL_NUMBER = "@I_vVS_Seal_Number";
        private static string PARM_VS_TRAILER_NUMBER = "@I_vVS_Trailer_Number";
        private static string PARM_VS_LOAD_NUMBER = "@I_vVS_Load_Number";
        private static string PARM_VS_PRO_NUMBER = "@I_vVS_PRO_Number";
        private static string PARM_VS_SCC18 = "@I_vVS_SCC18";
        private static string PARM_VS_SCAC = "@I_vVS_SCAC";
        private static string PARM_VS_CARRIER_NAME = "@I_vVS_Carrier_Name";
        private static string PARM_VS_CARTON_DIMENSIONS = "@I_vVS_Carton_Dimensions";
        private static string PARM_VS_CARRIER_TYPE = "@I_vVS_Carrier_Type";
        private static string PARM_VS_PAYMENT_METHOD = "@I_vVS_Payment_Method";

        private static string PARM_SHIP_ID = "@SHIPID";
        private static string PARM_INTERID = "@INTERID";
        private static string PARM_ERR = "@ERR";

        private static string PARM_ERROR_STATE = "@O_iErrorState";
        private static string PARM_ERROR_STRING = "@oErrString";

        private static string PARM_CUSTNMBR = "@CUSTNMBR";

        private static string PARM_ESIPKCID = "@I_ESIPKCID";
        private static string PARM_ESICONTY = "@I_ESICONTY";
        private static string PARM_ESIUOFM1 = "@I_ESIUOFM1";
        private static string PARM_ESICONHT = "@I_ESICONHT";
        private static string PARM_ESICONWD = "@I_ESICONWD";
        private static string PARM_ESICONLN = "@I_ESICONLN";
        private static string PARM_ESICONCF = "@I_ESICONCF";

        #endregion

        #region Settings

        public static PowerhouseWsSetting GetPowerhouseIntegrationSettings()
        {
            SqlParameter[] parameters = {
                                            new SqlParameter(PARM_CMPANYID, SqlDbType.SmallInt)

                                        };
            parameters[0].Value = AppSettings.GPCompanyID;

            using (var dr = SqlHelper.ExecuteReader(AppSettings.GPConnectionString, CommandType.StoredProcedure, SQL_GET_POWERHOUSE_SETTINGS, parameters))
            {
                if (dr.HasRows)
                {
                    return TrimStrings(MapDataToBusinessEntityCollection<PowerhouseWsSetting>(dr).FirstOrDefault());
                }
            }
            return null;
        }

        #endregion

        #region SOP

        public static SOP10200_DTO GetSalesTransactionLine(short sopType, string sopNumber, int componentSequence, int lineItemSequence)
        {
            SqlParameter[] parameters = {
                                            new SqlParameter(PARM_SOPTYPE, SqlDbType.SmallInt),
                                            new SqlParameter(PARM_SOPNUMBE, SqlDbType.Char),
                                            new SqlParameter(PARM_CMPNTSEQ, SqlDbType.Int),
                                            new SqlParameter(PARM_LNITMSEQ, SqlDbType.Int)
                                        };
            parameters[0].Value = sopType;
            parameters[1].Value = sopNumber;
            parameters[2].Value = componentSequence;
            parameters[3].Value = lineItemSequence;

            using (var dr = SqlHelper.ExecuteReader(AppSettings.GPConnectionString, CommandType.StoredProcedure, SQL_GET_SOP_LINE, parameters))
            {
                if (dr.HasRows)
                {
                    return TrimStrings(MapDataToBusinessEntityCollection<SOP10200_DTO>(dr).FirstOrDefault());
                }
            }
            return null;
        }

        public static bool CustomerHasASN(string custId)
        {
            RM00101_DTO customer = null;

            SqlParameter[] parameters = {
                                            new SqlParameter(PARM_CUSTNMBR, SqlDbType.Char)
                                        };
            parameters[0].Value = custId;

            using (var dr = SqlHelper.ExecuteReader(AppSettings.GPConnectionString, CommandType.StoredProcedure, SQL_GET_CUSTOMER, parameters))
            {
                if (dr.HasRows)
                {
                    customer = TrimStrings(MapDataToBusinessEntityCollection<RM00101_DTO>(dr).FirstOrDefault());
                }
            }

            return customer?.CUSTPRIORITY == 2;
        }

        public static SOP10100_DTO GetSalesTransactionHeader(short sopType, string sopNumber)
        {
            SqlParameter[] parameters = {
                                            new SqlParameter(PARM_SOPTYPE, SqlDbType.SmallInt),
                                            new SqlParameter(PARM_SOPNUMBE, SqlDbType.Char)
                                        };
            parameters[0].Value = sopType;
            parameters[1].Value = sopNumber;

            using (var dr = SqlHelper.ExecuteReader(AppSettings.GPConnectionString, CommandType.StoredProcedure, SQL_GET_SOP_HEADER, parameters))
            {
                if (dr.HasRows)
                {
                    return TrimStrings(MapDataToBusinessEntityCollection<SOP10100_DTO>(dr).FirstOrDefault());
                }
            }
            return null;
        }

        public static CustomerBatch GetCustomerBatch(string customerNumber)
        {
            SqlParameter[] parameters = {
                                            new SqlParameter(PARM_CUSTNMBR, SqlDbType.Char, 15)                                            
                                        };
            parameters[0].Value = customerNumber;            

            using (var dr = SqlHelper.ExecuteReader(AppSettings.GPConnectionString, CommandType.StoredProcedure, SQL_GET_CUSTOMER_BATCH, parameters))
            {
                if (dr.HasRows)
                {
                    return TrimStrings(MapDataToBusinessEntityCollection<CustomerBatch>(dr).FirstOrDefault());
                }
            }
            return null;
        }

        public static bool UpdateEDI(short sopType, Shipment shipment)
        {

            foreach (var carton in shipment.shipmentCartons)
            {
                foreach (var detail in carton.shipmentCartonDetail)
                {
                    //Get the Line Item Sequence First
                    //var lnitmseq = Convert.ToInt32(Math.Truncate(Convert.ToDecimal(detail.orderLine / 16384)) * 16384);
                    var lnitmseq = Convert.ToInt32(detail.olCust29);

                    SqlParameter[] parameters = {
                                            new SqlParameter(PARM_SOPTYPE_ECONNECT, SqlDbType.SmallInt),
                                            new SqlParameter(PARM_SOPNUMBE_ECONNECT, SqlDbType.Char, 21),
                                            new SqlParameter(PARM_LNITMSEQ_ECONNECT, SqlDbType.Int),
                                            new SqlParameter(PARM_VS_SEQUENCE_NUMBER, SqlDbType.Int),
                                            new SqlParameter(PARM_VS_CARTON_ASNSTR, SqlDbType.Char, 41),
                                            new SqlParameter(PARM_VS_PALLET_ASNSTR, SqlDbType.Char, 41),
                                            new SqlParameter(PARM_VS_PRODUCT_CODE, SqlDbType.Char, 21),
                                            new SqlParameter(PARM_VS_QTY_PACKEDASN, SqlDbType.Decimal),
                                            new SqlParameter(PARM_VS_QTY_PACKEDTHISSLIP, SqlDbType.Decimal),
                                            new SqlParameter(PARM_VS_CARTON_WEIGHT, SqlDbType.Int),
                                            new SqlParameter(PARM_VS_TOTAL_WEIGHT, SqlDbType.Decimal),
                                            new SqlParameter(PARM_VS_DATE_SHIPPED, SqlDbType.DateTime),
                                            new SqlParameter(PARM_VS_SHIPMENT_NUMBER, SqlDbType.Char, 21),
                                            new SqlParameter(PARM_VS_SHIP_CARRIER_USED, SqlDbType.Char, 41),
                                            new SqlParameter(PARM_VS_PICKUP_NUMBER, SqlDbType.Char, 41),
                                            new SqlParameter(PARM_VS_TT_NUMBER, SqlDbType.Char, 21),
                                            new SqlParameter(PARM_VS_SEAL_NUMBER, SqlDbType.Char, 17),
                                            new SqlParameter(PARM_VS_TRAILER_NUMBER, SqlDbType.Char, 17),
                                            new SqlParameter(PARM_VS_LOAD_NUMBER, SqlDbType.Char, 17),
                                            new SqlParameter(PARM_VS_PRO_NUMBER, SqlDbType.Char, 41),
                                            new SqlParameter(PARM_VS_SCC18, SqlDbType.Char, 21),
                                            new SqlParameter(PARM_VS_SCAC, SqlDbType.Char, 15),
                                            new SqlParameter(PARM_VS_CARRIER_NAME, SqlDbType.Char, 15),
                                            new SqlParameter(PARM_VS_CARTON_DIMENSIONS, SqlDbType.Char, 15),
                                            new SqlParameter(PARM_VS_CARRIER_TYPE, SqlDbType.SmallInt),
                                            new SqlParameter(PARM_VS_PAYMENT_METHOD, SqlDbType.SmallInt)
                                        };
                    parameters[0].Value = sopType;
                    parameters[1].Value = shipment.orderId;
                    parameters[2].Value = lnitmseq;
                    parameters[3].Value = detail.workDetSeqNum;
                    parameters[4].Value = carton.cartonIdFrom;
                    //parameters[5].Value = string.Empty;
                    parameters[5].Value = detail.palletIdFrom;
                    parameters[6].Value = detail.itemId;
                    parameters[7].Value = detail.piecesToMove;
                    parameters[8].Value = 0;
                    parameters[9].Value = carton.stdActWeight;
                    parameters[10].Value = shipment.totalWeight;
                    parameters[11].Value = shipment.dateShipped;
                    parameters[12].Value = string.IsNullOrEmpty(shipment.truckId) ? shipment.orderId : shipment.truckId;
                    parameters[13].Value = carton.shipMethod;
                    parameters[14].Value = shipment.bolId;
                    parameters[15].Value = carton.packageTraceId;
                    parameters[16].Value = shipment.sealNum;
                    parameters[17].Value = shipment.trailerId;
                    parameters[18].Value = shipment.custLoadId;
                    parameters[19].Value = shipment.proNumber;
                    parameters[20].Value = carton.cartonIdFrom;
                    parameters[21].Value = string.Empty;
                    parameters[22].Value = shipment.carrierId;
                    parameters[23].Value = string.Empty;
                    parameters[24].Value = 0;
                    parameters[25].Value = 0;

                    var dr = SqlHelper.ExecuteNonQuery(AppSettings.GPConnectionString, CommandType.StoredProcedure, SQL_UPDATE_EDI, parameters);
                }

                SqlParameter[] parametersESI40500 = {
                                            new SqlParameter(PARM_ESIPKCID, SqlDbType.Char, 11),
                                            new SqlParameter(PARM_ESICONTY, SqlDbType.Char, 11),
                                            new SqlParameter(PARM_ESIUOFM1, SqlDbType.Char, 3),
                                            new SqlParameter(PARM_ESICONHT, SqlDbType.Decimal),
                                            new SqlParameter(PARM_ESICONWD, SqlDbType.Decimal),
                                            new SqlParameter(PARM_ESICONLN, SqlDbType.Decimal),
                                            new SqlParameter(PARM_ESICONCF, SqlDbType.Decimal)
                                        };
                parametersESI40500[0].Value = ""; // Pending
                parametersESI40500[1].Value = "CTN";
                parametersESI40500[2].Value = "EA";
                parametersESI40500[3].Value = carton.stdCartonHeight;
                parametersESI40500[4].Value = carton.stdCartonWidth;
                parametersESI40500[5].Value = carton.stdCartonDepth;
                parametersESI40500[6].Value = 0d;

                var drESI40500 = SqlHelper.ExecuteNonQuery(AppSettings.GPConnectionString, CommandType.StoredProcedure, SQL_UPDATE_EDI_ESI40500, parametersESI40500);
            }

            return DynamicsGpDB.UpdateASNManager(string.IsNullOrEmpty(shipment.truckId) ? shipment.orderId : shipment.truckId) == 0;

        }

        public static void FulfillmentPost(short sopType, string sopNumber, out int errorState, out string errorString)
        {
            SqlParameter[] parameters = {
                                            new SqlParameter(PARM_SOPTYPE_ECONNECT, SqlDbType.SmallInt),
                                            new SqlParameter(PARM_SOPNUMBE_ECONNECT, SqlDbType.Char, 21),
                                            new SqlParameter(PARM_ERROR_STATE, SqlDbType.Int),
                                            new SqlParameter(PARM_ERROR_STRING, SqlDbType.VarChar, 255)
                                        };
            parameters[0].Value = sopType;
            parameters[1].Value = sopNumber;
            parameters[2].Direction = ParameterDirection.Output;
            parameters[3].Direction = ParameterDirection.Output;

            var dr = SqlHelper.ExecuteNonQuery(AppSettings.GPConnectionString, CommandType.StoredProcedure, SQL_FULFILLMENT_POST, parameters);

            errorState = Convert.ToInt32(parameters[2].Value);
            errorString = parameters[3]?.Value.ToString();
        }

        public static int UpdateASNManager(string shipmentId)
        {
            int rtn = 0;

            try
            {
                SqlParameter[] parameters = {
                                            new SqlParameter(PARM_SHIP_ID, SqlDbType.VarChar, 23),
                                            new SqlParameter(PARM_INTERID, SqlDbType.VarChar, 5),
                                            new SqlParameter(PARM_ERR, SqlDbType.Int)
            };

                parameters[0].Value = shipmentId;
                //parameters[1].Value = AppSettings.GPCompanyID;
                parameters[1].Value = AppSettings.InterID;
                parameters[2].Direction = ParameterDirection.Output;

                var dr = SqlHelper.ExecuteNonQuery(AppSettings.GPConnectionString, CommandType.StoredProcedure, SQL_UPDATE_ASN_MANAGER, parameters);

                if (!Convert.IsDBNull(parameters[2].Value))
                    int.TryParse(parameters[2].Value.ToString(), out rtn);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return rtn;
        }

        #endregion  

        #region Utility Helper Methods
        public static T TrimStrings<T>(T instance)
        {
            var props = instance.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    // Ignore non-string properties
                    .Where(prop => prop.PropertyType == typeof(string))
                    // Ignore indexers
                    .Where(prop => prop.GetIndexParameters().Length == 0)
                    // Must be both readable and writable
                    .Where(prop => prop.CanWrite && prop.CanRead);

            foreach (PropertyInfo prop in props)
            {
                string value = (string)prop.GetValue(instance, null);
                if (value != null)
                {
                    value = value.Trim();
                    prop.SetValue(instance, value, null);
                }
            }

            return instance;
        }

        public static List<T> MapDataToBusinessEntityCollection<T>(IDataReader dr) where T : new()
        {
            Type businessEntityType = typeof(T);
            List<T> entitys = new List<T>();
            Hashtable hashtable = new Hashtable();
            PropertyInfo[] properties = businessEntityType.GetProperties();
            foreach (PropertyInfo info in properties)
            {
                hashtable[info.Name.ToUpper()] = info;
            }
            while (dr.Read())
            {
                T newObject = new T();
                for (int index = 0; index < dr.FieldCount; index++)
                {
                    PropertyInfo info = (PropertyInfo)
                                        hashtable[dr.GetName(index).ToUpper()];
                    if ((info != null) && info.CanWrite)
                    {
                        info.SetValue(newObject, dr.GetValue(index), null);
                    }
                }
                entitys.Add(newObject);
            }
            dr.Close();
            return entitys;
        }
        
        #endregion
    }
}
