using System;
using System.Collections.Generic;
using System.Text;
using JThreads.Data.Enums;

namespace JThreads.Data.Dto.Comments
{
    public class CreateCommentRatingDto
    {
        public Rating Type { get; set; }

        public int CommentId { get; set; }

    }
}
