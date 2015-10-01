using Cocoon.HttpAPI.Attributes;
using System;
using System.IO;
using System.Web;

namespace Cocoon.HttpAPI.Tests
{

    [EndPoint]
    public class TestAPIHandler : APIHandler
    {

        public class TestModel
        {

            public string a { get; set; }
            public string b { get; set; }
            public int c { get; set; }
            public int d { get; set; }
            public int? e { get; set; }
            public Guid f { get; set; }
            public DateTime g { get; set; }

        }

        [EndPointMethod(ResponseFormat: SerializationFormat.JSON)]
        public object test1(
            [QueryParam]string a,
            [QueryParam]string b,
            [QueryParam]int c,
            [QueryParam]int d,
            [QueryParam]int? e,
            [QueryParam]Guid f,
            [QueryParam]DateTime g)
        {

            return new { Concat = a + b, Added = c + d + e, Guid = f, Date = g };

        }

        [EndPointMethod(ResponseFormat: SerializationFormat.JSON)]
        public TestModel test2([QueryParam(IsModel: true)]TestModel model)
        {

            model.a = "changed a";
            model.b = "changed b";
            model.c *= 2;
            model.d *= 2;
            model.e *= 2;
            model.f = Guid.NewGuid();
            model.g = model.g.AddYears(10);

            return model;

        }

        [EndPointMethod(ResponseFormat: SerializationFormat.JSON)]
        public TestModel test3([PayloadParam]TestModel model)
        {

            model.a = "changed a";
            model.b = "changed b";
            model.c *= 2;
            model.d *= 2;

            return model;

        }

        [EndPointMethod(ResponseFormat: SerializationFormat.None)]
        public void test4([FileParam]HttpPostedFile file)
        {

            file.SaveAs(httpContext.Server.MapPath("~/" + file.FileName));

        }

    }
}
