using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JThreads.Application.Services;
using JThreads.Data.Dto.Threads;
using Microsoft.AspNetCore.Mvc;
using JThreads.API.Extensions;
using JThreads.Data.Dto.Comments;
using JThreads.Data.Dto.User;
using JThreads.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using JThreads.Data.Dto;
using Microsoft.AspNetCore.Hosting;

namespace JThreads.API.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private readonly UserService _userService;

        private readonly IHostingEnvironment _currentEnvironment;

        public UserController(UserService userService, IHostingEnvironment currentEnvironment)
        {
            _userService = userService;
            _currentEnvironment = currentEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Me()
        {
            return this.GenerateResponse(await _userService.GetUserDto());
        }

        [HttpPost]
        public IActionResult Logout()
        {
            var cookieOptions = new CookieOptions()
            {
                Expires = DateTimeOffset.Now.AddDays(7),
                SameSite = SameSiteMode.Strict
            };
            if (_currentEnvironment.IsProduction())
            {
                cookieOptions.Domain = "jrdn.tech";
                cookieOptions.Secure = true;
            }
            HttpContext.Response.Cookies.Delete("token", cookieOptions);
            return this.GenerateResponse(new Response().WithSuccess());
        }

        [HttpPost]
        public async Task<IActionResult> SetAvatar(AvatarDto avatarDto)
        {
            return this.GenerateResponse(await _userService.SetAvatar(avatarDto));
        }

        [HttpPost]
        public async Task<IActionResult> UploadAndSetAvatar([FromForm]IFormFile imageFile)
        {
            return this.GenerateResponse(await _userService.UploadAndSetAvatar(imageFile));
        }
    }
}
