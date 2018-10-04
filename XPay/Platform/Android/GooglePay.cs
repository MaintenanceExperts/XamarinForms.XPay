using Android.Gms.Common.Apis;
using Android.Gms.Wallet;
using Android.Gms.Wallet.Fragment;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using XPayNS.Platform.Android;
using XPayNS.Shared;

using AndroidApp = Android.App;
using AndroidGms = Android.Gms;

[assembly: Dependency(typeof(GooglePay))]
namespace XPayNS.Platform.Android
{
    public class GooglePay : IXPay
    {
        public event EventHandler<Action<bool>> AuthorizePayment;
        public event EventHandler DidFinish;

        GoogleApiClient apiClient;

        public async Task<bool> Show(XPayRequest request)
        {
            WalletClass.WalletOptions walletOptions = new WalletClass.WalletOptions.Builder()
                .SetEnvironment(WalletConstants.EnvironmentTest)
                .Build();

            GoogleApiClient client = new GoogleApiClient.Builder(AndroidApp.Application.Context)
                .AddApi(WalletClass.API)
                .Build();

            BooleanResult boolResult = await WalletClass.Payments.IsReadyToPayAsync(client);
            if(!boolResult.Value)
            {
                throw new NotSupportedException();
            }




            return true;
        }

        //public async Task<bool> Show(XPayRequest request)
        //{
        //    this.apiClient = new GoogleApiClient.Builder(AndroidApp.Application.Context)
        //        .AddApi(WalletClass.API)
        //        .Build();

        //    SupportWalletFragment sWalletFragment = SupportWalletFragment.NewInstance(WalletFragmentOptions.NewBuilder()
        //        .SetEnvironment(WalletConstants.EnvironmentTest)
        //        .SetMode(WalletFragmentMode.BuyButton)
        //        .SetTheme(WalletConstants.ThemeLight)
        //        .Build()
        //        );

        //    MaskedWalletRequest mWalletRequestBuilder = MaskedWalletRequest.NewBuilder()
        //        .SetPaymentMethodTokenizationParameters(PaymentMethodTokenizationParameters.NewBuilder()
        //            .SetPaymentMethodTokenizationType(PaymentMethodTokenizationType.PaymentGateway)
        //            .Build())
        //        .SetShippingAddressRequired(false)
        //        .SetMerchantName("GitBit")
        //        .SetPhoneNumberRequired(false)
        //        .SetShippingAddressRequired(false)
        //        .SetEstimatedTotalPrice(request.Items.Sum(y => y.Amount).ToString())
        //        .SetCurrencyCode(request.CurrencyCode)
        //        .Build();


        //    sWalletFragment.Initialize(WalletFragmentInitParams.NewBuilder()
        //        .SetMaskedWalletRequest(mWalletRequestBuilder)
        //        .SetMaskedWalletRequestCode(1)
        //        .Build());

        //    WalletClass.Payments.LoadMaskedWallet(this.apiClient, mWalletRequestBuilder, 50);

        //    return true;
        //}

        public void Hide()
        {
            
        }
    }
}
