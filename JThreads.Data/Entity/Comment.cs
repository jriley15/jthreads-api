using System;
using System.Collections.Generic;
using System.Text;
using JThreads.Data.Enums;

namespace JThreads.Data.Entity
{
    public class Comment
    {
        public Comment()
        {
            Replies = new List<Comment>();
            CommentRatings = new List<CommentRating>();
        }

        public int CommentId { get; set; }

        public string Body { get; set; }

        public CommentStatus Status { get; set; }
        
        public DateTime CreatedOn { get; set; }

        //ratings
        public ICollection<CommentRating> CommentRatings { get; set; }

        //parent comment
        public Comment Parent { get; set; }

        //child comments
        public ICollection<Comment> Replies { get; set; }

        //parent thread
        public Thread Thread { get; set; }

        //created by
        public ApplicationUser User { get; set; }

        //comment either belongs to application user (Identity or OAuth), or Guest (with nick name)??
        public Guest Guest { get; set; }
    }
}
