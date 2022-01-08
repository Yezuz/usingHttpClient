using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Movies.Client.Models;
using Newtonsoft.Json;

namespace Movies.Client.Services
{
    public class StreamService : IIntegrationService
    { 
      HttpClient _httpClient = new HttpClient();

      public StreamService() {
         _httpClient.BaseAddress = new Uri("http://localhost:57683");
         _httpClient.Timeout = new TimeSpan(0, 0, 30);
         _httpClient.DefaultRequestHeaders.Clear();
      }


      public async Task Run() => await GetPosterWithStreamAndCompletionMode();


      async Task GetPosterWithStream() {
        var request = new HttpRequestMessage(
            HttpMethod.Get, 
            $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
        request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.SendAsync(request);


        using (var stream = await response.Content.ReadAsStreamAsync()) {
          response.EnsureSuccessStatusCode();
          var poster = stream.ReadAndDeserializeFromJson<Poster>();
          System.Console.WriteLine(poster.Name);
          // using (var streamReader = new StreamReader(stream)) {
          //   using (var jsonTextReader = new JsonTextReader(streamReader)) {
          //     var jsonSerializer = new JsonSerializer();
          //     var poster = jsonSerializer.Deserialize<Poster>(jsonTextReader);
              // do something with the poster
            // }
          // }
        }
      }


      async Task GetPosterWithStreamAndCompletionMode() {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
        request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        using (var stream = await response.Content.ReadAsStreamAsync()) {
          response.EnsureSuccessStatusCode();
          var poster = stream.ReadAndDeserializeFromJson<Poster>();
          System.Console.WriteLine(poster.Name);
          // using (var streamReader = new StreamReader(stream)) {
          //   using (var jsonTextReader = new JsonTextReader(streamReader)) {
          //     var jsonSerializer = new JsonSerializer();
          //     var poster = jsonSerializer.Deserialize<Poster>(jsonTextReader);
          //     // do something with the poster
          //   }
          // }
        }
      }


      async Task PostPosterWithStream() {
        // generatea movie poster of 500KB
        var random = new Random();
        var generetedBytes = new byte[524288];
        random.NextBytes(generetedBytes);

        var posterForCreation = new PosterForCreation() {
          Name = "A new poster for the Big Lebowski",
          Bytes = generetedBytes
        };

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"api/movies/"
            );
      }


    
}
