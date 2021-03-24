// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Enum
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse EnumFilterType. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public enum EnumFilterType
    {
        [Description("Alles")] All,
        [Description("Kategorien")] Categories,
        [Description("Bezahlmethoden")] PaymentMethods,
        [Description("Liefermöglichkeiten")] Deliveryoptions
    }


    public enum EnumUserType
    {
        Customer = 0,
        ShopEmployee = 1
    }
}