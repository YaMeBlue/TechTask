using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using TechTask.Configurations;
using TechTask.Handlers;
using TechTask.Models;

#if DEBUG
using TechTask.Constants;
#endif

namespace TechTask.Controllers.HackerNews
{
    /// <summary>
    /// HackerNews Api REST Controller
    /// </summary>
    [Route("api/hackerNews")]
    public partial class HackerNewsApiController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;
        private readonly HackerNewsConfiguration _newsConfiguration;

        public HackerNewsApiController(IConfiguration configuration, IMemoryCache cache)
        {
            _cache = cache;
            _newsConfiguration = configuration.GetSection("HackerNews").Get<HackerNewsConfiguration>();
            _httpClient = new HttpClient(new ThrottleHandler(new HttpClientHandler(), _newsConfiguration));
        }

        /// <summary>
        /// Obtains best stories from HackerNews API
        /// </summary>
        /// <param name="number">Number of items to retrive</param>
        /// <returns>Top # best stories ordered by score in descending order</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// GET /api/hackerNews/best?number=5
        ///     
        /// </remarks>
        [HttpGet("best")]
        [Produces("application/json")]
        public async Task<ActionResult> GetBestStories(int number)
        {
            IEnumerable<StoryDto> result;
#if DEBUG
            if(_cache.TryGetValue(CacheConstants.HACKER_NEWS_BEST_STORIES_KEY, out result))
            {
                _httpClient.Dispose();
                return Ok(result.Take(number));
            }
#endif
            var bestStoriesResponseContent = await GetBestStoriesFromOutside();
            var capacity = bestStoriesResponseContent?.Length ?? 0;

            if (capacity > 0)
            {
                var allBestStoryResponses = await ProcessBestStoriesDataAsync(bestStoriesResponseContent, capacity);
                var allBestStoriesContent = await GetBestStoriesContentFromOutsideAsync(allBestStoryResponses, capacity);
                var bestStoriesData = ProcessBestStoriesContent(allBestStoriesContent);

                result = bestStoriesData
                   .OrderByDescending(s => s.score)
                   .Select(s => new StoryDto
                   {
                       Title = s.title,
                       Uri = s.url,
                       PostedBy = s.by,
                       Time = s.time,
                       Score = s.score,
                       CommentCount = s.descendants
                   });
#if DEBUG
                _cache.Set(CacheConstants.HACKER_NEWS_BEST_STORIES_KEY, result, DateTimeOffset.UtcNow.AddDays(7));
#endif
                _httpClient.Dispose();
                return Ok(result.Take(number));
            }
            else
            {
                _httpClient.Dispose();
                return NoContent();
            }
        }
    }
}