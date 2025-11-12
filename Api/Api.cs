using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
namespace Floatly.Api
{
    public static class Api
    {
        private static readonly string _serverurl = Prefs.ServerUrl;
        public static HttpClient client = new HttpClient();
        
    }
}
