﻿namespace DotNet_WebAPI.Models
{
    public class PostModels
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string PostTitle { get; set; } = string.Empty;
        public string PostContent { get; set; } = string.Empty;
        public DateTime PostCreated { get; set; }
        public DateTime PostUpdated { get; set; }
    }
}
