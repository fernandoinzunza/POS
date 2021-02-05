

using Microsoft.VisualStudio.PlatformUI;
using POS.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;

namespace POS.ViewModels
{
    public class StartPageViewModel : BaseViewModel
    {
        private INavigationService _navigationService;
        public ICommand ShowDialog
        {
            get; set;
        }
        public StartPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            ShowDialog = new RelayCommand(o =>
            {
                
                    var dialog = new MessageDialog("Modulo en desarrollo", "Mensaje");

                    // If you want to add custom buttons
                    dialog.Commands.Add(new UICommand("Click me!", delegate (IUICommand command)
                    {
                    // Your command action here
                }));

                    // Show dialog and save result
                    var result = dialog.ShowAsync();

                
            });
            NavigationCommand = new RelayCommand(o => {
                navigationService.Navigate(typeof(ArticlePage));
            });
            ProviderPage = new RelayCommand(o => { navigationService.Navigate(typeof(Providers)); });
            InventoryPage = new RelayCommand(o => { navigationService.Navigate(typeof(Inventory)); });
        }
        

        public RelayCommand NavigationCommand
        {
            get; set;
           
        }
        public RelayCommand ProviderPage { get; set; }
        public RelayCommand InventoryPage { get; set; }
        

    }
}
