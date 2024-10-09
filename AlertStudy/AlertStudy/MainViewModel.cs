using MaterialDesignThemes.Wpf;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Windows.Input;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace AlertStudy
{
    internal class MainViewModel : BaseViewModel
    {
        public ICommand ToastCommand { get; }
        public ICommand SnackbarCommand { get; }
        public ISnackbarMessageQueue SnackbarMessageQueue { get; }
        
        

        public MainViewModel()
        {
            ToastCommand = new RelayCommand(ShowToastNotification);
            SnackbarCommand = new RelayCommand(ShowSnackbarNotification);
            
            SnackbarMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
        }
        private void ShowToastNotification()
        {
           
        }

        private void ShowSnackbarNotification()
        {
            SnackbarMessageQueue.Enqueue("This is a Snackbar notification!");
        }

       

    }
}
