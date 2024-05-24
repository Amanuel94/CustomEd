using AutoMapper;
using CusotmEd.Contracts.User.Events;
using CusotmEd.User.Servce.DTOs;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Contracts;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.Response;
using CustomEd.User.Service.DTOs;
using CustomEd.User.Service.Model;
using CustomEd.User.Service.PasswordService.Interfaces;
using CustomEd.User.Service.Validators;
using CustomEd.User.Student.Events;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomEd.User.Service.Controllers
{
    [ApiController]
    [Route("api/user/student")]
    public class StudentController : UserController<Model.Student>
    {
        public StudentController(
            IGenericRepository<Model.Student> userRepository,
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

        [HttpGet("student-id")]
        public async Task<ActionResult<SharedResponse<StudentDto>>> SearchStudentBySchoolId(
            [FromQuery] string id
        )
        {
            var student = await _userRepository.GetAsync(u =>
                u.StudentId == id && u.IsVerified == true
            );
            var studentDto = _mapper.Map<StudentDto>(student);
            return Ok(
                SharedResponse<StudentDto>.Success(studentDto, "Students retrieved successfully")
            );
        }

        [HttpGet("student-name")]
        public async Task<
            ActionResult<SharedResponse<IEnumerable<StudentDto>>>
        > SearchStudentByName([FromQuery] string name)
        {
            var students = await _userRepository.GetAllAsync(u =>
                u.IsVerified && (u.FirstName!.Contains(name) || u.LastName!.Contains(name))
            );
            var studentsDto = _mapper.Map<IEnumerable<StudentDto>>(students);
            return Ok(
                SharedResponse<IEnumerable<StudentDto>>.Success(
                    studentsDto,
                    "Students retrieved successfully"
                )
            );
        }

        [HttpPost]
        public async Task<ActionResult<SharedResponse<Model.Student>>> CreateUser(
            [FromBody] CreateStudentDto studentDto
        )
        {
            var createStudentDtoValidator = new CreateStudentDtoValidator(_userRepository);
            var validationResult = await createStudentDtoValidator.ValidateAsync(studentDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(
                    SharedResponse<Model.Student>.Fail(
                        "Invalid input",
                        validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                    )
                );
            }

            try{

            var response = await _userApiClient.CheckEmailExistsAsync(studentDto.Email);
            if(response == null)
            {
                return BadRequest(SharedResponse<Model.Student>.Fail("School Database is down.", null));
            }
            if(response!.userExisits == false || (response.Role != null && response.Role != Shared.Model.Role.Student))
            {
                return BadRequest(SharedResponse<Model.Student>.Fail("User email Does not exist in database.", null));
            }
            }
            catch (Exception e)
            {
                return BadRequest(SharedResponse<Model.Student>.Fail(e.Message, null));
            }
            var passwordHash = _passwordHasher.HashPassword(studentDto.Password);
            studentDto.Password = passwordHash;

            var student = _mapper.Map<Model.Student>(studentDto);
            student.Role = Model.Role.Student;

            await _userRepository.CreateAsync(student);

            var unverifiedUserEvent = new UnverifiedUserEvent{
            Id = student.Id,
            Role = (Shared.Model.Role)Role.Student
        };
            unverifiedUserEvent.Id = student.Id;
            Console.WriteLine("Log: Student Created");
            Console.WriteLine($"Log: {student.Id}");
            Console.WriteLine($"Log: {unverifiedUserEvent.Id}");
            await _publishEndpoint.Publish(unverifiedUserEvent);
            return CreatedAtAction(
                nameof(GetUserById),
                new { id = student.Id },
                SharedResponse<Model.Student>.Success(student, "User created successfully")
            );
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<SharedResponse<StudentDto>>> RemoveUser(Guid id)
        {
            if (id == Guid.Empty || await _userRepository.GetAsync(id) == null)
            {
                return BadRequest(
                    SharedResponse<StudentDto>.Fail("Invalid Id", new List<string> { "Invalid id" })
                );
            }

            var identityProvider = new IdentityProvider(
                _httpContextAccessor,
                _jwtService
            );
            var currentUserId = identityProvider.GetUserId();

            if (currentUserId != id)
            {
                return Unauthorized(
                    SharedResponse<StudentDto>.Fail(
                        "Unauthorized",
                        new List<string> { "Unauthorized" }
                    )
                );
            }

            var student = await _userRepository.GetAsync(id);
            await _userRepository.RemoveAsync(id);
            var studentDeletedEvent = new StudentDeletedEvent { Id = id };
            if (student.IsVerified == true)
            {
                await _publishEndpoint.Publish(studentDeletedEvent);
            }
            return Ok(SharedResponse<StudentDto>.Success(null, "User deleted successfully"));
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult<SharedResponse<StudentDto>>> UpdateUser(
            [FromBody] UpdateStudentDto studentDto
        )
        {
            var updateStudentDtoValidator = new UpdateStudentDtoValidator(_userRepository);
            var validationResult = await updateStudentDtoValidator.ValidateAsync(studentDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(
                    SharedResponse<StudentDto>.Fail(
                        "Invalid input",
                        validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                    )
                );
            }

            var identityProvider = new IdentityProvider(_httpContextAccessor, _jwtService);
            var currentUserId = identityProvider.GetUserId();

            if (currentUserId != studentDto.Id)
            {
                return Unauthorized(
                    SharedResponse<StudentDto>.Fail(
                        "Unauthorized",
                        new List<string> { "Unauthorized" }
                    )
                );
            }

            string passwordHash;
            var oldUser = await _userRepository.GetAsync(studentDto.Id);
            if(studentDto.Password == null)
            {
                passwordHash = oldUser.Password;
            }
            else
            {
                passwordHash = _passwordHasher.HashPassword(studentDto.Password);
            }

            // var passwordHash = _passwordHasher.HashPassword(studentDto.Password);
            studentDto.Password = passwordHash;

            var student = _mapper.Map<Model.Student>(studentDto);
            student.Role = Model.Role.Student;

            // var oldUser = await _userRepository.GetAsync(student.Id);
            student.IsVerified = oldUser.IsVerified;

            await _userRepository.UpdateAsync(student);
            var studentUpdatedEvent = _mapper.Map<StudentCreatedEvent>(student);
            if (student.IsVerified == true)
            {
                await _publishEndpoint.Publish(studentUpdatedEvent);
            }
            return Ok(SharedResponse<StudentDto>.Success(null, "User updated successfully"));
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

            var fileName = user.Role.ToString() + "_" + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine("./Uploads", fileName);

            try
            {
                if(user.ImageUrl != null)
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

                return Ok(SharedResponse<TeacherDto>.Success(teacherDto, "Image uploaded successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, SharedResponse<TeacherDto>.Fail("Error uploading image", new List<string>{ex.Message}));
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
