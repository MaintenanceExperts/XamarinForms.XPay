using Foundation;
using PassKit;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public event EventHandler<Action<bool>> AuthorizePayment;
        public event EventHandler DidFinish;

        public async Task<bool> Show(XPayRequest request)
        {
            NSString[] paymentNetworks = new NSString[] { PKPaymentNetwork.MasterCard, PKPaymentNetwork.Visa };

            if (!PKPaymentAuthorizationViewController.CanMakePayments || !PKPaymentAuthorizationViewController.CanMakePaymentsUsingNetworks(paymentNetworks))
            {
                throw new NotSupportedException();
            }

            List<PKPaymentSummaryItem> items = new List<PKPaymentSummaryItem>();
            for (int i = 0; i < request.Items.Count; i++)
            {
                XPayRequestItem requestItem = request.Items[i];
                items.Add(PKPaymentSummaryItem.Create(requestItem.Label, new NSDecimalNumber(requestItem.Amount.ToString())));
            }

            // add the total PKPaymentSummaryItem by summing together all the amounts
            // it will add a Pay To "GitBit" or whatever label you put in here
            items.Add(PKPaymentSummaryItem.Create("GitBit", new NSDecimalNumber(items.Sum(x => x.Amount.FloatValue).ToString())));

            PKPaymentRequest paymentRequest = new PKPaymentRequest();
            paymentRequest.PaymentSummaryItems = items.ToArray();
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
            this.AuthorizePayment?.Invoke(this, new Action<bool>((auth) =>
            {
                if (auth)
                {
                    completion(PKPaymentAuthorizationStatus.Success);
                }
                else
                {
                    completion(PKPaymentAuthorizationStatus.Failure);
                }
            }));
        }

        public override void PaymentAuthorizationViewControllerDidFinish(PKPaymentAuthorizationViewController controller)
        {
            this.DidFinish?.Invoke(this, new EventArgs());
        }

        public override void WillAuthorizePayment(PKPaymentAuthorizationViewController controller) { }


        #endregion
    }
}
