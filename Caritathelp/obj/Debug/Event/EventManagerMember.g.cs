﻿

#pragma checksum "C:\Users\audes\Documents\EIP\Caritathelp\Event\EventManagerMember.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "9FC55B5549D67EECAF15AC43ABC254A8"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Caritathelp.Event
{
    partial class EventManagerMember : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 20 "..\..\Event\EventManagerMember.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.inviteUser;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 24 "..\..\Event\EventManagerMember.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).GotFocus += this.searchTextBox_GotFocus;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 33 "..\..\Event\EventManagerMember.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.goBackClick;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


