// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace BaseApp.View.Custom
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse BissTabViewHeaderItem. (C) 2019 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class BissTabViewHeaderItem : TabViewHeaderItem
    {
        public static readonly BindableProperty GlyphProperty =
            BindableProperty.Create(nameof(Glyph), typeof(string), typeof(BissTabViewHeaderItem));

        #region Properties

        public string Glyph
        {
            get => (string) GetValue(GlyphProperty);
            set => SetValue(GlyphProperty, value);
        }

        #endregion
    }
}