using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace PDFSaver.Models
{
    /// <summary>
    /// Method to Manage PDF documents
    /// Can save any website to a PDF document based on a URL
    /// Can open any PDF document based on a file path
    /// </summary>
    public class PDFManager
    {
        //Instance variables
        /// <summary>
        /// Default File Location to save a new PDF to if no others can be found
        /// </summary>
        private string DefaultFileLocation { get; set; }

        //Constructors
        /// <summary>
        /// Default 0 arguments constructor for PDFManager
        /// Sets DefaultFileLocation to the desktop 
        /// </summary>
        public PDFManager()
        {
            DefaultFileLocation = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\";
        }

        /// <summary>
        /// Constructor for PDFManager taking 1 argument
        /// Sets DefaultFileLocation to the given directory path (Only if it is a well formed Uri string)
        /// </summary>
        /// <param name="DefaultFileLocation">The staring default file location</param>
        public PDFManager(string DefaultFileLocation)
        {
            this.DefaultFileLocation = DefaultFileLocation;
        }

        //Main Methods
        /// <summary>
        /// Method to save a PDF to the default location with a generated name
        /// </summary>
        /// <param name="websiteURL">The URL of the website to be saved to a PDF</param>
        /// <returns>True if the process was deemed sucessful, else false</returns>
        public bool SaveWebsite(string websiteURL)
        {
            try
            {
                //Get's the html of the webpage
                WebClient x = new WebClient();
                string source = x.DownloadString(websiteURL);

                //Get's the title of the webpage using the html
                string title = Regex.Match(source, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;

                // Create a PDF from any existing web page
                IronPdf.HtmlToPdf Renderer = new IronPdf.HtmlToPdf();

                // This boy is problem
                Renderer.RenderUrlAsPdf(websiteURL).SaveAs(DefaultFileLocation + title + ".pdf");

                return IsPdf(DefaultFileLocation + title + ".pdf");
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Method to save a PDF document to a given location with a given name
        /// </summary>
        /// <param name="websiteURL">URL of the website to convert to a PDF</param>
        /// <param name="filename">The name to save the PDF with</param>
        /// <param name="fileLocation">The location to save the pdf to</param>
        /// <returns>True if the process was deemed sucessful, else false</returns>
        public bool SaveWebsite(string websiteURL, string filename, string fileLocation)
        {
            try
            {
                // Create a PDF from any existing web page
                IronPdf.HtmlToPdf Renderer = new IronPdf.HtmlToPdf();
                Renderer.RenderUrlAsPdf(websiteURL).SaveAs(fileLocation + filename + ".pdf");

                //Saves the pdf to the selected location
                //PDF.SaveAs(fileLocation + filename + ".pdf");

                //Checks if the PDF was successfully created
                return IsPdf(fileLocation + filename + ".pdf");
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Method to open / run a PDF
        /// </summary>
        /// <param name="fileLocation">The location of the PDF document</param>
        /// <returns>True if the process is deemed a success, else returns false</returns>
        public bool OpenPDF(string fileLocation)
        {
            if (IsPdf(fileLocation))
            {
                try
                {
                    System.Diagnostics.Process.Start(fileLocation);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Method to set a new DefaultFileLocation
        /// </summary>
        /// <param name="DefaultFileLocation">The new DefaultFileLocation</param>
        /// <returns>True if the change was successful, else false</returns>
        public bool SetDefaultFileLocation(string DefaultFileLocation)
        {
            this.DefaultFileLocation = DefaultFileLocation;
            return true;
        }

        //Assister Methods
        /// <summary>
        /// Method to check if a given path points to a PDF
        /// </summary>
        /// <param name="path">The path to be checked</param>
        /// <returns>True if the path refers to a PDF, else returns false</returns>
        private bool IsPdf(string path)
        {
            //Checks if the path is a valid path(Neat as hell code not really useful here though)
            //if (!Uri.IsWellFormedUriString(path, 1))
            //    return false;
            //Checks if the path leads to a file which exists
            if (!IsFile(path))
                return false;

            var pdfString = "%PDF-";
            var pdfBytes = Encoding.ASCII.GetBytes(pdfString);
            var len = pdfBytes.Length;
            var buf = new byte[len];
            var remaining = len;
            var pos = 0;
            using (var f = File.OpenRead(path))
            {
                while (remaining > 0)
                {
                    var amtRead = f.Read(buf, pos, remaining);
                    if (amtRead == 0) return false;
                    remaining -= amtRead;
                    pos += amtRead;
                }
            }
            return pdfBytes.SequenceEqual(buf);
        }

        /// <summary>
        /// Method to check if a file exists
        /// </summary>
        /// <param name="path">The path to the file</param>
        /// <returns>True if the file exists, else false</returns>
        private bool IsFile(string path)
        {
            return File.Exists(path);
        }
    }
}
