// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Resources
{
    /// <summary>
    ///     <para>Culture der Resource Files via Code setzen</para>
    ///     Klasse Language. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class Language
    {
        /// <summary>
        ///     Unterstütze Sprachen (erste Sprache ist die Default Sprache welche verwendet wird als Fallback)
        /// </summary>
        public static List<string> SupportedLanguages = new List<string> {"en", "de", "es"};

        /// <summary>
        ///     Aktuelle Kultur
        /// </summary>
        public static CultureInfo CurrentCulture = CultureInfo.CurrentCulture;

        /// <summary>
        ///     Aktuelle Kultur des Gerätes
        /// </summary>
        public static CultureInfo CurrentDeviceCulture = null;

        /// <summary>
        ///     Resource Files auf neue Kultur setzen
        /// </summary>
        /// <param name="culture"></param>
        public static void SetLanguage(CultureInfo culture)
        {
            CurrentCulture = culture;

            ResView.Culture = culture;
            ResViewMenu.Culture = culture;
            ResViewSettings.Culture = culture;
            ResWebCommon.Culture = culture;
        }

        /// <summary>
        ///     Texte welche im Apps.Base verwendet werden
        /// </summary>
        /// <returns></returns>
        public static ExLanguageContent GetText()
        {
            return new ExLanguageContent(
                ResView.Command_Back,
                ResView.Command_Continue,
                ResView.Command_Login,
                ResView.Command_ResendPassword,
                ResView.MsgBox_CancelText,
                ResView.MsgBox_DataNotSaved,
                ResView.MsgBox_HeaderNoInternet,
                ResView.MsgBox_NewPasswordSent,
                ResView.MsgBox_NoInternet,
                ResView.MsgBox_NoText,
                ResView.MsgBox_OkText,
                ResView.MsgBox_SaveError,
                ResView.MsgBox_SaveSuccess,
                ResView.MsgBox_ServerFail,
                ResView.MsgBox_ServerTimeout,
                ResView.MsgBox_ServerTokenFail,
                ResView.MsgBox_ServerMultiConnectionFail,
                ResView.MsgBox_YesText,
                ResView.MsgBoxHeader_DataNotSaved,
                ResView.MsgBoxHeader_SaveError,
                ResView.MsgBoxHeader_SaveSuccess,
                ResView.MsgBoxHeader_ServerRestError,
                ResView.MsgBoxNewUpdate,
                ResView.MsgBoxNewUpdateError,
                ResView.MsgBoxNewUpdateMandatory,
                ResView.MsgBoxTitleUpdateAvailable,
                ResView.MsgBoxCameraAccess,
                ResView.MsgBoxCameraAccessTitle,
                ResView.MsgBoxCameraAccessInfo,
                ResView.MsgBoxCameraAccessInfoTitle,
                ResView.MsgBoxFileAccess,
                ResView.MsgBoxFileAccessTitle,
                ResView.MsgBoxFileAccessInfo,
                ResView.MsgBoxFileAccessInfoTitle,
                ResView.MsgBoxLocationAccess,
                ResView.MsgBoxLocationAccessTitle,
                ResView.MsgBoxLocationAccessInfo,
                ResView.MsgBoxLocationAccessInfoTitle
            );
        }
    }
}