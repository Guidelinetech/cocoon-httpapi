﻿using Cocoon.HttpAPI;
using System;
using System.Reflection;

namespace Cocoon.HttpAPI.Tests
{
    public class Global : APIHttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterAPI(Assembly.GetExecutingAssembly());
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}