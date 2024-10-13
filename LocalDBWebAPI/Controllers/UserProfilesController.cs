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
        [HttpPost("{checkString}")] // checkString passed as parameter can be either email or username
        public IActionResult Get(string checkString, [FromBody] UserDataIntermed pUser) 
        {

            if (string.IsNullOrEmpty(pUser.password))
            {
                DBManager.AddLogEntry("getaccount failed, password is required");
                return BadRequest("Password is required.");
            }

            var userProfile = DBManager.GetUserProfile(checkString, pUser.password);

            if (userProfile == null)
            {
                DBManager.AddLogEntry("getaccount failed, input do not match any email or username in database");
                return NotFound("Parameter does not match any emails or usernames in the database.");
            }
            else if (userProfile.password != pUser.password)
            {
                DBManager.AddLogEntry("getaccount failed, password do not match");

                return NotFound("Password does not match.");
            }
            else
            {
                DBManager.AddLogEntry("getAccount successful, returning user");
                Debug.WriteLine($"Retrieved user. IsAdmin: {userProfile.isAdmin}, Phone: {userProfile.phoneNum}");
                return Ok(userProfile);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] UserDataIntermed userProfile)
        {
            Debug.WriteLine("DDDDDDDDDDDD " + userProfile.username + userProfile.phoneNum + userProfile.address + userProfile.email + userProfile.password + " @@@ " + userProfile.isAdmin);


            if (userProfile == null)
            {
                DBManager.AddLogEntry("createAccount failed, user profile is required");

                return BadRequest("User profile is required.");
            }

            if (string.IsNullOrEmpty(userProfile.email) || string.IsNullOrEmpty(userProfile.username) || string.IsNullOrEmpty(userProfile.password))
            {
                DBManager.AddLogEntry("createAccount failed, email, username and/or password are required");

                return BadRequest("Email, username, and password are required.");
            }

            if (DBManager.InsertUserProfile(userProfile))
            {
                DBManager.AddLogEntry("createAccount successful");

                return Ok("User profile added.");
            }
            DBManager.AddLogEntry("createAccount failed, user with same username and email already exist");

            return BadRequest("User profile with the same email or username already exists.");
        }

        [HttpDelete]
        [Route("{checkString}")] // checkString passed as parameter can be either email or username
        public IActionResult Delete(string checkString)
        {
            if (DBManager.DeleteUserProfile(checkString))
            {
                DBManager.AddLogEntry("deleteProfile successful");

                return Ok("User profile deleted.");
            }
            DBManager.AddLogEntry("deleteProfile failed, user profile with email/username " + checkString + "do not exist in database");

            return NotFound("User profile not found.");
        }

        [HttpPut]
        public IActionResult Put([FromBody] UserDataIntermed userProfile, [FromQuery]string oldUsername, [FromQuery]string oldEmail)
        {
            if (userProfile == null)
            {
                DBManager.AddLogEntry("updateProfile failed, userProfile is required");

                return BadRequest("User profile is required.");
            }
            Debug.WriteLine("UserProfilesController.Put: " + userProfile.username + " " + userProfile.email + " " + oldUsername + " " + oldEmail);
            if (DBManager.UpdateUserProfile(userProfile, oldUsername, oldEmail))
            {
                DBManager.AddLogEntry("updateProfile successful");

                return Ok("User profile updated.");
            }
            DBManager.AddLogEntry("updateProfile failed");

            return BadRequest("Could not update UserProfile.");
        }
    }
}
