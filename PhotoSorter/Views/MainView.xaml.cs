using PhotoSorter.Extensions;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace PhotoSorter.Views
{
    public partial class MainView : Window, IMainView
    {
        private MainViewmodel _viewmodel;
        public MainView()
        {
            InitializeComponent();
            _viewmodel = new MainViewmodel(this);
            base.DataContext = _viewmodel;
        }

        # region View Methods

        public DirectoryInfo SelectDirectory()
        {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return new DirectoryInfo(dialog.SelectedPath);
            }
            return null;
        }

        public void Alert(string message, string caption = "Oops!")
        {
            System.Windows.MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public bool Confirm(string question, string caption = "Warning!")
        {
            MessageBoxResult confirm = System.Windows.MessageBox.Show(question, caption, MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            return confirm == MessageBoxResult.Yes;
        }

        # endregion

        # region Event Handlers

        private void MenuItem_LoadPhotos_Click(object sender, RoutedEventArgs e)
        {
            var di = SelectDirectory();
            if (di != null)
            {
                _viewmodel.SetPhotoSourceDir(di);
            }
        }

        private void MenuItem_SetDestination_Click(object sender, RoutedEventArgs e)
        {
            var di = SelectDirectory();
            if (di != null)
            {
                _viewmodel.SetPhotoDestinationDir(di);
            }
        }

        private void Button_LoadPhotos_Click(object sender, RoutedEventArgs e)
        {
            var di = SelectDirectory();
            if (di != null)
            {
                _viewmodel.SetPhotoSourceDir(di);
            }
        }

        private void Button_SetDestination_Click(object sender, RoutedEventArgs e)
        {
            var di = SelectDirectory();
            if (di != null)
            {
                _viewmodel.SetPhotoDestinationDir(di);
            }
        }

        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            _viewmodel.PreviousPhoto();
        }

        private void Button_Accept_Click(object sender, RoutedEventArgs e)
        {
            _viewmodel.AcceptPhoto();
        }

        private void Button_Reject_Click(object sender, RoutedEventArgs e)
        {
            _viewmodel.NextPhoto();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _viewmodel.SaveSettings();
        }

        private void StatusBarItem_Progress_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //new Jump().ShowDialog();
        }
        # endregion
    }

    // Allow Viewmodel to access these methods on the view only
    public interface IMainView
    {
        DirectoryInfo SelectDirectory();
        void Alert(string message, string caption = "Oops!");
        bool Confirm(string question, string caption = "Warning!");
    }
}
