using System.Buffers;
using Confluent.Kafka;
using Microsoft.IO;
using DummyProtoc;
using MessageExtensions = Google.Protobuf.MessageExtensions;
using SerializationContext = Confluent.Kafka.SerializationContext;

namespace Common;

public class DummySerializer : ISerializer<Dummy>, IDeserializer<Dummy>
{
    private static readonly RecyclableMemoryStreamManager StreamManager = new ();
 
    public byte[] Serialize(Dummy data, SerializationContext context)
    {
        var ms = StreamManager.GetStream() as RecyclableMemoryStream;
        MessageExtensions.WriteTo(data, ms as Stream);
        
        if (ms != null)
        {
            var converted = ms.GetReadOnlySequence();
            var ret = new byte[converted.Length];
            converted.CopyTo(ret);
            return ret;
        }

        throw new Exception("Failed to get stream from RecyclableMemoryStreamManager");
    }
    public Dummy Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull) return new Dummy();
        try
        {
            var ret = Dummy.Parser.ParseFrom(data);
            return ret ?? new Dummy();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception while deserializing protobuf value {e.Message}");
        }
        return new Dummy();
    }
}