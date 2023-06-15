using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizCover.Application.Renewals.Helpers
{
    public static class ValidationConstants
    {
        public static readonly string PolicyRenewalInitiated = "Expiring policy {0} is not Initiated";
        public static readonly string PolicyRenewed = "Expiring policy {0} is already renewed with new policy {1}";
        public static readonly string SpecialCircumstance = "Expiring Policy {0} special circumstances is true";
        public static readonly string PolicyInArrears = "Expiring policy {0}  has arrears";
        public static readonly string PolicyIsNotActive = "Expiring Policy {0} must be active or expired, but it is {1}";
        public static readonly string PolicyAutoUnEligible = "Expiring Policy {0} is not eligible, {1}";
        public static readonly string ProductAutoUnEligible = "Product {0} is not eligible for auto and express renewals";
    }
}
