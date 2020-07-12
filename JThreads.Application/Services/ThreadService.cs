using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using JThreads.Data;
using JThreads.Data.Dto;
using JThreads.Data.Dto.Comments;
using JThreads.Data.Dto.Threads;
using JThreads.Data.Entity;
using JThreads.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace JThreads.Application.Services
{
    public class ThreadService : BaseService
    {
        private readonly JThreadsDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserService _userService;

        public ThreadService(JThreadsDbContext context, IMapper mapper, UserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<Response> Rate(CreateThreadRatingDto createThreadRatingDto)
        {
            var thread = await _context.Threads.
                SingleOrDefaultAsync(t => t.ThreadId == createThreadRatingDto.ThreadId);

            //var existingThreadRating = await _context.ThreadRatings
            //    .Include(tr => tr.Thread)
            //    .Include(tr => tr.User)
            //    .SingleOrDefaultAsync(tr =>
            //        tr.Thread.ThreadId == thread.ThreadId &&
            //        tr.User.Id == _userService.GetUserId());

            if (!Enum.IsDefined(typeof(Rating), createThreadRatingDto.Type))
            {
                return new Response().WithError("*", "Invalid Rating Type");
            }

            //if (existingThreadRating != null)
            //{
            //    //user already rated this thread, either change the rating type or do nothing
            //    if (existingThreadRating.Type != createThreadRatingDto.Type)
            //    {
            //        existingThreadRating.Type = createThreadRatingDto.Type;
            //        await _context.SaveChangesAsync();
            //        return new Response().WithSuccess();
            //    }
            //    return new Response()
            //        .WithError("*", "Rating already exists");
            //}

            var threadRatingEntity = _mapper.Map<ThreadRating>(createThreadRatingDto);
            threadRatingEntity.CreatedOn = DateTime.UtcNow;
            //threadRatingEntity.User = await _userService.GetUser();
            thread.ThreadRatings.Add(threadRatingEntity);
            await _context.SaveChangesAsync();

            return new Response().WithSuccess();
        }

        public async Task<Response> Search(int namespaceId, int? index, int? threadId)
        {

            var namespaceEntity = await _context.Namespaces
                .Include(n => n.User)
                .SingleOrDefaultAsync(n => n.NamespaceId == namespaceId);

            if (namespaceEntity == null)
            {
                return new Response()
                    .WithError("*", "Namespace doesn't exist");
            }

            if (namespaceEntity.User.Id != _userService.GetUserId())
            {
                return new Response()
                    .WithError("*", "Unauthorized");
            }

            var data = _mapper.Map<IEnumerable<ThreadDto>>(_context.Threads
                .Include(t => t.Namespace)
                .Include(t => t.Comments)
                .Where(t => t.Namespace.NamespaceId == namespaceId &&
                            (!threadId.HasValue || t.ThreadId == threadId.Value))
                .Skip(index.GetValueOrDefault() * 20)
                .Take(20));

            return new DataResponse<IEnumerable<ThreadDto>>()
                .WithData(data);
        }

        public async Task<Response> Init(InitThreadDto initThreadDto)
        {
            //check if this thread already exists
            var existingThread = await GetThreadWithEverything(t =>
                t.Namespace.NamespaceId == initThreadDto.NamespaceId && t.Identifier == initThreadDto.Identifier);

            if (existingThread != null)
            {
                existingThread.Views++;
                await _context.SaveChangesAsync();

                var existingThreadDto = _mapper.Map<ThreadDto>(existingThread);
                existingThreadDto.IsAdmin = _userService.IsAuthenticated ? existingThread.Namespace.User.Id == _userService.GetUserId() : false;

                //thread exists, return thread
                return new DataResponse<ThreadDto>()
                    .WithData(existingThreadDto);
            }

            //thread doesn't exist, so we need to create a record for it

            //find existing namespace to add this thread to
            var namespaceEntity = await _context.Namespaces.Include(n => n.Threads).SingleOrDefaultAsync(n =>
                n.NamespaceId == initThreadDto.NamespaceId);

            //check if namespace exists
            if (namespaceEntity == null)
            {
                return new Response().WithError("*", "Namespace doesn't exist");
            }

            //namespace exists, add the new thread to it

            //map thread dto to entity
            var threadEntity = _mapper.Map<Thread>(initThreadDto);

            threadEntity.CreatedOn = DateTime.UtcNow;
            threadEntity.Views = 1;

            //add new thread to the namespace
            namespaceEntity.Threads.Add(threadEntity);

            //save
            await _context.SaveChangesAsync();

            var newThreadDto = _mapper.Map<ThreadDto>(threadEntity);
            newThreadDto.IsAdmin = _userService.IsAuthenticated ? threadEntity.Namespace.User.Id == _userService.GetUserId() : false;

            return new DataResponse<ThreadDto>()
                .WithData(newThreadDto);

        }

        public async Task<Response> GetThreadDtoAsync(InitThreadDto initThreadDto)
        {
            var existingThread = await GetThreadWithEverything(t =>
                t.Namespace.NamespaceId == initThreadDto.NamespaceId && t.ThreadId == initThreadDto.ThreadId);

            if (existingThread == null)
            {
                return Respond().WithError("*", "Thread doesn't exist");
            }
            var threadDto = _mapper.Map<ThreadDto>(existingThread);
            threadDto.IsAdmin = _userService.IsAuthenticated ? existingThread.Namespace.User.Id == _userService.GetUserId() : false;

            return RespondWithData(threadDto);

        }

        private async Task<Thread> GetThreadWithEverything(Expression<Func<Thread, bool>> predicate)
        {
            var thread = await _context.Threads
                .Include(t => t.Namespace)
                    .ThenInclude(n => n.User)
                //.Include(t => t.Comments)
                //.Include(t => t.ThreadRatings)
                .Include(t => t.ThreadStats)
                .SingleOrDefaultAsync(predicate);

            return thread;
        }

        //private async Task GetThreadStats(ThreadDto thread)
        //{
        //    var stats = await _context.ThreadStats.Where(t => t.ThreadId == thread.ThreadId).SingleOrDefaultAsync();
        //    thread.TotalComments = stats?.TotalComments ?? 0;
        //    thread.Likes = stats?.TotalLikes ?? 0;
        //}
    }
}
