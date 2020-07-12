using AutoMapper;
using JThreads.Data;
using JThreads.Data.Dto;
using JThreads.Data.Dto.User;
using JThreads.Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JThreads.Application.Services
{
    public class GuestService : BaseService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        private readonly JThreadsDbContext _context;

        private readonly IMapper _mapper;


        public GuestService(IHttpContextAccessor contextAccessor, JThreadsDbContext context, IMapper mapper)
        {
            _contextAccessor = contextAccessor;
            _context = context;
            _mapper = mapper;
        }

        public async Task<Response> GetGuestDtoAsync()
        {
            var existingGuest = await GetGuestAsync();
            if (existingGuest != null)
            {
                return RespondWithData(_mapper.Map<GuestDto>(existingGuest));
            }

            return default;
        }

        public async Task<Guest> GetGuestAsync()
        {
            if (_contextAccessor.HttpContext.Items.TryGetValue("sessionId", out var guid))
            {
                var existingGuest = await _context.Guests.FirstOrDefaultAsync(g => g.Identifier == guid.ToString());
                if (existingGuest != null)
                {
                    return existingGuest;
                }
            }
            return default;
        }

        public Guest CreateGuest(string name)
        {
            if (_contextAccessor.HttpContext.Items.TryGetValue("sessionId", out var guid)) {
                var guest = new Guest()
                {
                    Name = name,
                    Identifier = guid.ToString()
                };
                _context.Guests.Add(guest);
                return guest;
            }
            return null;
        }
    }
}
