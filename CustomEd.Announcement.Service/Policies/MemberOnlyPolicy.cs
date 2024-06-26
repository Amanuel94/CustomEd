using CustomEd.Announcement.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CustomEd.Announcement.Service.Policies
{
    public class MemberOnlyRequirement : IAuthorizationRequirement { }

    public class MemberOnlyPolicy : AuthorizationHandler<MemberOnlyRequirement>
    {
        private readonly IGenericRepository<ClassRoom> _classRoomRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtService _jwtService;

        public MemberOnlyPolicy(IGenericRepository<ClassRoom> classRoomRepository, IHttpContextAccessor httpContextAccessor, IJwtService jwtService)
        {
            _classRoomRepository = classRoomRepository;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MemberOnlyRequirement requirement)
        {
            var classroomId = Guid.Parse((string)_httpContextAccessor.HttpContext!.Request.RouteValues["classRoomId"]!);
            var identityProvider = new IdentityProvider(_httpContextAccessor, _jwtService);
            var userId = identityProvider.GetUserId();
            var userRole = identityProvider.GetUserRole();
            try{
                var classroom = await _classRoomRepository.GetAsync(classroomId);
                if ((classroom.MemberIds != null && classroom.MemberIds.Contains(userId)) || classroom.CreatorId == userId || userRole == Role.Admin)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
            catch (Exception)
            {
                context.Fail();
            }
        }
    }
}