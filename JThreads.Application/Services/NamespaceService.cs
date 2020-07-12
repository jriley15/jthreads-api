using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using JThreads.Data;
using JThreads.Data.Dto;
using JThreads.Data.Dto.Namespace;
using JThreads.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace JThreads.Application.Services
{
    public class NamespaceService
    {
        private readonly JThreadsDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserService _userService;

        public NamespaceService(JThreadsDbContext context, IMapper mapper, UserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }

        //temporary - need to create a search method to limit the data returned
        public async Task<DataResponse<IEnumerable<NamespaceDto>>> GetAll()
        {
            return new DataResponse<IEnumerable<NamespaceDto>>()
                .WithData(_mapper.Map<IEnumerable<NamespaceDto>>(
                    await _context.Namespaces
                        .Include(n => n.Threads)
                        .Where(n => n.User.Id == _userService.GetUserId())
                        .ToListAsync()));
        }

        public async Task<DataResponse<NamespaceDto>> Create(CreateNamespaceDto createNamespaceDto)
        {
            var namespaceEntity = _mapper.Map<Namespace>(createNamespaceDto);
            namespaceEntity.User = await _userService.GetUser();
            await _context.Namespaces.AddAsync(namespaceEntity);
            await _context.SaveChangesAsync();
            return new DataResponse<NamespaceDto>()
                .WithData(_mapper.Map<NamespaceDto>(namespaceEntity));
        }
    }
}
