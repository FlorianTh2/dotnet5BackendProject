﻿using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace homepageBackend.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // returned email and not user-id
        // public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        
        // returns userid
        public string UserId => _httpContextAccessor.HttpContext?.User?.Claims.Single(a => a.Type == "id").Value;

    }
}