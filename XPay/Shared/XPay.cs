using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XPayNS.Shared
{
    public interface IXPay
    {
        event EventHandler<Action<bool>> AuthorizePayment;
        event EventHandler DidFinish;

        Task<bool> Show(XPayRequest request);
        void Hide();
    }

    public class XPay : IXPay
    {
        public event EventHandler<Action<bool>> AuthorizePayment;
        public event EventHandler DidFinish;

        IXPay PaymentPlatform { get; set; }

        public XPay()
        {
            if(Device.RuntimePlatform != Device.iOS && Device.RuntimePlatform != Device.Android)
            {
                throw new NotSupportedException();
            }

            this.PaymentPlatform = DependencyService.Get<IXPay>(DependencyFetchTarget.NewInstance);

            if(this.PaymentPlatform == null)
            {
                throw new NotImplementedException();
            }

            this.PaymentPlatform.AuthorizePayment += PaymentPlatform_AuthorizePayment;
            this.PaymentPlatform.DidFinish += PaymentPlatform_DidFinish;
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

        private void PaymentPlatform_AuthorizePayment(object sender, Action<bool> e)
        {
            this.AuthorizePayment?.Invoke(sender, e);
        }

        private async void PaymentPlatform_DidFinish(object sender, EventArgs e)
        {
            if (this.DidFinish != null)
            {
                this.DidFinish(this, new EventArgs());
            }
            else
            {
                await Task.Delay(100);
                this.Hide();
            }
        }
    }

    public class XPayRequest
    {
        public string MerchantIdentifier { get; set; }
        public string CountryCode { get; set; }
        public string CurrencyCode { get; set; }
        public List<string> SupportedCountries { get; set; }

        public List<XPayRequestItem> Items { get; set; } = new List<XPayRequestItem>();

        public XPayRequest AddItem(string label, decimal amount)
        {
            this.Items.Add(new XPayRequestItem { Label = label, Amount = amount });
            return this;
        }

        internal void Validate()
        {
            if (this.CountryCode.Length != 2)
            {
                throw new Exception("CountryCode must be 2 characters only (ISO 3166-1 alpha-2)");
            }
        }
    }

    public class XPayRequestItem
    {
        public string Label { get; set; }
        public decimal Amount { get; set; }
    }
}
