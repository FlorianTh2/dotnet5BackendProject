﻿using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace dotnet5BackendProject.IntegrationTests.Helpers
{
    public static class HttpClientExtensions
    {
        public static bool ExampleExtension(this HttpClient client, string exampleExtensionParameter)
        {
            return true;
        }
    }
}