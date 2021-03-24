// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange.Resources;

namespace BaseApp.MarkupExtensions
{
    /// <summary>
    ///     <para>TranslateExtension</para>
    ///     Klasse TranslateExtension. (C) 2018 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [ContentProperty(nameof(Key))]
    public class TranslateExtension : IMarkupExtension
    {
        const string ResourceNamespace = "Exchange.Resources";
        public static Dictionary<string, ResourceManager> Res = new Dictionary<string, ResourceManager>();
        readonly CultureInfo _ci;

        public TranslateExtension()
        {
            _ci = Language.CurrentCulture;
        }

        #region Properties

        /// <summary>
        ///     Key im Format "ResViewNAME.ID"
        /// </summary>
        public string Key { get; set; }

        #endregion

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            string translation = String.Empty;

            var tmp = Key.Split('.');
            if (tmp.Length != 2) throw new ArgumentException("Language key invalid!");

            string resourceId = $"{ResourceNamespace}.{tmp[0].Trim()}";
            if (!Res.ContainsKey(resourceId))
            {
                ResourceManager res = null;
                try
                {
                    res = new ResourceManager(resourceId, Assembly.GetAssembly(typeof(Language)));
                }
                catch
                {
                    ;
                }

                Res.Add(resourceId, res);
            }

            var resMgr = Res[resourceId];
            if (resMgr == null)
            {
#if DEBUG
                throw new ArgumentException(string.Format($"Resource {resourceId} not found!"));
#else
				translation = Key; // HACK: returns the key, which GETS DISPLAYED TO THE USER
#endif
            }

            string key = tmp[1].Trim();

            if (String.IsNullOrEmpty(key))
                return string.Empty;

            translation = resMgr.GetString(key, _ci);
            if (String.IsNullOrEmpty(translation))
            {
#if DEBUG
                throw new ArgumentException(
                    string.Format("Key '{0}' was not found in resources '{1}' for culture '{2}'.", key, resourceId, _ci.Name),
                    "Text");
#else
				translation = Key; // HACK: returns the key, which GETS DISPLAYED TO THE USER
#endif
            }

            return translation;
        }
    }
}