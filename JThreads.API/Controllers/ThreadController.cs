using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JThreads.Application.Services;
using JThreads.Data.Dto.Threads;
using Microsoft.AspNetCore.Mvc;
using JThreads.API.Extensions;
using JThreads.Data.Dto.Comments;
using Microsoft.AspNetCore.Authorization;

namespace JThreads.API.Controllers
{
    public class ThreadController : BaseController
    {
        private readonly ThreadService _threadService;

        public ThreadController(ThreadService threadService)
        {
            _threadService = threadService;
        }

        /// <summary>
        /// This method will initialize all threads
        /// If the Namespace / Id combo don't exist, this will create a new one
        /// If the Namespace / Id combo do exist, this will return all comments belonging to the thread
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Init(InitThreadDto initThreadDto)
        {
            return this.GenerateResponse(await _threadService.Init(initThreadDto));
        }

        [HttpGet]
        public async Task<IActionResult> Get(int namespaceId, int threadId)
        {
            return this.GenerateResponse(await _threadService.GetThreadDtoAsync(new InitThreadDto()
            {
                NamespaceId = namespaceId,
                ThreadId = threadId
            }));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Search(int namespaceId, int? index, int? threadId)
        {
            return this.GenerateResponse(await _threadService.Search(namespaceId, index, threadId));
        }

        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> Rate(CreateThreadRatingDto createThreadRatingDto)
        {
            return this.GenerateResponse(await _threadService.Rate(createThreadRatingDto));
        }
    }
}
