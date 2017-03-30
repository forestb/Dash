using System;

namespace Dash.Lib.Models
{
    public class DashListenerResponse : EventArgs
    {
        public bool Started { get; set; }
        public string Message { get; set; }
    }
}
