using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace POS.Views
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class ArticlePage : Page
    {
        public ArticlePage()
        {
            InitializeComponent();
          var coreTitleBar =  CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
           
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(DragGrid);
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            DragGrid.Height = sender.Height;
        }

        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            
            Microsoft.UI.Xaml.Controls.NavigationViewItem nvi = args.SelectedItem as Microsoft.UI.Xaml.Controls.NavigationViewItem;
            if(nvi.Tag.ToString() !="")
            {
                contentFrame.HorizontalAlignment = HorizontalAlignment.Stretch;
                contentFrame.VerticalAlignment = VerticalAlignment.Stretch;
            }               
            switch (nvi.Tag.ToString())
            {
                case "NewArticle":
                    {

                        contentFrame.Navigate(typeof(NewArticle));
                        break;
                    }
                case "ArticleList":
                    {
                        contentFrame.Navigate(typeof(ArticleList));
                        break;
                    }
                case "DepartmentView":
                    {
                        contentFrame.Navigate(typeof(DepartmentView));
                        break;
                    }
                case "Discounts":
                    {
                        contentFrame.Navigate(typeof(Discounts));
                        break;
                    }
                case "NewMeasureUnity":
                    {
                        contentFrame.Navigate(typeof(NewMeasureUnity));
                        break;
                    }
            }

        }

        private void NavigationView_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            Frame.Navigate(typeof(StartPage), null);
        }

    }
}