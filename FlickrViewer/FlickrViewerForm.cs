using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Drawing.Imaging;

namespace FlickrViewer
{
    public partial class FlickrViewerForm : Form
    {
        // Flickr API Key
        private const string KEY = "6c9bc2b0a585893da7f5cadd0688d58d";

        // object used to invoke Flickr web service      
        private static HttpClient flickrClient = new HttpClient();

        // Task<string> that queries Flickr
        Task<string> flickrTask = null;

        //Counters for naming images throwing exceptions including duplicates, invalid
        //titles, and titles containing invalid path characters
        private static int dupCounter, invalidCounter, invalidPathCounter;

        //Objects that act as access granters to the counter variables declared above.
        //These objects are locked on by the thread currently using and updating them
        private static object invalidCounterLock = new object();
        private static object invalidPathCounterLock = new object();
        private static object dupCounterLock = new object();

        private byte[] imageBytes;
        private string currentSearch;
        private FlickrResult selectedItem;

        //CONSTRUCTOR
        public FlickrViewerForm()
        {
            InitializeComponent();
        }


        #region Event Handlers
        /// <summary>
        ///     Initiates asynchronous Flickr search query and displays results when
        ///     query completes
        /// </summary>
        private async void searchButton_Click(object sender, EventArgs e)
        {
            // Flickr's API URL for searches                         
            var flickrURL = "https://api.flickr.com/services/rest/?method=" +
            $"flickr.photos.search&api_key={KEY}&" +
            $"tags={txtSearch.Text.Replace(" ", ",")}" +
            "&tag_mode=all&per_page=500&privacy_filter=1";

            lsbImages.DataSource = null; // remove prior data source
            lsbImages.Items.Clear(); // clear imagesListBox
            pbSelected.Image = null; // clear pictureBox
            lsbImages.Items.Add("Loading..."); // display Loading...

            // invoke Flickr web service to search Flick with user's tags
            flickrTask = flickrClient.GetStringAsync(flickrURL);

            // await flickrTask then parse results with XDocument and LINQ
            XDocument flickrXML = XDocument.Parse(await flickrTask);

            // gather information on all photos
            var flickrPhotos =
            from photo in flickrXML.Descendants("photo")
            let id = photo.Attribute("id").Value
            let title = photo.Attribute("title").Value
            let secret = photo.Attribute("secret").Value
            let server = photo.Attribute("server").Value
            let farm = photo.Attribute("farm").Value
            select new FlickrResult
            {
                Title = title,
                URL = $"https://farm{farm}.staticflickr.com/" +
                    $"{server}/{id}_{secret}.jpg"
            };

            lsbImages.Items.Clear(); // clear imagesListBox

            // set ListBox properties only if results were found
            if (flickrPhotos.Any())
            {
                lsbImages.DataSource = flickrPhotos.ToList();
                lsbImages.DisplayMember = "Title";
            }
            else // no matches were found
            {
                lsbImages.Items.Add("No matches");
            }
        }

        /// <summary>
        ///     Displays the image selected, saves the image locally, while also 
        ///     creating and saving a thumbnail of the same.
        /// </summary>
        private async void imagesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //If something is selected ...then
            if (lsbImages.SelectedItem != null)
            {
                selectedItem = (lsbImages.SelectedItem as FlickrResult);

                imageBytes = await flickrClient.GetByteArrayAsync(selectedItem.URL);

                //Delegate type representing display of images 'to be run' in parallel
                //**Use of named methods as per instruction
                Action displayImageAction = DisplayImage;            

                currentSearch = txtSearch.Text;

                //Assumes that if the folder exists, all the subfolders exist as well
                if (!Directory.Exists($@"{Directory.GetCurrentDirectory()}\{currentSearch}"))
                {
                    CreateDirectories(currentSearch);
                }

                //Delegate type representing saving image locally 'to be run' in parallel
                Action saveImageAction = SaveImage;

                //Delegate type representing creating and saving thumbnail locally 'to be run' in parallel
                Action saveThumbnailAction = SaveThumbnail;

                //Runs all three Actions declared above in Parallel
                Parallel.Invoke(displayImageAction, saveImageAction, saveThumbnailAction);
            }
        }
        #endregion

        #region Tasks to be run in parallel
        /// <summary>
        ///     Loads the image from a stream and passes it onto the PictureBox
        /// </summary>
        private void DisplayImage()
        {
            // display downloaded image in pictureBox                  
            using (var memoryStream = new MemoryStream(imageBytes))
            {
                pbSelected.Image = Image.FromStream(memoryStream);
            }
        }

        /// <summary>
        ///     Loads and saves the image from a stream with exception handling for 
        ///     cases such as unsupported titles, invalid path character in file 
        ///     names and duplicate file names.
        /// </summary>
        private void SaveImage()
        {
            using (var memoryStream = new MemoryStream(imageBytes))
            {
                Image image = Image.FromStream(memoryStream);
                FileStream selectedImage = null;
                try
                {
                    //If not duplicate... then
                    if (!File.Exists($@"{Directory.GetCurrentDirectory()}\{currentSearch}\{selectedItem.Title}.jpeg"))
                    {
                        selectedImage = new FileStream($@"{currentSearch}\{selectedItem.Title}.jpeg", FileMode.Create, FileAccess.Write);
                    }
                    else //Exception thrown when duplicate...handled below
                    {
                        throw new IOException();
                    }
                }
                catch (ArgumentException) //Unsupported file names (titles)
                {
                    selectedImage = new FileStream($@"{currentSearch}\InvalidNames\untitled{invalidCounter}.jpeg", FileMode.Create, FileAccess.Write);

                    //A lock is placed to avoid multiple threads updating the
                    //invalidCounter at once
                    lock (invalidCounterLock)
                    {
                        invalidCounter += File.Exists($@"{currentSearch}\InvalidNames\Thumbnails\T_untitled{invalidCounter}.jpeg") ? 1 : 0;
                    }
                }
                catch (NotSupportedException) //Invalid Characters in Title
                {
                    selectedImage = new FileStream($@"{currentSearch}\InvalidPathCharacters\untitled{invalidPathCounter}.jpeg", FileMode.Create, FileAccess.Write);

                    //A lock is placed to avoid multiple threads updating the
                    //invalidPathCounter at once
                    lock (invalidPathCounterLock)
                    {
                        invalidPathCounter += File.Exists($@"{currentSearch}\InvalidPathCharacters\Thumbnails\T_untitled{invalidPathCounter}.jpeg") ? 1 : 0;
                    }
                }
                catch (IOException) //Duplicate File Names
                {
                    selectedImage = new FileStream($@"{currentSearch}\DuplicateNames\untitled{dupCounter}.jpeg", FileMode.Create, FileAccess.Write);

                    //A lock is placed to avoid multiple threads updating the
                    //dupCounter at once
                    lock (dupCounterLock)
                    {
                        dupCounter += File.Exists($@"{currentSearch}\DuplicateNames\Thumbnails\T_untitled{dupCounter}.jpeg") ? 1 : 0;
                    }
                }
                finally //Saves the image and closes the FileStream
                {
                    if (selectedImage != null)
                    {
                        image.Save(selectedImage, ImageFormat.Jpeg);
                        selectedImage.Close();
                    }
                }
            }
        }

        /// <summary>
        ///     Loads and resizes the image from a stream, upon which it is saved. 
        ///     The method also provides exception handling for cases such as 
        ///     unsupported titles, invalid path character in file names and
        ///     duplicate file names.
        /// </summary>
        private void SaveThumbnail()
        {
            int width = 200;
            using (var memoryStream = new MemoryStream(imageBytes))
            {
                Image image = Image.FromStream(memoryStream);

                //Height determination to preserve image aspect ratio
                var newHeight = (width * image.Height) / image.Width;

                //The resized image, aka the thumbnail
                Image resized = image.GetThumbnailImage(width, newHeight, null, IntPtr.Zero);

                FileStream resizedImage = null;
                try
                {
                    //If not duplicate... then
                    if (!File.Exists($@"{Directory.GetCurrentDirectory()}\{currentSearch}\Thumbnails\T_{selectedItem.Title}.jpeg"))
                    {
                        resizedImage = new FileStream($@"{currentSearch}\Thumbnails\T_{selectedItem.Title}.jpeg", FileMode.Create, FileAccess.Write);
                    }
                    else //Exception thrown when duplicate...handled below
                    {
                        throw new IOException();
                    }
                }
                catch (ArgumentException)
                {
                    resizedImage = new FileStream($@"{currentSearch}\InvalidNames\Thumbnails\T_untitled{invalidCounter}.jpeg", FileMode.Create, FileAccess.Write);

                    //A lock is placed to avoid multiple threads updating the
                    //invalidCounter at once
                    lock (invalidCounterLock)
                    {
                        invalidCounter += File.Exists($@"{currentSearch}\InvalidNames\untitled{invalidCounter}.jpeg") ? 1 : 0;
                    }
                }
                catch (NotSupportedException)
                {
                    resizedImage = new FileStream($@"{currentSearch}\InvalidPathCharacters\Thumbnails\T_untitled{invalidPathCounter}.jpeg", FileMode.Create, FileAccess.Write);

                    //A lock is placed to avoid multiple threads updating the
                    //invalidPathCounter at once
                    lock (invalidPathCounterLock)
                    {
                        invalidPathCounter += File.Exists($@"{currentSearch}\InvalidPathCharacters\untitled{invalidPathCounter}.jpeg") ? 1 : 0;
                    }
                }
                catch (IOException)
                {
                    resizedImage = new FileStream($@"{currentSearch}\DuplicateNames\Thumbnails\T_untitled{dupCounter}.jpeg", FileMode.Create, FileAccess.Write);

                    //A lock is placed to avoid multiple threads updating the
                    //dupCounter at once
                    lock (dupCounterLock)
                    {
                        dupCounter += File.Exists($@"{currentSearch}\DuplicateNames\untitled{dupCounter}.jpeg") ? 1 : 0;
                    }
                }
                finally // Saves the thumbnail and closes the FileStream
                {
                    if (resizedImage != null)
                    {
                        resized.Save(resizedImage, ImageFormat.Jpeg);
                        resizedImage.Close();
                    }
                }
            }
        }
        #endregion

        #region Miscellaneous
        /// <summary>
        ///     Creates directories for storing images, duplicates, invalid titles, 
        ///     invalid path characters and all of their thumbnails
        /// </summary>
        /// <param name="currentSearch">
        ///     The user-entered search tag used as the name of the root directory
        ///     for storing images
        /// </param>
        private void CreateDirectories(string currentSearch)
        {
            Directory.CreateDirectory($@"{Directory.GetCurrentDirectory()}\{currentSearch}");
            Directory.CreateDirectory($@"{Directory.GetCurrentDirectory()}\{currentSearch}\InvalidPathCharacters");
            Directory.CreateDirectory($@"{Directory.GetCurrentDirectory()}\{currentSearch}\InvalidNames");
            Directory.CreateDirectory($@"{Directory.GetCurrentDirectory()}\{currentSearch}\DuplicateNames");

            Directory.CreateDirectory($@"{Directory.GetCurrentDirectory()}\{currentSearch}\Thumbnails");
            Directory.CreateDirectory($@"{Directory.GetCurrentDirectory()}\{currentSearch}\InvalidPathCharacters\Thumbnails");
            Directory.CreateDirectory($@"{Directory.GetCurrentDirectory()}\{currentSearch}\InvalidNames\Thumbnails");
            Directory.CreateDirectory($@"{Directory.GetCurrentDirectory()}\{currentSearch}\DuplicateNames\Thumbnails");
        }
        #endregion
    }
}

///References
/**************************************************************************
 * (C) Copyright 1992-2017 by Deitel & Associates, Inc. and               *
 * Pearson Education, Inc. All Rights Reserved.                           *
 *                                                                        *
 * DISCLAIMER: The authors and publisher of this book have used their     *
 * best efforts in preparing the book. These efforts include the          *
 * development, research, and testing of the theories and programs        *
 * to determine their effectiveness. The authors and publisher make       *
 * no warranty of any kind, expressed or implied, with regard to these    *
 * programs or to the documentation contained in these books. The authors *
 * and publisher shall not be liable in any event for incidental or       *
 * consequential damages in connection with, or arising out of, the       *
 * furnishing, performance, or use of these programs.                     *
 **************************************************************************/
