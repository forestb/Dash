using System;

namespace Dash.Lib.Models
{
    public class DashResponse : EventArgs
    {
        public string DashMac { get; set; }
        public int DashId { get; set; }
        public string Device { get; set; }
    }
}
