using Cocoon.HttpAPI.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Routing;

namespace Cocoon.HttpAPI
{
    public class APIHttpApplication : HttpApplication
    {

        internal static bool registered = false;
        internal static Dictionary<string, MethodInfo> endPointMethods = new Dictionary<string, MethodInfo>();

        public static List<string> SpecialPrefixList = new List<string>() { "handler", "controller" };

        protected void RegisterAPI(string apiBaseRoute = null)
        {
            RegisterAPI(Assembly.GetCallingAssembly(), apiBaseRoute);
        }

        protected void RegisterAPI(Assembly websiteAssembly, string apiBaseRoute = null)
        {

            if (registered)
                return;
            
            //find rest endpoint classes in assembly
            foreach (Type type in websiteAssembly.GetTypes())
            {

                EndPoint attribute = type.GetCustomAttribute<EndPoint>(true);
                if (attribute != null && type.BaseType == typeof(APIHandler))
                {

                    //get base route
                    string baseRoute = type.Name.ToLower();
                    
                    //remove special names off of the end
                    foreach(string prefix in SpecialPrefixList)
                        if(baseRoute.EndsWith(prefix.ToLower()))
                            baseRoute = baseRoute.Substring(0, baseRoute.Length - prefix.Length);
                    
                    //override route name
                    if (!string.IsNullOrEmpty(attribute.baseRouteOverride))
                        baseRoute = attribute.baseRouteOverride;

                    //create the handler
                    APIHandler httpHandler = (APIHandler)Activator.CreateInstance(type);
                    httpHandler.app = this;

                    APIRouteHandler routeHandler = new APIRouteHandler(httpHandler);

                    //find endpoint methods
                    MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                    foreach (MethodInfo method in methods)
                    {

                        EndPointMethod restMethod = method.GetCustomAttribute<EndPointMethod>(true);
                        if (restMethod != null)
                        {
                            
                            //get second part of route
                            string route = string.Format("{0}/{1}", baseRoute, method.Name);
                            if (!string.IsNullOrEmpty(restMethod.routeOverride))
                                route = restMethod.routeOverride;
                            route = route.ToLower();

                            if (!string.IsNullOrEmpty(apiBaseRoute))
                                route = string.Format("{0}/{1}", apiBaseRoute, route);

                            //add to route table
                            RouteTable.Routes.Add(new Route(route, routeHandler));

                            //add to our list of route methods
                            endPointMethods.Add("/" + route, method);

                        }

                    }

                }
            }

            registered = true;

        }

        public virtual string CompressString(string str)
        {

            throw new NotImplementedException();

        }

        public virtual string DecompressString(string base64Str)
        {

            throw new NotImplementedException();

        }

    }
}
