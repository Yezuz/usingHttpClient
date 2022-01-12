using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
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


      public async Task Run() => 
           // await GetPosterWithStreamAndCompletionMode();
           await PostPosterWithStream();
           // await PostPosterWithStreamAndCompletionOption();


      async Task GetPosterWithStream() {
        var request = new HttpRequestMessage(
            HttpMethod.Get, 
            $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
        request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.SendAsync(request);


        using var stream = await response.Content.ReadAsStreamAsync();
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


      async Task GetPosterWithStreamAndCompletionMode() {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
        request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        using var stream = await response.Content.ReadAsStreamAsync();
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


      async Task PostPosterWithStream() { 
        // generatea movie poster of 500KB
        var random = new Random();
        var generetedBytes = new byte[524288];
        random.NextBytes(generetedBytes);

        var posterForCreation = new PosterForCreation() {
          Name = "A new poster for the Big Lebowski",
          Bytes = generetedBytes
        };

        var memoryContentStream = new MemoryStream();
        // Depreceated: we know use the extension method "SerializeToJsonAndWrite"
        // using var streamWriter = new StreamWriter(memoryContentStream, new UTF8Encoding(), 1024, true);
        // using var jsonTextWriter = new JsonTextWriter(streamWriter);
        //
        // var jsonSerializer = new JsonSerializer();
        // jsonSerializer.Serialize(jsonTextWriter, posterForCreation);
        // jsonTextWriter.Flush();

        memoryContentStream.SerializeToJsonAndWrite(posterForCreation, new UTF8Encoding(), 1024, true);
        memoryContentStream.Seek(0, SeekOrigin.Begin);

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters"
            );
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var streamContent = new StreamContent(memoryContentStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        request.Content = streamContent;

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var createdContent = await response.Content.ReadAsStringAsync();
        var createdPoster = JsonConvert.DeserializeObject<Poster>(createdContent);
        System.Console.WriteLine(createdPoster.Name);
      }


      async Task PostPosterWithStreamAndCompletionOption() { 
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
            $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}"
            );
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        using var stream = await response.Content.ReadAsStreamAsync();
        response.EnsureSuccessStatusCode();
        var poster = stream.ReadAndDeserializeFromJson<Poster>();
        System.Console.WriteLine(poster.Name);
      }
    
  }
}
