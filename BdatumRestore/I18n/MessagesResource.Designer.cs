﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BdatumRestore.I18n {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class MessagesResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal MessagesResource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("BdatumRestore.I18n.MessagesResource", typeof(MessagesResource).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ocorreu um erro de autenticação, verifique suas credenciais e sua conexão..
        /// </summary>
        internal static string ConectionOrAuthError {
            get {
                return ResourceManager.GetString("ConectionOrAuthError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Erro no download..
        /// </summary>
        internal static string DownloadErrorGeneric {
            get {
                return ResourceManager.GetString("DownloadErrorGeneric", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Você precisa selecionar uma pasta ou arquivos para fazer restore ou não selecionou uma pasta para gravar os arquivos..
        /// </summary>
        internal static string NoDirOrFileSelected {
            get {
                return ResourceManager.GetString("NoDirOrFileSelected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Nem um arquivo encontrado..
        /// </summary>
        internal static string NoFileFound {
            get {
                return ResourceManager.GetString("NoFileFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to O tamanho do caminho do arquivo é muito grande.
        /// </summary>
        internal static string PathTooLong {
            get {
                return ResourceManager.GetString("PathTooLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Você deseja interromper o restore?
        ///Você pode retoma-lo na proxima vez que executar o programa..
        /// </summary>
        internal static string PausedDownloadMessage {
            get {
                return ResourceManager.GetString("PausedDownloadMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Você tem um restore pendente. Deseja retoma-lo?.
        /// </summary>
        internal static string PendingRestore {
            get {
                return ResourceManager.GetString("PendingRestore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Preparando Download..
        /// </summary>
        internal static string PreparingDownload {
            get {
                return ResourceManager.GetString("PreparingDownload", resourceCulture);
            }
        }
    }
}
