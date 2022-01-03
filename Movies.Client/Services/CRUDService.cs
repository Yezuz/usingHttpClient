using System;
using System.Collections.Generic;
using System.Net.Http;
// using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Movies.Client.Models;
using System.Net.Http.Headers;
using System.Xml.Serialization;
using System.IO;

namespace Movies.Client.Services {

    public class CRUDService : IIntegrationService {

      static HttpClient _httpClient = new HttpClient();

      public CRUDService() {
        _httpClient.BaseAddress = new System.Uri("http://localhost:57683");
        _httpClient.Timeout = new TimeSpan(0, 0, 30);
        _httpClient.DefaultRequestHeaders.Clear();
        // _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        // _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
      }


      public async Task Run() => 
        // await GetResource();
        await GetResourceThroughHttpRequestMessage();
        // await CreateResource();
        // await UpdateResource();
        // await DeleteResource();


      public async Task GetResource() {
        var response = await _httpClient.GetAsync("api/movies");
        response.EnsureSuccessStatusCode();
        var movies = new List<Movie>();

        if (response.Content.Headers.ContentType.MediaType == "application/json") {
          var content = await response.Content.ReadAsStringAsync();
          movies = JsonSerializer.Deserialize<List<Movie>>(content, new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
              });
        } 
        else if (response.Content.Headers.ContentType.MediaType == "application/xml") {
          var content = await response.Content.ReadAsStringAsync();
          var serializer = new XmlSerializer(typeof(List<Movie>));
          movies = (List<Movie>)serializer.Deserialize(new StringReader(content));
        }

        LoggingResponsePayload(movies);

      }


      public async Task GetResourceThroughHttpRequestMessage() {
        var request = new HttpRequestMessage(HttpMethod.Get, "api/movies");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var movies = JsonSerializer.Deserialize<IEnumerable<Movie>>(content, new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
              });

        LoggingResponsePayload(movies);

      }


      public async Task CreateResource() {
        var MovieToCreate = new MovieForCreation() {
          Title = "Reservoir Dogs",
          Description = @"After a simple jewelry heist goes terribly wrong, the 
            surviving criminals begin to suspect that one of them is a police informant.",
          DirectorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
          ReleaseDate = new DateTimeOffset(new DateTime(1992, 9, 2)),
          Genre = "Crime, Drama"
        };

        var serializedMovieToCreate = JsonSerializer.Serialize(MovieToCreate);

        var request = new HttpRequestMessage(HttpMethod.Post, "api/movies");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        request.Content = new StringContent(serializedMovieToCreate);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var createdMovie = JsonSerializer.Deserialize<Movie>(content, new JsonSerializerOptions {
              PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        Console.WriteLine(createdMovie.Title);
      }


      public async Task UpdateResource() {
        var movieToUpdate = new MovieForUpdate() {
          Title = "Pulp Fiction",
          Description = "The movie with Zed",
          DirectorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
          ReleaseDate = new DateTimeOffset(new DateTime(1992, 9, 2)),
          Genre = "Crime, Drama"
        };

        var serializedMovieToUpdate = JsonSerializer.Serialize(movieToUpdate);

        var request = new HttpRequestMessage(HttpMethod.Put, "api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = new StringContent(serializedMovieToUpdate);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var updatedMovie = JsonSerializer.Deserialize<Movie>(content, new JsonSerializerOptions {
              PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        Console.WriteLine(updatedMovie.Description);

      }


      public async Task DeleteResource() {
        var request = new HttpRequestMessage(HttpMethod.Delete, "api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine(response.StatusCode);
      }

      
      void LoggingResponsePayload(IEnumerable<Movie> movies) {
        // testing if data is being returned
        foreach(var mov in movies) {
           Console.WriteLine($"Title: {mov.Title}");
           Console.WriteLine($"Description: {mov.Description}");
           Console.WriteLine($"Director: {mov.Director}");
           Console.WriteLine($"Genre: {mov.Genre}");
           Console.WriteLine($"Id: {mov.Id}");
           Console.WriteLine($"Release Date: {mov.ReleaseDate}");
           Console.WriteLine("----------------------");
        }
      }
    }
}
