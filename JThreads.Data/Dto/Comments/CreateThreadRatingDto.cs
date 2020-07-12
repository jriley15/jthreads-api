using System;
using System.Collections.Generic;
using System.Text;
using JThreads.Data.Enums;

namespace JThreads.Data.Dto.Comments
{
    public class CreateThreadRatingDto
    {
        public Rating Type { get; set; }

        public int ThreadId { get; set; }

    }
}
