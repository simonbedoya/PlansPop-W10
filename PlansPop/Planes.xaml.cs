using PlansPop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PlansPop
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Planes : Page
    {

        public Planes()
        {

            this.InitializeComponent();
            this.ViewModel = new PlanItemViewModel();

            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        public PlanItemViewModel ViewModel { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility
                    = AppViewBackButtonVisibility.Collapsed;
            PlanesGrid.SelectedIndex = -1;
        }

        private void PlanesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlanesGrid.SelectedIndex != -1)
            {
                Frame rootFrame = Window.Current.Content as Frame;

                rootFrame.Navigate(typeof(VerPlan), ViewModel.PlanItems.ElementAt(PlanesGrid.SelectedIndex));
            }
        }
    }
}