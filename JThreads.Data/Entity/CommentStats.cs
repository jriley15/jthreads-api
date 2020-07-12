using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JThreads.Data.Entity
{
    public class CommentStats
    {
        [Key]
        public int CommentId { get; set; }
        public int TotalReplies { get; set; }
        public int? TotalLikes { get; set; }
        public int? TotalDislikes { get; set; }
        public int? TotalDirectReplies { get; set; }
    }
}