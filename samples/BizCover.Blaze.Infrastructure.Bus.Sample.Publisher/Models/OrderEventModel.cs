using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BizCover.Blaze.Infrastructure.Bus.Sample.Publisher.Models
{
    public class OrderEventModel
    {
        public string ProductName { get; set; }
        public string OrderId { get; set; }
        
    }
}
