using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptaTrack.Models
{
    public class ResultMessage
    {
        public bool ShowMessage { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; }
    }
}
