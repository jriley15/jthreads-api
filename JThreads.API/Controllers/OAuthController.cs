using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JThreads.API.Extensions;
using JThreads.Application.Services;
using JThreads.Data.Dto.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace JThreads.API.Controllers
{
    public class OAuthController : BaseController
    {
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;

        public OAuthController(AuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Google(string code)
        {

            var response = await _authService.GoogleLogin(new OAuthLoginDto() { Code = code });
            var queryString = _configuration["OAuthReceiver"] + "?success=" + response.Success.ToString().ToLower();
            
            if (!response.Success)
            {
                var errors = JsonConvert.SerializeObject(response.Errors);
                queryString += "&errors=" + errors;
            }

            return Redirect(queryString);
        }

        [HttpGet]
        public async Task<IActionResult> Facebook(string code)
        {

            var response = await _authService.ExchangeFacebookCodeForTokenAsync(code);
            var queryString = _configuration["OAuthReceiver"] + "?success=" + response.Success.ToString().ToLower();

            if (!response.Success)
            {
                var errors = JsonConvert.SerializeObject(response.Errors);
                queryString += "&errors=" + errors;
            }

            return Redirect(queryString);
        }
    }
}
