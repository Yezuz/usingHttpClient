using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Movies.Client {

  public static class StreamExtensions {

    public static T ReadAndDeserializeFromJson<T>(this Stream stream)
    {
       if (stream == null) throw new ArgumentNullException(nameof(stream));
       if (!stream.CanRead) throw new NotSupportedException("Can't read from this stream");

       using var streamReader = new StreamReader(stream);
       using var jsonTextReader = new JsonTextReader(streamReader);
       var jsonSerializer = new JsonSerializer();
       return jsonSerializer.Deserialize<T>(jsonTextReader);
    }

    public static void SerializeToJsonAndWrite<T>(this Stream stream, T objectToWrite, UTF8Encoding uTF8Encoding, int bufferSize, bool keepOpen) {
      if (stream is null) throw new ArgumentNullException();
      if (!stream.CanWrite) throw new NotSupportedException();

      using var streamWriter = new StreamWriter(stream, new UTF8Encoding(), 8192, true);
      using var jsonTextWriter = new JsonTextWriter(streamWriter);
      var jsonSerializer = new JsonSerializer();
      jsonSerializer.Serialize(jsonTextWriter, objectToWrite);
      jsonTextWriter.Flush();
    }

  }
    
}
