using Floatly.Models.ApiModel;
using Floatly.Utils;
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
        private static readonly string _serverurl = Prefs.ServerUrl;
        public static HttpClient client = new HttpClient();

        public async static Task Login(string email, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _serverurl + "/auth/desktop/login");
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
            Prefs.LoginUsername = email;
            MessageBox.Show("Login successful");
            UserData.SaveLoginData(await response.Content.ReadAsStringAsync());
        }
        public async static Task Register(string email, string password, string username)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _serverurl + "/auth/desktop/register");
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
            Prefs.LoginUsername = username;
            MessageBox.Show("Registration successful");
        }
        public async static Task<bool> AutoLogin(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _serverurl + "/auth/desktop/autologin");
            var collection = new List<KeyValuePair<string, string>>();
            collection.Add(new("token", token));
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            var response = await client.SendAsync(request);
            if(response.StatusCode != HttpStatusCode.OK)
                return false;
            Prefs.LoginToken = token;
            return true;
        }
        public async static Task VerifyEmail(string email) // Maybe Done?
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _serverurl + "/auth/desktop/verify-email");
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
