using PhotoSorter.Viewmodels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace PhotoSorter
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private MainWindowViewmodel vm
        {
            get
            {
                return (MainWindowViewmodel)base.DataContext;
            }
        }

        # region UI Event Handlers

        private void MenuItem_LoadPhotos_Click(object sender, RoutedEventArgs e)
        {
            vm.SetPhotoSourceDir();
        }

        private void MenuItem_SetDestination_Click(object sender, RoutedEventArgs e)
        {
            vm.SetPhotoDestinationDir();
        }

        private void Button_LoadPhotos_Click(object sender, RoutedEventArgs e)
        {
            vm.SetPhotoSourceDir();
        }

        private void Button_SetDestination_Click(object sender, RoutedEventArgs e)
        {
            vm.SetPhotoDestinationDir();
        }

        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            vm.PreviousPhoto();
        }

        private void Button_Accept_Click(object sender, RoutedEventArgs e)
        {
            vm.AcceptPhoto();
        }

        private void Button_Reject_Click(object sender, RoutedEventArgs e)
        {
            vm.RejectPhoto();
        }

        # endregion
    }
}
