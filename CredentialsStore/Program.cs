using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Contracts;
using System.ServiceModel.Description;

namespace CredentialsStore
{
    class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;

            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

            string address = "net.tcp://localhost:9999/AccountManagement";

            ServiceHost host = new ServiceHost(typeof(AccountManagement));
            host.AddServiceEndpoint(typeof(IAccountManagement), binding, address);

            host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });


            host.Open();
            Console.WriteLine("WCFService is opened. Press <enter> to finish...");

            Console.ReadLine();

            host.Close();
        }
    }
}
