using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using JThreads.Application.Util;
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
                    opt.MapFrom(source => source.ThreadStats.TotalComments))
                .ForMember(t =>
                    t.Comments, opt =>
                    opt.MapFrom(source => source.ThreadStats.TotalDirectComments))
                .ForMember(t =>
                    t.Likes, opt =>
                    opt.MapFrom(source => source.ThreadStats.TotalLikes));
                //.ForMember(t =>
                //    t.Dislikes, opt =>
                //    opt.MapFrom(source => source.ThreadRatings
                //        .Count(cr => cr.Type == Rating.Negative))); 

            CreateMap<ApplicationUser, UserDto>();
            CreateMap<CreateCommentDto, Comment>();
            CreateMap<Comment, CommentDto>()
                .ForMember(t =>
                    t.Likes, opt =>
                    opt.MapFrom(source => source.CommentStats.TotalLikes))
                .ForMember(t =>
                    t.Dislikes, opt =>
                    opt.MapFrom(source => source.CommentStats.TotalDislikes))
                .ForMember(t =>
                    t.DirectReplyCount, opt =>
                    opt.MapFrom(source => source.CommentStats.TotalDirectReplies))
                .ForMember(dest => dest.Replies,
                    act => act.Ignore())
                .ForMember(t =>
                    t.TotalReplyCount, opt =>
                    opt.MapFrom(source => source.CommentStats.TotalReplies));

            CreateMap<CreateCommentRatingDto, CommentRating>();
            CreateMap<CreateThreadRatingDto, ThreadRating>();


            CreateMap<GuestDto, Guest>();
            CreateMap<Guest, GuestDto>();

        }
    }
}
