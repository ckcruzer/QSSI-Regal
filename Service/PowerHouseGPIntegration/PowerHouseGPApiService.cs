using System;
using System.Timers;
using BSP.PowerHouse.DynamicsGP.Integration.Configuration;
using BSP.PowerHouse.DynamicsGP.Integration.Service;
using System.Collections.Generic;
using BSP.PowerHouse.DynamicsGP.Integration.Model;
using BSP.PowerHouse.DynamicsGP.Integration.Data;

namespace BSP.PowerHouse.DynamicsGP.Integration
{
    class PowerHouseGPApiService
    {
        private readonly Timer _timer;
        private readonly PowerhouseWsSetting _powerhouseWsSetting;
        private readonly List<IPowerHouseGPService> _services;

        private static bool _synchronizing = false;
        private static bool _serviceStopped = false;
        public PowerHouseGPApiService()
        {
            try
            {
                _powerhouseWsSetting = DynamicsGpDB.GetPowerhouseIntegrationSettings();
                _services = new List<IPowerHouseGPService>
            {
                new PowerHouseGPWebOrderService(_powerhouseWsSetting),
                new PowerHouseGPShipmentService(_powerhouseWsSetting),
                new PowerHouseGPItemService(_powerhouseWsSetting),
                //new PowerHouseGPInventoryAdjustmentService(_powerhouseWsSetting)
            };
                //set timer
                //_timer = new Timer(_powerhouseWsSetting.BSPFrequency * 60000) { AutoReset = false };
                _timer = new Timer(_powerhouseWsSetting.BSPFrequency) { AutoReset = false };
                _timer.Elapsed += SynData;
            }
            catch(Exception ex)
            {
                EventLogUtility.LogException(ex);
            }
        }

        private void SynData(object sender, ElapsedEventArgs e)
        {
            //check if already running
            if (_synchronizing)
                return;

            _synchronizing = true;

            try
            {
                //check if dependency services installed and running
                if (!Utility.IsRequiredServiceRunning())
                {
                    //try to start services
                    Utility.StartRequiredServices();
                    //log error
                    EventLogUtility.LogErrorMessage("Required Services are not installed or running!");
                    //exit, dont' wait for it to start, the next run will pick up the from queue
                    return;
                }

                // Need to add code to check if the user is stuck in the ACTIVITY table in Dynamics

                EventLogUtility.LogInformationMessage("PowerHouse Dynamics GP Integration Started!");
                foreach (var service in _services)
                {
                    service.Process();
                }
                EventLogUtility.LogInformationMessage("PowerHouse Dynamics GP Integration Completed!");

            }
            catch (Exception ex)
            {
                EventLogUtility.LogException(ex);
            }
            finally
            {
                _synchronizing = false;

                if(!_serviceStopped)
                    _timer.Start();
            }
        }
        #region Timer Start/Stop
        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _serviceStopped = true;
            _timer.Stop();
        }

        #endregion
    }
}
