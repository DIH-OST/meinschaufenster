// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange.Model;
using Exchange.Resources;

namespace BaseApp.ViewModel.UiModel
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse UiMeeting. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class UiMeeting : INotifyPropertyChanged
    {
        private ImageSource _staffImageSource;

        public UiMeeting(ExMeeting meeting)
        {
            Meeting = meeting;
            Hours = $"{Meeting.Start.ToLocalTime().ToString("HH:mm")} - {Meeting.End.ToLocalTime().ToString("HH:mm")}";
            Date = Meeting.Start.Date.Equals(Meeting.End.ToLocalTime().Date) ? Meeting.Start.ToLocalTime().ToString("dd.MM.yy") : $"{Meeting.Start.ToLocalTime().Date.ToShortDateString()} - {Meeting.End.ToLocalTime().Date.ToString("dd.MM.yy")}";

            try
            {
                //if (Device.RuntimePlatform == Device.iOS)
                //{
                //    var client = new WebClient();
                //    var bytes = client.DownloadData(_meeting.Staff.ImageUrl);
                //    StaffImageSource = ImageSource.FromStream(() => new MemoryStream(bytes));
                //    client.Dispose();
                //}
                //else
                //{
                var uri = new Uri(Meeting.Staff.ImageUrl);
                StaffImageSource = ImageSource.FromUri(uri);
                //}
            }
            catch (Exception e)
            {
                StaffImageSource = ImageSource.FromStream(() => Images.ReadImageAsStream(EmbeddedImages.DefaultUserImage_png));
            }
        }

        #region Properties

        public ExMeeting Meeting { get; }

        public string Hours { get; }

        public string Date { get; }

        public ImageSource StaffImageSource
        {
            get
            {
                try
                {
                    return _staffImageSource;
                }
                catch (Exception e)
                {
                    Logging.Log.LogError($"There was an error getting the imageSource for employee {Meeting.Staff.Id}. Exception {e}");
                    return ImageSource.FromStream(() => Images.ReadImageAsStream(EmbeddedImages.DefaultUserImage_png));
                }
            }
            private set => _staffImageSource = value;
        }

        #endregion

#pragma warning disable 0067
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }
}