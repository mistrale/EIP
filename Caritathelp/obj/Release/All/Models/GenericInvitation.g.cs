﻿

#pragma checksum "C:\Users\Sikorav\Desktop\EIP\Caritathelp\All\Models\GenericInvitation.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7FCE742E2324A3B6929C9FCF349E5CF5"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Caritathelp.All.Models
{
    partial class GenericInvitation : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 15 "..\..\..\All\Models\GenericInvitation.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ToggleButton)(target)).Checked += this.receivedClick;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 16 "..\..\..\All\Models\GenericInvitation.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ToggleButton)(target)).Checked += this.sendClick;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 35 "..\..\..\All\Models\GenericInvitation.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.inviteVolunteer;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 40 "..\..\..\All\Models\GenericInvitation.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).GotFocus += this.TextBox_GotFocus;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}

