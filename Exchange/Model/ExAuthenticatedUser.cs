// DigitalesSchaufenster (C) 2020 DIH-OST

namespace Exchange.Model
{
    public class ExAuthenticatedUser
    {
        #region Properties

        /// <summary>
        ///     Benutzername
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     BenutzerID
        /// </summary>
        public long UserId { get; set; }

        #endregion
    }
}