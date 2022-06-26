using EventsServer.Models;
using EventsServer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Auth.Common;
using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace EventsServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IOptions<AuthOptions> _authOptions;

        public UsersController(IUserRepository userRepository, IOptions<AuthOptions> authOptions)
        {
            _userRepository = userRepository;
            _authOptions = authOptions;
        }

        [HttpGet]
        public async Task<ActionResult> GetUsers()
        {
            try
            {
                return Ok(await _userRepository.GetUsers());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            try
            {
                var result = await _userRepository.GetUser(id);

                if (result == null)
                {
                    return NotFound("User not found!");
                }

                return result;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<User>> Register(User user) // AddUser
        {
            try
            {
                if (user == null)
                {
                    return BadRequest("Invalid form");
                }

                var user_t = await _userRepository.GetUserByEmail(user.Email); // Проверка почты на уникальность

                if (user_t != null) // Если пользователь с такой почтой есть
                {
                    return BadRequest("User with this email already registered!");
                }

                var createdEvent = await _userRepository.AddUser(user);

                return CreatedAtAction(nameof(GetUser), new { id = createdEvent.Id },
                    createdEvent);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating event " + ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateEmployee(Guid id, User new_user)
        {
            try
            {
                if (id != new_user.Id)
                    return BadRequest("User ID mismatch");

                var userToUpdate = await _userRepository.GetUser(id);

                if (userToUpdate == null)
                    return NotFound($"User with Id = {id} not found");

                return await _userRepository.UpdateUser(new_user);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(Guid id)
        {
            try
            {
                var userToDelete = await _userRepository.GetUser(id);

                if (userToDelete == null)
                {
                    return NotFound($"User with Id = {id} not found");
                }

                return await _userRepository.DeleteUser(id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }

        [Route("login")]
        [HttpPost]
        public async Task<ActionResult<User>> Login([FromBody] Login request)
        {
           try
            {
                var user = await _userRepository.GetUserByEmail(request.Email); // Поиск по почте

                if (user == null) // Если не найден
                {
                    return NotFound("User don't finded!");
                }

                bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.Password); // Сверяем пароль

                if (!isValidPassword) // Если не верный пароль
                {
                    return Unauthorized("Invalid password!");
                }

                var token = GenerateJWT(user); // Генерация токена

                return Ok(new
                {
                    access_token = token
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }

        [HttpPut]
        [Route("set_user_role/{id}")]
        public async Task<ActionResult<User>> SetUserRole(Guid id, [FromBody]Role[] roles)
        {
            try
            {
                var userToUpdate = await _userRepository.GetUser(id);

                if (userToUpdate == null)
                    return NotFound($"User with Id = {id} not found");

                return await _userRepository.SetUserRole(id, roles);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }

        //---------------------------------------------------

        private string GenerateJWT(User user)
        {
            var authParams = _authOptions.Value;

            var securityKey = authParams.GetSymmetricSecurityKey();
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>() // Claim - некое утверждение об объекте
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim("role", role.ToString())); // добавление инфы о ролях пользователя (для этого создали свой Claim)
            }

            var token = new JwtSecurityToken(authParams.Issuer,
                authParams.Audience,
                claims,
                expires: DateTime.Now.AddSeconds(authParams.TokenLifetime),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token); // передаем пользователю в респонсе
        }
    }
}