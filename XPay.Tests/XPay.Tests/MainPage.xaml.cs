using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XPayNS.Shared;

namespace XPayNS.Tests
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

        private async void btnTest_Clicked(object sender, EventArgs e)
        {
            XPay xPay = new XPay();
            await xPay.Show(new XPayRequest
            {
                CurrencyCode = "AUD",
                CountryCode = "AU"
            }.AddItem("Test amount", 10).AddItem("Fee", 0));
        }
    }
}
