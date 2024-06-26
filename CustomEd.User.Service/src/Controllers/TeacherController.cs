using AutoMapper;
using CusotmEd.Contracts.User.Events;
using CusotmEd.User.Servce.DTOs;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Contracts;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.Response;
using CustomEd.User.Contracts;
using CustomEd.User.Contracts.Teacher.Events;
using CustomEd.User.Service.DTOs;
using CustomEd.User.Service.Model;
using CustomEd.User.Service.PasswordService.Interfaces;
using CustomEd.User.Service.Validators;
using CustomEd.User.Teacher.Events;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomEd.User.Service.Controllers
{
    [ApiController]
    [Route("api/user/teacher")]
    public class TeacherController : UserController<Model.Teacher>
    {
        public TeacherController(
            IGenericRepository<Model.Teacher> userRepository,
            IMapper mapper,
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            IPublishEndpoint publishEndpoint,
            IHttpContextAccessor httpContextAccessor,
            UserApiClient userApiClient
        )
            : base(
                userRepository,
                mapper,
                passwordHasher,
                jwtService,
                publishEndpoint,
                httpContextAccessor,
                userApiClient
            ) { }

        [HttpGet("teacher-name")]
        public async Task<ActionResult<SharedResponse<List<TeacherDto>>>> SearchTeacherByName(
            [FromQuery] string name
        )
        {
            var teacher = await _userRepository.GetAllAsync(u =>
                u.IsVerified == true && (u.FirstName!.Contains(name) || u.LastName!.Contains(name))
            );
            var teacherDto = _mapper.Map<List<TeacherDto>>(teacher);
            return Ok(
                SharedResponse<List<TeacherDto>>.Success(
                    teacherDto,
                    "Teacher retrieved successfully"
                )
            );
        }

        [HttpPost]
        public async Task<ActionResult<SharedResponse<TeacherDto>>> CreateUser(
            [FromBody] CreateTeacherDto createTeacherDto
        )
        {
            var createTeacherDtoValidator = new CreateTeacherDtoValidator(_userRepository);
            var validationResult = await createTeacherDtoValidator.ValidateAsync(createTeacherDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(
                    SharedResponse<Model.Teacher>.Fail(
                        "Invalid input",
                        validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                    )
                );
            }

            try
            {
                var response = await _userApiClient.CheckEmailExistsAsync(createTeacherDto.Email);
                if (
                    response!.UserExists == false
                    || (response.Role != null && response.Role != Shared.Model.Role.Teacher)
                )
                {
                    return BadRequest(
                        SharedResponse<Model.Student>.Fail(
                            "User email Does not exist in database.",
                            null
                        )
                    );
                }
            }
            catch (Exception e)
            {
                return BadRequest(SharedResponse<Model.Teacher>.Fail(e.Message, null));
            }

            var teacherInfo = await _userApiClient.FetchTeacherProfile(createTeacherDto.Email);
            if (teacherInfo == null)
            {
                return BadRequest(
                    SharedResponse<Model.Teacher>.Fail("User email Does not exist in database.", null)
                );
            }
            Department teacherDep = (Department)int.Parse(teacherInfo[1]); 
            if (teacherDep != createTeacherDto.Department)
            {
                return BadRequest(
                    SharedResponse<Model.Teacher>.Fail("Incorrect department", null)
                );
            }

            var passwordHash = _passwordHasher.HashPassword(createTeacherDto.Password);
            createTeacherDto.Password = passwordHash;

            var teacher = _mapper.Map<Model.Teacher>(createTeacherDto);
            teacher.Role = Model.Role.Teacher;

            await _userRepository.CreateAsync(teacher);
            var unverifiedUserEvent = new UnverifiedUserEvent
            {
                Id = teacher.Id,
                Role = (Shared.Model.Role)Role.Teacher
            };
            var dto = _mapper.Map<TeacherDto>(teacher);
            await _publishEndpoint.Publish(unverifiedUserEvent);
            return CreatedAtAction(
                nameof(GetUserById),
                new { id = teacher.Id },
                SharedResponse<TeacherDto>.Success(dto, "User created successfully")
            );
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<SharedResponse<Model.Teacher>>> RemoveUser(Guid id)
        {
            if (id == Guid.Empty || await _userRepository.GetAsync(id) == null)
            {
                return BadRequest(
                    SharedResponse<Model.Teacher>.Fail(
                        "Invalid Id",
                        new List<string> { "Invalid id" }
                    )
                );
            }

            var identityProvider = new IdentityProvider(_httpContextAccessor, _jwtService);
            var currentUserId = identityProvider.GetUserId();
            if (currentUserId != id)
            {
                return Unauthorized(
                    SharedResponse<Model.Teacher>.Fail("Unauthorized to delete user", null)
                );
            }

            await _userRepository.RemoveAsync(id);
            var teacherDeletedEvent = new TeacherDeletedEvent { Id = id };
            await _publishEndpoint.Publish(teacherDeletedEvent);
            return Ok(SharedResponse<Model.Teacher>.Success(null, "User deleted successfully"));
        }

        [HttpPut]
        public async Task<ActionResult<SharedResponse<TeacherDto>>> UpdateUser(
            [FromBody] UpdateTeacherDto updateTeacherDto
        )
        {
            var updateTeacherDtoValidator = new UpdateTeacherDtoValidator(_userRepository);
            var validationResult = await updateTeacherDtoValidator.ValidateAsync(updateTeacherDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(
                    SharedResponse<TeacherDto>.Fail(
                        "Invalid input",
                        validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                    )
                );
            }

            var identityProvider = new IdentityProvider(_httpContextAccessor, _jwtService);
            var currentUserId = identityProvider.GetUserId();
            if (currentUserId != updateTeacherDto.Id)
            {
                return Unauthorized(
                    SharedResponse<TeacherDto>.Fail("Unauthorized to update user", null)
                );
            }

            string passwordHash;
            var oldUser = await _userRepository.GetAsync(updateTeacherDto.Id);
            if (updateTeacherDto.Password == null)
            {
                passwordHash = oldUser.Password;
            }
            else
            {
                passwordHash = _passwordHasher.HashPassword(updateTeacherDto.Password);
            }

            updateTeacherDto.Password = passwordHash;
            updateTeacherDto.Email = oldUser.Email;

            var user = _mapper.Map<Model.Teacher>(updateTeacherDto);
            user.Role = Model.Role.Teacher;

            user.IsVerified = oldUser.IsVerified;
            await _userRepository.UpdateAsync(user);

            var teacherUpdatedEvent = _mapper.Map<TeacherUpdatedEvent>(user);
            await _publishEndpoint.Publish(teacherUpdatedEvent);
            var dto = _mapper.Map<TeacherDto>(user);
            return Ok(SharedResponse<TeacherDto>.Success(dto, "User updated successfully"));
        }

        [HttpPost("login")]
        public override async Task<ActionResult<SharedResponse<UserDto>>> SignIn(
            [FromBody] LoginRequestDto request
        )
        {
            return await base.SignIn(request);
        }

        [Authorize]
        [HttpPost("upload")]
        public async Task<ActionResult<SharedResponse<TeacherDto>>> UploadImage(IFormFile file)
        {
            var userId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
            var user = await _userRepository.GetAsync(userId);
            if (user == null)
            {
                return NotFound(SharedResponse<TeacherDto>.Fail("User not found", null));
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest(SharedResponse<TeacherDto>.Fail("Invalid file", null));
            }

            var fileName =
                user.Role.ToString()
                + "_"
                + Guid.NewGuid().ToString()
                + Path.GetExtension(file.FileName);
            var filePath = Path.Combine("./Uploads", fileName);

            try
            {
                if (user.ImageUrl != null)
                {
                    var oldFilePath = Path.Combine("./Uploads", user.ImageUrl);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                }

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                user.ImageUrl = fileName;
                await _userRepository.UpdateAsync(user);

                var teacherDto = _mapper.Map<TeacherDto>(user);

                return Ok(
                    SharedResponse<TeacherDto>.Success(teacherDto, "Image uploaded successfully")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    SharedResponse<TeacherDto>.Fail(
                        "Error uploading image",
                        new List<string> { ex.Message }
                    )
                );
            }
        }

        [HttpGet("picture/{userId}")]
        public async Task<IActionResult> GetUploadedFile(Guid userId)
        {
            var user = await _userRepository.GetAsync(userId);
            if (user == null)
            {
                Console.WriteLine("User not found");
                return NotFound("User not found");
            }
            var fileName = user.ImageUrl;
            if (string.IsNullOrEmpty(fileName))
            {
                Console.WriteLine("File not found");
                return NotFound("File not found");
            }
            var filePath = Path.Combine("./Uploads", fileName);
            if (!System.IO.File.Exists(filePath))
            {
                Console.WriteLine("File not found");
                return NotFound("File not found");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return Ok(File(fileBytes, "application/octet-stream", fileName));
        }
    }
}
