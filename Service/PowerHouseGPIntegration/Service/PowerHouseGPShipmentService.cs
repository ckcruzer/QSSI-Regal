using BSP.PowerHouse.DynamicsGP.Integration.Configuration;
using BSP.PowerHouse.DynamicsGP.Integration.eConnectObjects;
using BSP.PowerHouse.DynamicsGP.Integration.PowerHouseWS;
using Microsoft.Dynamics.GP.eConnect;
using RestSharp;
using System;
using RestSharp.Authenticators;
using System.Net;
using BSP.PowerHouse.DynamicsGP.Integration.Model;
using BSP.PowerHouse.DynamicsGP.Integration.Data;
using BSP.PowerHouse.DynamicsGP.Integration.Tools;

namespace BSP.PowerHouse.DynamicsGP.Integration.Service
{
    public class PowerHouseGPShipmentService : IPowerHouseGPService
    {
        private readonly PowerhouseWsSetting _powerhouseWsSetting;
        public PowerHouseGPShipmentService(PowerhouseWsSetting powerhouseWsSetting)
        {
            this._powerhouseWsSetting = powerhouseWsSetting;
        }
        public void Process()
        {
            PHWebServicesPortTypeClient serviceProxy = new PHWebServicesPortTypeClient(AppSettings.PowerhouseEndpoint);
            string sessionId = null;
            try
            {
                //connect
                sessionId = serviceProxy.reLogIn(null, _powerhouseWsSetting.USERID, _powerhouseWsSetting.BSPPowerhousePassword);
                if (!string.IsNullOrWhiteSpace(sessionId))
                {
                    var shipments = serviceProxy.getShipments(sessionId);
                    if (AppSettings.TracingEnabled)
                    {
                        Utility.LogResult(shipments, "shipment");
                    }
                    //check if any thing returned, checking null for first element because the service returns an array with one null element if no data
                    if (shipments != null && shipments.Length > 0 && shipments[0] != null)
                    {
                        foreach (var shipment in shipments)
                        {
                            try
                            {                               
                                var gpSopTransacation = new SopTransaction(shipment, _powerhouseWsSetting);
                                using (eConnectMethods econnect = new eConnectMethods())
                                {
                                    string sDocumentXml = gpSopTransacation.GetXmlSerializedObject();
                                    if (AppSettings.TracingEnabled)
                                    {
                                        EventLogUtility.LogInformationMessage(sDocumentXml);
                                    }
                                    if (econnect.UpdateTransactionEntity(Utility.GetEconnectConnectionString(AppSettings.GPConnectionString), sDocumentXml))
                                    {
                                        if (DynamicsGpDB.CustomerHasASN(shipment.custCode))
                                        {
                                            if (!DynamicsGpDB.UpdateEDI((short)GpSopType.Order, shipment))
                                            {
                                                if (AppSettings.SendShipResponse)
                                                {
                                                    var shipmentResponse = new ShipmentResponse
                                                    {
                                                        orderId = shipment.orderId,
                                                        releaseNum = shipment.releaseNum,
                                                        ownerId = shipment.ownerId,
                                                        warehouseId = shipment.warehouseId,
                                                        ifStatus = "E",
                                                        ifErrorMsg = "Order has been fulfilled but there was an error generating the ASN."
                                                    };
                                                    serviceProxy.sendShipmentResponse(sessionId, shipmentResponse);
                                                }
                                                continue;
                                            }
                                                
                                        }

                                        //update power house flags
                                        if (AppSettings.SendShipResponse)
                                        {
                                            var shipmentResponse = new ShipmentResponse
                                            {
                                                orderId = shipment.orderId,
                                                releaseNum = shipment.releaseNum,
                                                ownerId = shipment.ownerId,
                                                warehouseId = shipment.warehouseId,
                                                ifStatus = "X"
                                            };
                                            serviceProxy.sendShipmentResponse(sessionId, shipmentResponse);
                                        }
                                        if (AppSettings.TracingEnabled)
                                        {
                                            EventLogUtility.LogInformationMessage(shipment.ToString());
                                        }
                                    }
                                }
                            }
                            catch (eConnectException exc)
                            {
                                EmailHelper.SendEmail(AppSettings.EmailSubject + " PH Shipped Order Download Econnect Failure " + shipment.orderId + "." , exc.Message);

                                if (!string.IsNullOrWhiteSpace(exc.Message) && !exc.Message.Contains("4706")) // don't flag if tran is edit by another user
                                {
                                    if (AppSettings.SendShipResponse == true)
                                    {
                                        var shipmentResponse = new ShipmentResponse
                                        {
                                            orderId = shipment.orderId,
                                            releaseNum = shipment.releaseNum,
                                            ownerId = shipment.ownerId,
                                            warehouseId = shipment.warehouseId,
                                            ifStatus = "E",
                                            ifErrorMsg = exc.Message
                                        };
                                        serviceProxy.sendShipmentResponse(sessionId, shipmentResponse);
                                    }
                                }
                                EventLogUtility.LogException(exc);
                            }
                            catch (Exception exc)
                            {
                                EmailHelper.SendEmail(AppSettings.EmailSubject + " PH Shipped Order Download General Exception Failure " + shipment.orderId + ".", exc.Message);

                                if (AppSettings.SendShipResponse == true)
                                {
                                    var shipmentResponse = new ShipmentResponse
                                    {
                                        orderId = shipment.orderId,
                                        releaseNum = shipment.releaseNum,
                                        ownerId = shipment.ownerId,
                                        warehouseId = shipment.warehouseId,
                                        ifStatus = "E",
                                        ifErrorMsg = exc.Message
                                    };
                                    serviceProxy.sendShipmentResponse(sessionId, shipmentResponse);
                                }

                                EventLogUtility.LogException(exc);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogUtility.LogException(ex);
            }
            finally
            {
                if (serviceProxy != null)
                {
                    try
                    {
                        serviceProxy.logOut(sessionId);
                    }
                    catch { }
                }
            }
        }
    }
}
