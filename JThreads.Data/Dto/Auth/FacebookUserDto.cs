using System;
using System.Collections.Generic;
using System.Text;

namespace JThreads.Data.Dto.Auth
{
    public class FacebookUserDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public Picture Picture { get; set; }

    }

    public class Picture
    {
        public Data Data { get; set; }
    }


    public class Data
    {
        public string Url { get; set; }
    }
}
