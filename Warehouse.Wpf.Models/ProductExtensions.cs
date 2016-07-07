namespace Warehouse.Wpf.Models
{
    public static class ProductExtensions
    {
        public static decimal CalculatePriceRozn(double priceOpt, double k, double length, bool isSheet)
        {
            var _priceOpt = (decimal) priceOpt;
            var _k = (decimal) k;
            var rozn = _priceOpt * _k / 1000m * 1.2m;
            if (isSheet)
            {
                var _l = (decimal) length;
                rozn *= _l;
            }
            return decimal.Ceiling(rozn * 10) / 10;
        }

        public static decimal CalculatePriceRozn(this Product p)
        {
            return CalculatePriceRozn(p.PriceOpt, p.K, p.Length, p.IsSheet);
        }
    }
}
