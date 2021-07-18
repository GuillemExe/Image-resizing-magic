using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ImageMagick;
using Microsoft.Win32;

namespace Image_resizing_magic
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<ImageClassAux> _ListImages;

        private string _DirectoryOriginalFile;
        private string _DirectoryNewFile;

        private int _HeightImage;
        private int _WidthImage;

        private bool _AddFinalMessageInImage;
        private bool _ResizeInJPG;
        private bool _ResizeInPNG;

        public MainWindow()
        {
            InitializeComponent();

            _ListImages = new List<ImageClassAux>();
        }

        private void ButtonBaseOriginalFiles(object sender, RoutedEventArgs e)
        {
            _DirectoryOriginalFile = OpenDialog();
            OriginalFiles.Text = _DirectoryOriginalFile;
        }

        private void ButtonBaseNewFiles(object sender, RoutedEventArgs e)
        {
            _DirectoryNewFile = OpenDialog();
            NewFiles.Text = _DirectoryNewFile;
        }

        private string OpenDialog()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowDialog();
            return folderBrowserDialog.SelectedPath;
        }

        private void ResizeImages(object sender, RoutedEventArgs e)
        {
            GetDesiredDimension();

            GetAllImages();

            GetMoreOptions();

            ResizeSystem();
        }

        private void GetMoreOptions()
        {
            if (CheckBoxFinalNameImage.IsChecked != null)  _AddFinalMessageInImage = (bool) CheckBoxFinalNameImage.IsChecked;
            if (CheckBoxJPGConverter.IsChecked != null) _ResizeInJPG = (bool) CheckBoxJPGConverter.IsChecked;
            if (CheckBoxPNGConverter.IsChecked != null) _ResizeInPNG = (bool) CheckBoxPNGConverter.IsChecked;
        }

        void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            //for (int i = 0; i < _ListImages.Count; i++)
            //{
            //    (sender as BackgroundWorker).ReportProgress(i);
            //    Thread.Sleep(100);
            //}

            for (int i = 0; i < _ListImages.Count; i++)
            {
                ImageClassAux Image = _ListImages[i];

                using (MagickImage magickImage = new MagickImage(Image.GetDirectoryRoute()))
                {
                    magickImage.Resize(_WidthImage, _HeightImage);

                    string fullUrl;
                    string nameImage = Image.GetNameImage();
                    string directoryNewFile = _DirectoryNewFile;

                    nameImage += _AddFinalMessageInImage ? "-resized." : ".";
                    // nameImage += CheckBoxFinalNameImage.IsChecked ?? false ? "-resized." : ".";
                    // nameImage += "-resized.";

                    fullUrl = directoryNewFile + "\\" + nameImage;

                    if (_ResizeInJPG)
                    {
                        magickImage.Write(fullUrl + "jpg");
                    }

                    if (_ResizeInPNG)
                    {
                        magickImage.Write(fullUrl + "png");
                    }

                    (sender as BackgroundWorker)?.ReportProgress(i);
                    Thread.Sleep(100);
                }
            }
        }

        void WorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBarResize.Value = e.ProgressPercentage;
        }

        private void GetDesiredDimension()
        {
            _HeightImage = Convert.ToInt32(TextBoxHeightDimension.Text);
            _WidthImage = Convert.ToInt32(TextBoxWidthDimension.Text);
        }

        public void ResizeSystem()
        {
            BackgroundWorker worker = new BackgroundWorker();

            foreach (ImageClassAux Image in _ListImages)
            {
                using (MagickImage magickImage = new MagickImage(Image.GetDirectoryRoute()))
                {
                    magickImage.Resize(_WidthImage, _HeightImage);

                    string fullUrl;
                    string nameImage = Image.GetNameImage();
                    string directoryNewFile = _DirectoryNewFile;

                    nameImage += CheckBoxFinalNameImage.IsChecked ?? false ? "-resized." : ".";

                    fullUrl = directoryNewFile + "\\" + nameImage;

                    if (CheckBoxJPGConverter.IsChecked ?? false)
                    {
                        magickImage.Write(fullUrl + "jpg");
                    }

                    if (CheckBoxPNGConverter.IsChecked ?? false)
                    {
                        magickImage.Write(fullUrl + "png");
                    }
                }
            }
        } 

        private void GetAllImages()
        {
            foreach (string directory in Directory.GetFiles(_DirectoryOriginalFile))
            {
                if (!string.IsNullOrEmpty(directory))
                {
                    // FIRST WE ASK THE TYPE OF EXTENSION
                    string extension = "";
                    // WE SEGMENT BY THE "/"
                    string[] rootDirectory = directory.Split('\\');
                    // I PICK UP THE LAST WORD
                    string lastItem = rootDirectory[rootDirectory.Length - 1];
                    // SEPARATED THE EXTENSION
                    string[] fileNameAndExtension = lastItem.Split('.');
                    // I CHECK THAT THE EXTENSION IS VALID
                    if (fileNameAndExtension[fileNameAndExtension.Length - 1].Contains("jpg"))
                    {
                        ImageClassAux imageClassAux = new ImageClassAux(directory, fileNameAndExtension[0], fileNameAndExtension[fileNameAndExtension.Length - 1]);
                        _ListImages.Add(imageClassAux);
                    }
                }
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
