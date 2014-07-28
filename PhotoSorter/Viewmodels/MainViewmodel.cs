using PhotoSorter.Extensions;
using PhotoSorter.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoSorter.Viewmodels
{
    public class MainViewmodel : INotifyPropertyChanged
    {
        private IMainView _view;
        public MainViewmodel(IMainView view)
        {
            _view = view;
            var settings = PhotoSorter.Properties.Settings.Default;
            if (!String.IsNullOrEmpty(settings.SourceDirectory))
                SetPhotoSourceDir(new DirectoryInfo(settings.SourceDirectory));
            if (!String.IsNullOrEmpty(settings.DestinationDirectory))
                SetPhotoDestinationDir(new DirectoryInfo(settings.DestinationDirectory));
            if (!String.IsNullOrEmpty(settings.Bookmark))
                GoToPhoto(settings.Bookmark);
        }

        # region Public Properties (bound to view)

        
        public bool SortingMode
        {
            get
            {
                return _PhotoSourceDir != null && _PhotoDestinationDir != null;
            }
        }
        public bool SetupMode
        {
            get
            {
                return _PhotoSourceDir == null || _PhotoDestinationDir == null;
            }
        }
        public ImageSource Photo
        {
            get
            {
                if (_Photos == null || _Photos.Current == null)
                    return null;

                var img = new BitmapImage(new Uri(String.Format(_Photos.Current.FullName), UriKind.Relative));
                img.Freeze(); // -> to prevent error: "Must create DependencySource on same Thread as the DependencyObject"
                return img;
            }
        }

        # endregion

        # region Private Properties
        private DirectoryInfo _PhotoSourceDir { get; set; }
        private DirectoryInfo _PhotoDestinationDir { get; set; }
        private IterableList<FileInfo> _Photos { get; set; }
        # endregion

        # region Public Methods (imperative actions)

        public void SetPhotoSourceDir(DirectoryInfo di)
        {
            _PhotoSourceDir = di;
            var photos = new IterableList<FileInfo>();
            foreach (FileInfo f in _PhotoSourceDir.EnumerateFiles("*.*", SearchOption.AllDirectories))
            {
                if (f.Extension.ToUpper() == ".JPG" || f.Extension.ToUpper() == ".JPEG" || f.Extension.ToUpper() == ".PNG")
                    photos.Add(f);
            }
            if (photos.Count > 0)
            {
                _Photos = photos;
                _Photos.MoveNext();
            }
            else
            {
                _view.Alert("No photos found");
                _PhotoSourceDir = null;
            }
            OnPropertyChanged("SortingMode");
            OnPropertyChanged("SetupMode");
            OnPropertyChanged("Photo");
        }
        public void SetPhotoDestinationDir(DirectoryInfo di)
        {
            _PhotoDestinationDir = di;
            OnPropertyChanged("SortingMode");
            OnPropertyChanged("SetupMode");
        }
        public void AcceptPhoto()
        {
            var photo = _Photos.Current;
            try
            {
                photo.CopyTo(String.Format("{0}\\{1}", _PhotoDestinationDir.FullName, photo.Name));
            }
            catch (IOException e)
            {
                if (_view.Confirm("This file already exists in the destination. Do you want to overwrite?"))
                {
                    photo.CopyTo(String.Format("{0}\\{1}", _PhotoDestinationDir.FullName, photo.Name), true);
                }
                else
                {
                    return;
                }
            }

            NextPhoto();
        }

        public void NextPhoto()
        {
            if (!_Photos.MoveNext())
            {
                _Photos.Reset();
                _Photos.MoveNext();
            }
            OnPropertyChanged("Photo");
        }

        public void PreviousPhoto()
        {
            // Try to go back, but do nothing if already at the beginning of the list
            if (_Photos.MovePrevious())
            {
                OnPropertyChanged("Photo");
            }
        }

        public void GoToPhoto(string path)
        {
            FileInfo match = null;
            foreach (var photo in _Photos) {
                if (photo.FullName.Equals(path))
                    match = photo;
            }
            if (match != null)
            {
                _Photos.Position = _Photos.IndexOf(match);
                OnPropertyChanged("Photo");
            }
            else
            {
                _view.Alert(String.Format("Photo '{0}' not found!", path));
            }
        }

        public void SaveSettings()
        {
            var settings = PhotoSorter.Properties.Settings.Default;
            if (SortingMode)
            {
                settings.Bookmark = _Photos.Current.FullName;
                settings.SourceDirectory = _PhotoSourceDir.FullName;
                settings.DestinationDirectory = _PhotoDestinationDir.FullName;
                settings.Save();
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
