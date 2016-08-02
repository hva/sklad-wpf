using Microsoft.Practices.Prism.Mvvm;
using Warehouse.Wpf.Models;

namespace Warehouse.Wpf.Module.ChangePrice
{
    public class ChangePriceItem : BindableBase
    {
        private decimal newPriceOpt;
        private decimal newPriceRozn;

        public ChangePriceItem(Product p)
        {
            Product = p;
        }

        public Product Product { get; }

        public decimal NewPriceOpt
        {
            get { return newPriceOpt; }
            set { SetProperty(ref newPriceOpt, value); }
        }

        public decimal NewPriceRozn
        {
            get { return newPriceRozn; }
            set { SetProperty(ref newPriceRozn, value); }
        }

        public void Refresh(double percentage)
        {
            var a = (decimal) Product.PriceOpt;
            var x = (decimal) percentage;
            var b = a * (1 + x / 100);

            NewPriceOpt = decimal.Ceiling(b * 100) / 100;

            NewPriceRozn = ProductExtensions.CalculatePriceRozn((double) newPriceOpt, Product.K, Product.Length, Product.IsSheet);
        }
    }
}
