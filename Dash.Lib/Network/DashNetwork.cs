using System;
using System.Collections.Generic;
using System.ComponentModel;
using Dash.Db;
using Dash.Lib.Exceptions;
using Dash.Lib.Models;
using PacketDotNet;
using SharpPcap;

namespace Dash.Lib.Network
{
    public class DashNetwork
    {
        private const int READ_TIMEOUT_MILLISECONDS = 1000;

        private const int DASH_CLICK_INTERVAL = 5000;

        private static readonly Dictionary<string, BackgroundWorker> DashButtonWorkers =
            new Dictionary<string, BackgroundWorker>();

        public event EventHandler ListenerStarted;
        public event EventHandler DashButtonProbed;

        public void StartListening()
        {
            // Retrieve the device list
            try
            {
                var devices = CaptureDeviceList.Instance;

                if (devices.Count < 1)
                {
                    throw new PcapMissingException("No interfaces found! Make sure WinPcap is installed.");
                }

                foreach (var device in devices)
                {
                    if (device == null)
                    {
                        continue;
                    }

                    device.OnPacketArrival += delegate(object sender, CaptureEventArgs e)
                    {
                        var packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);

                        var eth = (EthernetPacket) packet;

                        var dashMac = eth.SourceHwAddress.ToString();

                        if (dashMac.Equals(device.MacAddress.ToString()))
                        {
                            //ignore packets from our own device
                            return;
                        }

                        // using our Wireshark-manufacturer dataset, determine if the device is an Amazon device
                        string macSubset = dashMac.Substring(0, 6);

                        if (!Data.AmazonDataSet.Contains(macSubset))
                        {
                            return;
                        }

                        var dashId = dashMac.GetHashCode();

                        DashResponse probe = new DashResponse
                        {
                            DashMac = dashMac,
                            DashId = dashId,
                            Device = device.MacAddress.ToString()
                        };

                        ListenToDevice(probe);
                    };

                    device.Open(DeviceMode.Promiscuous, READ_TIMEOUT_MILLISECONDS);

                    // tcpdump filter to capture only ARP Packets
                    device.Filter = "arp";

                    Action action = device.Capture;
                    action.BeginInvoke(ar => action.EndInvoke(ar), null);
                    
                    ListenerStarted?.Invoke(this,
                        new DashListenerResponse
                        {
                            Started = true,
                            Message = $"Started listener on {device.MacAddress}"
                        });
                }
            }
            catch (Exception)
            {
                throw new PcapMissingException(PcapMissingException.PcapMissingErrorMessage);
            }
        }

        private void ListenToDevice(DashResponse probe)
        {
            if (!DashButtonWorkers.ContainsKey(probe.DashMac))
            {
                DashButtonWorkers.Add(probe.DashMac, new BackgroundWorker());
                DashButtonWorkers[probe.DashMac].DoWork += DashBackgroundWorker_DoWork;
            }

            if (!DashButtonWorkers[probe.DashMac].IsBusy)
            {
                DashButtonWorkers[probe.DashMac].RunWorkerAsync(probe);
            }
        }

        private void DashBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var probe = (DashResponse)e.Argument;

            DashButtonProbed?.Invoke(this, probe);

            System.Threading.Thread.Sleep(DASH_CLICK_INTERVAL);
        }
    }
}