using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cocoon.HttpAPI
{
    
    public abstract class MimeTypeHandler
    {

        public APIHttpApplication httpApplication;
        public abstract string MimeType { get; }
        public abstract byte[] Serialize(object response, Type type);
        public abstract object Deserialize(Stream inputStream, Type type);

    }

    public class JsonMimeTypeHandler : MimeTypeHandler
    {

        public override string MimeType { get { return "application/json"; } }

        public override object Deserialize(Stream inputStream, Type type)
        {
            using (var reader = new StreamReader(inputStream))
            {
                JObject j = (JObject)JsonConvert.DeserializeObject(reader.ReadToEnd());
                return j.ToObject(type);
            }
        }

        public override byte[] Serialize(object response, Type type)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));
        }

    }

    public class JsonCompressedMimeTypeHandler : JsonMimeTypeHandler
    {

        public override string MimeType { get { return "application/json-compressed"; } }

        public override object Deserialize(Stream inputStream, Type type)
        {
            string json = (string)base.Deserialize(inputStream, type);
            return httpApplication.DecompressString(json);
        }

        public override byte[] Serialize(object response, Type type)
        {

            string json = Encoding.UTF8.GetString(base.Serialize(response, type));
            string json_compressed = httpApplication.CompressString(json);
            return Encoding.UTF8.GetBytes(json_compressed);

        }

    }

    public class XmlMimeTypeHandler : MimeTypeHandler
    {

        public override string MimeType { get { return "application/xml"; } }

        public override object Deserialize(Stream inputStream, Type type)
        {
            XmlSerializer x = new XmlSerializer(type);
            return x.Deserialize(inputStream);
        }

        public override byte[] Serialize(object response, Type type)
        {
            XmlSerializer x = new XmlSerializer(type);
            using (StringWriter writer = new StringWriter())
            {
                x.Serialize(writer, response);
                return Encoding.UTF8.GetBytes(writer.ToString());
            }
        }

    }

    public class XmlCompressedMimeTypeHandler : XmlMimeTypeHandler
    {

        public override string MimeType { get { return "application/xml-compressed"; } }

        public override object Deserialize(Stream inputStream, Type type)
        {
            string xml = (string)base.Deserialize(inputStream, type);
            return httpApplication.DecompressString(xml);
        }

        public override byte[] Serialize(object response, Type type)
        {
            string xml = Encoding.UTF8.GetString(base.Serialize(response, type));
            string xml_compressed = httpApplication.CompressString(xml);
            return Encoding.UTF8.GetBytes(xml_compressed);
        }

    }

    public class TextPlainMimeTypeHandler : MimeTypeHandler
    {

        public override string MimeType { get { return "text/plain"; } }

        public override object Deserialize(Stream inputStream, Type type)
        {
            using (var reader = new StreamReader(inputStream))
                return reader.ReadToEnd();
        }

        public override byte[] Serialize(object response, Type type)
        {

            return Encoding.UTF8.GetBytes(response.ToString());

        }

    }

    public class TextPlainCompressedMimeTypeHandler : MimeTypeHandler
    {

        public override string MimeType { get { return "text/plain-compressed"; } }

        public override object Deserialize(Stream inputStream, Type type)
        {
            using (var reader = new StreamReader(inputStream))
                return httpApplication.DecompressString(reader.ReadToEnd());
        }

        public override byte[] Serialize(object response, Type type)
        {

            return Encoding.UTF8.GetBytes(httpApplication.CompressString(response.ToString()));

        }

    }

}
