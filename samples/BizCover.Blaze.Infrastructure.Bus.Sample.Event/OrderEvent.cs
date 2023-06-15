using System;

namespace BizCover.Blaze.Infrastructure.Bus.Sample.Event
{
    public interface OrderEvent
    {
        public string ProductName { get; set; }
        public string OrderId { get; set; }
    }
}
