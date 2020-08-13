﻿using BSP.PowerHouse.DynamicsGP.Integration.Configuration;
using BSP.PowerHouse.DynamicsGP.Integration.Model;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Net;

namespace BSP.PowerHouse.DynamicsGP.Integration.Service
{
    public class PowerHouseGPWebOrderService : IPowerHouseGPService
    {
        private readonly PowerhouseWsSetting _powerhouseWsSetting;
        public PowerHouseGPWebOrderService(PowerhouseWsSetting powerhouseWsSetting)
        {
            this._powerhouseWsSetting = powerhouseWsSetting;
        }
        public void Process()
        {
            //call send to PH
            if (!string.IsNullOrWhiteSpace(_powerhouseWsSetting.BSPGPWSURL))
            {
                try
                {
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                    var client = new RestClient(_powerhouseWsSetting.BSPGPWSURL)
                    {
                        Timeout = 600000
                    };

                    //var resource = $"Tenants(DefaultTenant)/Companies({Utility.UrlEncodeValue(AppSettings.GPCompanyName)})/BusinessSolutionPartners/Sales/PHSOPIntegration";
                    var resource = $"Tenants(DefaultTenant)/Companies(" + AppSettings.GPCompanyName + ")/BusinessSolutionPartners/Sales/PHSOPIntegration";
                    var request = new RestRequest(resource, Method.POST);

                    
                    request.UseDefaultCredentials = true;
                    

                    var response = client.Execute(request);
                    if (!response.IsSuccessful)
                    {
                        //log error
                        var errorMsg = $"GP Service Error:\r\n\r\nResource: {request.Resource} \r\n\r\nParameters: \r\n\r\n{string.Join("\r\n", request.Parameters)} \r\n\r\nResponse Content: \r\n\r\n{response.Content}";
                        EventLogUtility.LogWarningMessage(errorMsg);
                    }
                }
                catch (Exception ex)
                {
                    EventLogUtility.LogException(ex);
                }
            }
        }
    }
}
