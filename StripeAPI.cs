using Stripe;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveRoulette
{
    public class StripeAPI
    {
        //Dev Test
        public string APIKey = "rk_test_51If5I8JXvUYCCEaPhNSLS6Ge0fUj5HQtHNkFmYyNoZc8R5k2Dg5fGKvhoyVVlVsyl6J80Si0Qe8rfP1MUq1Jw7ng0003ZOgmOA";
        public string ProductId = "prod_JbUvBFiohUGwbr";

        //AIS TEST
        //public string APIKey = "rk_test_51IftpUDbJrg6YR0nECG9RTRSJXYasiA7LHEFBBsGWLNjC8dLxbLrTgfvTHH65OTwtCYU82LBCepHmKM7soz5Y98u00oMgkq4oG";
        //public string ProductId = "prod_JbUcaTnEIxwDt3";

        //AIS LIVE
        //public string APIKey = "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";
        //public string ProductId = "prod_JvTbvJnAp6Q7WC";


        public string PriceId = "";
        public Subscription m_subscription;
        public Customer m_customer;
        public ProductInfo feeProductInfo;
        public StripeAPI()
        {
            StripeConfiguration.SetApiKey(APIKey);
            feeProductInfo = new ProductInfo();
            feeProductInfo = GetProduct();

            Thread subscriptionTh = new Thread(GetSubscription);
            subscriptionTh.Start();
            subscriptionTh.IsBackground = true;
        }
        public bool CheckExistMail(string mailAddress, string mahcineID)
        {
            var options = new CustomerListOptions
            {
                Email = mailAddress,
            };
            var service = new CustomerService();
            StripeList<Customer> customers =  service.List(options);
            foreach(Customer cu in customers)
            {
                if (cu.Email == mailAddress)
                {
                    if (cu.Description != mahcineID)
                    {
                        MessageBox.Show("You haven't registerd this email on this computer.\nPlease register again.", "Notification");
                        return false;
                    }
                    m_customer = cu;
                    var subscriptionService = new SubscriptionService();
                    var subscriptionList = new SubscriptionListOptions
                    {
                        Customer = cu.Id
                    };
                    Global.card = GetCardInfo();
                    StripeList<Subscription> subscript_list =  subscriptionService.List(subscriptionList);
                    if (subscript_list != null && subscript_list.Count() != 0)
                        m_subscription = subscript_list.ElementAt(0);
                    else
                    {
                        MessageBox.Show("Something wrong. Please contact the seller.", "Notification");
                        return false;
                        // RegisterSubscription();
                    }
                    return true;
                }
            }
            MessageBox.Show("You haven't registered this email.\nPlease register email to use software.", "Notification");
            return false;
        }

        public CardInfo GetCardInfo()
        {
            CardInfo cardIn = new CardInfo();
            //Card card;
            //            CardService cardService = new CardService();
            //            card = cardService.Get(m_customer.Id, m_customer.DefaultSource.Id);
            //cardIn.cardNo = card.Brand + "  ---- " + card.Last4;
            //cardIn.expMonth = card.ExpMonth;
            //cardIn.expYear = card.ExpYear;

            var options_payment = new PaymentMethodListOptions
            {
                Customer = m_customer.Id,
                Type = "card",
            };
            var service_payment = new PaymentMethodService();
            StripeList<PaymentMethod> paymentMethods = service_payment.List(options_payment);
            string strBrand = char.ToUpper(paymentMethods.FirstOrDefault().Card.Brand[0]) + paymentMethods.FirstOrDefault().Card.Brand.Substring(1);
            cardIn.cardNo = strBrand + "  ---- " + paymentMethods.FirstOrDefault().Card.Last4;
            cardIn.expMonth = paymentMethods.FirstOrDefault().Card.ExpMonth;
            cardIn.expYear = paymentMethods.FirstOrDefault().Card.ExpYear;
            if (paymentMethods.Count() != 0)
                cardIn.Id = paymentMethods.FirstOrDefault().Id;

            return cardIn;
        }
        public bool CheckCardNo(CardInfo card_info)
        {
            StripeConfiguration.SetApiKey(APIKey);
            var options = new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Number = card_info.cardNo,
                    ExpMonth = card_info.expMonth,
                    ExpYear = card_info.expYear,
                    Cvc = card_info.cvc,
                },
            };
            var service = new TokenService();
            try { 
                Token tk = service.Create(options);
                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Notification");
            }
            return false;
        }

        public bool RegisterMail(string mailAddress, CardInfo card_info, string hwid)
        {
            bool isCreatedCu = false;
            var list_options = new CustomerListOptions
            {
                Limit = 10000,
            };
            var service = new CustomerService();
            StripeList<Customer> customers = service.List(list_options);
            foreach (Customer cu in customers)
            {
                if (cu.Email == mailAddress && cu.Description == hwid)
                {
                    m_customer = cu;
                    GetCurrentSubscription();
                    MessageBox.Show("You have already registered this email on this computer.\n Please try to login to use the software.", "Notification");
                    return true;
                }
            }
            var options = new CustomerCreateOptions()
            {
                Email = mailAddress,
                Description = hwid,
                Name = "Created by Roulette APP",
            };

            var cuService = new CustomerService();
            try
            {
                m_customer = cuService.Create(options);
                isCreatedCu = true;
                RegisterCard(card_info);
                RegisterSubscription();

                MessageBox.Show("Registration has been finished successfully!" + Environment.NewLine + $"You can this software to {GetCurrentTime().AddDays(feeProductInfo.metaDataTrialDay).ToString("yyyy-MM-dd")} in trial mode.", "Notification");
                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Notification");
                if (isCreatedCu)
                    cuService.Delete(m_customer.Id);
                m_customer = null;
            }
            return false;
        }
        public void RegisterCard(CardInfo card_info)
        {
            var options = new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Number = card_info.cardNo,
                    ExpMonth = card_info.expMonth,
                    ExpYear = card_info.expYear,
                    Cvc = card_info.cvc,
                },
            };
            var service = new TokenService();
            Token tk =  service.Create(options);
            
            var cardOptions = new CardCreateOptions()
            {
                Source = tk.Id,
            };
            var cardService = new CardService();
            cardService.Create(m_customer.Id, cardOptions);
        }
        private void RegisterSubscription()
        {
            var subscriptionOptions = new SubscriptionCreateOptions
            {
                Customer = m_customer.Id,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Price = PriceId,
                        Quantity = 1,
                    },
                },
                CollectionMethod = "send_invoice",
                DaysUntilDue = 30,
                TrialPeriodDays = feeProductInfo.metaDataTrialDay,
            };
            var subService = new SubscriptionService();
            m_subscription = subService.Create(subscriptionOptions);
        }
        public bool CheckTrialMode()
        {
            if (m_subscription.Status.ToLower() == "trialing")
                return true;
            return false;
        }

        public ProductInfo GetProduct()
        {
            ProductInfo proInfo = new ProductInfo();
            Price pri;
            Product pro;
            PriceService priService = new PriceService();

            ProductService proService = new ProductService();
            pro = proService.Get(ProductId);

            var options = new PriceListOptions { Product = pro.Id };
            var service = new PriceService();
            StripeList<Price> prices = service.List(options);
            PriceId = prices.ElementAt(0).Id;

            pri = priService.Get(PriceId);
            proInfo.intervalPeriod = pri.Recurring.Interval;
            proInfo.intervalDay = (int)pri.Recurring.IntervalCount;
            proInfo.price = pri.UnitAmount.Value;
            proInfo.currencyUnit = pri.Currency.ToUpper();
            
            if (pro.Metadata.ContainsKey("trial"))
                proInfo.metaDataTrialDay = int.Parse(pro.Metadata["trial"]);


            return proInfo;
        }

        public double GetRemainPaidDays()
        {
            double result = 0.0;

            DateTime dtEnd = m_subscription.CurrentPeriodEnd.ToLocalTime();
            DateTime dtNow = GetCurrentTime();
            if (dtNow >= dtEnd)
                return result;
            result = (dtEnd - dtNow).TotalDays;
            return result;
        }

        public double GetRemainTrialDays()
        {
            double result = 0;

            DateTime dtEnd = (DateTime)(m_subscription.TrialEnd?.ToLocalTime());
            DateTime dtNow = GetCurrentTime();
            if (dtNow >= dtEnd)
                return result;
            result = (dtEnd - dtNow).TotalDays;
            return result;
        }
        public string GetLastPaymentDate()
        {
            string result = "";

            var options = new PaymentIntentListOptions
            {
                Customer = m_customer.Id,
            };

            var service = new PaymentIntentService();
            StripeList<PaymentIntent> paymentIntents = service.List(
              options
            );
            DateTime dt = paymentIntents.ElementAt(0).Created.ToLocalTime();
            result = dt.ToString();
            return result;
        }

        public bool CheckInCompleteInvoice()
        {
            var InvoiceOption = new InvoiceListOptions
            {
                Customer = m_customer.Id,
            };
            var inv_service = new InvoiceService();
            StripeList<Invoice> invoiceList = inv_service.List(InvoiceOption);
            DateTime dt = DateTime.Now.AddMonths(-1);
            foreach(Invoice inv in invoiceList)
            {
                if (inv.Created < dt)
                    continue;

                if (inv.Status.ToLower() == "open" ||
                    (inv.Status.ToLower() == "draft" && inv.Created.ToLocalTime().Day == DateTime.Now.Day))
                {
                    if(inv.Total == feeProductInfo.price)
                        return true;

                    var options = new PriceListOptions { Product = ProductId };
                    var service = new PriceService();
                    StripeList<Price> prices = service.List(options);
                    foreach(Price sItem in prices)
                    {
                        if (sItem.UnitAmount.Value == inv.Total)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void CheckSubscriptionProduct()
        {
            var data = m_subscription.Items;
            if (data.ElementAt(0).Price.Id != PriceId)
            {
                var options = new SubscriptionUpdateOptions
                {
                    ProrationBehavior = "none",
                    Items = new List<SubscriptionItemOptions>()
                    {
                        new SubscriptionItemOptions
                        {
                            Price = PriceId,
                            Quantity = 1,
                        },
                    },
                };
                var service = new SubscriptionService();
                m_subscription = service.Update(m_subscription.Id, options);

                var delete_options = new SubscriptionItemDeleteOptions
                {
                    ProrationBehavior = "none"
                };
                var service1 = new SubscriptionItemService();
                service1.Delete(data.ElementAt(0).Id, delete_options);
            }
        }
        public void UpdateSubscription()
        {
            if (m_subscription.Status.ToLower() == "trialing")
            {
                var options = new SubscriptionUpdateOptions
                {
                    BillingCycleAnchor = SubscriptionBillingCycleAnchor.Now,
                    ProrationBehavior = "create_prorations",
                    TrialEnd = SubscriptionTrialEnd.Now,
                };
                var service = new SubscriptionService();
                m_subscription = service.Update(m_subscription.Id, options);
            }
            else
            {
                var options = new SubscriptionUpdateOptions
                {
                    BillingCycleAnchor = SubscriptionBillingCycleAnchor.Now,
                    ProrationBehavior = "create_prorations",
                };
                var service = new SubscriptionService();
                m_subscription = service.Update(m_subscription.Id, options);
            }
        }

        public bool UpdateCardInfo(CardInfo card_info)
        {
            try 
            {
                var options = new TokenCreateOptions
                {
                    Card = new TokenCardOptions
                    {
                        Number = card_info.cardNo,
                        ExpMonth = card_info.expMonth,
                        ExpYear = card_info.expYear,
                        Cvc = card_info.cvc,
                    },
                };
                var service = new TokenService();
                Token tk = service.Create(options);

                Card card = new Card();
                var options_update = new CardCreateOptions
                {
                    Source = tk.Id,
                };
                var service_update = new CardService();
                card = service_update.Create(m_customer.Id, options_update);


                var customerService = new CustomerService();
                customerService.Update(m_customer.Id, options: new CustomerUpdateOptions
                {
                    InvoiceSettings = new CustomerInvoiceSettingsOptions
                    {
                        DefaultPaymentMethod = card.Id,
                    }
                });

                service_update.Delete(m_customer.Id, Global.card.Id);
                Global.card = GetCardInfo();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message , "Notification");
            }

            return false;
        }
        public bool PayInCompleteInvoice(out string endDate, out string result)
        {
            endDate = "";
            result = "";
            string inv_id = "";
            DateTime dt = DateTime.Now.AddMonths(-1);
            bool useInvoice = true;
            var InvoiceOption = new InvoiceListOptions
            {
                Customer = m_customer.Id,
            };
            var inv_service = new InvoiceService();
            StripeList<Invoice> invoiceList = inv_service.List(InvoiceOption);

            foreach (Invoice invEle in invoiceList)
            {
                if (invEle.Created < dt)
                    continue;

                if (invEle.Status.ToLower() == "open" ||
                    ((invEle.Status.ToLower() == "draft" && invEle.Created.ToLocalTime().Day == DateTime.Now.Day)))
                {
                    inv_id = invEle.Id;
                    if (invEle.Total == feeProductInfo.price)
                        useInvoice = true;
                    else
                        useInvoice = false;
                    break;
                }
            }

            try { 
                if (useInvoice == false)
                {
                    if (Payment((long)feeProductInfo.price, out endDate, out result))
                    {
                        var service = new InvoiceService();
                        service.VoidInvoice(inv_id);
                    }
                }
                else if (useInvoice == true)
                { 
                    Invoice inv = inv_service.Pay(inv_id);
                    if (inv.Status.ToLower() == "paid")
                    {
                        UpdateSubscription();
                        endDate = m_subscription.CurrentPeriodEnd.ToLocalTime().ToString("yyyy-MM-dd");
                        result = "Payment has been successfully processed";

                        MessageBox.Show($"Payment has been successfully processed.\nYou can use this software until {endDate}.", "Notification");

                        return true;
                    }
                }
            }
            catch(Exception ex)
            {
                result = ex.Message;
            }
            return false;
        }

        public bool Payment(long payAmount, out string endDate, out string outResult)
        {
            endDate = "";
            outResult = "";
            var options = new ChargeCreateOptions
            {
                Amount = payAmount,
                Currency = "usd",
                Customer = m_customer.Id,
                
            };

            var service = new ChargeService();
            try { 
                Charge charge = service.Create(options);
                string response = charge.Status;
                if (response.ToLower() == "succeeded")
                {
                    UpdateSubscription();
                    endDate = m_subscription.CurrentPeriodEnd.ToLocalTime().ToString("yyyy-MM-dd");
                    return true;
                }
            }
            catch(Exception ex)
            {
                outResult = ex.Message;
            }

            return false;

        }
        
        public DateTime GetCurrentTime()
        {
            try
            {
                using (var response =
                  WebRequest.Create("http://www.google.com").GetResponse())
                    //string todaysDates =  response.Headers["date"];
                    return DateTime.ParseExact(response.Headers["date"],
                        "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                        CultureInfo.InvariantCulture.DateTimeFormat,
                        DateTimeStyles.AssumeUniversal);
            }
            catch (WebException)
            {
            }
            return DateTime.Now; //In case something goes wrong. 
        }

        private void GetCurrentSubscription()
        {
            var subscriptionService = new SubscriptionService();
            var subscriptionList = new SubscriptionListOptions
            {
                Customer = m_customer.Id
            };
            StripeList<Subscription> subscript_list = subscriptionService.List(subscriptionList);
            if (subscript_list != null && subscript_list.Count() != 0)
                m_subscription = subscript_list.ElementAt(0);

            CheckSubscriptionProduct();
        }

        public void UpcommingInvoice()
        {
            if (m_subscription == null)
                return;

            var options = new UpcomingInvoiceOptions
            {
                Customer = m_customer.Id,
            };
            var service = new InvoiceService();
            Invoice upcoming = service.Upcoming(options);
            Invoice up = service.Pay(upcoming.Id);
        }

        public void GetSubscription()
        {
            while(true)
            {
                if (m_customer != null)
                {
                    GetCurrentSubscription();
                }
                Thread.Sleep(1000 * 10);
            }
        }
    }
}
