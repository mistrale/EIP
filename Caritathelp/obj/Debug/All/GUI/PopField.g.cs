﻿

#pragma checksum "C:\Users\Sikorav\Desktop\EIP\Caritathelp\All\GUI\PopField.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "5CA5B74479F4F0EAF6FB158A316ED062"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Caritathelp.All.GUI
{
    partial class PopField : global::Windows.UI.Xaml.Controls.UserControl, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 49 "..\..\..\All\GUI\PopField.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.updateClick;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 38 "..\..\..\All\GUI\PopField.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.deleteClick;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 24 "..\..\..\All\GUI\PopField.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.closeWindow;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


