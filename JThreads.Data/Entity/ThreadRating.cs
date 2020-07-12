using System;
using System.Collections.Generic;
using System.Text;
using JThreads.Data.Enums;

namespace JThreads.Data.Entity
{
    public class ThreadRating
    {
        public int ThreadRatingId { get; set; }

        public Rating Type { get; set; }

        public ApplicationUser User { get; set; }

        public DateTime CreatedOn { get; set; }

        //parent thread
        public Thread Thread { get; set; }
    }
}
