using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MetadataExtractor;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Png;
using MetadataExtractor.Formats.Bmp;
using MetadataExtractor.Formats.Gif;
using MetadataExtractor.Formats.Tiff;
using MetadataExtractor.Formats.Pcx;
using MetadataExtractor.Formats.Exif;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Concurrent;

namespace PKG_Lab2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.MinWidth = 950;
            this.MinHeight = 450;
        }

        private async void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            var metadataList = new ConcurrentBag<ImageMetadata>();
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var folderPath = dialog.SelectedPath;
                var imageFiles = System.IO.Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                    .Where(s => s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                s.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                s.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                s.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
                                s.EndsWith(".tif", StringComparison.OrdinalIgnoreCase) ||
                                s.EndsWith(".pcx", StringComparison.OrdinalIgnoreCase) ||
                                s.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var tasks = new List<Task>();

                Parallel.ForEach(imageFiles, filePath =>
                {
                    tasks.Add(Task.Run(() =>
                    {
                        var bitmap = new BitmapImage(new Uri(filePath));
                        var metadata = new ImageMetadata
                        {
                            FileName = Path.GetFileName(filePath),
                            Dimensions = $"{bitmap.PixelWidth} x {bitmap.PixelHeight}",
                            Resolution = $"{bitmap.DpiX} x {bitmap.DpiY}",
                            ColorDepth = $"{bitmap.Format.BitsPerPixel}",
                            Compression = GetCompressionRatio(filePath),
                            ColorCount = GetUniqueColorCount(filePath),
                            Size = GetFileSize(filePath),
                            Date = GetFileCreationDate(filePath)
                        };

                        metadataList.Add(metadata);
                    }));
                });

                await Task.WhenAll(tasks);
            }

            MetadataGrid.ItemsSource = metadataList.ToList();
        }

        private void SelectFolder_Click1(object sender, RoutedEventArgs e)
        {
            var metadataList = new List<ImageMetadata>();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                FileInfo fileInfo = new FileInfo(filePath);

                var metadata = new ImageMetadata
                {
                    FileName = Path.GetFileName(filePath),
                    Dimensions = GetImageResolution(filePath),
                    Resolution = GetImageDpi(filePath),
                    ColorDepth = GetImageColorDepth(filePath),
                    Compression = GetCompressionRatio(filePath),
                    ColorCount = GetUniqueColorCount(filePath),
                    Size = GetFileSize(filePath),
                    Date = GetFileCreationDate(filePath)
                };

                metadataList.Add(metadata);
            }

            MetadataGrid.ItemsSource = metadataList;

        }


        public string GetImageResolution(string imagePath)
        {
            BitmapImage bitmap = new BitmapImage(new Uri(imagePath));
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            return $"{width} x {height}";
        }

        public static string GetImageDpi(string imagePath)
        {
            BitmapImage bitmap = new BitmapImage(new Uri(imagePath));
            double dpiX = bitmap.DpiX;
            double dpiY = bitmap.DpiY;
            return $"{dpiX} x {dpiY}";
        }

        public string GetImageColorDepth(string imagePath)
        {
            BitmapImage bitmap = new BitmapImage(new Uri(imagePath));
            int bitsPerPixel = bitmap.Format.BitsPerPixel;
            return $"{bitsPerPixel}";
        }

        private string GetFileSize(string imagePath)
        {
            FileInfo fileInfo = new FileInfo(imagePath);
            double d = Convert.ToDouble(fileInfo.Length) / 1024 / 1024;
            d = Math.Round(d, 1);
            return $"{Convert.ToString(d)} Mb";
        }

        public static string GetFileCreationDate(string filePath)
        {
            DateTime dt = File.GetCreationTime(filePath);
            string s = dt.ToString();
            return s;
        }

        public static string GetCompressionRatio(string imagePath)
        {
            FileInfo fileInfo = new FileInfo(imagePath);
            long originalSize = fileInfo.Length;

            using (Bitmap bitmap = new Bitmap(imagePath))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                    EncoderParameters encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);

                    bitmap.Save(ms, jpgEncoder, encoderParameters);
                    long compressedSize = ms.Length;

                    double result = (double)compressedSize / originalSize * 100;

                    return Convert.ToString(result);
                }
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public static string GetUniqueColorCount(string imagePath)
        {
            BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath));
            FormatConvertedBitmap formatConvertedBitmap = new FormatConvertedBitmap(bitmapImage, System.Windows.Media.PixelFormats.Bgra32, null, 0);

            int width = formatConvertedBitmap.PixelWidth;
            int height = formatConvertedBitmap.PixelHeight;
            int stride = width * (formatConvertedBitmap.Format.BitsPerPixel / 8);
            byte[] pixelData = new byte[height * stride];
            formatConvertedBitmap.CopyPixels(pixelData, stride, 0);

            HashSet<System.Windows.Media.Color> uniqueColors = new HashSet<System.Windows.Media.Color>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + x * 4;
                    byte b = pixelData[index];
                    byte g = pixelData[index + 1];
                    byte r = pixelData[index + 2];
                    byte a = pixelData[index + 3];

                    System.Windows.Media.Color color = System.Windows.Media.Color.FromArgb(a, r, g, b);
                    uniqueColors.Add(color);
                }
            }

            return Convert.ToString(uniqueColors.Count);
        }

        public class ImageMetadata
        {
            public string FileName { get; set; }
            public string Dimensions { get; set; }
            public string Resolution { get; set; }
            public string ColorDepth { get; set; }
            public string Compression { get; set; }
            public string ColorCount { get; set; }
            public string Size { get; set; }
            public string Date { get; set; }
        }
    }
}
