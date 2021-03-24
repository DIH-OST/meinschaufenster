// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange.Model;

namespace Exchange.Interfaces
{
    /// <summary>
    ///     <para>Projektbezogene Telerik Berichte</para>
    ///     Klasse IProjectReports. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public interface IProjectReports
    {
        /// <summary>
        ///     Bericht für die Telefonlist (DEMO)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        object GeneratePhoneListReport(ExPhoneListData data);
    }
}