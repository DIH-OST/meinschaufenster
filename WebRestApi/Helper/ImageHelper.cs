// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace WebRestApi.Helper
{
    /// <summary>
    ///     <para>Helper for processing images</para>
    ///     Klasse ImageHelper. (C) 2019 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class ImageHelper
    {
        public static byte[] ReduceImage(byte[] imageBytes, int maxWidth = 400)
        {
            if (imageBytes == null || imageBytes.Length <= 0)
                return null;

            // Format is automatically detected though can be changed.
            ISupportedImageFormat format = new PngFormat {Quality = 70};
            Size size = new Size(maxWidth, 0);
            using (MemoryStream inStream = new MemoryStream(imageBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                    using (ImageFactory imageFactory = new ImageFactory(true))
                    {
                        // Load, resize, set the format and quality and save an image.
                        imageFactory.Load(inStream)
                            .Resize(size)
                            .Format(format)
                            .Save(outStream);
                    }

                    // Do something with the stream.
                    return outStream.ToArray();
                }
            }
        }
    }
}