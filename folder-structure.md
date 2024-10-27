```
CustomEd
├── CustomEd.Announcement.Service
│   ├── Consumers
│   │   ├── ClassroomCreatedEventConsumer.cs
│   │   ├── ClassroomDeletedEventConsumer.cs
│   │   ├── ClassroomUpdatedEventConsumer.cs
│   │   ├── MemberJoinedEventConsumer.cs
│   │   └── MemberLeftEventConsumer.cs
│   ├── Controllers
│   │   └── AnnouncementController.cs
│   ├── CustomEd.Announcement.Service.csproj
│   ├── CustomEd.Announcement.Service.sln
│   ├── DTOs
│   │   ├── AnnouncementDto.cs
│   │   ├── CreateAnnouncementDto.cs
│   │   ├── UpdateAnnouncementDto.cs
│   │   └── Validation
│   │       ├── CreateAnnouncementDtoValidator.cs
│   │       └── UpdateAnnouncementDtoValidator.cs
│   ├── Model
│   │   ├── Announcement.cs
│   │   ├── ClassRoom.cs
│   │   └── Teacher.cs
│   ├── Policies
│   │   ├── CreatorOnlyPolicy.cs
│   │   └── MemberOnlyPolicy.cs
│   ├── Profiles
│   │   └── MapperProfile.cs
│   ├── Program.cs
│   └── Uploads
│
├── CustomEd.Assessment.Service
│   ├── AnalyticsSevice
│   │   └── AnalysisService.cs
│   ├── Consumers
│   │   ├── ClassroomCreatedEventConsumer.cs
│   │   ├── ClassroomDeletedEventConsumer.cs
│   │   ├── ClassroomUpdatedEventConsumer.cs
│   │   ├── MemberJoinedEventConsumer.cs
│   │   └── MemberLeftEventConsumer.cs
│   ├── Controllers
│   │   ├── AnalyticsController.cs
│   │   └── AssessmentController.cs
│   ├── CustomEd.Assessment.Service.csproj
│   ├── CustomEd.Assessment.Service.sln
│   ├── DTOs
│   │   ├── AnalyticsDto
│   │   │   ├── AnalyticsDto.cs
│   │   │   └── CrossStudent.cs
│   │   ├── AnswerDtos
│   │   │   └── AnswerDto.cs
│   │   ├── AssessmentDtos
│   │   │   ├── AssessmentDto.cs
│   │   │   ├── CreateAssessmentDto.cs
│   │   │   ├── UpdateAssessmentDto.cs
│   │   │   └── Validation
│   │   │       ├── CreateAssessmentDtoValidator.cs
│   │   │       └── UpdateAssessmentDtoValidator.cs
│   │   ├── ClassroomDto.cs
│   │   ├── QuestionDtos
│   │   │   ├── CreateQuestionDto.cs
│   │   │   ├── QuestionDto.cs
│   │   │   ├── UpdateQuestionDto.cs
│   │   │   └── Validation
│   │   │       ├── CreateQuestionDtoValidator.cs
│   │   │       └── UpdateQuestionDtoValidator.cs
│   │   └── SubmissionDtos
│   │       ├── CreateSubmissionDto.cs
│   │       ├── SubmissionDto.cs
│   │       ├── UpdateSubmissionDto.cs
│   │       └── Validation
│   │           ├── CreateSubmissionDtoValidator.cs
│   │           └── UpdateSubissionDtoValidator.cs
│   ├── Model
│   │   ├── Analytics.cs
│   │   ├── Answer.cs
│   │   ├── Assessment.cs
│   │   ├── Classroom.cs
│   │   ├── Question.cs
│   │   └── Submission.cs
│   ├── Policies
│   │   ├── CreatorOnlyPolicy.cs
│   │   ├── MemberOnlyPolicy.cs
│   │   └── StudentOnlyPolicy.cs
│   ├── Profiles
│   │   └── MapperProfile.cs
│   ├── Program.cs
│   └── Properties
│       └── launchSettings.json
├── CustomEd.Classroom.Service
│   ├── Clients
│   │   └── HttpClient.cs
│   ├── Consumers
│   │   ├── StudentEvents
│   │   │   ├── StudentCreatedEventConsumer.cs
│   │   │   ├── StudentDeletedEventConsumer.cs
│   │   │   └── StudentUpdatedEventConsumer.cs
│   │   └── TeacherEvents
│   │       ├── TeacherCreatedEventConsumer.cs
│   │       ├── TeacherDeletedEventConsumer.cs
│   │       └── TeacherUpdatedEventConsumer.cs
│   ├── Controllers
│   │   └── ClassroomController.cs
│   ├── CustomEd.Classroom.Service.csproj
│   ├── CustomEd.Classroom.Service.sln
│   ├── DTOs
│   │   ├── AddBatchDto.cs
│   │   ├── AddStundetDto.cs
│   │   ├── ClassroomDto.cs
│   │   ├── CreateClassroomDto.cs
│   │   ├── RemoveStudentDto.cs
│   │   ├── SearchResult.cs
│   │   ├── UpdateClassroomDto.cs
│   │   └── Validation
│   │       ├── AddBatchDtoValidator.cs
│   │       ├── AddStudentDtoValidator.cs
│   │       ├── CreateClassroomDtoValidator.cs
│   │       ├── RemoveStudentDtoValidator.cs
│   │       └── UpdateClassroomDtoValidator.cs
│   ├── Model
│   │   ├── Classroom.cs
│   │   ├── Role.cs
│   │   ├── Student.cs
│   │   └── Teacher.cs
│   ├── Profiles
│   │   └── MapperProfile.cs
│   ├── Program.cs
│   ├── Properties
│   │   └── launchSettings.json
│   └── SearchService
│       ├── SeachService.cs
│       └── SearchItem.cs
├── CustomEd.Contracts
│   ├── CustomEd.Announcement.Events
│   │   ├── AnnouncementCreatedEvent.cs
│   │   ├── AnnouncementDeletedEvent.cs
│   │   └── AnnouncementUpdatedEvent.cs
│   ├── CustomEd.Classroom.Events
│   │   ├── ClassroomCreatedEvent.cs
│   │   ├── ClassroomDeletedEvent.cs
│   │   ├── ClassroomUpdatedEvent.cs
│   │   ├── MemberJoinedEvent.cs
│   │   └── MemberLeftEvent.cs
│   ├── CustomEd.Contracts.csproj
│   ├── CustomEd.Contracts.sln
│   ├── CustomEd.Notification.Events
│   │   ├── NotifyClassroomEvent.cs
│   │   └── NotifyUserEvent.cs
│   ├── CustomEd.Otp.Events
│   │   └── UserVerifiedEvent.cs
│   └── CustomEd.User.Events
│       ├── OtpGeneratedEvent.cs
│       ├── Student.Events
│       │   ├── StudentCreatedEvent.cs
│       │   ├── StudentDeletedEvent.cs
│       │   └── StudentUpdatedEvent.cs
│       ├── Teacher.Events
│       │   ├── TeacherCreatedEvent.cs
│       │   ├── TeacherDeletedEvent.cs
│       │   └── TeacherUpdatedEvent.cs
│       └── UnverifiedUser.cs
├── CustomEd.Forum.Service
│   ├── Consumers
│   │   ├── ClassroomEvents
│   │   │   ├── ClassroomCreatedEventConsumer.cs
│   │   │   ├── ClassroomDeletedEventConsumer.cs
│   │   │   ├── ClassroomUpdatedEventConsumer.cs
│   │   │   ├── MemberJoinedEventConsumer.cs
│   │   │   └── MemberLeftEventConsumer.cs
│   │   └── UserEvents
│   │       ├── StudentEvents
│   │       │   ├── StudentCreatedEventConsumer.cs
│   │       │   ├── StudentDeletedEventConsumer.cs
│   │       │   └── StudentUpdatedEventConsumer.cs
│   │       └── TeacherEvents
│   │           ├── TeacherCreatedEventConsumer.cs
│   │           ├── TeacherDeletedEventConsumer.cs
│   │           └── TeacherUpdatedEventConsumer.cs
│   ├── Controllers
│   │   └── ForumController.cs
│   ├── CustomEd.Forum.Service.csproj
│   ├── CustomEd.Forum.Service.sln
│   ├── Dtos
│   │   ├── ClassroomDto.cs
│   │   ├── CreateMessageDto.cs
│   │   ├── MessageDto.cs
│   │   ├── UpdateMessageDto.cs
│   │   ├── UserDto.cs
│   │   └── Validation
│   │       ├── CreateMessageDtoValidator.cs
│   │       └── UpdateMessageDtoValidator.cs
│   ├── Hubs
│   │   ├── ForumHub.cs
│   │   └── Interfaces
│   │       └── IForumClient.cs
│   ├── Model
│   │   ├── Classroom.cs
│   │   ├── Message.cs
│   │   ├── Student.cs
│   │   ├── Teacher.cs
│   │   └── User.cs
│   ├── Policies
│   │   └── MemberOnlyPolicy.cs
│   ├── Profiles
│   │   └── MapperProfile.cs
│   ├── Program.cs
│   ├── Properties
│   │   └── launchSettings.json
│   └── Response
│       └── PaginatedResponse.cs
├── CustomEd.Otp.Service
│   ├── Consumers
│   │   ├── StudentEvents
│   │   │   ├── StudentCreatedEventConsumer.cs
│   │   │   └── StudentDeletedEventConsumer.cs
│   │   ├── TeacherEvents
│   │   │   ├── TeacherCreatedEventConsumer.cs
│   │   │   └── TeacherDeletedEventConsumer.cs
│   │   └── UnverifiedUserEventConsumer.cs
│   ├── Controllers
│   │   └── OtpController.cs
│   ├── CustomEd.Otp.Service.csproj
│   ├── CustomEd.Otp.Service.sln
│   ├── Dtos
│   │   ├── SendOtpDto.cs
│   │   └── VerifyOtpDto.cs
│   ├── EmailService
│   │   ├── EmailService.cs
│   │   ├── EmailSettings.cs
│   │   └── IEmailService.cs
│   ├── Errors
│   │   └── NonUserException.cs
│   ├── Model
│   │   ├── Otp.cs
│   │   ├── User.cs
│   │   └── VerifiedUser.cs
│   ├── OTPService
│   │   ├── IOtpService.cs
│   │   └── OtpService.cs
│   ├── Program.cs
│   └── Properties
│       └── launchSettings.json
├── CustomEd.RTNotification.Service
│   ├── Consumption
│   │   ├── ClassroomEvents
│   │   │   ├── ClassroomCreatedEventConsumer.cs
│   │   │   ├── ClassroomDeletedEventConsumer.cs
│   │   │   ├── ClassroomUpdatedEventConsumer.cs
│   │   │   ├── MemberJoinedEventConsumer.cs
│   │   │   └── MemberLeftEventConsumer.cs
│   │   ├── NotificationEvents
│   │   │   ├── NotifyClassroomEventConsumer.cs
│   │   │   └── NotifyUserEventConsumer.cs
│   │   └── UserEvents
│   │       ├── StudentEvents
│   │       │   ├── StudentCreatedEventConsumer.cs
│   │       │   ├── StudentDeletedEventConsumer.cs
│   │       │   └── StudentUpdatedEventConsumer.cs
│   │       └── TeacherEvents
│   │           ├── TeacherCreatedEventConsumer.cs
│   │           ├── TeacherDeletedEventConsumer.cs
│   │           └── TeacherUpdatedEventConsumer.cs
│   ├── Controllers
│   │   └── NotificationController.cs
│   ├── CustomEd.RTNotification.Service.csproj
│   ├── CustomEd.RTNotification.Service.sln
│   ├── Dtos
│   │   └── NotificationDto.cs
│   ├── Hubs
│   │   ├── Interfaces
│   │   │   └── INotificationClient.cs
│   │   └── NotificationHub.cs
│   ├── Model
│   │   ├── Classroom.cs
│   │   ├── Notification.cs
│   │   └── User.cs
│   ├── Profiles
│   │   └── MapperProfile.cs
│   ├── Program.cs
│   └── Properties
│       └── launchSettings.json
├── CustomEd.Shared
│   ├── CustomEd.Shared.csproj
│   ├── CustomEd.Shared.sln
│   ├── Data
│   │   ├── Extensions.cs
│   │   ├── GenericRepository.cs
│   │   └── Interfaces
│   │       └── IGenericRepository.cs
│   ├── Entity
│   │   ├── BaseEntity.cs
│   │   ├── Department.cs
│   │   ├── NotificationType.cs
│   │   └── Role.cs
│   ├── JWT
│   │   ├── Contracts
│   │   │   ├── Role.cs
│   │   │   └── UserDto.cs
│   │   ├── IdentityProvider.cs
│   │   ├── Interfaces
│   │   │   └── IJwtService.cs
│   │   └── JwtService.cs
│   ├── RabbitMQ
│   │   └── Extensions.cs
│   ├── Response
│   │   └── SharedResponse.cs
│   └── Settings
│       ├── JWT
│       │   ├── Extensions.cs
│       │   └── JWTSettings.cs
│       ├── MongoSettings
│       │   └── MongoSettings.cs
│       ├── RabbitMQSettings
│       │   └── RabbitMQSettings.cs
│       └── ServiceSettings
│           └── ServiceSettings.cs
├── CustomEd.sln
├── CustomEd.User.Service
│   └── src
│       ├── Clients
│       │   └── HttpClient.cs
│       ├── Consumers
│       │   └── UserVerifiedEventConsumer.cs
│       ├── Controllers
│       │   ├── AdminController.cs
│       │   ├── StudentController.cs
│       │   ├── TeacherController.cs
│       │   └── UserController.cs
│       ├── DBSynceService
│       │   └── DBSyncService.cs
│       ├── DTOs
│       │   ├── LoginRequestDto.cs
│       │   ├── SchoolResponseDTO.cs
│       │   ├── SendMailDto.cs
│       │   ├── Student
│       │   │   ├── CreateStudentDto.cs
│       │   │   ├── StudentDto.cs
│       │   │   ├── UpdateStudentDto.cs
│       │   │   └── Validation
│       │   │       ├── CreateStudentDtoValidator.cs
│       │   │       └── UpdateStudentDtoValidator.cs
│       │   ├── Teacher
│       │   │   ├── CreateTeacherDto.cs
│       │   │   ├── TeacherDto.cs
│       │   │   ├── UpdateTeacherDto.cs
│       │   │   └── Validation
│       │   │       ├── CreateTeacherDtoValidator.cs
│       │   │       └── UpdateTeacherDtoValidator.cs
│       │   ├── UserDto.cs
│       │   └── VerifyUserDto.cs
│       ├── EmailService
│       │   ├── EmailService.cs
│       │   ├── EmailSettings.cs
│       │   └── IEmailService.cs
│       ├── Model
│       │   ├── Admin.cs
│       │   ├── Department.cs
│       │   ├── Otp.cs
│       │   ├── Role.cs
│       │   ├── Student.cs
│       │   ├── Teacher.cs
│       │   └── User.cs
│       ├── PasswordService
│       │   ├── Interfaces
│       │   │   └── IPasswordHasher.cs
│       │   └── PasswordHasher.cs
│       ├── Profiles
│       │   └── MapperProfile.cs
│       ├── Program.cs
│       ├── Properties
│       │   └── launchSettings.json
│       ├── Settings
│       │   └── PasswordHasherSettings.cs
│       ├── src.csproj
│       └── src.sln
├── docker-compose.yml
├── FlexEd.csproj
├── FlexEd.sln
├── folder-structure.md
├── ISchoolDatabase
│   ├── Controllers
│   │   └── ISchoolDatabase.cs
│   ├── DTO
│   │   └── SchoolResponseDTO.cs
│   ├── ISchoolDatabase.csproj
│   ├── ISchoolDatabase.sln
│   ├── Models
│   │   ├── Student.cs
│   │   └── Teacher.cs
│   ├── Program.cs
│   ├── Properties
│   │   └── launchSettings.json
│   └── SchoolResponse.cs
├── Packages
└── README.md
```
