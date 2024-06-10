using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.Model;
using Microsoft.AspNetCore.Authorization;

namespace CustomEd.Forum.Service.Policies
{
    public class MemberOnlyRequirement : IAuthorizationRequirement { }

    public class MemberOnlyPolicy : AuthorizationHandler<MemberOnlyRequirement>
    {
        private readonly IGenericRepository<Model.Classroom> _classRoomRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtService _jwtService;

        public MemberOnlyPolicy(IGenericRepository<Model.Classroom> classRoomRepository, IHttpContextAccessor httpContextAccessor, IJwtService jwtService)
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

            var classroom = await _classRoomRepository.GetAsync(classroomId);
            if(classroom == null)
            {
                context.Fail();
                return;
            }
            
            if ((classroom.Members != null && (classroom.Members.Select(x => x.Id)).ToList().Contains(userId)) || classroom.Creator.Id == userId || userRole == Role.Admin)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}
