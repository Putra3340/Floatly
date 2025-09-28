using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Floatly.Models.ApiModel;
namespace Floatly.Api
{
    public static class Api
    {
        private static readonly string _serverurl = Prefs.ServerUrl;
        public static HttpClient client = new HttpClient();
        public async static Task Play(int songid)
        {
            if(Prefs.LoginToken == "ANONYMOUS_USER")
                return;
            var request = new HttpRequestMessage(HttpMethod.Post, _serverurl + "/api/play");
            var collection = new List<KeyValuePair<string, string>>();
            collection.Add(new("token", Prefs.LoginToken));
            collection.Add(new("songId", songid.ToString()));
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            var response = await client.SendAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                string a = (await response.Content.ReadAsStringAsync());
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }
    }
}
