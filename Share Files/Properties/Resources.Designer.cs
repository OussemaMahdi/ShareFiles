﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.34014
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Share_Files.Properties {
    using System;
    
    
    /// <summary>
    ///   Une classe de ressource fortement typée destinée, entre autres, à la consultation des chaînes localisées.
    /// </summary>
    // Cette classe a été générée automatiquement par la classe StronglyTypedResourceBuilder
    // à l'aide d'un outil, tel que ResGen ou Visual Studio.
    // Pour ajouter ou supprimer un membre, modifiez votre fichier .ResX, puis réexécutez ResGen
    // avec l'option /str ou régénérez votre projet VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Retourne l'instance ResourceManager mise en cache utilisée par cette classe.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Share_Files.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Remplace la propriété CurrentUICulture du thread actuel pour toutes
        ///   les recherches de ressources à l'aide de cette classe de ressource fortement typée.
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
        ///   Recherche une chaîne localisée semblable à &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;
        ///&lt;fichiers xmlns=&quot;http://tempuri.org/AnnexXMLshema.xsd&quot;&gt;
        ///  &lt;fichier&gt;
        ///    &lt;nom&gt;nom1&lt;/nom&gt;
        ///    &lt;path&gt;path1&lt;/path&gt;
        ///    &lt;createur&gt;createur1&lt;/createur&gt;
        ///    &lt;commentaire&gt;commentaire1&lt;/commentaire&gt;
        ///  &lt;/fichier&gt;
        ///&lt;/fichiers&gt;.
        /// </summary>
        internal static string Annex {
            get {
                return ResourceManager.GetString("Annex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;
        ///&lt;xs:schema id=&quot;AnnexXMLshema&quot;
        ///    targetNamespace=&quot;http://tempuri.org/AnnexXMLshema.xsd&quot;
        ///    elementFormDefault=&quot;qualified&quot;
        ///    xmlns=&quot;http://tempuri.org/AnnexXMLshema.xsd&quot;
        ///    xmlns:mstns=&quot;http://tempuri.org/AnnexXMLshema.xsd&quot;
        ///    xmlns:xs=&quot;http://www.w3.org/2001/XMLSchema&quot;
        ///&gt;
        ///
        ///  &lt;!-- definition of simple elements --&gt;
        ///  &lt;xs:element name=&quot;nom&quot; type=&quot;xs:string&quot;/&gt;
        ///  &lt;xs:element name=&quot;path&quot; type=&quot;xs:string&quot;/&gt;
        ///  &lt;xs:element name=&quot;createur&quot; type=&quot;xs:string&quot;/&gt;
        ///   [le reste de la chaîne a été tronqué]&quot;;.
        /// </summary>
        internal static string AnnexXMLshema {
            get {
                return ResourceManager.GetString("AnnexXMLshema", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une ressource localisée de type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap CercleRouge {
            get {
                object obj = ResourceManager.GetObject("CercleRouge", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Recherche une ressource localisée de type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap CercleVert {
            get {
                object obj = ResourceManager.GetObject("CercleVert", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Recherche une ressource localisée de type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap close_256 {
            get {
                object obj = ResourceManager.GetObject("close_256", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Recherche une ressource localisée de type System.Drawing.Icon semblable à (Icône).
        /// </summary>
        internal static System.Drawing.Icon iconpro {
            get {
                object obj = ResourceManager.GetObject("iconpro", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
        
        /// <summary>
        ///   Recherche une ressource localisée de type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap logo5 {
            get {
                object obj = ResourceManager.GetObject("logo5", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Recherche une ressource localisée de type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap logo51 {
            get {
                object obj = ResourceManager.GetObject("logo51", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
    }
}
