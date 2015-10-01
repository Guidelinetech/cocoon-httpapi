using System;

namespace Cocoon.HttpAPI.Attributes
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EndPoint : Attribute
    {

        internal string baseRouteOverride;

        public EndPoint(string BaseRouteOverride = null)
        {
            baseRouteOverride = BaseRouteOverride;
        }

    }
    
    [AttributeUsage(AttributeTargets.Parameter)]
    public class QueryParam : Attribute
    {

        internal string overrideParamName;
        internal bool isModel;

        public QueryParam(string OverrideParamName = null, bool IsModel = false)
        {

            overrideParamName = OverrideParamName;
            isModel = IsModel;

        }

    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class PayloadParam : Attribute
    {

        internal string overrideParamName;
  
        public PayloadParam(string OverrideParamName = null)
        {

            overrideParamName = OverrideParamName;

        }

    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class HeaderParam : Attribute
    {

        internal string overrideParamName;
        internal bool isModel;

        public HeaderParam(string OverrideParamName = null, bool IsModel = false)
        {

            overrideParamName = OverrideParamName;
            isModel = IsModel;

        }

    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class FormParam : Attribute
    {

        internal string overrideParamName;
        internal bool isModel;

        public FormParam(string OverrideParamName = null, bool IsModel = false)
        {

            overrideParamName = OverrideParamName;
            isModel = IsModel;

        }

    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class FileParam : Attribute
    {

        internal string overrideParamName;

        public FileParam(string OverrideParamName = null)
        {

            overrideParamName = OverrideParamName;

        }

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class EndPointMethod : Attribute
    {

        internal string routeOverride;
        internal SerializationFormat responseFormat;

        public EndPointMethod(
            string RouteOverride = null,
            SerializationFormat ResponseFormat = SerializationFormat.TextPlain
        )
        {
            routeOverride = RouteOverride;
            responseFormat = ResponseFormat;
        }
    }

    public enum SerializationFormat
    {
        None,
        JSON,
        XML,
        TextPlain
    }
    
}