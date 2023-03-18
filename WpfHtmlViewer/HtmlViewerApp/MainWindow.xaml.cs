namespace HtmlViewerApp;

using System.Collections.Generic;
using System.IO;
using System;
using System.Windows;
using System.Windows.Forms;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private List<string> _htmlFiles;

    public List<string> HtmlFiles => _htmlFiles ??= new List<string>();

    private int _htmlIndex;

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
                    HtmlFiles.Clear();
                    HtmlFiles.AddRange(Directory.EnumerateFiles(selectedPath, "*.html", SearchOption.TopDirectoryOnly));

                    _htmlIndex = 0;
                    if (HtmlFiles.Count > 0)
                    {
                        Previous.IsEnabled = false;
                        WebBrowser.Navigate(new Uri(HtmlFiles[0]));
                        HtmlContainer.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        HtmlContainer.Visibility = Visibility.Hidden;

                        WebBrowser.Navigate(new Uri("about:blank"));
                    }
                }
            }
        }
    }

    private void Previous_Click(object sender, RoutedEventArgs e)
    {
        if (_htmlIndex == 0)
        {
            return;
        }

        WebBrowser.Navigate(new Uri(HtmlFiles[--_htmlIndex]));

        Previous.IsEnabled = _htmlIndex != 0;
        Next.IsEnabled = _htmlIndex != HtmlFiles.Count - 1;
    }

    private void Next_Click(object sender, RoutedEventArgs e)
    {
        if (_htmlIndex == HtmlFiles.Count - 1)
        {
            return;
        }

        WebBrowser.Navigate(new Uri(HtmlFiles[++_htmlIndex]));

        Previous.IsEnabled = _htmlIndex != 0;
        Next.IsEnabled = _htmlIndex != HtmlFiles.Count - 1;
    }
}