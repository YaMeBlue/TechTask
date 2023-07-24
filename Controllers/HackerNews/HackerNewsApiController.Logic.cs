using Microsoft.AspNetCore.Mvc;

using System.Text.Json;

using TechTask.Models;

namespace TechTask.Controllers.HackerNews
{
    public partial class HackerNewsApiController
    {
        private async Task<int[]?> GetBestStoriesFromOutside()
        {
            var bestStoriesResponse = await _httpClient.GetAsync(_newsConfiguration.BaseUrl + _newsConfiguration.BestStoriesUrl);

            var bestStoriesResponseContent = JsonSerializer.Deserialize<int[]>(await bestStoriesResponse.Content.ReadAsStringAsync());
            return bestStoriesResponseContent;
        }

        private async Task<HttpResponseMessage[]> ProcessBestStoriesDataAsync(int[] bestStoriesResponseContent, int capacity)
        {
            var allBestStoryResponses = new List<Task<HttpResponseMessage>>(capacity);

            foreach (var item in bestStoriesResponseContent ?? Enumerable.Empty<int>())
            {
                allBestStoryResponses.Add(_httpClient.GetAsync(_newsConfiguration.BaseUrl + string.Format(_newsConfiguration.SpecificStoryUrl, item)));
            }

            return await Task.WhenAll(allBestStoryResponses);
        }

        private static async Task<string[]> GetBestStoriesContentFromOutsideAsync(HttpResponseMessage[] allBestStoryResponses, int capacity)
        {
            var allBestStoriesContent = new List<Task<string>>(capacity);

            foreach (var response in allBestStoryResponses)
            {
                allBestStoriesContent.Add(response.Content.ReadAsStringAsync());
            }

            return await Task.WhenAll(allBestStoriesContent);
        }

        private static IEnumerable<Story> ProcessBestStoriesContent(string[] allBestStoriesContent)
        {
            var stories = new List<Story>();
            foreach (var response in allBestStoriesContent)
            {
                stories.Add(JsonSerializer.Deserialize<Story>(response));
            }

            // yield return is slower
            return stories;
        }
    }
}