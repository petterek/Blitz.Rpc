
using Blitz.Rpc.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace UsingServer
{
    public class MySerializer : ISerializer
    {

        JsonSerializer serializer;
        public MySerializer()
        {
            serializer = new JsonSerializer();
        }
        public List<string> AcceptMimeType { get; } = new List<string> { "application/json", "text/json" };

        public string ProduceMimeType { get; set; } = "application/json";

        public object FromStream(Stream stream, Type returnType)
        {
            return serializer.Deserialize(new StreamReader(stream), returnType);
        }

        public object[] FromStream(Stream stream, Type[] returnType)
        {
            var data = JToken.Load(new JsonTextReader(new StreamReader(stream)));
            if(!(data is JArray))
            {
                throw new Exception();
            }
            var list = new List<object>();
            int i = 0;
            foreach(var item in data as JArray)
            {
                list.Add(item.ToObject(returnType[i++]));
            }
            return list.ToArray();
        }


        public void ToStream(Stream outstream, object v)
        {
            StreamWriter textWriter = new StreamWriter(outstream);
            serializer.Serialize(textWriter, v);
            textWriter.Flush();
        }
    }


}
