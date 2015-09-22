using App1.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Media;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.Storage.FileProperties;

// Pour en savoir plus sur le modèle d'élément Page de base, consultez la page http://go.microsoft.com/fwlink/?LinkId=234237

namespace App1
{
    /// <summary>
    /// Page de base qui inclut des caractéristiques communes à la plupart des applications.
    /// </summary>
    public partial class MainPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private bool isManipulate = false;
        /// <summary>
        /// Cela peut être remplacé par un modèle d'affichage fortement typé.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper est utilisé sur chaque page pour faciliter la navigation et 
        /// gestion de la durée de vie des processus
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public MainPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Remplit la page à l'aide du contenu passé lors de la navigation. Tout état enregistré est également
        /// fourni lorsqu'une page est recréée à partir d'une session antérieure.
        /// </summary>
        /// <param name="sender">
        /// La source de l'événement ; en général <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Données d'événement qui fournissent le paramètre de navigation transmis à
        /// <see cref="Frame.Navigate(Type, Object)"/> lors de la requête initiale de cette page et
        /// un dictionnaire d'état conservé par cette page durant une session
        /// antérieure. L'état n'aura pas la valeur Null lors de la première visite de la page.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Conserve l'état associé à cette page en cas de suspension de l'application ou de la
        /// suppression de la page du cache de navigation.  Les valeurs doivent être conformes aux
        /// exigences en matière de sérialisation de <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">La source de l'événement ; en général <see cref="NavigationHelper"/></param>
        /// <param name="e">Données d'événement qui fournissent un dictionnaire vide à remplir à l'aide de
        /// état sérialisable.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region Inscription de NavigationHelper

        /// Les méthodes fournies dans cette section sont utilisées simplement pour permettre
        /// NavigationHelper pour répondre aux méthodes de navigation de la page.
        /// 
        /// La logique spécifique à la page doit être placée dans les gestionnaires d'événements pour  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// et <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// Le paramètre de navigation est disponible dans la méthode LoadState 
        /// en plus de l'état de page conservé durant une session antérieure.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
            var file = e.Parameter as string;
            if (file.Length > 0)
                PlayFile(file);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void backButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void switchLibrary(object sender, RoutedEventArgs e)
        {
            var frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(BasicPage1));
            //this.Frame.Navigate(typeof(BasicPage1), null);
        }

        bool played = false;
        bool isLoaded = false;

        private void play(object sender, RoutedEventArgs e)
        {
            if (!played && isLoaded)
            {
                buttonPlay.Content = "Pause";
                ElementMedia.Play();
                timerVideoTime.Tick += timer_Tick;
                TimeBar.Maximum = ElementMedia.NaturalDuration.TimeSpan.TotalSeconds;
                played = true;
            }
            else if (isLoaded)
            {
                 buttonPlay.Content = "Play";
                 timerVideoTime.Tick -= timer_Tick;
                 ElementMedia.Pause();
                 played = false;
            }
        }

        private async void search(object sender, RoutedEventArgs e)
        {
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".wma");
            openPicker.FileTypeFilter.Add(".mp3");
            openPicker.FileTypeFilter.Add(".m4a");
            openPicker.FileTypeFilter.Add(".jpg");

            var file = await openPicker.PickSingleFileAsync();
            my_play(file);
        }

        private async void PlayFile(string path)
        {
            StorageFile file = await Windows.Storage.StorageFile.GetFileFromPathAsync(path);
            my_play(file);
        }

        private static async Task<BitmapImage> LoadImage(StorageFile file)
        {
            BitmapImage bitmapImage = new BitmapImage();
            FileRandomAccessStream stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read);

            bitmapImage.SetSource(stream);

            return bitmapImage;

        }

        private void stop(object sender, RoutedEventArgs e)
        {
            my_stop();
        }

        private TimeSpan TotalTime;
        private DispatcherTimer timerVideoTime;
        private void setSlider(object sender, RoutedEventArgs e)
        {
            TotalTime = ElementMedia.NaturalDuration.TimeSpan;

            TimeBar.Maximum = ElementMedia.NaturalDuration.TimeSpan.TotalSeconds;

            timerVideoTime = new DispatcherTimer();
            timerVideoTime.Interval = TimeSpan.FromSeconds(1);
            timerVideoTime.Tick += timer_Tick;
            timerVideoTime.Start();
        }

        protected void timer_Tick(object sender, object e)
        {
            if (ElementMedia.NaturalDuration.TimeSpan.TotalSeconds > 0 & TotalTime.TotalSeconds > 0)
                TimeBar.Value = ElementMedia.Position.TotalSeconds;

        }

        private void updateTimeBar(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (ElementMedia.NaturalDuration.TimeSpan.TotalSeconds != 0)
                TimeBar.Value = ElementMedia.Position.Seconds / ElementMedia.NaturalDuration.TimeSpan.TotalSeconds;
        }

        private void test(object sender, RoutedEventArgs e)
        {
        }

        private void updateVolume(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (myAppBar != null)
                if (myAppBar.IsOpen)
                    if (isLoaded)
                        ElementMedia.Volume = (double)VolumeBar.Value / 100;
        }

        private void my_stop()
        {
            if (isLoaded)
            {
                ElementMedia.Stop();
                TimeBar.Value = 0;
                TimeBar.Maximum = 1;
                buttonPlay.Content = "Play";
                played = false;
                timerVideoTime.Tick -= timer_Tick;
            }
        }

        private async void my_play(StorageFile file)
        {
            if (null != file)
            {
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);

                if (file.FileType == ".jpg" | file.FileType == ".JPG")
                {
                    if (played)
                        my_stop();
                    BitmapImage img = new BitmapImage();
                    img = await LoadImage(file);
                    Visio.Source = img;
                    ElementMedia.Width = 1;
                    ElementMedia.Height = 1;
                }
                else
                {
                    MusicProperties property;

                    Visio.Source = null;
                    ElementMedia.SetSource(stream, file.ContentType);
                    Debug.WriteLine("\nEnvois du path: " + file.Path);
                    buttonPlay.Content = "Pause";
                    played = true;
                    isLoaded = true;
                    ElementMedia.Play();
                    if (file.FileType == ".mp3")
                    {
                        property = await file.Properties.GetMusicPropertiesAsync();
                        string name = property.Title + ".png";
                        Visio.Source =  new BitmapImage(new Uri("ms-appdata:///local/" + name));
                        ElementMedia.Width = 1;
                        ElementMedia.Height = 1;
                    }
                    else
                    {
                        ElementMedia.Width = Window.Current.Bounds.Width;
                        ElementMedia.Height = Window.Current.Bounds.Height;
                    }
                }
            }
            else
            {
                isLoaded = false;
                played = false;
            }
        }

        private void switchLibrary()
        {
            this.Frame.Navigate(typeof(BasicPage1), null);
        }

        private void init(object sender, object e)
        {
            if (Visio != null)
            {
                if (Visio.Source == null)
                {
                    ElementMedia.Width = Window.Current.Bounds.Width;
                    ElementMedia.Height = (Window.Current.Bounds.Height * 78.0) / 100;
                }
                else
                {
                    Visio.Width = Window.Current.Bounds.Width;
                    Visio.Height = (Window.Current.Bounds.Height * 78.0) / 100;
                }
            }
        }

        private void reset(object sender, object e)
        {
            if (Visio.Source == null)
            {
                ElementMedia.Width = Window.Current.Bounds.Width;
                ElementMedia.Height = Window.Current.Bounds.Height;
            }
            else
            {
                Visio.Width = Window.Current.Bounds.Width;
                Visio.Height = Window.Current.Bounds.Height;
            }
        }

        private void update(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (isManipulate)
                ElementMedia.Position = TimeSpan.FromSeconds(TimeBar.Value);
        }

        private void manipulateON(object sender, PointerRoutedEventArgs e)
        {
            isManipulate = true;
        }

        private void manipulateOFF(object sender, PointerRoutedEventArgs e)
        {
            isManipulate = false;
        }
    }
}
