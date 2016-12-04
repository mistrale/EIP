using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Caritathelp.All.GUI.Message
{
    public sealed partial class ListMessageControl : UserControl
    {
        Page _page;
        private ConfirmBox cfBox;
        GUI.ErrorControl err;
        Newtonsoft.Json.Linq.JObject obj;
        Caritathelp.Message.MessageInfos _infos { get; set; }

        private  void leaveConversation(object sender, RoutedEventArgs e)
        {
            cfBox.Visibility = Visibility.Visible;
            cfBox.setRoutedEvent(leaveConversation_real);
        }


        private async void leaveConversation_real(object sender, RoutedEventArgs e)
        {
            HttpHandler http = HttpHandler.getHttp();
            Newtonsoft.Json.Linq.JObject obj = await http.sendRequest("/chatrooms/" + _infos.id + "/leave", null, HttpHandler.TypeRequest.DELETE);
            if ((int)obj["status"] != 200)
            {
                err.printMessage((string)obj["message"], ErrorControl.Code.FAILURE);
            } else
            {
                _page.Frame.Navigate(typeof(Caritathelp.Message.Message));
            }
        }

        private void GoToMessageOptionsClick(object sender, RoutedEventArgs e)
        {
            _page.Frame.Navigate(typeof(Caritathelp.Message.UpdageMessage), _infos);
        }

        private void GoToMessageClick(object sender, RoutedEventArgs e)
        {
            _page.Frame.Navigate(typeof(Caritathelp.Message.MessageProfil), _infos);
        }

        public ListMessageControl(Caritathelp.Message.MessageInfos infos, Page page, GUI.ErrorControl err, ConfirmBox cf)
        {
            this.InitializeComponent();
            this._infos = infos;
            this.cfBox = cf;
            this._page = page;
            this.err = err;
            conversationButton.Content = _infos.name;
        }
    }
}
