using Microsoft.Toolkit.Uwp.UI.Controls;
using POS.Models;
using POS.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Brush = Windows.UI.Xaml.Media.Brush;
using Color = Windows.UI.Color;
using FontFamily = Windows.UI.Xaml.Media.FontFamily;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace POS.Views
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class Sells : Page
    {
        Microsoft.UI.Xaml.Controls.TabView tabView;
        public SellViewModel vm { get; set; }
        List<Microsoft.UI.Xaml.Controls.TabViewItem> listaTab;
        public Microsoft.UI.Xaml.Controls.TabViewItem TabSelelected { get; set; }
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public Sells()
        {
            this.InitializeComponent();
            vm = new SellViewModel();
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            tabView = new Microsoft.UI.Xaml.Controls.TabView();
            tabView.IsAddTabButtonVisible = false;
            tabView.TabCloseRequested += TabView_TabCloseRequested;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            vm.TabViewItems = new List<Microsoft.UI.Xaml.Controls.TabViewItem>();
            // tabView.TabItemsSource = vm.TabViewItems;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(DragGrid);
            reporteMasVendidos.IsEnabled = (bool)localSettings.Values["ReporteMasVendidos"];
            reporteTodasVentas.IsEnabled = (bool)localSettings.Values["ReporteDeVentas"];
            devolucionesRealizadas.IsEnabled = (bool)localSettings.Values["ReporteDeDevoluciones"];


        }

        private void TabView_TabCloseRequested(Microsoft.UI.Xaml.Controls.TabView sender, Microsoft.UI.Xaml.Controls.TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);
            vm.TabViewItems.Remove(args.Tab);
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            DragGrid.Height = sender.Height;
        }

        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            contentFrame.HorizontalAlignment = HorizontalAlignment.Stretch;
            contentFrame.VerticalAlignment = VerticalAlignment.Top;
            Microsoft.UI.Xaml.Controls.NavigationViewItem nvi = args.SelectedItem as Microsoft.UI.Xaml.Controls.NavigationViewItem;
            try
            {
                if (nvi.Tag != null)
                {
                    if (nvi.Tag.ToString() != "")
                    {
                        switch (nvi.Tag.ToString())
                        {
                            case "VentasRealizadas":
                                {
                                    tabView = new Microsoft.UI.Xaml.Controls.TabView();
                                        vm.TabViewItems = new List<Microsoft.UI.Xaml.Controls.TabViewItem>();
                                    contentFrame.Navigate(typeof(VentasRealizadas));
                                    break;
                                }
                            case "Devoluciones":
                                {
                                    tabView = new Microsoft.UI.Xaml.Controls.TabView();
                                    vm.TabViewItems = new List<Microsoft.UI.Xaml.Controls.TabViewItem>();
                                    contentFrame.Navigate(typeof(Devoluciones));
                                    break;
                                }
                            case "Reportes":
                                {
                                    
                                    if(contentFrame.Content.ToString() =="Selecciona una opcion")
                                    {
                                        contentFrame.HorizontalAlignment = HorizontalAlignment.Center;
                                        contentFrame.VerticalAlignment = VerticalAlignment.Center;
                                    }
                                    break;
                                }
                            case "Todas":
                                {

                                    var newTab = new Microsoft.UI.Xaml.Controls.TabViewItem();
                                    newTab.Header = "Todas las ventas";
                                    newTab.IsClosable = true;
                                    Frame frame = new Frame();
                                    frame.Content = new AllSellsReport();
                                    newTab.Content = frame;

                                    var existe = vm.TabViewItems.Where(b => b.Header == newTab.Header).Count();
                                    if(existe == 0)
                                    {
                                        newTab.Content = frame;
                                        newTab.IsClosable = true;
                                        tabView.TabItems.Add(newTab);
                                        contentFrame.Content = tabView;
                                        vm.TabViewItems.Add(newTab);
                                        var indice = vm.TabViewItems.IndexOf(newTab);
                                        tabView.SelectedIndex = indice;
                                    }
                                    else
                                    {

                                        var tab = vm.TabViewItems.Where(b => b.Header.ToString() == "Todas las ventas").First();
                                        var indice = vm.TabViewItems.IndexOf(tab);
                                        tabView.SelectedIndex = indice;
                                    }
                                    
                                    break;
                                }
                            case "MasVendidos":
                                {
                                    var newTab = new Microsoft.UI.Xaml.Controls.TabViewItem();
                                    newTab.Header = "Productos mas vendidos";                                  
                                    newTab.IsClosable = true;
                                    Frame frame = new Frame();                                   
                                    frame.Content = new MasVendidos();
                                    newTab.Content = frame;
                                    var existe = vm.TabViewItems.Where(b => b.Header == newTab.Header).Count();
                                    if (existe == 0)
                                    {

                                        tabView.TabItems.Add(newTab);
                                        contentFrame.Content = tabView;
                                        vm.TabViewItems.Add(newTab);
                                        var indice = vm.TabViewItems.IndexOf(newTab);

                                        tabView.SelectedIndex = indice;
                                    }
                                    else
                                    {
                                        var tab = vm.TabViewItems.Where(b => b.Header.ToString() == "Productos mas vendidos").First();
                                        var indice = vm.TabViewItems.IndexOf(tab);
                                        tabView.SelectedIndex = indice;
                                    }



                                    break;
                                }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                var md = new MessageDialog(e.Message);
                md.ShowAsync();
            }


            // contentFrame.Navigate(typeof(ReportesVentas));
        }

        private IEnumerable<object> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void NavigationView_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            Frame.Navigate(typeof(StartPage));
        }

        private void contentFrame_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            

        }
    }
}
