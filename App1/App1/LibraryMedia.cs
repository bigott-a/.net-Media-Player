using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;

namespace App1
{
    public class Music
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Path { get; set; }

        public Music(string Title_, string Artist_, string _Path)
        {
            Title = Title_;
            Artist = Artist_;
            Path = _Path;
        }

    }

    public class LibraryMedia
    {
        private ObservableCollection<Music> _ListTitle = new ObservableCollection<Music>();
        public ObservableCollection<Music> ListTitle { get { return _ListTitle; } set { _ListTitle = value; } }

        private ObservableCollection<Music> _ListVideo = new ObservableCollection<Music>();
        public ObservableCollection<Music> ListVideo { get { return _ListVideo; } set { _ListVideo = value; } }

        private static LibraryMedia instance;
        public static LibraryMedia Current
        {
            get
            {
                if (instance == null)
                    instance = new LibraryMedia();

                return instance;
            }
        }

        public LibraryMedia()
        {
        }

        public async void FillVideo()
        {
            StorageFolder videoFolder = KnownFolders.VideosLibrary;

            IReadOnlyList<StorageFolder> albumFolders = await videoFolder.GetFoldersAsync(CommonFolderQuery.GroupByAlbum);

            foreach (var albumFolder in albumFolders)
            {
                var Lolesque = albumFolder.CreateFileQueryWithOptions(new QueryOptions(CommonFileQuery.OrderByName, new string[] { ".mp4", ".mkv", ".avi" }));
                foreach (var track in await Lolesque.GetFilesAsync())
                {
                    VideoProperties trackInfos = await track.Properties.GetVideoPropertiesAsync();

                    Debug.WriteLine("ICI : " + albumFolder.Path);

                    string Tile = track.Name;
                    Debug.WriteLine(Tile);

                    if (trackInfos.Title != "" && trackInfos.Publisher != "")
                        _ListVideo.Add(new Music(trackInfos.Title, trackInfos.Publisher, track.Path));
                    else
                        _ListVideo.Add(new Music(track.DisplayName, "", track.Path));

                    //_ListTitle.Add(new Music(trackInfos.Title, trackInfos.Publisher, track.Path));
                }
            }
        }
        
        public async void FillSync()
        {
            StorageFolder musicFolder = KnownFolders.MusicLibrary;

            IReadOnlyList<StorageFolder> albumFolders = await musicFolder.GetFoldersAsync(CommonFolderQuery.GroupByAlbum);


            foreach (var albumFolder in albumFolders)
            {
                var Lolesque = albumFolder.CreateFileQueryWithOptions(new QueryOptions(CommonFileQuery.OrderByMusicProperties, new string[] { ".mp3", ".wma" }));
                foreach (var track in await Lolesque.GetFilesAsync())
                {
                    var trackInfos = await track.Properties.GetMusicPropertiesAsync();

                    if (trackInfos.Title != "" && trackInfos.Artist != "")
                        _ListTitle.Add(new Music(trackInfos.Title, trackInfos.Artist, track.Path));
                }
            }
        }
    }
}
