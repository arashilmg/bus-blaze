using System;

namespace BizCover.Messages.Renewals
{
    public class MigratePolicyCommand
    {
        public string PolicyId { get; set; }

        public string ProductCode { get; set; }

        public DateTime ExpiryDate { get; set; }

        public DateTime InceptionDate { get; set; }

        public string Status { get; set; }
    }

}

