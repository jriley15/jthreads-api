using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JThreads.Application.Services;
using JThreads.Data.Dto.Threads;
using Microsoft.AspNetCore.Mvc;
using JThreads.API.Extensions;
using JThreads.Data.Dto.Comments;
using JThreads.Data.Enums;
using Microsoft.AspNetCore.Authorization;

namespace JThreads.API.Controllers
{

    public class CommentController : BaseController
    {
        private readonly CommentService _commentService;

        public CommentController(CommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> Search(int? threadId, int? parentId, int? namespaceId, string threadIdentifier, int pageSize = 10, int pageIndex = 0, Sort sortType = Sort.MostRecent)
        {
            return this.GenerateResponse(await _commentService.Search(threadId, pageIndex, sortType, parentId, pageSize, threadIdentifier, namespaceId));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCommentDto createCommentDto)
        {
            return this.GenerateResponse(await _commentService.Create(createCommentDto));
        }

        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> Rate(CreateCommentRatingDto createCommentRatingDto)
        {
            return this.GenerateResponse(await _commentService.Rate(createCommentRatingDto));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int commentId)
        {
            return this.GenerateResponse(await _commentService.Delete(commentId));
        }

    }
}
