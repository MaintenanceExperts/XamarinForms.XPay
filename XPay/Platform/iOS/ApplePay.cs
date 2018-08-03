using Foundation;
using PassKit;
using System;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;
using XPayNS.Platform.iOS;
using XPayNS.Shared;

[assembly: Dependency(typeof(ApplePay))]
namespace XPayNS.Platform.iOS
{
    public class ApplePay : PKPaymentAuthorizationViewControllerDelegate, IXPay
    {
        PKPaymentAuthorizationViewController paymentController;

        public async Task<bool> Show(XPayRequest request)
        {
            NSString[] paymentNetworks = new NSString[] { PKPaymentNetwork.MasterCard, PKPaymentNetwork.Visa };

            if (!PKPaymentAuthorizationViewController.CanMakePayments || !PKPaymentAuthorizationViewController.CanMakePaymentsUsingNetworks(paymentNetworks))
            {
                throw new NotSupportedException();
            }

            PKPaymentSummaryItem
                paymentLine = PKPaymentSummaryItem.Create("Tap and go payment", new NSDecimalNumber("10")),
                feeLine = PKPaymentSummaryItem.Create("Fee", new NSDecimalNumber("0")),
                totalLine = PKPaymentSummaryItem.Create("GitBit", paymentLine.Amount.Add(feeLine.Amount));

            PKPaymentRequest paymentRequest = new PKPaymentRequest();
            paymentRequest.PaymentSummaryItems = new PKPaymentSummaryItem[] { paymentLine, feeLine, totalLine };
            paymentRequest.MerchantIdentifier = request.MerchantIdentifier;
            paymentRequest.MerchantCapabilities = PKMerchantCapability.ThreeDS;
            paymentRequest.CountryCode = request.CountryCode;
            paymentRequest.CurrencyCode = request.CurrencyCode;
            paymentRequest.SupportedNetworks = paymentNetworks;

            this.paymentController = new PKPaymentAuthorizationViewController(paymentRequest);
            this.paymentController.Delegate = this;

            UIWindow window = UIApplication.SharedApplication.KeyWindow;
            UIViewController vc = window.RootViewController;

            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }

            await vc.PresentViewControllerAsync(this.paymentController, true);
            return true;
        }

        public async void Hide()
        {
            if (this.paymentController == null) return;

            await this.paymentController.DismissViewControllerAsync(true);
            this.paymentController = null;
        }


        #region PKPaymentAuthorizationControllerDelegate Implementation

        public override void DidAuthorizePayment(PKPaymentAuthorizationViewController controller, PKPayment payment, Action<PKPaymentAuthorizationStatus> completion)
        {
            completion(PKPaymentAuthorizationStatus.Success);
        }

        public async override void PaymentAuthorizationViewControllerDidFinish(PKPaymentAuthorizationViewController controller)
        {
            await Task.Delay(100);
            this.Hide();
        }

        public override void WillAuthorizePayment(PKPaymentAuthorizationViewController controller)
        {

        }


        #endregion
    }
}
