using System;
using System.Diagnostics;
using Topshelf;

namespace BSP.PowerHouse.DynamicsGP.Integration
{
    class Program
    {
        static void Main(string[] args)
        {
            // Attach debugger if in Debug mode
#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif

            if (!EventLog.SourceExists("PowerHouseDynamicsGPService"))
            {
                EventLog.CreateEventSource("PowerHouseDynamicsGPService", "Application");
            }

            var exitCode = HostFactory.Run(x =>
            {
                x.Service<PowerHouseGPApiService>(s =>
                {
                    s.ConstructUsing(svc => new PowerHouseGPApiService());
                    s.WhenStarted(svc => svc.Start());
                    s.WhenStopped(svc => svc.Stop());
                });

                x.RunAsLocalSystem();

                x.SetServiceName("PowerHouseDynamicsGPService");
                x.SetDisplayName("Power House Dynamics GP Integration Service");
                x.SetDescription("This is an integration service between Power House WMS and Dynamics GP");
            });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}
