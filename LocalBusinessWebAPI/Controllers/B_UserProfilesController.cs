﻿using API_Classes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Diagnostics;

namespace LocalBusinessWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class B_UserProfilesController : Controller
    {
        RestClient restClient = new RestClient("http://localhost:5012");

        [HttpPost("{checkString}")] // checkString passed as parameter can be either email or username
        public IActionResult GetUser(string checkString, [FromBody] UserDataIntermed user)
        {
            var request = new RestRequest("user/UserProfiles/{checkString}", Method.Post); // GET: user/UserProfiles/{checkString}
            request.AddUrlSegment("checkString", checkString);
            request.AddJsonBody(user);

            var response = restClient.Execute(request);

            if (response.IsSuccessful)
            {
                var result = JsonConvert.DeserializeObject<UserDataIntermed>(response.Content);
                Debug.WriteLine($"Raw JSON response: {response.Content}");
                return Ok(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return BadRequest(response.Content);
            }
            else
            {
                return NotFound(response.Content);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] UserDataIntermed userProfile)
        {
            Debug.WriteLine("DDDDDDDDDDDD " + userProfile.username + userProfile.phoneNum + userProfile.address + userProfile.email + userProfile.password + " @@@ " + userProfile.isAdmin);

            var request = new RestRequest("user/userprofiles", Method.Post); // GET: user/UserProfiles/{checkString}?password={password}
            request.AddJsonBody(userProfile);
            var response = restClient.Execute(request);

            if (response.IsSuccessful)
            {
                return Ok(response.Content);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return BadRequest(response.Content);
            }
            else
            {
                return NotFound(response.Content);
            }
        }

        [HttpPut]
        public IActionResult ModifyAccount([FromBody] UserDataIntermed userProfile, [FromQuery] string oldUsername, [FromQuery] string oldEmail)
        {
            Debug.WriteLine(" Old username in ModifyAccount B_UserProfiles " +  oldUsername + " " + oldEmail);
            Debug.WriteLine("DDDDDDDDDDDD " + userProfile.username + userProfile.phoneNum + userProfile.address + userProfile.email + userProfile.password + " @@@ " + userProfile.isAdmin);

            var request = new RestRequest("user/userprofiles", Method.Put);
            request.AddJsonBody(userProfile);
            request.AddQueryParameter("oldUsername", oldUsername);
            request.AddQueryParameter("oldEmail", oldEmail);

            var response = restClient.Execute(request);

            if (response.IsSuccessful)
            {
                return Ok(response.Content);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return BadRequest(response.Content);
            }
            else
            {
                return NotFound(response.Content);
            }
        }

        [HttpDelete]
        [Route("{checkString}")]
        public IActionResult Delete(string checkString)
        {
            RestRequest request = new RestRequest($"user/userprofiles/{checkString}", Method.Delete);
            RestResponse response = restClient.Execute(request);

            if (response.IsSuccessful)
            {
                return Ok(response.Content);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return BadRequest(response.Content);
            }
            else
            {
                return NotFound(response.Content);
            }
        }
    }
}
