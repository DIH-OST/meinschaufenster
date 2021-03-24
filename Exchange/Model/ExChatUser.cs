// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Model
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse ExChatUser. (C) 2018 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExChatUser : IBissSerialize
    {
        #region Properties

        public byte[] Image { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsOnline { get; set; }

        public DateTime? LastOnline { get; set; }

        public string Avatar { get; set; }

        public object Data { get; set; }

        [DependsOn(nameof(IsOnline))]
        public string IsOnlineUi
        {
            get
            {
                if (IsOnline) return "\uE965";
                return "\uE966";
            }
        }

        #endregion
    }
}