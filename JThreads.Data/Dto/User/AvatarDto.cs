using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JThreads.Data.Dto.User
{
    public class AvatarDto
    {
        [Required]
        public string ImageUrl { get; set; }
    }
}
