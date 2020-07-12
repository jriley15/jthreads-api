using System;
using System.Collections.Generic;
using System.Text;

namespace JThreads.Data.Dto.Threads
{
    public class InitThreadDto
    {
        //Namespace Id (Pk) that this thread belongs to
        public int NamespaceId { get; set; }

        //User generated unique identifier for this thread
        public string Identifier { get; set; }

        public int ThreadId { get; set; }

    }
}
