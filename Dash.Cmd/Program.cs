using System;
using Dash.Lib.Exceptions;
using Dash.Lib.Models;
using Dash.Lib.Network;

namespace Dash.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Amazon Dash Button";

            var network = new DashNetwork();

            network.ListenerStarted += network_ListenerStarted;
            network.DashButtonProbed += network_DashProbed;

            try
            {
                network.StartListening();
            }
            catch (PcapMissingException)
            {
                throw new PcapMissingException(PcapMissingException.PcapMissingErrorMessage);
            }

            Console.Read();
        }

        private static void network_DashProbed(object sender, EventArgs e)
        {
            var probe = (DashResponse)e;

            Console.WriteLine($"Amazon Dash Connected: {probe.DashMac} seen on {probe.Device}.");
        }

        private static void network_ListenerStarted(object sender, EventArgs e)
        {
            Console.WriteLine(((DashListenerResponse)e).Message);
        }
    }
}
