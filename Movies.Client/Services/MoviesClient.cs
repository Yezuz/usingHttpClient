using System;
using System.Net.Http;

namespace Movies.Client.Services
{
    public class MoviesClient {

      public MoviesClient(HttpClient client) { 
        Client = client;
        Client.BaseAddress = new Uri("http://localhost:57683");
        Client.Timeout = new TimeSpan(0, 0, 2);
        Client.DefaultRequestHeaders.Clear();
      }

      public HttpClient Client { get; }
    }
}
