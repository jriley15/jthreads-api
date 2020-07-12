using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JThreads.API.Extensions;
using JThreads.Application.Services;
using JThreads.Data.Dto.Auth;
using Microsoft.AspNetCore.Mvc;

namespace JThreads.API.Controllers
{
    public class AuthController : BaseController
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            return this.GenerateResponse(await _authService.Login(loginDto));
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            return this.GenerateResponse(await _authService.Register(registerDto));
        }

        [HttpPost]
        public async Task<IActionResult> GoogleLogin(OAuthLoginDto loginDto)
        {
            return this.GenerateResponse(await _authService.GoogleLogin(loginDto));
        }

        [HttpPost]
        public async Task<IActionResult> FacebookLogin(OAuthLoginDto loginDto)
        {
            return this.GenerateResponse(await _authService.FacebookLogin(loginDto));
        }
    }
}
