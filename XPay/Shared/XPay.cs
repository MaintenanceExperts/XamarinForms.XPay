using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XPay.Shared
{
    public interface IXPay
    {
        Task<bool> Show(XPayRequest request);
        void Hide();
    }

    public class XPay : IXPay
    {
        IXPay PaymentPlatform { get; set; }

        public XPay()
        {
            if(Device.RuntimePlatform != Device.iOS && Device.RuntimePlatform != Device.Android)
            {
                throw new NotSupportedException();
            }

            this.PaymentPlatform = DependencyService.Get<IXPay>(DependencyFetchTarget.NewInstance);
        }

        public async Task<bool> Show(XPayRequest request)
        {
            request.Validate();
            return await this.PaymentPlatform.Show(request);
        }

        public void Hide()
        {
            this.PaymentPlatform.Hide();
        }
    }

    public class XPayRequest
    {
        public string MerchantIdentifier { get; set; }
        public string CountryCode { get; set; }
        public string CurrencyCode { get; set; }
        public List<string> SupportedCountries { get; set; }

        internal void Validate()
        {
            if (this.CountryCode.Length != 2)
            {
                throw new Exception("CountryCode must be 2 characters only (ISO 3166-1 alpha-2)");
            }
        }
    }
}
