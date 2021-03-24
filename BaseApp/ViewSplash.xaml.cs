// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange.Resources;

namespace BaseApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class ViewSplash : ContentPage
    {
        #region Nested Types

        internal class ViewModelSplash : ProjectViewModelBase
        {
            public ViewModelSplash() : base("")
            {
            }
        }

        #endregion


        public ViewSplash(VmFiles vmFiles)
        {
            var img = Images.ReadImage(Device.Idiom == TargetIdiom.Desktop || Device.Idiom == TargetIdiom.Tablet || Device.Idiom == TargetIdiom.TV
                ? EmbeddedImages.SplashScreenHorizontal_png
                : EmbeddedImages.SplashScreenVertical_png);

            InitializeComponent();
            SplashImage.Source = ImageSource.FromStream(() => new MemoryStream(img.Image));

            VmBase.CManager.InitDone += CManagerOnInitDone;
            VmBase.CManager.InitHigh();
        }


        private void CManagerOnInitDone(object sender, ComponentInitFinishEventArgs e)
        {
            VmBase.CManager.InitDone -= CManagerOnInitDone;

            Task.Run(async () =>
            {
                var vm = new ViewModelSplash();
                await vm.AppCenter.TrackEventAsync("App started ...");
            });


            ProjectViewModelBase.InitializeApp();
            ProjectViewModelBase.LaunchFirstView();
        }
    }
}