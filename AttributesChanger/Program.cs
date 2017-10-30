using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Deployment;
using System.Deployment.Application;
using System.Reflection;

namespace AttributesChanger
{
    class Program
    {
        /// <summary>
        /// Mains the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            Console.Title = "AttributesChanger " + GetVersion();
            string pathFolder = null;
            if (args.Length == 0)
            {
                Console.WriteLine("Insert folder path :");
                string result = Console.ReadLine();
                pathFolder = result;
            }

            DirectoryInfo directory = new DirectoryInfo(pathFolder);
            List<FileInfo> files = directory.GetFiles().ToList();

            foreach (var file in files)
            {
                DateTime? dateTaken = new DateTime();
                switch (file.Extension)
                {
                    case ".jpg":
                        //cas des images
                        Regex rImg = new Regex(":");
                        Image img = new Bitmap(file.FullName);
                        PropertyItem propItem = img.GetPropertyItem(36867);
                        var tmpImg = rImg.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                        dateTaken = DateTime.Parse(tmpImg);
                        img.Dispose();
                        break;
                    case ".mp4":
                        //cas des vidéos
                        Regex rVid = new Regex(@"\d+");
                        var result = rVid.Matches(file.Name);
                        var year = Convert.ToInt32(result[0].Value.Substring(0, 4));
                        var month = Convert.ToInt32(result[0].Value.Substring(4, 2));
                        var day = Convert.ToInt32(result[0].Value.Substring(6, 2));
                        var hours = Convert.ToInt32(result[1].Value.Substring(0, 2));
                        var mins = Convert.ToInt32(result[1].Value.Substring(2, 2));
                        var secs = Convert.ToInt32(result[1].Value.Substring(4, 2));
                        dateTaken = new DateTime(year, month, day, hours, mins, secs);
                        break;
                    default:
                        dateTaken = null;
                        break;
                }
                if (dateTaken.HasValue)
                {
                    var lastWrite = file.LastWriteTime;
                    Console.WriteLine(file.Name + " change ModificationDate (" + lastWrite + ") by (" + dateTaken.ToString() + ")");
                    file.LastWriteTime = dateTaken.Value;
                }
            }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <returns>The number of version.</returns>
        static string GetVersion()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                version = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.MajorRevision, version.Minor, version.MinorRevision);
        }
    }
}
