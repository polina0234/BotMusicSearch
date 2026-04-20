using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using System.Globalization;
using BotMusicSearch.Models;

class Program
{
    static async Task Main(string[] args)
    {
        string rapidApiKey = "949809a65bmsh10f285348c494d5p172789jsn928be670203a";
        string rapidApiHost = "youtube-music6.p.rapidapi.com";
        string youtubeApiKey = "AIzaSyD4YZMgz6w_IUsCsRmWEw4KNFnapqy1j5w";

        Console.Write("Введи назву піснi або виконавця: ");
        string query = Console.ReadLine();

        using HttpClient client = new HttpClient();

        // ========== ЗАПИТ №1: ПОШУК ==========
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("x-rapidapi-key", rapidApiKey);
        client.DefaultRequestHeaders.Add("x-rapidapi-host", rapidApiHost);

        string searchUrl = $"https://youtube-music6.p.rapidapi.com/ytmusic/?query={Uri.EscapeDataString(query)}&limit=15";

        var searchResponse = await client.GetAsync(searchUrl);
        if (!searchResponse.IsSuccessStatusCode)
        {
            Console.WriteLine($"Помилка пошуку: {searchResponse.StatusCode}");
            Console.ReadKey();
            return;
        }
        string searchJson = await searchResponse.Content.ReadAsStringAsync();

        var searchResults = JsonConvert.DeserializeObject<Class1[]>(searchJson);
        var songs = searchResults.Where(x => x.resultType == "song").ToList();
        var sortedSongs = songs.OrderByDescending(x => ParseViews(x.views)).ToList();

        Console.WriteLine($"\nЗнайдено {sortedSongs.Count} пiсень:\n");
        for (int i = 0; i < Math.Min(15, sortedSongs.Count); i++)
        {
            var song = sortedSongs[i];
            string artists = string.Join(", ", song.artists?.Select(a => a.name) ?? new[] { "Невідомий" });
            Console.WriteLine($"{i + 1}. {artists} - {song.title}");
            Console.WriteLine($"   Переглядiв: {song.views}");
            Console.WriteLine($"   https://youtu.be/{song.videoId}\n");
        }

        // ========== ВИБІР ВІДЕО ==========
        Console.Write("\nВиберiть номер вiдео для детальної iнформацiї: ");
        int selected = int.Parse(Console.ReadLine()) - 1;
        string selectedVideoId = sortedSongs[selected].videoId;

        // ========== ЗАПИТ №2: ДЕТАЛЬНА ІНФОРМАЦІЯ ==========
        client.DefaultRequestHeaders.Clear();
        string videoUrl = $"https://www.googleapis.com/youtube/v3/videos?part=snippet,contentDetails,statistics&id={selectedVideoId}&key={youtubeApiKey}";

        var videoResponse = await client.GetAsync(videoUrl);
        if (!videoResponse.IsSuccessStatusCode)
        {
            Console.WriteLine($"Помилка отримання деталей: {videoResponse.StatusCode}");
            Console.ReadKey();
            return;
        }
        string videoJson = await videoResponse.Content.ReadAsStringAsync();

        var videoData = JsonConvert.DeserializeObject<VideoDetailsResponse>(videoJson);

        Console.WriteLine("\nДЕТАЛЬНА IНФОРМАЦIЯ:\n");
        if (videoData.items != null && videoData.items.Length > 0)
        {
            var video = videoData.items[0];
            var snippet = video.snippet;

            string duration = "Невідомо";
            if (video.contentDetails?.duration != null)
            {
                duration = video.contentDetails.duration;
                duration = duration.Replace("PT", "").Replace("H", " год ").Replace("M", " хв ").Replace("S", " сек");
                if (duration == "") duration = "0 сек";
            }

            Console.WriteLine($"   Назва: {snippet.title}");
            Console.WriteLine($"   Канал: {snippet.channelTitle}");
            Console.WriteLine($"   Дата: {snippet.publishedAt:yyyy-MM-dd}");
            Console.WriteLine($"   Тривалiсть: {duration}");
            Console.WriteLine($"   https://www.youtube.com/watch?v={selectedVideoId}");
        }
        else
        {
            Console.WriteLine("   Не вдалося отримати деталi.");
        }

        // ========== ЗАПИТ №3: СХОЖІ ТРЕКИ (ЗАПИТУЄМО КОРИСТУВАЧА) ==========
        Console.Write("\nБажаєте знайти схожi треки? (так/нi): ");
        string answer = Console.ReadLine()?.ToLower();

        if (answer == "так" || answer == "yes" || answer == "т" || answer == "y")
        {
            string artistName = sortedSongs[selected].artists?[0]?.name ?? "";
            string songTitle = sortedSongs[selected].title ?? "";
            string searchQuery = $"{artistName} {songTitle} official music video";

            string relatedUrl = $"https://www.googleapis.com/youtube/v3/search?part=snippet&q={Uri.EscapeDataString(searchQuery)}&type=video&maxResults=8&key={youtubeApiKey}";

            var relatedResponse = await client.GetAsync(relatedUrl);
            if (!relatedResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"Помилка отримання схожих трекiв: {relatedResponse.StatusCode}");
                Console.ReadKey();
                return;
            }
            string relatedJson = await relatedResponse.Content.ReadAsStringAsync();

            var relatedData = JsonConvert.DeserializeObject<RelatedSearchResponse>(relatedJson);

            Console.WriteLine("\nСХОЖI ТРЕКИ:\n");
            if (relatedData.items != null && relatedData.items.Length > 0)
            {
                var relatedVideos = relatedData.items
                    .Where(x => x.id?.videoId != null)
                    .OrderByDescending(x => x.snippet.publishedAt)
                    .Take(8)
                    .ToList();

                for (int i = 0; i < relatedVideos.Count; i++)
                {
                    var video = relatedVideos[i];
                    Console.WriteLine($"{i + 1}. {video.snippet.title}");
                    Console.WriteLine($"   Канал: {video.snippet.channelTitle}");
                    Console.WriteLine($"   https://www.youtube.com/watch?v={video.id.videoId}\n");
                }
            }
            else
            {
                Console.WriteLine("   Не знайдено схожих трекiв.");
            }
        }
        else
        {
            Console.WriteLine("\nДякую за використання! До побачення.");
        }

        Console.ReadKey();
    }

    static double ParseViews(string views)
    {
        if (string.IsNullOrEmpty(views)) return 0;
        try
        {
            if (views.EndsWith("M"))
                return double.Parse(views.Replace("M", ""), CultureInfo.InvariantCulture) * 1000000;
            if (views.EndsWith("K"))
                return double.Parse(views.Replace("K", ""), CultureInfo.InvariantCulture) * 1000;
            return double.Parse(views, CultureInfo.InvariantCulture);
        }
        catch { return 0; }
    }
}