namespace BotMusicSearch.Models
{
    public class Rootobject
    {
        public Class1[] Property1 { get; set; }
    }

    public class Class1
    {
        public string category { get; set; }
        public string resultType { get; set; }
        public string videoId { get; set; }
        public string videoType { get; set; }
        public string title { get; set; }
        public Artist[] artists { get; set; }
        public string views { get; set; }
        public string duration { get; set; }
        public int duration_seconds { get; set; }
        public Thumbnail[] thumbnails { get; set; }
        public string year { get; set; }
        public object album { get; set; }
        public bool inLibrary { get; set; }
        public bool pinnedToListenAgain { get; set; }
        public bool isExplicit { get; set; }
        public string artist { get; set; }
        public string shuffleId { get; set; }
        public string radioId { get; set; }
        public string browseId { get; set; }
        public string type { get; set; }
        public string playlistId { get; set; }
        public object itemCount { get; set; }
        public string author { get; set; }
        public bool live { get; set; }
        public string date { get; set; }
        public Podcast podcast { get; set; }
    }

    public class Podcast
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Artist
    {
        public string name { get; set; }
        public string id { get; set; }
    }

    public class Thumbnail
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
}