using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using MoneyHeap.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MoneyHeap.Controllers
{
    [ApiController]
    [Route("/moneyheap")]
    public class HomeController : Controller
    {   
        private readonly ISqlDatabase sqlDatabase = null;
        SqlConnection conn = null;
        private IConfiguration _configuration = null;
        public HomeController(IConfiguration configuration,  ISqlDatabase _sqlDatabase)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            conn = new SqlConnection(_configuration["ConnectionStrings:DatabaseServer"]);
        }
        [HttpPost("/login")]
        public IActionResult checkCredential([FromBody] LoginUser loginUser)
        {
            Console.WriteLine(loginUser.email + "    --     " + loginUser.password);

            User? user = Authenticate(loginUser);
            if(user != null)
            {  
                var token = Generate(user);
                
                var tokenn = token?.GetType().GetProperty("Value")?.GetValue(token, null);
                HttpContext.Response.Headers.Add("token", tokenn.ToString());
                return Ok(true);
            }
            return NotFound(false);
    
        }

        private object Generate(User user)
        {
            var securityKey =  new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim(ClaimTypes.Sid, user.Id.ToString()));
            claimsForToken.Add(new Claim(ClaimTypes.Email, user.Email));
            claimsForToken.Add(new Claim(ClaimTypes.Name, user.Name));
            claimsForToken.Add(new Claim(ClaimTypes.Role, user.Role));
            
           

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            var tokenToReturn = new JwtSecurityTokenHandler()
               .WriteToken(jwtSecurityToken);

            return Ok(tokenToReturn);
        }

        private User? Authenticate(LoginUser loginUser)
        {
            User? user = null;
            conn.Open();
            try
            {
                String query = "Select name,email,role,Id from userInfo where email = '" +
                     loginUser.email + "' and password = '" + loginUser.password + "' and role = '" +loginUser.role+"'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    Console.WriteLine(query);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                    
                            User usr = new User();
                            usr.Name = reader.GetString(0);
                            usr.Email = reader.GetString(1);
                            usr.Role = reader.GetString(2);
                            usr.Id = reader.GetInt32(3);
                            user = usr; 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                conn.Close();
                return user;
            }
            conn.Close();
            if(user != null)
               return user;
            return user;

        }
        private User? GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            Console.WriteLine(identity);
            if (identity != null)
            {
                var userClaims = identity.Claims;
                var Name = userClaims.FirstOrDefault(v => v.Type == ClaimTypes.Name)?.Value;
                var Email = userClaims.FirstOrDefault(v => v.Type == ClaimTypes.Email)?.Value;
                var Role = userClaims.FirstOrDefault(v => v.Type == ClaimTypes.Actor)?.Value;
                var Id =  userClaims.FirstOrDefault(v => v.Type == ClaimTypes.Sid)?.Value;
                Console.WriteLine(Name+ " " + Email +" "+ Role);
                User usr = new User();
                usr.Name = Name;
                usr.Email = Email;
                usr.Role = Role;
                usr.Id = int.Parse(Id);
                return usr;

            }
            return null;
        }

        [HttpPost("/signup")]
        public IActionResult createUser([FromBody] SignupUser signupUser)
        {
            
            Console.WriteLine( "Signup :  "+signupUser.name + " " + signupUser.email + "  " + signupUser.password);
            





            try
            {
                conn.Open();
                if (signupUser.role == "Admin") {

                    string sql = "SELECT COUNT(*) FROM AdminUser where email = '" + signupUser.email+"'";
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        int count = (Int32)command.ExecuteScalar();
                        if(count == 0)
                        {
                            return BadRequest("invalid user for Admin");
                        }

                    }

                }


                String query = "INSERT INTO userInfo (name, email, password,role) " +
                             " VALUES('" + signupUser.name + "','" + signupUser.email + "','" + signupUser.password + "','" + signupUser.role + "')";
                Console.WriteLine(query);


                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                conn.Close();
                return BadRequest(false);   
            }
            conn.Close();
            return Ok(true); 
        }

      //  [Authorize(Roles = "Admin")]
        [HttpGet("/count")]
        public IActionResult getCount()
        {
            var currentUser = GetCurrentUser();
            
            return Ok(JsonConvert.SerializeObject(new { name = currentUser.Name}));
        }

    }
}
