namespace BizCover.Application.Renewals.Configuration;

public class CanAutoRenewProductConfig
{
    public IEnumerable<CanAutoRenewProduct> Products { get; set; }
}

public class CanAutoRenewProduct : ProductConfig
{
    public bool CanAutoRenew { get; set; }
}