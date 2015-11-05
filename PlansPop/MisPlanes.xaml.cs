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
        public sealed partial class MisPlanes : Page
        {
            public MisPlanes()
            {

                this.InitializeComponent();
                this.MiPlanViewModel = new MiPlanItemViewModel();
                this.NavigationCacheMode = NavigationCacheMode.Enabled;

            }

            public MiPlanItemViewModel MiPlanViewModel { get; set; }

            protected override void OnNavigatedTo(NavigationEventArgs e)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility
                    = AppViewBackButtonVisibility.Collapsed;
                MisPlanesGrid.SelectedIndex = -1;
            }



            private void MisPlanesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                if (MisPlanesGrid.SelectedIndex != -1)
                {
                    Frame rootFrame = Window.Current.Content as Frame;

                    rootFrame.Navigate(typeof(EditPlan), MiPlanViewModel.MiPlanItems.ElementAt(MisPlanesGrid.SelectedIndex));
                }
            }
        }
    }


