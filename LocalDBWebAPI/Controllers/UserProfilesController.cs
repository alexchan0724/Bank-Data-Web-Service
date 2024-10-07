using LocalDBWebAPI.Data;
using Microsoft.AspNetCore.Mvc;
using LocalDBWebAPI.Models;
using System.Diagnostics;
using API_Classes;

namespace LocalDBWebAPI.Controllers
{
    [Route("user/[controller]")]
    [ApiController]
    public class UserProfilesController : ControllerBase
    {
        [HttpGet("{checkString}")] // checkString passed as parameter can be either email or username
        public IActionResult Get(string checkString, [FromQuery] string password) 
        {
            if (string.IsNullOrEmpty(password))
            {
                return BadRequest("Password is required.");
            }

            var userProfile = DBManager.GetUserProfile(checkString, password);

            if (userProfile == null)
            {
                return NotFound("Parameter does not match any emails or usernames in the database.");
            }
            else if (userProfile.password != password)
            {
                return NotFound("Password does not match.");
            }
            else
            {
                UserDataIntermed returnUser = new UserDataIntermed();
                returnUser.username = userProfile.username;
                returnUser.email = userProfile.email;
                returnUser.address = userProfile.address;
                returnUser.phoneNum = userProfile.phoneNum;
                returnUser.profilePicture = userProfile.profilePicture;
                returnUser.password = userProfile.password;
                return Ok(returnUser);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] UserDataIntermed userProfile)
        {
            if (userProfile == null)
            {
                return BadRequest("User profile is required.");
            }

            if (string.IsNullOrEmpty(userProfile.email) || string.IsNullOrEmpty(userProfile.username) || string.IsNullOrEmpty(userProfile.password))
            {
                return BadRequest("Email, username, and password are required.");
            }

            if (DBManager.InsertUserProfile(userProfile))
            {
                return Ok("User profile added.");
            }
            return BadRequest("User profile with the same email or username already exists.");
        }

        [HttpDelete]
        [Route("{checkString}")] // checkString passed as parameter can be either email or username
        public IActionResult Delete(string checkString)
        {
            if (DBManager.DeleteUserProfile(checkString))
            {
                return Ok("User profile deleted.");
            }
            return NotFound("User profile not found.");
        }

        [HttpPut]
        public IActionResult Put([FromBody] UserDataIntermed userProfile, [FromQuery]string oldUsername, [FromQuery]string oldEmail)
        {
            if (userProfile == null)
            {
                return BadRequest("User profile is required.");
            }

            if (DBManager.UpdateUserProfile(userProfile, oldUsername, oldEmail))
            {
                return Ok("User profile updated.");
            }
            return BadRequest("Could not update UserProfile.");
        }
    }
}
