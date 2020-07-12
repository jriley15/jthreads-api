using System;
using System.Collections.Generic;
using System.Text;
using JThreads.Data.Enums;

namespace JThreads.Data.Entity
{
    public class CommentRating
    {
        public int CommentRatingId { get; set; }

        public Rating Type { get; set; }

        public ApplicationUser User { get; set; }

        public DateTime CreatedOn { get; set; }

        //parent comment
        public Comment Comment { get; set; }
    }
}
