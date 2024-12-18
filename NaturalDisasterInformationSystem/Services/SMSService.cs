﻿using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace NaturalDisasterInformationSystem.Services
{
    public class SMSService
    {
        private readonly string _apiKey;
        private readonly string _secretKey;
        private readonly HttpClient httpClient;
        private readonly DO_ANContext context;

        public SMSService(string apiKey, string secretKey, DO_ANContext context)
        {
            _apiKey = apiKey;
            _secretKey = secretKey;
            this.context = context;
            httpClient = new HttpClient();
        }

        public async Task<List<string>> SendSmsAsync(string message,string? AddressFilter)
        {
            IQueryable<Models.User> usersQuery = context.Users;

            if (!string.IsNullOrEmpty(AddressFilter))
            {
                // Use EF.Functions.Like for case-insensitive comparison
                usersQuery = usersQuery.Where(u => EF.Functions.Like(u.Address, "%" + AddressFilter + "%"));
            }

            // Get the list of phone numbers of users whose address matches the filter
            var phoneNumbers = await usersQuery
                .Where(u => !string.IsNullOrEmpty(u.PhoneNumber)) // Make sure phone number is not null or empty
                .Select(u => u.PhoneNumber)
                .ToListAsync();
/*            var phoneNumbers = await context.Users.Select(p => p.PhoneNumber).ToListAsync();
*/            var results = new List<string>();

            foreach (var phoneNumber in phoneNumbers)
            {
                var url = $"https://rest.esms.vn/MainService.svc/json/SendMultipleMessage_V4_get?ApiKey={_apiKey}&SecretKey={_secretKey}&Phone={phoneNumber}&Content={message}&SmsType=4";

                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    results.Add($"Gửi SMS tới {phoneNumber}: Thành công");
                }
                else
                {
                    results.Add($"Gửi SMS tới {phoneNumber}: Thất bại");
                }
            }

            return results;
        }
    }
}
