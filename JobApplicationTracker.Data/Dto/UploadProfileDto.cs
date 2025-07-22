using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.Data.Dto
{
    public class UploadProfileDto
    {
        public string UserId { get; set; } // User's unique ID
        public IFormFile ProfileImage { get; set; } // Profile picture
        public string Bio { get; set; } // Short bio
    }
}
