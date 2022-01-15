using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Movies.Client.Models;
using System.Net.Http.Headers;
using System.Threading;

namespace Movies.Client.Services
{
    public class CancellationService : IIntegrationService
    {

      static HttpClient _httpClient = new HttpClient(new HttpClientHandler() {
              AutomaticDecompression = System.Net.DecompressionMethods.GZip })
              {
                BaseAddress = new Uri("http://localhost:57683"),
                Timeout = new TimeSpan(0, 0, 2)
              };

      static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();


      public CancellationService() => _httpClient.DefaultRequestHeaders.Clear();


      public async Task Run() { 
        // _cancellationTokenSource.CancelAfter(1000);
        await GetTrailerAndCancel(_cancellationTokenSource);
      }


      async Task GetTrailerAndCancel(CancellationTokenSource cancellationTokenSource) {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/trailers/{Guid.NewGuid()}");

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

        try {
          var response = await _httpClient.SendAsync(
            request, 
            HttpCompletionOption.ResponseContentRead,
            cancellationTokenSource.Token
            );

          using var stream = await response.Content.ReadAsStreamAsync();
          response.EnsureSuccessStatusCode();
        
          var trailer = stream.ReadAndDeserializeFromJson<Trailer>();

          System.Console.WriteLine($"{trailer.Description}");
        }
        catch (OperationCanceledException ocException) {
            System.Console.WriteLine($"An operation was cancelled with message: {ocException.Message}");
        }
                
      }


      async Task GetTrailerAndHandleTimeOut() {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/trailers/{Guid.NewGuid()}");

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

        try {
          var response = await _httpClient.SendAsync(
            request, 
            HttpCompletionOption.ResponseContentRead
            );

          using var stream = await response.Content.ReadAsStreamAsync();
          response.EnsureSuccessStatusCode();
        
          var trailer = stream.ReadAndDeserializeFromJson<Trailer>();

          System.Console.WriteLine($"{trailer.Description}");
        }
        catch (OperationCanceledException ocException) {
            System.Console.WriteLine($"An operation was cancelled with message: {ocException.Message}");
        }
                
      }
    }
}
