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
    public static class ApiAuth
    {
        public static HttpClient client = new HttpClient();

        public async static Task Login(string email, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://floatly.putrartx.my.id/auth/desktop/login");
            var collection = new List<KeyValuePair<string, string>>();
            collection.Add(new("username", email));
            collection.Add(new("password", password));
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            var response = await client.SendAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            string token = doc.RootElement.GetProperty("token").GetString();
            Prefs.LoginToken = token;
            MessageBox.Show("Login successful");
        }
        public async static Task Register(string email, string password, string username)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://floatly.putrartx.my.id/auth/desktop/register");
            var collection = new List<KeyValuePair<string, string>>();
            collection.Add(new("email", email));
            collection.Add(new("password", password));
            collection.Add(new("username", username));
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            var response = await client.SendAsync(request);
            if(response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            string token = doc.RootElement.GetProperty("token").GetString();
            Prefs.LoginToken = token;
            MessageBox.Show("Registration successful");
        }
        public async static Task AutoLogin(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://floatly.putrartx.my.id/auth/desktop/autologin");
            var collection = new List<KeyValuePair<string, string>>();
            collection.Add(new("email", token));
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

        }
        public async static Task VerifyEmail(string email) // Maybe Done?
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://floatly.putrartx.my.id/auth/desktop/verify-email");
            var collection = new List<KeyValuePair<string, string>>();
            collection.Add(new("email", email));
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            var response = await client.SendAsync(request);
            if(response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
            MessageBox.Show("Verification request sent.");
        }

    }
}
