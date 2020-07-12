using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JThreads.API.Extensions;
using JThreads.Application.Services;
using JThreads.Data.Dto.Auth;
using JThreads.Data.Dto.User;
using Microsoft.AspNetCore.Mvc;

namespace JThreads.API.Controllers
{
    public class GuestController : BaseController
    {
        private readonly GuestService _guestService;

            

        public GuestController(GuestService guestService)
        {
            _guestService = guestService;
        }

        [HttpGet]
        public async Task<IActionResult> Me()
        {
            return this.GenerateResponse(await _guestService.GetGuestDtoAsync());
        }
    }
}
