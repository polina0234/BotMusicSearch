namespace BotMusicSearch.Models
{
    public class VideoDetailsResponse
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public VideoItem[] items { get; set; }
        public VideoPageInfo pageInfo { get; set; }
    }

    public class VideoPageInfo
    {
        public int totalResults { get; set; }
        public int resultsPerPage { get; set; }
    }

    public class VideoItem
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string id { get; set; }
        public VideoSnippet snippet { get; set; }
        public VideoContentDetails contentDetails { get; set; }
        public VideoStatistics statistics { get; set; }
    }
    public class VideoContentDetails
    {
        public string duration { get; set; }
    }

    public class VideoStatistics
    {
        public string viewCount { get; set; }
        public string likeCount { get; set; }
        public string commentCount { get; set; }
    }
    public class VideoSnippet
    {
        public DateTime publishedAt { get; set; }
        public string channelId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public VideoThumbnails thumbnails { get; set; }
        public string channelTitle { get; set; }
        public string[] tags { get; set; }
        public string categoryId { get; set; }
        public string liveBroadcastContent { get; set; }
        public string defaultLanguage { get; set; }
        public Localized localized { get; set; }
        public string defaultAudioLanguage { get; set; }
    }

    public class VideoThumbnails
    {
        public VideoThumbnail _default { get; set; }
        public VideoThumbnail medium { get; set; }
        public VideoThumbnail high { get; set; }
        public VideoThumbnail standard { get; set; }
        public VideoThumbnail maxres { get; set; }
    }

    public class VideoThumbnail
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Localized
    {
        public string title { get; set; }
        public string description { get; set; }
    }
}