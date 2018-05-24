# Dash
Amazon Dash project.

- [Introduction](#introduction)
- [Amazon Dash Button Setup](#amazon-dash-button-setup)
- [How It Works](#how-it-works)
- [Usage](#usage)
- [Todo](#todo)
- [Links](#links)

## Introduction
An [Amazon Dash Button](https://en.wikipedia.org/wiki/Amazon_Dash) is an inexpensive, internet connected device that let's an Amazon user purchase an item by clicking a button.

The goal of this project, was to leverage these novel, inexpensive, internet connected buttons for my own efficacy; anything other than their intended use.  As it goes, I was both fortunate and unfortunate enough that someone else had already attempted to solve this problem, so this solution owes thanks in part to that project (linked below).

Improvements over the previous project include:
- Filter out non-Amazon devices making requests on the network by using a [wireshark.org](https://www.wireshark.org/tools/oui-lookup.html) oui-lookup dataset.
- Ensure the `network_DashProbed` event is raised once per Amazon Dash button click.

## Amazon Dash Button Setup
Follow Amazon's guide to setting up your dash button - [I'm happy to Google that for you](http://lmgtfy.com/?q=amazon+dash+button+setup). The goal of this step is to get your Amazon Dash button connected to your wireless network.

Just before linking the button to an Amazon product, close the application.

Note: Disable app notifications or you'll experience reminders to complete the setup.

## How It Works
When the Amazon Dash button is pressed, the device wakes up and, as part of it's process, makes a DHCP request which is broadcast from the router on the network back to all devices on the network. The `Dash.Cmd` application picks up that "traffic happened" from what it thinks is an Amazon device and `network_DashProbed` event is raised.

## Usage
Using this library is simple. The entire `main` method can be seen below:
```
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
```

## Links
- [DashSharp](https://github.com/Codeusa/DashSharp) by Codeusa
