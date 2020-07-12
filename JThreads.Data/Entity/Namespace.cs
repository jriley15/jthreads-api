using System;
using System.Collections.Generic;
using System.Text;

namespace JThreads.Data.Entity
{
    /// <summary>
    /// User created 'spaces' that threads belong to, associated with a url/name
    /// </summary>
    public class Namespace
    {
        public Namespace()
        {
            Threads = new List<Thread>();
        }

        public int NamespaceId { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public ICollection<Thread> Threads { get; set; }

        public ApplicationUser User { get; set; }
    }
}
