using System;
using System.Configuration;
using System.Linq;
using Microsoft.ServiceBus.Notifications;

namespace Azure.UpdateExpiration
{
    class Program
    {
        private static string _connectionString = "---";
        private static string _hubName = "---";
        private static int _nbElementToUpdate;

        static void Main(string[] args)
        {
            _connectionString = ConfigurationManager.AppSettings.Get("Azure.ConnectionString");
            _hubName = ConfigurationManager.AppSettings.Get("Azure.HubName");
            _nbElementToUpdate = Convert.ToInt32(ConfigurationManager.AppSettings.Get("NbRegistrationsToUpdate"));

            NotificationHubClient client = NotificationHubClient.CreateClientFromConnectionString(_connectionString, _hubName);

            var allRegistrationsAsync = client.GetAllRegistrationsAsync(_nbElementToUpdate);
            allRegistrationsAsync.Wait();

            var registrationDescriptions = allRegistrationsAsync.Result;

            int totalRegistrations = registrationDescriptions.Count();

            Console.WriteLine("Registrations : {0}", totalRegistrations);

            int currentRegistration = 1;

            foreach (var registrationDescription in registrationDescriptions)
            {
                Console.WriteLine("Update registration {0}/{1}", currentRegistration, totalRegistrations);
                try
                {
                    client.UpdateRegistrationAsync(registrationDescription);
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Unable to update registration - {0}", exception.Message);
                }

                currentRegistration++;
            }
        }
    }
}
