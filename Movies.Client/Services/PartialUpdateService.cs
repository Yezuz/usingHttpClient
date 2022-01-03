﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Movies.Client.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Movies.Client.Services
{
    public class PartialUpdateService : IIntegrationService
    { 
      static HttpClient _httpClient = new HttpClient();

      public PartialUpdateService() {
         _httpClient.BaseAddress = new System.Uri("http://localhost:57683") ;
         _httpClient.Timeout = new TimeSpan(0, 0, 30);
         _httpClient.DefaultRequestHeaders.Clear();
      }


      public async Task PatchResource() {
        var patchDoc = new JsonPatchDocument<MovieForUpdate>();
        patchDoc.Replace(m => m.Title, "Updated Title");
        patchDoc.Remove(m => m.Description);

        var serializedChangedSet = JsonConvert.SerializeObject(patchDoc);
        var request = new HttpRequestMessage(HttpMethod.Patch, "api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = new StringContent(serializedChangedSet);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("applicatin/json-patch+json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var patchedMovie = JsonConvert.DeserializeObject<Movie>(content);

           Console.WriteLine($"Title: {patchedMovie.Title}");
           Console.WriteLine($"Description: {patchedMovie.Description}");
           Console.WriteLine($"Director: {patchedMovie.Director}");
           Console.WriteLine($"Genre: {patchedMovie.Genre}");
           Console.WriteLine($"Id: {patchedMovie.Id}");
           Console.WriteLine($"Release Date: {patchedMovie.ReleaseDate}");
      }


      public async Task PatchResourceShorcut() {
        var patchDoc = new JsonPatchDocument<MovieForUpdate>();
        patchDoc.Replace(m => m.Title, "Perros de Reserva");
        patchDoc.Remove(m => m.ReleaseDate);

        var response = await _httpClient.PatchAsync(
            "api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b",
            new StringContent(
              JsonConvert.SerializeObject(patchDoc), 
              Encoding.UTF8, 
              "application/json-patch+json")
            );
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var patchedMovie = JsonConvert.DeserializeObject<Movie>(content);

           Console.WriteLine($"Title: {patchedMovie.Title}");
           Console.WriteLine($"Description: {patchedMovie.Description}");
           Console.WriteLine($"Director: {patchedMovie.Director}");
           Console.WriteLine($"Genre: {patchedMovie.Genre}");
           Console.WriteLine($"Id: {patchedMovie.Id}");
           Console.WriteLine($"Release Date: {patchedMovie.ReleaseDate}");
      }


      public async Task Run() {
        // await PatchResource();
        await PatchResourceShorcut();
      }         


    }
}