using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.Storage.Blobs;
using JThreads.Data;
using JThreads.Data.Dto;
using JThreads.Data.Dto.User;
using JThreads.Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace JThreads.Application.Services
{
    public class UserService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly JThreadsDbContext _context;
        private readonly IConfiguration _configuration;

        public UserService(IHttpContextAccessor contextAccessor, UserManager<ApplicationUser> userManager, IMapper mapper, JThreadsDbContext context, IConfiguration configuration)
        {
            _contextAccessor = contextAccessor;
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// anything calling this must first use the [authorize] middleware in the request
        /// </summary>
        /// <returns></returns>
        public string GetUserId()
        {
            return _contextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        }

        public bool IsAuthenticated => _contextAccessor.HttpContext.User.Identity.IsAuthenticated;

        public async Task<ApplicationUser> GetUser()
        {
            return await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
        }

        public async Task<Response> GetUserDto()
        {
            return new DataResponse<UserDto>()
                .WithData(_mapper.Map<UserDto>(await _userManager.GetUserAsync(_contextAccessor.HttpContext.User)));
        }

        public async Task<Response> SetAvatar(AvatarDto avatarDto)
        {
            var user = await GetUser();
            user.AvatarUrl = avatarDto.ImageUrl;
            await _userManager.UpdateAsync(user);
            return new Response().WithSuccess();
        }

        public async Task<Response> UploadAndSetAvatar(IFormFile imageFile)
        {
            var blobName = imageFile.FileName + "-" + DateTime.UtcNow.ToString();
            BlobContainerClient blobContainerClient = new BlobContainerClient(_configuration.GetConnectionString("azureStorage"), "avatars");
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

            await using (var readStream = imageFile.OpenReadStream())
            {
                await blobClient.UploadAsync(readStream);
            }

            var imageUrl = "https://jthreadsstorage.blob.core.windows.net/avatars/" + blobName;
            var user = await GetUser();
            user.AvatarUrl = imageUrl;
            await _userManager.UpdateAsync(user);

            return new DataResponse<string>()
                .WithData(imageUrl);
        }
    }
}
