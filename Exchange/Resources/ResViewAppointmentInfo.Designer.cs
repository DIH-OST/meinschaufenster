﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Exchange.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ResViewAppointmentInfo {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ResViewAppointmentInfo() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Exchange.Resources.ResViewAppointmentInfo", typeof(ResViewAppointmentInfo).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Abbrechen.
        /// </summary>
        public static string ButtonAbort {
            get {
                return ResourceManager.GetString("ButtonAbort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Vereinbaren.
        /// </summary>
        public static string ButtonAccept {
            get {
                return ResourceManager.GetString("ButtonAccept", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hast du Anmerkungen für den Händler?.
        /// </summary>
        public static string LblShopAdditionalInfo {
            get {
                return ResourceManager.GetString("LblShopAdditionalInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Problem.
        /// </summary>
        public static string MsgBoxCreateAppointmentCaption {
            get {
                return ResourceManager.GetString("MsgBoxCreateAppointmentCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Der Termin konnte leider nicht vereinbart werden. Bitte versuche es später nocheinmal..
        /// </summary>
        public static string MsgBoxCreateAppointmentError {
            get {
                return ResourceManager.GetString("MsgBoxCreateAppointmentError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Terminübersicht.
        /// </summary>
        public static string PageTitle {
            get {
                return ResourceManager.GetString("PageTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Anmerkungen hier eingeben.
        /// </summary>
        public static string PlaceholderDefaultAdditional {
            get {
                return ResourceManager.GetString("PlaceholderDefaultAdditional", resourceCulture);
            }
        }
    }
}
