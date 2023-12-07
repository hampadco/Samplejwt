
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

[ApiController]
 [Route("api/[controller]/[action]")]
public class CategoryController: ControllerBase
{



    //static value _username password
    private static string _username ;
    private static string _password ;

    //Register
    [HttpPost]
    public IActionResult Register(string username, string password)
    {
        //hash password
        _username = username;
        _password = BCrypt.Net.BCrypt.HashPassword(password);
        //return _password and _username
        return Ok(new {username = _username, password = _password});

    }


    //login
    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        //check username and password
        if (username == _username && BCrypt.Net.BCrypt.Verify(password, _password))
        {
            //create token
            var token = JwtAuthenticationManager.GenerateJWTToken(username);
        
            //return token
            return Ok(token);
        }
        else
        {
            //return error
            return BadRequest("Username or password is incorrect");
        }
    }














    //create statis value list cat
    private static List<Cat> cats = new List<Cat>()
    {
        new Cat(){Id = 1, Name = "Cat1"},
        new Cat(){Id = 2, Name = "Cat2"},
        new Cat(){Id = 3, Name = "Cat3"},
        new Cat(){Id = 4, Name = "Cat4"},
        new Cat(){Id = 5, Name = "Cat5"},
    };

    //get all cat
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(cats);
    }

    //Add new cat
    [HttpPost]
    [Authorize]
    public IActionResult Add(Cat cat)
    {
        cats.Add(cat);
        //message success
        return Ok("Add success");
        
    }

    //update cat
    [HttpPut]
    [Authorize]
    public IActionResult Update(Cat cat)
    {
        var catUpdate = cats.FirstOrDefault(x => x.Id == cat.Id);
        catUpdate.Name = cat.Name;
        //message success
        return Ok("Update success");
    }

    //delete cat
    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult Delete(int id)
    {

        
        var catDelete = cats.FirstOrDefault(x => x.Id == id);
        cats.Remove(catDelete);
        //message success
        return Ok("Delete success");
    }


   
    
}



//create class JwtAuthenticationManager
public static class JwtAuthenticationManager
{
    //create method GenerateJWTToken
    public static string GenerateJWTToken(string username)
    {
        //create key
        var key = Encoding.ASCII.GetBytes("iotguyiotuyuriutyeruityiurytyeruity");
        //create token handler
        var tokenHandler = new JwtSecurityTokenHandler();
        //create token descriptor
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, username),
               
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        //create token
        var token = tokenHandler.CreateToken(tokenDescriptor);
        //return token
        return tokenHandler.WriteToken(token);
    }
}