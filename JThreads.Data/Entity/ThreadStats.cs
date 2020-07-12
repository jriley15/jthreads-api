using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JThreads.Data.Entity
{
    public class ThreadStats
    {
        [Key]
        public int ThreadId { get; set; }
        public int? TotalComments { get; set; }
        public int? TotalLikes { get; set; }
        public int? TotalDirectComments { get; set; }
    }
}
