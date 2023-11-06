using System.ServiceProcess;

namespace wps.core
{
    public class ManageWindowsServiceStrategy : StrategyBase
    {
        public static string TypeName = "ManageWindowsService";
        private string serviceName = string.Empty;
        private string action = string.Empty;
        private ServiceController windowsServiceController;
        private ServiceControllerStatus initialStatusOfService;

        public override IStrategy Initialize()
        {
            Console.WriteLine($"{nameof(ManageWindowsServiceStrategy)}.Initialize is called.");
            serviceName = GetParameter("servicename");
            action = GetParameter("action");
            windowsServiceController = new ServiceController(serviceName);
            initialStatusOfService = windowsServiceController.Status;
            return this;
        }

        public override IStrategy Validate()
        {
            Console.WriteLine($"{nameof(ManageWindowsServiceStrategy)}.Validate is called.");
            ArgumentNullException.ThrowIfNullOrEmpty(GetParameter("servicename"), "servicename");
            ArgumentNullException.ThrowIfNullOrEmpty(GetParameter("action"), "action");
            return this;
        }
        public override Task ExecuteAsync()
        {
            Console.WriteLine($"Manage Windows Service strategy is executing.");

            StartOrStopWindowsService(serviceName, action, false);
            return Task.CompletedTask;
        }

        public override Task RollbackAsync()
        {
            Console.WriteLine($"Manage Windows Service strategy is rolling back.");


            return Task.CompletedTask;
        }

        private void StartOrStopWindowsService(string serviceName, string action, bool isRollback)
        {
            try
            {
                if (windowsServiceController.Status == ServiceControllerStatus.Running
                && action == "start"
                && isRollback
                && initialStatusOfService == ServiceControllerStatus.Stopped)
                {
                    windowsServiceController.Stop();
                    windowsServiceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                    Console.WriteLine($"Service '{serviceName}' stopped successfully due to rollback.");
                }
                else if (windowsServiceController.Status == ServiceControllerStatus.Stopped
                && action == "stop"
                && isRollback
                && initialStatusOfService == ServiceControllerStatus.Running)
                {
                    windowsServiceController.Start();
                    windowsServiceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                    Console.WriteLine($"Service '{serviceName}' started successfully due to rollback.");
                }
                else if (windowsServiceController.Status == ServiceControllerStatus.Stopped
                && action == "start"
                && !isRollback)
                {
                    windowsServiceController.Start();
                    windowsServiceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                    Console.WriteLine($"Service '{serviceName}' started successfully.");
                }
                else if (windowsServiceController.Status == ServiceControllerStatus.Running
                && action == "stop"
                && !isRollback)
                {
                    windowsServiceController.Stop();
                    windowsServiceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                    Console.WriteLine($"Service '{serviceName}' stopped successfully.");
                }
                else
                {
                    Console.WriteLine($"Service '{serviceName}' is in a {windowsServiceController.Status} state.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting/stopping service '{serviceName}': {ex.Message}");
                throw;
            }

        }
    }
}