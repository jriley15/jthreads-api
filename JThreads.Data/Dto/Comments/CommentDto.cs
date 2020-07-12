using System;
using System.Collections.Generic;
using System.Text;
using JThreads.Data.Dto.User;
using JThreads.Data.Entity;
using JThreads.Data.Enums;

namespace JThreads.Data.Dto.Comments
{
    public class CommentDto
    {
        public int CommentId { get; set; }

        public string Body { get; set; }

        public CommentStatus Status { get; set; }

        public ICollection<CommentDto> Replies { get; set; }

        public int TotalReplyCount { get; set; }

        public int DirectReplyCount { get; set; }

        public DateTime CreatedOn { get; set; }

        public int Likes { get; set; }

        public int Dislikes { get; set; }

        public UserDto User { get; set; }

        public GuestDto Guest { get; set; }
    }
}
