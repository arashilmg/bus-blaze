using System;
using System.Collections.Generic;
using System.Text;

namespace BizCover.Blaze.Infrastructure.Bus.Sample.Event
{
    public class AcceptOrderCommand
    {
        public string ProductName { get; set; }
        public string OrderId { get; set; }
        public string OrderedAcceptedId { get; set; }
    }
}
