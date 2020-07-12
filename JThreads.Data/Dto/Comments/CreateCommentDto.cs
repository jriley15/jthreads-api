using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JThreads.Data.Dto.Comments
{
    public class CreateCommentDto
    {
        [Required]
        public int ThreadId { get; set; }

        [Required]
        public int NamespaceId { get; set; }

        [Required, MaxLength(1000)]
        [DataType(DataType.Text)]
        public string Body { get; set; }

        public int? ParentCommentId { get; set; }

        public string Name { get; set; }


    }
}
