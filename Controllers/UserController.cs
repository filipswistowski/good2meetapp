using activitiesapp.Helpers;
using activitiesapp.Models;
using activitiesapp.Models.UserDTO;
using activitiesapp.Repositories.Interfaces;
using activitiesapp.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace activitiesapp.Controllers
{
    /// <summary>
    ///
    /// </summary>
    //[Authorize]
    [ApiController]
    [Route("/user")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public UserController(IUserRepository userRepository, IMapper mapper, LinkGenerator linkGenerator, IUserService userService, IEmailService emailService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
            _userService = userService;
            _emailService = emailService;
        }

        /// <summary>
        /// Returns all users in database
        /// </summary>
        /// <response code="200">Returns list of users</response>
        /// <response code="500">If there is a server error</response>  
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<ActionResult<UserFullDTO[]>> Get()
        {
            try
            {
                var userList = await _userRepository.GetAllUsersAsync();
                Response.Headers.Add("Content-Security-Policy", "default-src 'self', img-src 'none'");

                return Ok(_mapper.Map<UserFullDTO[]>(userList));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        /// <summary>
        /// Returns users of given id
        /// </summary>
        /// <param name="UserId">Id of the user we want to get</param>
        /// <response code="200">Returns user by given id</response>
        /// <response code="404">If there is no user with given id</response>  
        /// <response code="500">If there is a server error</response>  
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{UserId:int}")]
        public async Task<ActionResult<UserFullDTO>> GetUserById(int UserId)
        {
            {
                try
                {
                    var user = await _userRepository.GetUserByIdAsync(UserId);

                    if (user == null) return NotFound();
                    return Ok(_mapper.Map<UserFullDTO>(user));
                }
                catch (Exception)
                {
                    return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
                }
            }
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <response code="201">Creates user with given parameters</response>
        /// <response code="400">Invalid input</response>  
        /// <response code="500">If there is a server error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<UserFullDTO>> Post(UserFullDTO userFullDto)
        {
            var user = _mapper.Map<User>(userFullDto);
            var usersList = _userRepository.GetAllUsersAsync().Result;
            var user1 = usersList.Where(u => u.Name == user.Name).Select(u => u).ToList();

            if (user1.Count == 0)
            {
                string salt = _userService.CreateSalt();
                user.Salt = salt;
                user.Password = _userService.HashPassword(user.Password, salt);

                _userRepository.CreateUser(user);
                await _userRepository.SaveChangeAsync();

                MailModel mailmodel = new MailModel();
                mailmodel.From = "good2meet.info@gmail.com";
                mailmodel.To = userFullDto.Email;
                mailmodel.Subject = "Welcome to good2meet";
                mailmodel.Body = "Welcome to good2meet - the app you have been waiting for! Enjoy your stay and have fun!";
                _emailService.SendEmail(mailmodel);

                return StatusCode(StatusCodes.Status201Created, new { message = "Account Created Succesfuly" });
            }

            return Ok();
        }

        /// <summary>
        /// User login authentication
        /// </summary>
        /// <response code="200">Succesfull authentication</response>
        /// <response code="400">Invalid input</response>  
        /// <response code="500">If there is a server error</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult<User>> Authenticate(UserLoginDTO userLoginDto)
        {
            var user = await _userService.Authenticate(userLoginDto.Email, userLoginDto.Password);

            if (user == null)
                return BadRequest(new { message = "Email or password is incorrect" });

            return Ok(_mapper.Map<UserFullDTO>(user));
        }

        /// <summary>
        /// Changes name of user by given id
        /// </summary>
        /// <param name="EventId">Id of the user we want to change name</param>
        /// <response code="200">Succesfully changed name</response>
        /// <response code="400">Invalid input</response>  
        /// <response code="500">If there is a server error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch("{UserId:int}")]
        public async Task<ActionResult<UserUpdateDTO>> Put(int UserId, UserUpdateDTO userUpdateDto)
        {
            try
            {
                var oldUser = await _userRepository.GetUserByIdAsync(UserId);
                if (oldUser == null) return NotFound($"Could not find user with id of {UserId}");

                _mapper.Map(userUpdateDto, oldUser);

                if (await _userRepository.SaveChangeAsync())
                {
                    return _mapper.Map<UserUpdateDTO>(oldUser);
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }

        /// <summary>
        /// Deletes user with given ID
        /// </summary>
        /// <param name="UserId">Id of the user we want to delete</param>
        /// <response code="200">Succesfully removed user</response>
        /// <response code="400">Invalid input</response>  
        /// <response code="500">If there is a server error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{UserId:int}")]
        public async Task<ActionResult<UserFullDTO>> Delete(int UserId)
        {
            try
            {
                var oldUser = await _userRepository.GetUserByIdAsync(UserId);
                if (oldUser == null) return NotFound($"Could not find user with id of {UserId}");

                _userRepository.Delete(oldUser);

                if (await _userRepository.SaveChangeAsync())
                {
                    return Ok();
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }
    }
}