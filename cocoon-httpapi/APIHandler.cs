using Cocoon.HttpAPI.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;

namespace Cocoon.HttpAPI
{
    public class APIHandler : IHttpHandler
    {

        internal APIHttpApplication app;

        public bool IsReusable { get { return false; } }
        
        public virtual void ProcessRequest(HttpContext context)
        {

            //determine if this route is mapped to an endpoint
            if (!APIHttpApplication.endPointMethods.ContainsKey(context.Request.Path.ToLower()))
                return;
            
            //get method into
            MethodInfo method = APIHttpApplication.endPointMethods[context.Request.Path.ToLower()];
            EndPointMethod methodAttr = method.GetCustomAttribute<EndPointMethod>();
            ParameterInfo[] args = method.GetParameters();

            //list of parameters we will pass to the endpoint method
            List<object> parameters = new List<object>();

            //loop through each argument of the endpoint method to find matches
            foreach (ParameterInfo arg in args)
            {

                //find each type of custom attribute
                QueryParam queryParam = arg.GetCustomAttribute<QueryParam>();
                PayloadParam payloadParam = arg.GetCustomAttribute<PayloadParam>();
                HeaderParam headerParam = arg.GetCustomAttribute<HeaderParam>();
                FormParam formParam = arg.GetCustomAttribute<FormParam>();
                FileParam fileParam = arg.GetCustomAttribute<FileParam>();

                //try
                //{

                    if(arg.ParameterType == typeof(HttpContext))
                    {
                        parameters.Add(context);
                        continue;
                    }

                    //query param
                    if (queryParam != null)
                    {

                        string argName = arg.Name;
                        if (!string.IsNullOrEmpty(queryParam.overrideParamName))
                            argName = queryParam.overrideParamName;

                        if (queryParam.isModel)
                            parameters.Add(Utilities.MapCollectionToObject(context.Request.QueryString, arg.ParameterType));
                        else
                        {

                            var v = context.Request.QueryString[argName];
                            parameters.Add(Utilities.ChangeType(v, arg.ParameterType));

                        }

                        continue;

                    }

                    //is a request with a payload to deserialize
                    if (payloadParam != null && context.Request.InputStream.Length > 0)
                    {

                        string contentType = context.Request.ContentType.ToLower();
                        bool compressed = contentType.EndsWith("-compressed");

                        if (contentType.StartsWith("text/") || payloadParam.dataFormat == SerializationFormat.Text)
                            parameters.Add(Utilities.ChangeType(getStringPayload(context, compressed), arg.ParameterType));
                        else if (contentType == "application/json" || payloadParam.dataFormat == SerializationFormat.JSON)
                            parameters.Add(Utilities.DeserializeJSON(getStringPayload(context, compressed), arg.ParameterType));
                        else if (contentType == "application/xml" || payloadParam.dataFormat == SerializationFormat.XML)
                            parameters.Add(Utilities.DeserializeXML(getStringPayload(context, compressed), arg.ParameterType));
                        else
                        {

                            if (arg.ParameterType == typeof(string))
                                parameters.Add(Utilities.ChangeType(getStringPayload(context, compressed), arg.ParameterType));
                            else if (arg.ParameterType == typeof(byte[]))
                                using (var reader = new BinaryReader(context.Request.InputStream))
                                    parameters.Add(reader.ReadBytes(context.Request.ContentLength));
                            else if (arg.ParameterType == typeof(Stream))
                                parameters.Add(context.Request.InputStream);

                        }

                        continue;

                    }

                    //header param
                    if (headerParam != null)
                    {

                        string argName = arg.Name;
                        if (!string.IsNullOrEmpty(headerParam.overrideParamName))
                            argName = headerParam.overrideParamName;

                        if (headerParam.isModel)
                            parameters.Add(Utilities.MapCollectionToObject(context.Request.Headers, arg.ParameterType));
                        else
                        {

                            var v = context.Request.Headers[argName];
                            parameters.Add(Utilities.ChangeType(v, arg.ParameterType));

                        }

                        continue;

                    }

                    //form param
                    if (formParam != null)
                    {

                        string argName = arg.Name;
                        if (!string.IsNullOrEmpty(formParam.overrideParamName))
                            argName = formParam.overrideParamName;

                        if (formParam.isModel)
                            parameters.Add(Utilities.MapCollectionToObject(context.Request.Form, arg.ParameterType));
                        else
                        {

                            var v = context.Request.Form[argName];
                            parameters.Add(Utilities.ChangeType(v, arg.ParameterType));

                        }

                        continue;

                    }

                    //file param
                    if (fileParam != null)
                    {

                        if (arg.ParameterType != typeof(HttpPostedFile))
                            throw new ArgumentException("FileParam argument must be of type HttpPostedFile.", arg.Name);

                        string argName = arg.Name;
                        if (!string.IsNullOrEmpty(fileParam.overrideParamName))
                            argName = fileParam.overrideParamName;

                        parameters.Add(context.Request.Files[argName]);

                        continue;

                    }

                    //if we get here the arg doesn't have an attribute
                    if (arg.ParameterType.IsValueType)
                        parameters.Add(Activator.CreateInstance(arg.ParameterType));
                    else
                        parameters.Add(null);


                //}
                //catch (Exception ex)
                //{

                //    //add default parameter if there's an error
                //    if (arg.ParameterType.IsValueType)
                //        parameters.Add(Activator.CreateInstance(arg.ParameterType));
                //    else
                //        parameters.Add(null);

                //    throw ex;

                //}

            }

            //execute method
            object response = method.Invoke(this, parameters.ToArray());

            //if null is returned or method is void
            if (response == null)
                return;

            //serialize response
            switch (methodAttr.responseFormat)
            {

                case SerializationFormat.Text:

                    string txt = (string)response;

                    if (methodAttr.compressResponse)
                        SetResponse(context, app.CompressString(txt), "text/plain-compressed");
                    else
                        SetResponse(context, txt, "text/plain");

                    break;

                case SerializationFormat.JSON:

                    string json = JsonConvert.SerializeObject(response);
                    if (methodAttr.compressResponse)
                        SetResponse(context, app.CompressString(json), "application/json-compressed");
                    else
                        SetResponse(context, json, "application/json");

                    break;

                case SerializationFormat.XML:

                    string xml = Utilities.SerializeXML(response, method.ReturnType);
                    if (methodAttr.compressResponse)
                        SetResponse(context, app.CompressString(xml), "application/xml-compressed");
                    else
                        SetResponse(context, xml, "application/xml");
                    
                    break;
                    
            }
        }

        private void SetResponse(HttpContext context, object Response, string ContentType)
        {

            context.Response.ContentType = ContentType;
            context.Response.Write(Response);

        }

        private string getStringPayload(HttpContext context, bool compressed)
        {

            using (var reader = new StreamReader(context.Request.InputStream))
                return compressed ? app.DecompressString(reader.ReadToEnd().Trim()) : reader.ReadToEnd().Trim();

        }

    }
}