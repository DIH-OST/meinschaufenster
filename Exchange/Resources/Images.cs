// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

// ReSharper disable InconsistentNaming

namespace Exchange.Resources
{
    public enum EmbeddedImages
    {
        Logo_png,
        DefaultUserImage_png,
        Pin_png,
        SplashScreenHorizontal_png,
        SplashScreenVertical_png,
        AppIconTransparent_png,
        DefaultAvatar_png,
        WhatsAppAnleitung_png
    }

    /// <summary>
    ///     <para>Bilder laden (Projektweit)</para>
    ///     Klasse Images. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class Images
    {
        public static ExRawImage ReadImage(EmbeddedImages imageName)
        {
            string im = imageName.ToString();
            var r = new ExRawImage
                    {
                        ImageName = $"{im.Replace("_", ".")}"
                    };

            var image = $"Exchange.Resources.Images.{imageName.ToString().Replace("_", ".")}";
            Assembly _assembly = Assembly.Load(new AssemblyName("Exchange"));
            Stream _imageStream = _assembly.GetManifestResourceStream(image);
            using (MemoryStream ms = new MemoryStream())
            {
                _imageStream.CopyTo(ms);
                r.Image = ms.ToArray();
            }

            return r;
        }

        public static Stream ReadImageAsStream(EmbeddedImages imageName)
        {
            string im = imageName.ToString();
            var r = new ExRawImage
                    {
                        ImageName = $"{im.Replace("_", ".")}"
                    };

            var image = $"Exchange.Resources.Images.{imageName.ToString().Replace("_", ".")}";
            Assembly _assembly = Assembly.Load(new AssemblyName("Exchange"));
            Stream _imageStream = _assembly.GetManifestResourceStream(image);

            return _imageStream;
        }
    }
}