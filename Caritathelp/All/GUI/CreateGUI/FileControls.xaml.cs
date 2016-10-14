using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Caritathelp.All.GUI.CreateGUI
{
    public sealed partial class FileControls : UserControl
    {
        public class ImageSourcePath : INotifyPropertyChanged
        {
            private string _path;

            public event PropertyChangedEventHandler PropertyChanged;

            public String PathToImage
            {
                get { return _path; }
                set
                {
                    _path = value;
                    NotifyPropertyChanged("PathToImage");
                }
            }

            public void NotifyPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this,
                        new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        string Base64String;
        GUI.ErrorControl err;
        CoreApplicationView view;
        StorageFile storageFileWP;
        Binding myBinding;
        public static ImageSourcePath filePath;
        Image file;

        private async void viewActivated(CoreApplicationView sender, IActivatedEventArgs args1)
        {
            FileOpenPickerContinuationEventArgs args = args1 as FileOpenPickerContinuationEventArgs;

            if (args != null)
            {
                if (args.Files.Count == 0) return;

                view.Activated -= viewActivated;
                storageFileWP = args.Files[0];

            }
            Debug.WriteLine("testasdasdasd");
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
            () =>
            {
                filePath.PathToImage = storageFileWP.Name.ToString();
            });
        }

        public void chooseFileClick(object sender, RoutedEventArgs e)
        {
            view = CoreApplication.GetCurrentView();

            string ImagePath = string.Empty;
            FileOpenPicker filePicker = new FileOpenPicker();
            filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            filePicker.ViewMode = PickerViewMode.Thumbnail;

            // Filter to include a sample subset of file types
            filePicker.FileTypeFilter.Clear();
            filePicker.FileTypeFilter.Add(".pdf");
            filePicker.FileTypeFilter.Add(".png");
            filePicker.FileTypeFilter.Add(".jpeg");
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".gif");
            filePicker.PickSingleFileAndContinue();
            view.Activated += viewActivated;
        }

        private async void getFile()
        {
            IRandomAccessStream fileStream = await storageFileWP.OpenAsync(FileAccessMode.Read);
            var reader = new DataReader(fileStream.GetInputStreamAt(0));
            await reader.LoadAsync((uint)fileStream.Size);
            byte[] byteArray = new byte[fileStream.Size];
            reader.ReadBytes(byteArray);
            Base64String = Convert.ToBase64String(byteArray);
        }

        public bool checkField(string type, out string field)
        {
            if (fileText.Text.Equals(type, StringComparison.Ordinal)
                || fileText.Text.Equals("", StringComparison.Ordinal))
            {
                field = "";
                err.printMessage("Champs '" + type + "' vide.", ErrorControl.Code.FAILURE);
                return false;
            }
            if (storageFileWP == null)
            {
                field = "";

                err.printMessage("Fichier invalide.Creation valide !", GUI.ErrorControl.Code.FAILURE);
                return false;
            }
            field = Base64String;
            return true;
        }

        public FileControls(string title, string content, GUI.ErrorControl err)
        {
            this.InitializeComponent();

            
            filePath = new ImageSourcePath();
            titleBloc.Text = title;
            Debug.WriteLine("content : " + content);

            try
            {
                Binding myBinding = new Binding();
                myBinding.Source = filePath;
                myBinding.Path = new PropertyPath("PathToImage");
                myBinding.Mode = BindingMode.TwoWay;
                myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                BindingOperations.SetBinding(fileText, TextBox.TextProperty, myBinding);
                filePath.PathToImage = content;
            } catch (Exception e)
            {
                Debug.WriteLine("err : " + e.Message);
            }


            this.err = err;
        }
    }
}
