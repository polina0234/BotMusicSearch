namespace BotMusicSearch.Models
{
    public class RelatedSearchResponse
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string nextPageToken { get; set; }
        public string regionCode { get; set; }
        public RelatedPageInfo pageInfo { get; set; }
        public RelatedItem[] items { get; set; }
    }

    public class RelatedPageInfo
    {
        public int totalResults { get; set; }
        public int resultsPerPage { get; set; }
    }

    public class RelatedItem
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public RelatedId id { get; set; }
        public RelatedSnippet snippet { get; set; }
    }

    public class RelatedId
    {
        public string kind { get; set; }
        public string videoId { get; set; }
    }

    public class RelatedSnippet
    {
        public DateTime publishedAt { get; set; }
        public string channelId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public RelatedThumbnails thumbnails { get; set; }
        public string channelTitle { get; set; }
        public string liveBroadcastContent { get; set; }
        public DateTime publishTime { get; set; }
    }

    public class RelatedThumbnails
    {
        public RelatedThumbnail _default { get; set; }
        public RelatedThumbnail medium { get; set; }
        public RelatedThumbnail high { get; set; }
    }

    public class RelatedThumbnail
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
}