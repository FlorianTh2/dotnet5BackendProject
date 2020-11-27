using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using homepageBackend.Cache;
using homepageBackend.Contracts.V1;
using homepageBackend.Contracts.V1.Requests;
using homepageBackend.Contracts.V1.Requests.Queries;
using homepageBackend.Contracts.V1.Responses;
using homepageBackend.Domain;
using homepageBackend.Extensions;
using homepageBackend.Helpers;
using homepageBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;

namespace homepageBackend.Controllers
{
    // this authorization attribute checks if the user is authenticated
    // because we defined our default AuthenticationScheme in MvcInstaller
    // we can simple write [Authorize] instead of:
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;

        public ProjectsController(IProjectService projectService, IMapper mapper, IUriService uriService)
        {
            _projectService = projectService;
            _mapper = mapper;
            _uriService = uriService;
        }

        [HttpGet]
        [Route(ApiRoutes.Projects.GetAll)]
        [Cache(600)]
        // [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery]PaginationQuery paginationQuery)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var projects = await _projectService.GetProjectsAsync(paginationFilter);
            var projectsResponse = _mapper.Map<List<ProjectResponse>>(projects);

            if (paginationFilter == null || paginationFilter.PageNumber < 1 || paginationFilter.PageSize < 1)
            {
                return Ok(new PagedResponse<ProjectResponse>(projectsResponse));
            }
            
            var paginationResponse =
                PaginationHelpers.CreatePaginatedResponse(_uriService, paginationFilter, projectsResponse);
            return Ok(paginationResponse);
        }

        [HttpGet]
        [Route(ApiRoutes.Projects.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid projectId)
        {
            var project = await _projectService.GetProjectByIdAsync(projectId);

            if (project == null)
                NotFound();

            return Ok(new Response<ProjectResponse>(_mapper.Map<ProjectResponse>(project)));
        }

        [HttpPut]
        [Route(ApiRoutes.Projects.Update)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromRoute] Guid projectId, [FromBody] UpdateProjectRequest request)
        {
            var userOwnsProject = await _projectService.UserOwnsPostAsync(projectId, HttpContext.GetUserId());

            if (!userOwnsProject)
            {
                return BadRequest(new {error = "You do not own this post"});
            }

            var project = await _projectService.GetProjectByIdAsync(projectId);
            project.Name = request.Name;

            var updated = await _projectService.UpdateProjectAsync(project);

            if (updated)
                return Ok(new Response<ProjectResponse>(_mapper.Map<ProjectResponse>(project)));

            return NotFound();
        }

        [HttpDelete]
        [Route(ApiRoutes.Projects.Delete)]
        [Authorize(Roles = "Admin")]
        [Authorize(Policy = "MustWorkForDotCom")]
        public async Task<IActionResult> Delete([FromRoute] Guid projectId)
        {
            var userOwnsProject = await _projectService.UserOwnsPostAsync(projectId, HttpContext.GetUserId());

            if (!userOwnsProject)
            {
                return BadRequest(new {error = "You do not own this post"});
            }

            var deleted = await _projectService.DeleteProjectAsync(projectId);

            if (deleted)
                return NoContent(); // 204

            return NotFound();
        }

        [HttpPost]
        [Route(ApiRoutes.Projects.Create)]
        public async Task<IActionResult> Create([FromBody] CreateProjectRequest projectRequest)
        {
            var newProjectId = Guid.NewGuid();
            var project = new Project
            {
                Name = projectRequest.Name,
                UserId = HttpContext.GetUserId(),
                Tags = projectRequest.Tags.Select(a => new ProjectTag()
                {
                    ProjectId = newProjectId,
                    TagName = a
                }).ToList()
            };

            await _projectService.CreateProjectAsync(project);

            var locationUri = _uriService.GetProjectUri(project.Id.ToString());
            return Created(locationUri, new Response<ProjectResponse>(_mapper.Map<ProjectResponse>(project)));
        }
    }
}