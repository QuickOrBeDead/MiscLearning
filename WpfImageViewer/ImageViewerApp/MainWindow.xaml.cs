namespace ImageViewerApp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private List<string> _images;

        public List<string> Images => _images ?? (_images = new List<string>());

        private int _imageIndex;

        private void OpenFolderMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog
                                    {
                                        Description = "Please select a folder",
                                        UseDescriptionForTitle = true,
                                        ShowNewFolderButton = true
                                    })
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var selectedPath = dialog.SelectedPath;
                    if (!string.IsNullOrWhiteSpace(selectedPath))
                    {
                        Images.Clear();
                        Images.AddRange(Directory.EnumerateFiles(selectedPath, "*.png", SearchOption.TopDirectoryOnly));

                        _imageIndex = 0;
                        if (Images.Count > 0)
                        {
                            Previous.IsEnabled = false;
                            CurrentImage.Source = new BitmapImage(new Uri(Images[0]));
                            ImageContainer.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            ImageContainer.Visibility = Visibility.Hidden;

                            CurrentImage.Source = null;
                        }
                    }
                }
            }
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (_imageIndex == 0)
            {
                return;
            }

            CurrentImage.Source = new BitmapImage(new Uri(Images[--_imageIndex]));

            Previous.IsEnabled = _imageIndex != 0;
            Next.IsEnabled = _imageIndex != Images.Count - 1;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (_imageIndex == Images.Count - 1)
            {
                return;
            }

            CurrentImage.Source = new BitmapImage(new Uri(Images[++_imageIndex]));

            Previous.IsEnabled = _imageIndex != 0;
            Next.IsEnabled = _imageIndex != Images.Count - 1;
        }
    }
}
