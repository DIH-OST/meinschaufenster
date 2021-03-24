// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Android.Runtime
{
    /// <summary>
    ///     <para>Preserve Attribute für den Linker</para>
    ///     Klasse PreserveAttribute. (C) 2018 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public sealed class PreserveAttribute : Attribute
    {
        public bool AllMembers;
        public bool Conditional;
    }
}