// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange;
using Exchange.Model;
using WebRestApi.Controllers;

namespace WebRestApi.Helper
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse GlyphConverter. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class GlyphConverter
    {
        /// <summary>
        ///     Glyph laden und bei bedarf erzeugen
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ExGlyphData GetGlyph(ExGlyphDataPost data)
        {
            var result = new ExGlyphData();
            result.ImageUri = Constants.ServiceClientEndPointWithApiPrefix + nameof(ShopController.GlyphToIcon) + $"/{data.FontCodeNumber}" +
                              $"/{data.PinColor.ToArgb()}" +
                              $"/{data.GlyphColor.ToArgb()}" +
                              $"/{data.BackgroundColor.ToArgb()}" +
                              $"/{data.SquaredSize}" +
                              $"/{data.PinOnly}";

            string fileName;

            if (data.PinOnly)
                fileName = $"Pin{data.PinColor.ToArgb()}_{data.SquaredSize}.png";
            else
                fileName = $"Pin{data.GlyphColor.ToArgb()}_{data.PinColor.ToArgb()}_{data.BackgroundColor.ToArgb()}_{data.FontCode.Trim()}_{data.SquaredSize}.png";

            result.Name = fileName;
            string localFile = Path.Combine(AppContext.BaseDirectory, "Images", fileName);

            if (File.Exists(localFile))
            {
                result.ImageData = File.ReadAllBytes(localFile);

                return result;
            }

            Bitmap image;
            if (data.PinOnly)
            {
                image = GetColoredPin(data.PinColor, data.SquaredSize);
            }
            else
            {
                string fontFamiliesFileName = Path.Combine("Resources", "DigitalShopping.ttf");
                var fontfamily = GetFontFamilyFromFile("DigitalShopping", fontFamiliesFileName);
                image = GeneratePin(data.PinColor, data.FontCode, fontfamily, data.GlyphColor, data.BackgroundColor, data.SquaredSize);
            }

            // Nochmal checken, ob nicht in der Zwischenzeit ein anderer Thread das Bild erzeugt hat  
            if (File.Exists(localFile))
            {
                result.ImageData = File.ReadAllBytes(localFile);

                return result;
            }

            try
            {
                image.Save(localFile);
                using (var stream = new MemoryStream())
                {
                    image.Save(stream, ImageFormat.Png);
                    result.ImageData = stream.ToArray();
                }
            }
            catch (Exception e)
            {
                Logging.Log.LogError($"Fehler beim erzeugen des Glyphs! {e}");
            }

            if (!File.Exists(localFile))
            {
                using (var stream = new MemoryStream())
                {
                    image.Save(stream, ImageFormat.Png);
                    result.ImageData = stream.ToArray();
                }
            }

            return result;
        }


        /// <summary>
        ///     Gets a bitmap containing the specified text or glyph in the center of the bitmap.
        /// </summary>
        /// <param name="text">The text or glyph in the center of the bitmap.</param>
        /// <param name="fontFamily">The font family of the text or glyph.</param>
        /// <param name="textColor">The text color of the text or glyph.</param>
        /// <param name="backgroundColor">The background color of the bitmap.</param>
        /// <param name="width">The width of the bitmap.</param>
        /// <param name="height">The height of the bitmap.</param>
        /// <returns>A bitmap containing the specified text or glyph in the center of the bitmap.</returns>
        public static Bitmap TextToBitmap(string text, string fontFamily, Color textColor, Color backgroundColor, int width = 64, int height = 64)
        {
            var resultingBitMap = new Bitmap(width, height);

            using (Graphics grp = Graphics.FromImage(resultingBitMap))
            {
                // draw background
                Brush backgroundBrush = new SolidBrush(backgroundColor);
                grp.Clear(backgroundColor);

                // prepare text drawing
                Brush textBrush = new SolidBrush(textColor);
                Font font = new Font(fontFamily, (float) (Math.Min(width, height) / 2.5), FontStyle.Regular, GraphicsUnit.Pixel);
                SizeF textSize = grp.MeasureString(text, font);

                // set position of glyph to the middle of the bitmap
                Point position = new Point((resultingBitMap.Width / 2) - (int) textSize.Width / 2,
                    (resultingBitMap.Height / 2) - (int) textSize.Height / 2);

                // draw text (glyph)
                grp.DrawString(text, font, textBrush, position);
            }

            return resultingBitMap;
        }


        /// <summary>
        ///     Generates a Pin as Bitmap in squared format.
        /// </summary>
        /// <param name="pinColor">The color of the pin.</param>
        /// <param name="text">The text or glyph inside the pin.</param>
        /// <param name="fontFamily">The font family of the text or glyph.</param>
        /// <param name="textColor">The color of the text or glyph.</param>
        /// <param name="backgroundColor">The background color around the pin (consider Color.Transparent!).</param>
        /// <param name="squaredSize">The width and height of the bitmap.</param>
        /// <returns></returns>
        public static Bitmap GeneratePin(Color pinColor, string text, FontFamily fontFamily, Color textColor, Color backgroundColor, int squaredSize = 64)
        {
            var resultingBitMap = new Bitmap(squaredSize, squaredSize);

            using (var bitmap = GetColoredPin(pinColor, squaredSize))
            using (Graphics grp = Graphics.FromImage(resultingBitMap))
            {
                // draw background
                Brush backgroundBrush = new SolidBrush(backgroundColor);
                grp.Clear(backgroundColor);

                var squaredScaledBitmap = ScaleBitMap(SquareBitMap(bitmap), squaredSize, squaredSize);

                // draw background image
                grp.DrawImage(squaredScaledBitmap, 0, 0);
                squaredScaledBitmap.Dispose();

                // prepare text drawing
                Brush textBrush = new SolidBrush(textColor);
                Font font = new Font(fontFamily, (float) (squaredSize / 2.5), FontStyle.Regular, GraphicsUnit.Pixel);
                SizeF textSize = grp.MeasureString(text, font);

                // set position of glyph to the middle of the bitmap
                Point position = new Point((resultingBitMap.Width / 2) - (int) textSize.Width / 2,
                    (resultingBitMap.Height / 3) - (int) textSize.Height / 2);

                // draw text (glyph)
                grp.DrawString(text, font, textBrush, position);
            }

            return resultingBitMap;
        }

        /// <summary>
        ///     Gets the font families contained in the specified file name.
        /// </summary>
        /// <param name="fileName">The file name containing the font families.</param>
        /// <returns>All font families contained in the specified name.</returns>
        public static FontFamily[] GetFontFamiliesFromFile(string fileName)
        {
            var coll = new PrivateFontCollection();
            coll.AddFontFile(fileName);

            return coll.Families;
        }

        /// <summary>
        ///     Gets the font family with the specified name from the specified file.
        /// </summary>
        /// <param name="familyName">The name of the font familiy.</param>
        /// <param name="fileName">The file where to get the font family from.</param>
        /// <returns>The font family with the specified name from the specified file.</returns>
        public static FontFamily GetFontFamilyFromFile(string familyName, string fileName)
        {
            var families = GetFontFamiliesFromFile(fileName);

            return families.FirstOrDefault(f => f.Name == familyName);
        }

        /// <summary>
        ///     Gets a squared bitmap of the given bitmap given as file name.
        /// </summary>
        /// <param name="fileName">The file name of the bitmap.</param>
        /// <returns>A squared bitmap, i.e. same width and height.</returns>
        public static Bitmap SquareBitMap(string fileName)
        {
            Bitmap resultingBitMap;

            using (var bitmap = Image.FromFile(fileName))
            {
                var squareSize = Math.Max(bitmap.Width, bitmap.Height);

                resultingBitMap = new Bitmap(squareSize, squareSize);

                using (Graphics grp = Graphics.FromImage(resultingBitMap))
                {
                    grp.Clear(Color.Transparent);

                    grp.DrawImage(bitmap, (squareSize - bitmap.Width) / 2, (squareSize - bitmap.Height) / 2);
                }
            }

            return resultingBitMap;
        }

        /// <summary>
        ///     Gets a squared bitmap of the given bitmap.
        /// </summary>
        /// <param name="bitmap">The source bitmap.</param>
        /// <returns>A squared bitmap, i.e. same width and height.</returns>
        public static Bitmap SquareBitMap(Bitmap bitmap)
        {
            Bitmap resultingBitMap;

            var squareSize = Math.Max(bitmap.Width, bitmap.Height);

            resultingBitMap = new Bitmap(squareSize, squareSize);

            using (Graphics grp = Graphics.FromImage(resultingBitMap))
            {
                grp.Clear(Color.Transparent);

                grp.DrawImage(bitmap, (squareSize - bitmap.Width) / 2, (squareSize - bitmap.Height) / 2);
            }

            return resultingBitMap;
        }

        /// <summary>
        ///     Gets a bitmap scaled to the specified width and height.
        /// </summary>
        /// <param name="sourceBitmap">The bitmap to be scaled.</param>
        /// <param name="width">The width of the new scaled bitmap.</param>
        /// <param name="height">The height of the new scaled bitmap.</param>
        /// <returns>A new bitmap scaled to the specified width and height.</returns>
        public static Bitmap ScaleBitMap(Bitmap sourceBitmap, int width = 64, int height = 64)
        {
            return new Bitmap(sourceBitmap, width, height);
        }

        /// <summary>
        ///     Gets a bitmap of the pin in the specified color.
        /// </summary>
        /// <param name="color">The color of the pin.</param>
        /// <returns>A bitmap containing a pin in the specified color.</returns>
        public static Bitmap GetColoredPin(Color color, int squaredSize = 64)
        {
            //var bitmap = Properties.ResourcesPin.redPinNoBorder;
            var bitmap = new Bitmap(Image.FromFile(Path.Combine(AppContext.BaseDirectory, "Resources", "redPinNoBorder.png")));

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    var pixColor = bitmap.GetPixel(i, j);

                    if (pixColor.R == 255 && pixColor.G == 0 && pixColor.B == 0)
                    {
                        bitmap.SetPixel(i, j, color);
                    }
                }
            }

            var resultingBitMap = new Bitmap(squaredSize, squaredSize);
            using (Graphics grp = Graphics.FromImage(resultingBitMap))
            {
                // draw background
                Brush backgroundBrush = new SolidBrush(Color.Transparent);
                grp.Clear(Color.Transparent);

                var squaredScaledBitmap = ScaleBitMap(SquareBitMap(bitmap), squaredSize, squaredSize);

                // draw background image
                grp.DrawImage(squaredScaledBitmap, 0, 0);
                squaredScaledBitmap.Dispose();
            }

            return resultingBitMap;
        }
    }
}