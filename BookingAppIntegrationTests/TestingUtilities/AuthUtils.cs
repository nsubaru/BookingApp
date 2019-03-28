﻿using BookingApp.DTOs;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BookingAppIntegrationTests.TestingUtilities
{
    public static class AuthUtils
    {
        public static async Task AddAdminsBearer(HttpClient client)
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await GetToken(client, isAdmin: true)}");
        }

        public static async Task AddUsersBearer(HttpClient client)
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await GetToken(client, isAdmin: false)}");
        }

        static readonly StringBuilder adminsTokenCache = new StringBuilder();
        static readonly StringBuilder usersTokenCache = new StringBuilder();
        static async Task<string> GetToken(HttpClient client, bool isAdmin )
        {
            var tokenCache = isAdmin ? adminsTokenCache : usersTokenCache;

            if (tokenCache.Length < 1)
            {
                string email;
                string password;

                if (isAdmin)
                {
                    email = "superadmin@admin.cow";
                    password = "SuperAdmin";
                }
                else
                {
                    email = "lion@user.cow";
                    password = "Lion";
                }

                var responseToken = await client.PostAsJsonAsync("/api/auth/login", new
                {
                    Password = password,
                    Email = email
                });

                string tokenJson = responseToken.Content.ReadAsStringAsync().Result;
                AuthTokensDto tokenDto = JsonConvert.DeserializeObject<AuthTokensDto>(tokenJson);

                tokenCache.Append(tokenDto.AccessToken);
            }
            return tokenCache.ToString();
        }
    }
}