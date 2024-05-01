using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using CustomEd.User.Service.PasswordService.Interfaces;
using CusotmEd.User.Servce.DTOs;
using CustomEd.Shared.JWT.Contracts;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.Response;
using MassTransit;
using CustomEd.User.Service.Model;

namespace CustomEd.User.Service.Controllers
{
    [ApiController]
    public abstract class UserController<T> : ControllerBase where T : Model.User 
    {
        protected readonly IGenericRepository<T> _userRepository; 
        protected readonly IMapper _mapper;
        protected readonly IPasswordHasher _passwordHasher;
        protected readonly IJwtService _jwtService;
        protected readonly IPublishEndpoint _publishEndpoint;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        public UserController(IGenericRepository<T> userRepository, IMapper mapper, IPasswordHasher passwordHasher, IJwtService jwtService, IPublishEndpoint publishEndpoint, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _publishEndpoint = publishEndpoint;
            _httpContextAccessor = httpContextAccessor;
        }
    
        [HttpGet]
        public  async Task<ActionResult<SharedResponse<IEnumerable<T>>>> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(SharedResponse<IEnumerable<T>>.Success(users, "Users retrieved successfully"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SharedResponse<T>>> GetUserById(Guid id)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
            {
                return NotFound(SharedResponse<T>.Fail("User not found", null));
            }
            return Ok(SharedResponse<T>.Success(user, "User retrieved successfully"));
        }

        [HttpPost("user/login")]
        public virtual async Task<ActionResult<SharedResponse<UserDto>>> SignIn([FromBody] LoginRequestDto request)
        {

            var user = await _userRepository.GetAsync(x => x.Email == request.Email && x.IsVerified == true );
            if(user == null)
            {
                return BadRequest(SharedResponse<UserDto>.Fail("User not found or not verified", null));
            }
            if(!_passwordHasher.VerifyPassword(request.Password, user.Password))
            {
                return BadRequest(SharedResponse<bool>.Fail("Incorrect Password", null));
            }
            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = (IdentityRole) user.Role
            };

            var token = _jwtService.GenerateToken(userDto);
            userDto.Token = token;

            return Ok(SharedResponse<UserDto>.Success(userDto, null));
        }        
    }
}
