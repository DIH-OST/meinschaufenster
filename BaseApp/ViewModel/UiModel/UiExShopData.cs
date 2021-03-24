// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange.Model;
using Exchange.Resources;

namespace BaseApp.ViewModel.UiModel
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse UiExShopData. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class UiExShopData : INotifyPropertyChanged
    {
        private ImageSource _imageSource;

        public UiExShopData(ExShop shop)
        {
            Shop = shop;

            try
            {
                if (string.IsNullOrWhiteSpace(Shop.ImageUrl))
                {
                    ImageSource = ImageSource.FromStream(() => Images.ReadImageAsStream(EmbeddedImages.Logo_png));
                }
                else
                {
                    //if (Device.RuntimePlatform == Device.iOS)
                    //{
                    //    var client = new WebClient();
                    //    var bytes = client.DownloadData(_shop.ImageUrl);
                    //    ImageSource = ImageSource.FromStream(() => new MemoryStream(bytes));
                    //    client.Dispose();
                    //}
                    //else
                    ImageSource = ImageSource.FromUri(new Uri(Shop.ImageUrl));
                }
            }
            catch (Exception e)
            {
                ImageSource = ImageSource.FromStream(() => Images.ReadImageAsStream(EmbeddedImages.Logo_png));
            }

            try
            {
                var sorted = Shop.OpeningHours.OrderByDescending(s => s.Day.DayOfWeek).Reverse().ToList();
                var sunday = sorted.FirstOrDefault();
                sorted.Remove(sunday);
                sorted.Add(sunday);
                Shop.OpeningHours = sorted;
            }
            catch (Exception e)
            {
                Logging.Log.LogError(e.Message);
            }
        }

        #region Properties

        public ExShop Shop { get; }

        public string Categories
        {
            get
            {
                try
                {
                    var categories = Shop.MainCategory.Name;

                    if (string.IsNullOrWhiteSpace(categories))
                    {
                        if (Shop.Categories != null && Shop.Categories.Any() && !string.IsNullOrWhiteSpace(Shop.Categories.FirstOrDefault().Name))
                        {
                            return Shop.Categories.FirstOrDefault().Name;
                        }
                    }
                    else
                    {
                        return "";
                    }

                    return categories;
                }
                catch (Exception e)
                {
                    return "";
                }
            }
        }

        public string Payments
        {
            get
            {
                try
                {
                    var payments = string.Empty;
                    if (Shop.PaymentMethods.Count > 0)
                    {
                        foreach (var paymentMethod in Shop.PaymentMethods)
                        {
                            payments += $"{char.ConvertFromUtf32(Convert.ToUInt16(paymentMethod.Glyph, 16))} ";
                        }

                        return payments;
                    }
                }
                catch (Exception e)
                {
                    return string.Empty;
                }

                return string.Empty;
            }
        }

        public ImageSource ImageSource
        {
            get
            {
                try
                {
                    return _imageSource;
                }
                catch (Exception e)
                {
                    Logging.Log.LogError($"Getting ImageSource for Shop {Shop.Id} failed. Exception {e}");
                    return ImageSource.FromStream(() => Images.ReadImageAsStream(EmbeddedImages.Logo_png));
                }
            }
            private set => _imageSource = value;
        }

        #endregion

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}