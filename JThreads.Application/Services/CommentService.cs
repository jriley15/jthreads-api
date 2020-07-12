using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using JThreads.Data;
using JThreads.Data.Dto;
using JThreads.Data.Dto.Comments;
using JThreads.Data.Entity;
using JThreads.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace JThreads.Application.Services
{
    public class CommentService : BaseService
    {
        private readonly JThreadsDbContext _context;
        private readonly UserService _userService;
        private readonly GuestService _guestService;
        private readonly IMapper _mapper;

        public CommentService(JThreadsDbContext context, UserService userService, IMapper mapper, GuestService guestService)
        {
            _context = context;
            _userService = userService;
            _guestService = guestService;
            _mapper = mapper;
        }

        public async Task<Response> Delete(int commentId)
        {
            if (!_userService.IsAuthenticated)
            {
                return Respond().WithError("*", "Not Authorized");
            }

            var comment = await _context.Comments
                .Include(c => c.Thread)
                    .ThenInclude(t => t.Namespace)
                        .ThenInclude(n => n.User)
                .SingleOrDefaultAsync(c => c.CommentId == commentId);

            if (comment == null)
            {
                return Respond().WithError("*", "Comment doesn't exist");
            }
            if (comment.Thread.Namespace.User.Id != _userService.GetUserId()) 
            {
                return Respond().WithError("*", "Not Authorized");
            }

            _context.Comments.Remove(comment);
            var diff = await _context.SaveChangesAsync();

            return diff > 0 ? Respond().WithSuccess() : Respond().WithError("*", "Error deleting comment");
        }

        public async Task<Response> Create(CreateCommentDto createCommentDto)
        {
            var thread = await _context.Threads
                .Include(t => t.Namespace)
                .ThenInclude(n => n.User)
                .SingleOrDefaultAsync(t =>
                    t.Namespace.NamespaceId == createCommentDto.NamespaceId &&
                    t.ThreadId == createCommentDto.ThreadId);

            if (thread == null)
            {
                return Respond().WithError("*", "Namespace/Thread doesn't exist");
            }

            var commentEntity = _mapper.Map<Comment>(createCommentDto);
            commentEntity.CreatedOn = DateTime.UtcNow;

            if (_userService.IsAuthenticated)
            {
                // Authenticated user 
                commentEntity.User = await _userService.GetUser();
            }
            else
            {
                var guest = await _guestService.GetGuestAsync();

                if (string.IsNullOrEmpty(createCommentDto.Name) && guest == null)
                {
                    return Respond().WithError("*", "Must provide a guest name");
                }
                if (guest == null)
                {
                    guest = _guestService.CreateGuest(createCommentDto.Name);
                }
                commentEntity.Guest = guest;
            }

            if (createCommentDto.ParentCommentId.HasValue)
            {
                var parentComment = await _context.Comments
                    .Include(c => c.Parent)
                    .SingleOrDefaultAsync(c =>
                    c.CommentId == createCommentDto.ParentCommentId);

                // since we only allow a comment depth of 2,
                // make sure the parent comment we're replying to
                // doesn't belong to another higher comment in the tree
                //if (parentComment.Parent != null)
                //{
                //    return Respond().WithError("*", "Cannot reply to this comment");
                //}
                parentComment.Replies.Add(commentEntity);
            }
            thread.Comments.Add(commentEntity);

            await _context.SaveChangesAsync();

            return new DataResponse<CommentDto>()
                .WithData(_mapper.Map<CommentDto>(commentEntity));
        }

        public async Task<Response> Rate(CreateCommentRatingDto createCommentRatingDto)
        {
            var comment = await _context.Comments
                .SingleOrDefaultAsync(c => c.CommentId == createCommentRatingDto.CommentId);

            //var existingCommentRating = await _context.CommentRatings
            //    .Include(cr => cr.Comment)
            //    .Include(cr => cr.User)
            //    .SingleOrDefaultAsync(cr =>
            //        cr.Comment.CommentId == createCommentRatingDto.CommentId
            //        && cr.User.Id == _userService.GetUserId());

            if (!Enum.IsDefined(typeof(Rating), createCommentRatingDto.Type))
            {
                return Respond().WithError("*", "Invalid Rating Type");
            }

            //if (existingCommentRating != null)
            //{
            //    //user already rated this comment, either change the rating type or do nothing
            //    if (existingCommentRating.Type == createCommentRatingDto.Type)
            //    {
            //        return Respond().WithError("*", "Rating already exists");
            //    }

            //    existingCommentRating.Type = createCommentRatingDto.Type;
            //    await _context.SaveChangesAsync();
            //    return Respond().WithSuccess();

            //}

            var commentRatingEntity = _mapper.Map<CommentRating>(createCommentRatingDto);
            commentRatingEntity.CreatedOn = DateTime.UtcNow;
            //commentRatingEntity.User = await _userService.GetUser();
            comment.CommentRatings.Add(commentRatingEntity);
            await _context.SaveChangesAsync();

            return Respond().WithSuccess();
        }

        public async Task<Response> Search(int? threadId, int pageIndex, Sort sortType, int? parentId, int pageSize, string threadIdentifier, int? namespaceId)
        {
            //Max of 30 comments per request for now
            if (pageSize > 30) pageSize = 30;

            if (!threadId.HasValue && (string.IsNullOrEmpty(threadIdentifier) || !namespaceId.HasValue))
                return Respond().WithError("*", "Must provide a threadId or threadIdentifier & namespaceId");

            //var thread = await _context.Threads
            //    .Include(t => t.Namespace)
            //    .ThenInclude(t => t.User)
            //    .SingleOrDefaultAsync(t => t.ThreadId == threadId);

            //if (thread == null)
            //    return Respond()
            //        .WithError("*", "Thread doesn't exist");

            var query = _context.Comments
                .Include(c => c.User)
                .Include(c => c.Guest)
                .Include(c => c.Thread)
                .Include(c => c.Parent)

                // 10 levels of hack
                .Include(c => c.Replies)
                    .ThenInclude(c => c.Replies)
                        .ThenInclude(c => c.Replies)
                            .ThenInclude(c => c.Replies)
                                .ThenInclude(c => c.Replies)
                                    .ThenInclude(c => c.Replies)
                                        .ThenInclude(c => c.Replies)
                                             .ThenInclude(c => c.Replies)
                                                 .ThenInclude(c => c.Replies)
                                                     .ThenInclude(c => c.Replies)
                //.ThenInclude(r => r.User)
                .Include(c => c.CommentRatings)
                .AsQueryable();

            query = string.IsNullOrEmpty(threadIdentifier) ?
                query.Where(c => c.Thread.ThreadId == threadId) :
                query.Where(c => c.Thread.Identifier.ToLower() == threadIdentifier.ToLower()
                    && c.Thread.Namespace.NamespaceId == namespaceId.Value);

            query = parentId.HasValue ?
                query.Where(c => c.Parent.CommentId == parentId) :
                query.Where(c => c.Parent == null);

            query = sortType switch
            {
                Sort.MostRecent => query.OrderByDescending(c => c.CreatedOn),
                Sort.HighestRating => query
                    .OrderByDescending(c => c.CommentRatings.Count(cr => cr.Type == Rating.Positive))
                    .ThenByDescending(c => c.CreatedOn),
                Sort.MostReplies => query.OrderByDescending(c => c.Replies.Count)
                    .ThenByDescending(c => c.CreatedOn),
                _ => query.OrderByDescending(c => c.CreatedOn)
            };

            return new DataResponse<IEnumerable<CommentDto>>()
                .WithData(_mapper.Map<IEnumerable<CommentDto>>(await query
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize)
                    .ToListAsync()));
        }
    }
}
