using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using BotMusicSearch.Models;

class Program
{
    static async Task Main(string[] args)
    {
        string youtubeApiKey = "AIzaSyD4YZMgz6w_IUsCsRmWEw4KNFnapqy1j5w";

        Console.Write("Введи назву піснi або виконавця: ");
        string query = Console.ReadLine();

        using HttpClient client = new HttpClient();

        // ========== ЗАПИТ №1: ПОШУК ==========
        string searchQuery = $"{query} official music video";
        string searchUrl = $"https://www.googleapis.com/youtube/v3/search?part=snippet&maxResults=30&q={Uri.EscapeDataString(searchQuery)}&type=video&key={youtubeApiKey}";
        var searchResponse = await client.GetAsync(searchUrl);

        if (!searchResponse.IsSuccessStatusCode)
        {
            Console.WriteLine($"Помилка пошуку: {searchResponse.StatusCode}");
            Console.ReadKey();
            return;
        }

        string searchJson = await searchResponse.Content.ReadAsStringAsync();
        var searchData = JsonConvert.DeserializeObject<Search>(searchJson);

        // ========== ОБРОБКА ДАНИХ: ФІЛЬТРАЦІЯ (тільки пісні) ==========
        var filteredSongs = searchData.items
            .Where(item =>
            {
                string title = item.snippet.title.ToLower();
                string channelTitle = item.snippet.channelTitle.ToLower();

                // Канали, які часто видають музику
                bool isMusicChannel = channelTitle.Contains("topic") ||
                                      channelTitle.Contains("vevo") ||
                                      channelTitle.Contains("official") ||
                                      channelTitle.Contains("music");

                // Ключові слова в назві, що вказують на пісню
                bool isSong = title.Contains("official") ||
                              title.Contains("music video") ||
                              title.Contains("audio") ||
                              title.Contains("lyrics") ||
                              title.Contains("official video") ||
                              title.Contains("track") ||
                              title.Contains("song") ||
                              title.Contains("клип") ||
                              title.Contains("пісня") ||
                              title.Contains("альбом");

                // Виключаємо явно не музичне
                bool isNotSong = title.Contains("live") ||
                                 title.Contains("реакция") ||
                                 title.Contains("обзор") ||
                                 title.Contains("топ") ||
                                 title.Contains("fact") ||
                                 title.Contains("наука") ||
                                 title.Contains("эксперимент") ||
                                 title.Contains("фокус") ||
                                 title.Contains("проводка") ||
                                 title.Contains("аккумулятор");

                return (isMusicChannel || isSong) && !isNotSong;
            })
            .Select(item => new
            {
                videoId = item.id.videoId,
                title = item.snippet.title,
                channelTitle = item.snippet.channelTitle,
                publishedAt = item.snippet.publishedAt
            })
            .ToList();

        // ========== ОБРОБКА ДАНИХ: СОРТУВАННЯ (за датою, новіші перші) ==========
        var sortedSongs = filteredSongs.OrderByDescending(s => s.publishedAt).ToList();

        Console.WriteLine($"\nЗнайдено пiсень: {sortedSongs.Count}\n");
        for (int i = 0; i < Math.Min(15, sortedSongs.Count); i++)
        {
            var song = sortedSongs[i];
            Console.WriteLine($"{i + 1}. {song.channelTitle} - {song.title}");
            Console.WriteLine($"   https://youtu.be/{song.videoId}\n");
        }

        if (sortedSongs.Count == 0)
        {
            Console.WriteLine("Не знайдено пiсень.");
            Console.ReadKey();
            return;
        }

        // ========== ВИБІР ВІДЕО ==========
        Console.Write("\nВиберiть номер пiснi для детальної iнформацiї: ");
        int selected = int.Parse(Console.ReadLine()) - 1;
        string selectedVideoId = sortedSongs[selected].videoId;

        // ========== ЗАПИТ №2: ДЕТАЛЬНА ІНФОРМАЦІЯ ==========
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
            Console.WriteLine($"   Переглядiв: {video.statistics?.viewCount ?? "Немає даних"}");
            Console.WriteLine($"   https://www.youtube.com/watch?v={selectedVideoId}");
        }
        else
        {
            Console.WriteLine("   Не вдалося отримати деталi.");
        }

        // ========== ЗАПИТ №3: СХОЖІ ТРЕКИ ==========
        Console.Write("\nБажаєте знайти схожi треки? (так/нi): ");
        string answer = Console.ReadLine()?.ToLower();

        if (answer == "так" || answer == "yes" || answer == "т" || answer == "y")
        {
            string artistName = sortedSongs[selected].channelTitle;
            string songTitle = sortedSongs[selected].title;
            string similarQuery = $"{artistName} {songTitle} official music video";

            string relatedUrl = $"https://www.googleapis.com/youtube/v3/search?part=snippet&q={Uri.EscapeDataString(similarQuery)}&type=video&maxResults=8&key={youtubeApiKey}";
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
                for (int i = 0; i < Math.Min(8, relatedData.items.Length); i++)
                {
                    var video = relatedData.items[i];
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
}