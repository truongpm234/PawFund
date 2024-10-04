using MyWebApp1.Data;
using MyWebApp1.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyWebApp1.Services
{
    public class UserService
    {
        private readonly MyDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public UserService(MyDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public string Register(UserDTO userDTO)
        {
            var objUser = _dbContext.Users.FirstOrDefault(x => x.Email == userDTO.Email);
            if (objUser != null)
            {
                throw new Exception("User already exists with same email.");
            }

            var newUser = new User
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Email = userDTO.Email,
                Password = userDTO.Password,
            };

            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            return "User registered successfully.";
        }

        public string Login(LoginDTO loginDTO)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Email == loginDTO.Email && x.Password == loginDTO.Password); // Băm mật khẩu tại đây
            if (user == null)
            {
                throw new Exception("Invalid credentials.");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.UserId.ToString()),
                new Claim("Email", user.Email.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: signIn
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public List<User> GetUsers()
        {
            return _dbContext.Users.ToList();
        }

        public User GetUser(int id)
        {
            return _dbContext.Users.FirstOrDefault(x => x.UserId == id);
        }

        public string UpdateProfile(int userId, UserDTO userDTO)
        {
            var userToUpdate = _dbContext.Users.FirstOrDefault(x => x.UserId == userId);
            if (userToUpdate == null)
            {
                throw new Exception("User not found.");
            }

            userToUpdate.FirstName = userDTO.FirstName;
            userToUpdate.LastName = userDTO.LastName;
            userToUpdate.Email = userDTO.Email;

            // Update password
            if (!string.IsNullOrEmpty(userDTO.Password))
            {
                userToUpdate.Password = userDTO.Password; // Sử dụng băm mật khẩu tại đây
            }

            _dbContext.Users.Update(userToUpdate);
            _dbContext.SaveChanges();

            return "User profile update success.";
        }
    }
}
