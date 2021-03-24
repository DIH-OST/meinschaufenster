// DigitalesSchaufenster (C) 2020 DIH-OST

using Biss.Apps.Components.Map.Base;
using Biss.Apps.Components.Map.IOs;
using IOsApp;

[assembly: ExportRenderer(typeof(BissMap), typeof(IOsBissMapRenderer))]

namespace IOsApp
{
    /// <summary>
    ///     <para>Renderer der Karte für iOS Workaround damit der Renderer geht</para>
    ///     Klasse BissMapRenderer. (C) 2018 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class IOsBissMapRenderer : BissMapRenderer
    {
    }
}