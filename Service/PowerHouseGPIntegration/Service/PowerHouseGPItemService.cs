using BSP.PowerHouse.DynamicsGP.Integration.Configuration;
using BSP.PowerHouse.DynamicsGP.Integration.Model;
using BSP.PowerHouse.DynamicsGP.Integration.Tools;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Net;

namespace BSP.PowerHouse.DynamicsGP.Integration.Service
{
    public class PowerHouseGPItemService : IPowerHouseGPService
    {
        private readonly PowerhouseWsSetting _powerhouseWsSetting;
        public PowerHouseGPItemService(PowerhouseWsSetting powerhouseWsSetting)
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
                    var resource = $"Tenants(DefaultTenant)/Companies(" + AppSettings.GPCompanyName + ")/BusinessSolutionPartners/Inventory/PHItemIntegration";
                    var request = new RestRequest(resource, Method.POST);

                    
                    request.UseDefaultCredentials = true;
                    

                    var response = client.Execute(request);
                    if (!response.IsSuccessful)
                    {
                        EmailHelper.SendEmail(AppSettings.EmailSubject + " PH Item Upload General Exception Failure.", response.ErrorMessage);

                        //log error
                        var errorMsg = $"GP Service Error:\r\n\r\nResource: {request.Resource} \r\n\r\nParameters: \r\n\r\n{string.Join("\r\n", request.Parameters)} \r\n\r\nResponse Content: \r\n\r\n{response.Content}";
                        EventLogUtility.LogWarningMessage(errorMsg);
                    }
                }
                catch (Exception ex)
                {
                    EmailHelper.SendEmail(AppSettings.EmailSubject + " PH Item Upload General Exception Failure.", ex.Message);

                    EventLogUtility.LogException(ex);
                }
            }
        }
    }
}
