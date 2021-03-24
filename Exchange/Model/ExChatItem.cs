// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Model
{
    /// <summary>
    ///     <para>ChatItem</para>
    ///     Klasse ExChatItem. (C) 2018 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExChatItem
    {
        #region Properties

        public ExChatUser Author { get; set; }

        public string Text { get; set; }

        #endregion
    }
}