using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Cocoon.HttpAPI
{
    internal class Utilities
    {

        public static object ChangeType(object value, Type conversionType)
        {

            if (value == null)
                return null;

            if (value.GetType() == conversionType)
                return value;

            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                conversionType = Nullable.GetUnderlyingType(conversionType);
            
            return TypeDescriptor.GetConverter(conversionType).ConvertFrom(value);// Convert.ChangeType(value, conversionType);

        }

        public static object MapCollectionToObject(NameValueCollection collection, Type type)
        {
            object obj = Activator.CreateInstance(type);
            PropertyInfo[] props = type.GetProperties();
           
            foreach (PropertyInfo prop in props)
            {

                object value = ChangeType(collection[prop.Name], prop.PropertyType);
                prop.SetValue(obj, value, null);

            }

            return obj;

        }

        public static object DeserializeJSON(string data, Type type)
        {

            JObject j = (JObject)JsonConvert.DeserializeObject(data);
            return j.ToObject(type);

        }

        public static object DeserializeXML(string data, Type type)
        {

            XmlSerializer x = new XmlSerializer(type);

            using (TextReader reader = new StringReader(data))
                return x.Deserialize(reader);

        }

        public static string SerializeXML(object obj, Type type)
        {

            XmlSerializer x = new XmlSerializer(type);

            using (StringWriter writer = new StringWriter())
            {
                x.Serialize(writer, obj);
                return writer.ToString();
            }

        }
        
    }
}
