﻿namespace homepageBackend.Contracts.V1.Requests
{
    public class CreateTagRequest
    {
        /// <summary>
        /// the new Name of the tag
        /// </summary>
        public string TagName { get; set; }
    }
}