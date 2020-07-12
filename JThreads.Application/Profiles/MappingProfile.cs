using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using JThreads.Data.Dto.Comments;
using JThreads.Data.Dto.Namespace;
using JThreads.Data.Dto.Threads;
using JThreads.Data.Dto.User;
using JThreads.Data.Entity;
using JThreads.Data.Enums;

namespace JThreads.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            //CreateMap<CreateFormDto, Form>();
            //CreateMap<FormFieldDto, FormField>();
            //CreateMap<FormFieldValidationDto, FormFieldValidation>();
            //CreateMap<FormFieldValidationRuleDto, FormFieldValidationRule>();

            //CreateMap<FormDto, Form>();
            //CreateMap<Form, FormDto>();

            CreateMap<CreateNamespaceDto, Namespace>();
            CreateMap<Namespace, NamespaceDto>()
                .ForMember(n =>
                    n.Threads, opt =>
                    opt.MapFrom(source => source.Threads.Count));

            CreateMap<InitThreadDto, Thread>();
            CreateMap<Thread, ThreadDto>()
                .ForMember(t =>
                    t.TotalComments, opt =>
                    opt.MapFrom(source => source.Comments.Count))
                .ForMember(t =>
                    t.Comments, opt =>
                    opt.MapFrom(source => source.Comments
                    .Count(c => c.Parent == null)))
                .ForMember(t =>
                    t.Likes, opt =>
                    opt.MapFrom(source => source.ThreadRatings
                        .Count(cr => cr.Type == Rating.Positive)))
                .ForMember(t =>
                    t.Dislikes, opt =>
                    opt.MapFrom(source => source.ThreadRatings
                        .Count(cr => cr.Type == Rating.Negative))); 

            CreateMap<ApplicationUser, UserDto>();

            Func<Comment, int> getTotalReplies = (Comment comment) =>
            {
                var total = 0;
                var stack = new Stack<Comment>();
                stack.Push(comment);

                while (stack.TryPop(out var current))
                {
                    total += current.Replies.Count;
                    foreach (var reply in current.Replies)
                    {  
                        stack.Push(reply);
                    }
                }

                return total;
            };
            
            CreateMap<CreateCommentDto, Comment>();
            CreateMap<Comment, CommentDto>()
                .ForMember(t =>
                    t.Likes, opt =>
                    opt.MapFrom(source => source.CommentRatings
                        .Count(cr => cr.Type == Rating.Positive)))
                .ForMember(t =>
                    t.Dislikes, opt =>
                    opt.MapFrom(source => source.CommentRatings
                        .Count(cr => cr.Type == Rating.Negative)))
                .ForMember(t =>
                    t.DirectReplyCount, opt =>
                    opt.MapFrom(source => source.Replies.Count));
                //.ForMember(dest => dest.Replies,
                //    act => act.Ignore())
                //.ForMember(t =>
                //    t.TotalReplyCount, opt =>
                //    opt.MapFrom(source => getTotalReplies(source)));

            CreateMap<CreateCommentRatingDto, CommentRating>();
            CreateMap<CreateThreadRatingDto, ThreadRating>();


            CreateMap<GuestDto, Guest>();
            CreateMap<Guest, GuestDto>();

        }
    }
}
