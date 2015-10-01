using System.Web;
using System.Web.Routing;

namespace Cocoon.HttpAPI
{

    internal class APIRouteHandler : IRouteHandler
    {
        public IHttpHandler HttpHandler
        {
            get;
            private set;
        }

        public APIRouteHandler(IHttpHandler httpHandler)
        {
            HttpHandler = httpHandler;
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return HttpHandler;
        }
    }
    
}
