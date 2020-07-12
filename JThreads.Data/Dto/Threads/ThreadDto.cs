using System;
using System.Collections.Generic;
using System.Text;
using JThreads.Data.Dto.Comments;
using JThreads.Data.Dto.Namespace;
using JThreads.Data.Dto.User;

namespace JThreads.Data.Dto.Threads
{
    public class ThreadDto
    {
        public int ThreadId { get; set; }

        public string Identifier { get; set; }

        public NamespaceDto Namespace { get; set; }

        public DateTime CreatedOn { get; set; }

        public int TotalComments { get; set; }

        public int Comments { get; set; }

        public int Likes { get; set; }

        public int Dislikes{ get; set; }

        public int Views { get; set; }

        public bool IsAdmin { get; set; }

    }
}
