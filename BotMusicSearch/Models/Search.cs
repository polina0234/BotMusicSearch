
public class Search
{
    public string kind { get; set; }
    public string etag { get; set; }
    public string nextPageToken { get; set; }
    public string regionCode { get; set; }
    public SearchPageInfo pageInfo { get; set; }
    public SearchItem[] items { get; set; }
}

public class SearchPageInfo
{
    public int totalResults { get; set; }
    public int resultsPerPage { get; set; }
}

public class SearchItem
{
    public string kind { get; set; }
    public string etag { get; set; }
    public SearchId id { get; set; }
    public SearchSnippet snippet { get; set; }
}

public class SearchId
{
    public string kind { get; set; }
    public string videoId { get; set; }
}

public class SearchSnippet
{
    public DateTime publishedAt { get; set; }
    public string channelId { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public SearchThumbnails thumbnails { get; set; }
    public string channelTitle { get; set; }
    public string liveBroadcastContent { get; set; }
    public DateTime publishTime { get; set; }
}

public class SearchThumbnails
{
    public SearchThumbnail _default { get; set; }
    public SearchThumbnail medium { get; set; }
    public SearchThumbnail high { get; set; }
}

public class SearchThumbnail
{
    public string url { get; set; }
    public int width { get; set; }
    public int height { get; set; }
}
