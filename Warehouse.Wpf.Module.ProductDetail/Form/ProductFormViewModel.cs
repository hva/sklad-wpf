using System;
using System.Globalization;
using System.Linq;
using Warehouse.Wpf.Infrastructure;
using Warehouse.Wpf.Models;
using Warehouse.Wpf.Mvvm;

namespace Warehouse.Wpf.Module.ProductDetail.Form
{
    public class ProductFormViewModel : ValidationObject
    {
        private string id;
        private string name;
        protected string size;
        private string k;
        private string priceOpt;
        private decimal priceRozn;
        private decimal weight;
        private string count;
        private string nd;
        private string length;

        public ProductFormViewModel(Product product, bool canEditPrice)
        {
            IsPriceReadonly = !canEditPrice;
            ProductToProps(product);
        }

        public bool IsValid()
        {
            ValidateName();
            ValidateSize();
            ValidateK();
            ValidatePriceOpt();
            ValidateCount();
            ValidateNd();
            ValidateLength();
            ValidatePriceIcome();

            return !HasErrors;
        }

        public Product GetUpdatedProduct()
        {
            return new Product
            {
                Id = id,
                Name = name,
                Size = size,
                K = double.Parse(k),
                PriceOpt = double.Parse(priceOpt),
                PriceRozn = (double) priceRozn,
                Weight = (double) weight,
                Count = int.Parse(count),
                Nd = ParseNd(nd),
                Length = double.Parse(length),
                PriceIcome = double.Parse(priceIcome),
                Internal = Internal,
                IsSheet = GetIsSheet(),
                Firma = Firma,
            };
        }

        public bool IsPriceReadonly { get; private set; }

        #region Name

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    ValidateName();
                }
            }
        }

        private void ValidateName()
        {
            errorsContainer.ClearErrors(() => Name);
            errorsContainer.SetErrors(() => Name, Validate.Required(Name));
        }

        #endregion

        #region Size

        public virtual string Size
        {
            get { return size; }
            set
            {
                if (size != value)
                {
                    size = value;
                    ValidateSize();
                }
            }
        }

        protected virtual void ValidateSize()
        {
            errorsContainer.ClearErrors(() => Size);
            errorsContainer.SetErrors(() => Size, Validate.Required(Size));
        }

        #endregion

        #region K

        public string K
        {
            get { return k; }
            set
            {
                if (k != value)
                {
                    k = value;
                    ValidateK();
                    UpdatePriceRozn();
                    UpdateWeight();
                }
            }
        }

        private void ValidateK()
        {
            errorsContainer.ClearErrors(() => K);
            errorsContainer.SetErrors(() => K, Validate.DoubleMaxPrecision(K, 2));
        }

        #endregion

        #region PriceOpt

        public string PriceOpt
        {
            get { return priceOpt; }
            set
            {
                if (priceOpt != value)
                {
                    priceOpt = value;
                    ValidatePriceOpt();
                    UpdatePriceRozn();
                }
            }
        }

        private void ValidatePriceOpt()
        {
            errorsContainer.ClearErrors(() => PriceOpt);
            errorsContainer.SetErrors(() => PriceOpt, Validate.DoubleMaxPrecision(PriceOpt, 1));
        }

        #endregion

        #region PriceRozn

        public decimal PriceRozn
        {
            get { return priceRozn; }
            set { SetProperty(ref priceRozn, value); }
        }

        protected void UpdatePriceRozn()
        {
            if (errorsContainer.HasErrors(() => PriceOpt, () => K, () => Length))
            {
                PriceRozn = 0;
            }
            else
            {
                PriceRozn = ProductExtensions.CalculatePriceRozn(double.Parse(priceOpt), double.Parse(k), double.Parse(length), GetIsSheet());
            }
        }

        #endregion

        #region Weight

        public decimal Weight
        {
            get { return weight; }
            set { SetProperty(ref weight, value); }
        }

        private void UpdateWeight()
        {
            if (errorsContainer.HasErrors(() => Count, () => Length, () => K, () => Nd))
            {
                Weight = 0;
            }
            else
            {
                var _count = int.Parse(count);
                var _length = decimal.Parse(length);
                var _nd = GetTotalNd();
                var _k = decimal.Parse(k);

                Weight = decimal.Round((_count * _length + _nd) * _k, 3);
            }
        }

        #endregion

        #region Count

        public string Count
        {
            get { return count; }
            set
            {
                if (count != value)
                {
                    count = value;
                    ValidateCount();
                    UpdateWeight();
                }
            }
        }

        private void ValidateCount()
        {
            errorsContainer.ClearErrors(() => Count);
            errorsContainer.SetErrors(() => Count, Validate.Int(Count));
        }

        #endregion

        #region Nd

        public string Nd
        {
            get { return nd; }
            set
            {
                if (nd != value)
                {
                    nd = value;
                    ValidateNd();
                    UpdateWeight();
                }
            }
        }

        private void ValidateNd()
        {
            errorsContainer.ClearErrors(() => Nd);
            if (!string.IsNullOrEmpty(nd))
            {
                var parts = nd.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var x in parts)
                {
                    var errors = Validate.DoubleMaxPrecision(x, 2).ToArray();
                    if (errors.Length > 0)
                    {
                        errorsContainer.SetErrors(() => Nd, new[] { "дробные числа, разделенные пробелом\nне более 2 знаков после запятой" });
                        break;
                    }
                }
            }
        }

        public virtual string NdLabel { get { return "Н/Д (м)"; } }

        #endregion

        #region Length

        public string Length
        {
            get { return length; }
            set
            {
                if (length != value)
                {
                    length = value;
                    ValidateLength();
                    UpdateWeight();
                    OnPropertyChanged(() => Length);
                }
            }
        }

        protected virtual void ValidateLength()
        {
            errorsContainer.ClearErrors(() => Length);
            errorsContainer.SetErrors(() => Length, Validate.DoubleMaxPrecision(Length, 2));
        }

        public virtual string LenghtLabel { get { return "Длина штанги (м)"; } }

        public virtual bool IsLengthReadonly { get { return false;} }

        #endregion

        #region PriceIcome
        private string priceIcome;

        public string PriceIcome
        {
            get { return priceIcome; }
            set
            {
                if (priceIcome != value)
                {
                    priceIcome = value;
                    ValidatePriceIcome();
                }
            }
        }

        private void ValidatePriceIcome()
        {
            errorsContainer.ClearErrors(() => PriceIcome);
            errorsContainer.SetErrors(() => PriceIcome, Validate.DoubleMaxPrecision(PriceIcome, 2));
        }

        #endregion

        #region Internal

        public string Internal { get; set; }

        #endregion

        #region Firma

        public string Firma { get; set; }

        #endregion

        protected virtual bool GetIsSheet()
        {
            return false;
        }

        private void ProductToProps(Product product)
        {
            id = product.Id;
            name = product.Name;
            size = product.Size;
            k = product.K.ToString(CultureInfo.CurrentCulture);
            priceOpt = product.PriceOpt.ToString(CultureInfo.CurrentCulture);
            priceRozn = (decimal) product.PriceRozn;
            weight = (decimal) product.Weight;
            count = product.Count.ToString();
            if (product.Nd != null)
            {
                nd = string.Join(" ", product.Nd);
            }
            length = product.Length.ToString(CultureInfo.CurrentCulture);
            priceIcome = product.PriceIcome.ToString(CultureInfo.CurrentCulture);
            Internal = product.Internal;
            Firma = product.Firma;
        }

        private static double[] ParseNd(string nd)
        {
            return (nd ?? string.Empty)
                .Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                .Select(double.Parse)
                .OrderByDescending(x => x)
                .ToArray();
        }

        private decimal GetTotalNd()
        {
            if (string.IsNullOrEmpty(nd))
            {
                return 0;
            }
            return nd.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Select(decimal.Parse).Sum();
        }
    }
}
