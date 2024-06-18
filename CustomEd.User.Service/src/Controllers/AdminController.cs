using System.Collections.Generic;
using System.Net.Mail;
using CusotmEd.User.Servce.DTOs;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Contracts;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.Model;
using CustomEd.Shared.Response;
using CustomEd.User.Service.DTOs;
using CustomEd.User.Service.Email.Service;
using CustomEd.User.Service.PasswordService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomEd.User.Service.Controllers;

[ApiController]
[Route("api/user/admin")]
public class AdminController : ControllerBase
{
    private readonly IGenericRepository<Model.Admin> _adminRepository;
    private readonly IGenericRepository<Model.Teacher> _teacherRepository;
    private readonly IGenericRepository<Model.Student> _studentRepository;
    private readonly IEmailService _emailService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AdminController(
        IGenericRepository<Model.Admin> adminRepository,
        IGenericRepository<Model.Teacher> teacherRepository,
        IGenericRepository<Model.Student> studentRepository,
        IEmailService emailService,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IHttpContextAccessor httpContextAccessor

    )
    {
        _adminRepository = adminRepository;
        _teacherRepository = teacherRepository;
        _studentRepository = studentRepository;
        _emailService = emailService;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _httpContextAccessor = httpContextAccessor;
    }

    
    [HttpPost("email-teacher")]
    [Authorize]
    public async Task<ActionResult<SharedResponse<bool>>> SendEmail(
        [FromBody] SendMailDto sendMailDto
    )
    {
        var userRole = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserRole();
        if (userRole != Role.Admin)
        {
            return Unauthorized(SharedResponse<bool>.Fail("Unauthorized", null));
        }

        try
        {
            var teacherIds = sendMailDto.TeacherIds;
            var subject = sendMailDto.Subject;
            var message = sendMailDto.Message;

            var teachers = await _teacherRepository.GetAllAsync(x => teacherIds.Contains(x.Id));
            var emails = teachers.Select(x => x.Email).ToList();
            foreach (var email in emails)
            {
                await _emailService.SendEmailAsync(email, subject, message);
            }
            return Ok(SharedResponse<bool>.Success(true, null));
        }
        catch (Exception e)
        {
            return BadRequest(SharedResponse<bool>.Fail(e.Message, null));
        }
    }

    [HttpPost("email-all-teachers")]
    [Authorize]
    public async Task<ActionResult<SharedResponse<bool>>> SendEmailToAllTeachers(
        [FromBody] SendMailDto sendMailDto
    )
    {
        var userRole = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserRole();
        if (userRole != Role.Admin)
        {
            return Unauthorized(SharedResponse<bool>.Fail("Unauthorized", null));
        }
        try
        {
            var teachers = await _teacherRepository.GetAllAsync();
            var emails = teachers.Select(x => x.Email).ToList();
            var subject = sendMailDto.Subject;
            var message = sendMailDto.Message;

            foreach (var email in emails)
            {
                await _emailService.SendEmailAsync(email, subject, message);
            }
            return Ok(SharedResponse<bool>.Success(true, null));
        }
        catch (Exception e)
        {
            return BadRequest(SharedResponse<bool>.Fail(e.Message, null));
        }
    }

    [HttpPost("email-students")]
    [Authorize]
    public async Task<ActionResult<SharedResponse<string>>> SendEmailToStudents(
        [FromBody] SendMailDto sendMailDto
    )
    {
        var userRole = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserRole();
        if (userRole != Role.Admin)
        {
            return Unauthorized(SharedResponse<string>.Fail("Unauthorized", null));
        }
        try
        {
            var students = await _studentRepository.GetAllAsync();
            var emails = students.Select(x => x.Email).ToList();
            var subject = sendMailDto.Subject;
            var message = sendMailDto.Message;
            foreach (var email in emails)
            {
                await _emailService.SendEmailAsync(email, subject, message);
            }
            return Ok(SharedResponse<bool>.Success(true, null));
        }
        catch (Exception e)
        {
            return BadRequest(SharedResponse<bool>.Fail(e.Message, null));
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<SharedResponse<UserDto>>> SignIn(
        [FromBody] LoginRequestDto request
    )
    {
        var user = await _adminRepository.GetAsync(x => x.Email == request.Email);
            if(user == null)
            {
                return BadRequest(SharedResponse<UserDto>.Fail("User not found or not verified", null));
            }
            if(request.Password != user.Password)
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
