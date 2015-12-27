using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Img2Pdf
{
    class Program
    {
        static void Main(string[] args)
        {
            string dir;
            if (args.Length > 0)
            {
                dir = args[0];
            }
            else
            {
                dir = ".";
            }

            dir = Path.GetFullPath(dir);
            WriteDirPdf(dir);
        }

        static void WritePdf(IList<Image> images, string pdfPath, string directoryPath)
        {
            var doc = new Document(images[0]);
            doc.SetMargins(0, 0, 0, 0);
            using (var fs = new FileStream(pdfPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var writer = PdfWriter.GetInstance(doc, fs);
                doc.Open();
                foreach(var img in images)
                {
                    doc.Add(img);
                    doc.NewPage();
                }
                doc.Close();
            }
        }

        static IList<Image> GetImages(string directoryPath)
        {
            var list = new List<Tuple<string, Image>>();
            foreach (string path in Directory.GetFiles(directoryPath))
            {
                if (!IsImage(Path.GetExtension(path)))
                    continue;

                Image img = null;
                try
                {
                    img = Image.GetInstance(path);
                    list.Add(new Tuple<string, Image>(path, img));
                }
                catch
                {
                    // do nothing
                }
            }
            list.Sort(new ImageComparer());
            return list.Select(i => i.Item2).ToList();
        }

        static bool IsImage(string ext)
        {
            ext = ext.ToLowerInvariant();
            return ext == ".gif" || ext == ".bmp" ||
                ext == ".jpg" || ext == ".jfif" || ext == ".jpeg" || ext == ".jpe" || ext == ".jfi" || ext == ".jif" ||
                ext == ".png" || ext == ".wmf" || ext == ".tif" || ext == ".tiff" || ext == ".jbig2"
                ;
        }

        static void WriteDirPdf(string directoryPath)
        {
            var imgs = GetImages(directoryPath);
            if (imgs.Count > 0)
            {
                string pdf = directoryPath + ".pdf";
                Console.WriteLine("Saving " + pdf + " (" + imgs.Count + " entries)");
                WritePdf(imgs, pdf, directoryPath);
            }

            foreach (string path in Directory.GetDirectories(directoryPath))
            {
                WriteDirPdf(path);
            }
        }

        private class ImageComparer : IComparer<Tuple<string, Image>>
        {
            public int Compare(Tuple<string, Image> x, Tuple<string, Image> y)
            {
                return x.Item1.CompareTo(y.Item1);
            }
        }
    }
}
