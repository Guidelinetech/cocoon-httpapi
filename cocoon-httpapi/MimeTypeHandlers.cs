using System;
using System.IO;
using System.Text;

namespace Cocoon.HttpAPI
{

    public abstract class MimeTypeHandler
    {

        public APIHttpApplication httpApplication;
        public abstract string MimeType { get; }
        public abstract byte[] Serialize(object response, Type type);
        public abstract object Deserialize(Stream inputStream, Type type);

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
