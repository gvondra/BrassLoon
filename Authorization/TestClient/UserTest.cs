﻿using BrassLoon.Interface.Authorization;
using BrassLoon.Interface.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Account = BrassLoon.Interface.Account;

namespace BrassLoon.Authorization.TestClient
{
    public class UserTest
    {
        private readonly SettingsFactory _settingsFactory;
        private readonly AppSettings _settings;
        private readonly Account.ITokenService _tokenService;
        private readonly IUserService _userService;

        public UserTest(SettingsFactory settingsFactory, AppSettings settings, Account.ITokenService tokenService, IUserService userService)
        {
            _settingsFactory = settingsFactory;
            _settings = settings;
            _tokenService = tokenService;
            _userService = userService;
        }

        public async Task Execute()
        {
            if (string.IsNullOrEmpty(AccessToken.Get.GetGoogleIdToken()))
                await GoogleLogin.Login(_settings);
            string accessToken = await _tokenService.Create(_settingsFactory.CreateAccount(AccessToken.Get.GetGoogleIdToken()));
            AuthorizationSettings settings = _settingsFactory.CreateAuthorization(accessToken);
            Console.WriteLine($"Getting users in domain {_settings.AuthorizationDomainId.Value:D}");
            IAsyncEnumerable<User> users = await _userService.GetByDomainId(settings, _settings.AuthorizationDomainId.Value);
            await foreach (User user in users)
            {
                Console.WriteLine($"found {user.UserId:D} {user.Name}");
            }
        }
    }
}