using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JThreads.Data.Entity
{
    public class Thread
    {
        public Thread()
        {
            Comments = new List<Comment>();
            ThreadRatings = new List<ThreadRating>();
        }
        //Pk
        public int ThreadId { get; set; }

        //Identifier that the user provides to distinguish between threads in a single namespace
        public string Identifier { get; set; }

        //Origin Url that this thread was created from (potentially only allow comments to come from this Url)
        public string Origin { get; set; }

        public DateTime CreatedOn { get; set; }

        //ratings
        public ICollection<ThreadRating> ThreadRatings { get; set; }

        //Comments that belong to this thread
        public ICollection<Comment> Comments { get; set; }

        //Namespace that this thread belongs to
        public Namespace Namespace { get; set; }

        public int Views { get; set; }

    }
}
