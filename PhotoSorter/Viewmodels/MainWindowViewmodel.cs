using PhotoSorter.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoSorter.Viewmodels
{
    public class MainWindowViewmodel : INotifyPropertyChanged
    {
        # region Public Properties (bound to view)

        public DirectoryInfo PhotoSourceDir { get; set; }
        public DirectoryInfo PhotoDestinationDir { get; set; }
        public IterableList<FileInfo> Photos { get; set; }
        public bool SortingMode
        {
            get
            {
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
        public ImageSource Photo
        {
            get
            {
                if (Photos == null || Photos.Current == null)
                    return null;

                var img = new BitmapImage(new Uri(String.Format(Photos.Current.FullName), UriKind.Relative));
                img.Freeze(); // -> to prevent error: "Must create DependencySource on same Thread as the DependencyObject"
                return img;
            }
        }

        # endregion

        # region Public Methods (imperative actions)

        public void SetPhotoSourceDir()
        {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                PhotoSourceDir = new DirectoryInfo(dialog.SelectedPath);
                var photos = new IterableList<FileInfo>();
                foreach (FileInfo f in PhotoSourceDir.EnumerateFiles("*.*", SearchOption.AllDirectories))
                {
                    if (f.Extension.ToUpper() == ".JPG" || f.Extension.ToUpper() == ".JPEG" || f.Extension.ToUpper() == ".PNG")
                        photos.Add(f);
                }
                if (photos.Count > 0)
                {
                    Photos = photos;
                    Photos.MoveNext();
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
            var photo = Photos.Current;
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

            NextPhoto();
        }
        public void RejectPhoto()
        {
            NextPhoto();
        }

        public void NextPhoto()
        {
            if (!Photos.MoveNext())
            {
                Photos.Reset();
                Photos.MoveNext();
            }
            OnPropertyChanged("Photo");
        }

        public void PreviousPhoto()
        {
            // Try to go back, but do nothing if already at the beginning of the list
            if (Photos.MovePrevious())
            {
                OnPropertyChanged("Photo");
            }
        }

        # endregion

        # region Property Changed Notification Implementation

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
