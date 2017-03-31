using System;
using Dash.Db;
using Dash.Lib.Exceptions;
using Dash.Lib.Models;
using Dash.Lib.Network;

namespace Dash.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "DashSharp - Amazon Dash Button";
            Console.WriteLine("Dash Buttons have two MAC addresses, their wakeup and their pair. The last one you receive is the pair address. ");

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

            // using our Wireshark-Manufacturer dataset, determine if the device is an Amazon device
            string macSubset = probe.DashMac.Substring(0, 6);

            // if Amazon device, let the user know
            if (Data.AmazonDataSet.Contains(macSubset))
            {
                Console.WriteLine("Amazon Dash Connected: " + probe.DashMac + " seen on " + probe.Device);
            }
        }

        private static void network_ListenerStarted(object sender, EventArgs e)
        {
            Console.WriteLine(((DashListenerResponse)e).Message);
        }
    }
}
