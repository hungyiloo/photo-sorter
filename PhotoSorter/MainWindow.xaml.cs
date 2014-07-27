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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.ComponentModel;

namespace PhotoSorter
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public bool SortingMode {
            get {
                return PhotoSourceDir != null && PhotoDestinationDir != null;
            }
        }
        public bool SetupMode
        {
            get
            {
                return PhotoSourceDir == null || PhotoDestinationDir == null;
            }
        }

        public DirectoryInfo PhotoSourceDir { get; set; }
        public DirectoryInfo PhotoDestinationDir { get; set; }
        public IEnumerator<FileInfo> PhotosIterator { get; set; }

        public ImageSource Photo
        {
            get
            {
                if (PhotosIterator == null || PhotosIterator.Current == null)
                    return null;

                var img = new BitmapImage(new Uri(String.Format(PhotosIterator.Current.FullName), UriKind.Relative));
                img.Freeze(); // -> to prevent error: "Must create DependencySource on same Thread as the DependencyObject"
                return img;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        public void SetPhotoSourceDir() {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                PhotoSourceDir = new DirectoryInfo(dialog.SelectedPath);
                var photos = new List<FileInfo>();
                foreach (FileInfo f in PhotoSourceDir.EnumerateFiles("*.*", SearchOption.AllDirectories))
                {
                    if (f.Extension.ToUpper() == ".JPG" || f.Extension.ToUpper() == ".JPEG" || f.Extension.ToUpper() == ".PNG")
                        photos.Add(f);
                }
                if (photos.Count > 0)
                {
                    PhotosIterator = photos.GetEnumerator();
                    if (!PhotosIterator.MoveNext())
                    {
                        PhotosIterator = photos.GetEnumerator();
                        PhotosIterator.MoveNext();
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("No photos found", "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
                    PhotoSourceDir = null;
                }
                OnPropertyChanged("SortingMode");
                OnPropertyChanged("SetupMode");
                OnPropertyChanged("Photo");
            }
        }
        public void SetPhotoDestinationDir()
        {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                PhotoDestinationDir = new DirectoryInfo(dialog.SelectedPath);
                OnPropertyChanged("SortingMode");
                OnPropertyChanged("SetupMode");
            }
        }
        public void AcceptPhoto()
        {
            var photo = PhotosIterator.Current;
            try
            {
                photo.CopyTo(String.Format("{0}\\{1}", PhotoDestinationDir.FullName, photo.Name));
            }
            catch (IOException e)
            {
                MessageBoxResult confirm = System.Windows.MessageBox.Show("This file already exists in the destination. Do you want to overwrite?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (confirm == MessageBoxResult.Yes)
                {
                    photo.CopyTo(String.Format("{0}\\{1}", PhotoDestinationDir.FullName, photo.Name), true);
                }
                else
                {
                    return;
                }
            }
            
            if (!PhotosIterator.MoveNext())
            {
                PhotosIterator.Reset();
                PhotosIterator.MoveNext();
            }
            OnPropertyChanged("Photo");
        }
        public void RejectPhoto()
        {
            if (!PhotosIterator.MoveNext())
            {
                PhotosIterator.Reset();
                PhotosIterator.MoveNext();
            }
            OnPropertyChanged("Photo");
        }

        # region Click Handlers

        private void MenuItem_LoadPhotos_Click(object sender, RoutedEventArgs e)
        {
            SetPhotoSourceDir();
        }

        private void Button_LoadPhotos_Click(object sender, RoutedEventArgs e)
        {
            SetPhotoSourceDir();
        }

        private void Button_SetDestination_Click(object sender, RoutedEventArgs e)
        {
            SetPhotoDestinationDir();
        }

        private void Button_Accept_Click(object sender, RoutedEventArgs e)
        {
            AcceptPhoto();
        }

        private void Button_Reject_Click(object sender, RoutedEventArgs e)
        {
            RejectPhoto();
        }

        # endregion

        # region Property Changed Notification

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        # endregion

    }
}
