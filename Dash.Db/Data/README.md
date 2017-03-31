## OUI Lookup Tool
The Wireshark OUI lookup tool provides an easy way to look up OUIs and other MAC address prefixes. It uses the Wireshark manufacturer database, which is a list of OUIs and MAC addresses compiled from a number of sources.

## Addtl
Database provided by [wireshark.org](https://www.wireshark.org/tools/oui-lookup.html).

Wireshark "guesses" the company by doing a lookup on the first 6 hex values of a mac address.  We can use this too, to help validate we're connected to an Amazon device.  It won't ensure it's a Dash button, but it's a good start.