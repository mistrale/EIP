﻿

#pragma checksum "C:\Users\audes_000\Documents\eip\EIP\Caritathelp\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "355B3625A137ECED4D7DF91B8730578B"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Caritathelp
{
    partial class MainPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 19 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).GotFocus += this.TextBox_GotFocus;
                 #line default
                 #line hidden
                #line 25 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.TextBox)(target)).TextChanged += this.Email_TextChanged;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 33 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).GotFocus += this.PasswordBox_GotFocus;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 45 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.Connexion_click;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 47 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.HyperlinkButton_forgettenPassword;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 59 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.Inscription_click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


