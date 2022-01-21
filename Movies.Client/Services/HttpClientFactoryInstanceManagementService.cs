using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Movies.Client.Models;

namespace Movies.Client.Services
{
    public class HttpClientFactoryInstanceManagementService : IIntegrationService
    {

      private static CancellationTokenSource cancellationToken = new CancellationTokenSource();
      private readonly IHttpClientFactory httpClientFactory;
      private readonly MoviesClient moviesClient;

      public HttpClientFactoryInstanceManagementService(IHttpClientFactory httpClientFactory, MoviesClient moviesClient)
      {
          this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
          this.moviesClient = moviesClient ?? throw new ArgumentNullException(nameof(moviesClient));
      }


      public async Task Run() => await GetMoviesWithTypedHttpClientFromFactory(cancellationToken.Token);


      private async Task TestDisposeHttpClient(CancellationToken cancellationToken) {
        for (int i = 0; i < 10; i++) {
          using var httpClient = new HttpClient();
          var request = new HttpRequestMessage(HttpMethod.Get, "https://www.google.com");

          using var response = await httpClient.SendAsync(
              request, 
              HttpCompletionOption.ResponseHeadersRead,
              cancellationToken);

          var stream = await response.Content.ReadAsStreamAsync();
          response.EnsureSuccessStatusCode();

          Console.WriteLine($"{i} - Request completed with status code: {response.StatusCode}");
        }
        System.Console.WriteLine("Request completed");
      }


      private async Task GetMoviesWithHttpClientFromFactory(CancellationToken cancellationToken) {
        var httpClient = httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:57683/api/movies");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        using var stream = await response.Content.ReadAsStreamAsync();
        response.EnsureSuccessStatusCode();
        var movies = stream.ReadAndDeserializeFromJson<List<Movie>>();
        foreach (var item in movies) { System.Console.WriteLine(item.Title); }
      }


      private async Task GetMoviesWithNamedHttpClientFromFactory(CancellationToken cancellationToken) {
        var httpClient = httpClientFactory.CreateClient("MovieClient");
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/movies");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        using var stream = await response.Content.ReadAsStreamAsync();
        response.EnsureSuccessStatusCode();
        var movies = stream.ReadAndDeserializeFromJson<List<Movie>>();
        System.Console.WriteLine($"Called from {nameof(HttpClientFactoryInstanceManagementService.GetMoviesWithNamedHttpClientFromFactory)}");
        foreach (var item in movies) { System.Console.WriteLine(item.Title); }
      }


      private async Task GetMoviesWithTypedHttpClientFromFactory(CancellationToken cancellationToken) {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/movies");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await moviesClient.Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        using var stream = await response.Content.ReadAsStreamAsync();
        response.EnsureSuccessStatusCode();
        var movies = stream.ReadAndDeserializeFromJson<List<Movie>>();
        System.Console.WriteLine($"Called from {nameof(HttpClientFactoryInstanceManagementService.GetMoviesWithTypedHttpClientFromFactory)}");
        foreach (var item in movies) { System.Console.WriteLine(item.Title); }
      }
    }
}
