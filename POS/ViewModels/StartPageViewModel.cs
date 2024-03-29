﻿

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
            UsersPage = new RelayCommand(o => { navigationService.Navigate(typeof(Users)); });
            Ventas = new RelayCommand(o => { navigationService.Navigate(typeof(Sells)); });
            CerrarSesion = new RelayCommand(o => { Salir(); });
        }
        

        public RelayCommand NavigationCommand
        {
            get; set;
           
        }
        public RelayCommand Ventas { get; set; }
        public RelayCommand ProviderPage { get; set; }
        public RelayCommand InventoryPage { get; set; }
        public RelayCommand VentanaCobro { get => new RelayCommand((o) => _navigationService.Navigate(typeof(VentanaDeCobro))); }
        public RelayCommand UsersPage { get; set; }
        public RelayCommand CerrarSesion { get; set; }

        private void Salir()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["NoEmpleado"] = null;
            localSettings.Values["NomUsuario"] = null;
            localSettings.Values["ApPat"] = null;
            localSettings.Values["ApMat"] = null;
            localSettings.Values["Nombre"] = null;
            localSettings.Values["Puesto"] = null;
            localSettings.Values["NuevoArticulo"] = null;
            localSettings.Values["ModificarArticulo"] = null;
            localSettings.Values["EliminarArticulo"] = null;
            localSettings.Values["NuevoDescuento"] = null;
            localSettings.Values["NuevaEntrada"] = null;
            localSettings.Values["EntradaPorReposicion"] = null;
            localSettings.Values["BitacoraEntradas"] = null;
            localSettings.Values["NuevoProveedor"] = null;
            localSettings.Values["EditarProveedor"] = null;
            localSettings.Values["EliminarProveedor"] = null;
            localSettings.Values["NuevaSalida"] = null;
            localSettings.Values["BitacoraDeSalidas"] = null;
            localSettings.Values["StockDisponible"] = null;
            localSettings.Values["VentanaDeCobro"] = null;
            localSettings.Values["ReporteDeVentas"] = null;
            localSettings.Values["RealizarCancelacion"] = null;
            localSettings.Values["ReporteDeDevoluciones"] = null;
            localSettings.Values["ReporteMasVendidos"] = null;
            _navigationService.Navigate(typeof(LoginView));
        }

    }
}
