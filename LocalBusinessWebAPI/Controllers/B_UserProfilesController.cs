using API_Classes;
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

        [HttpGet("{checkString}")] // checkString passed as parameter can be either email or username
        public IActionResult Get(string checkString, [FromQuery] string password)
        {
            var request = new RestRequest("user/UserProfiles/{checkString}", Method.Get); // GET: user/UserProfiles/{checkString}?password={password}
            request.AddUrlSegment("checkString", checkString);
            request.AddParameter("password", password);

            var response = restClient.Execute(request);

            if (response.IsSuccessful)
            {
                var result = JsonConvert.DeserializeObject<UserDataIntermed>(response.Content);

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
        public IActionResult Put([FromBody] UserDataIntermed userProfile, [FromQuery] string oldUsername, [FromQuery] string oldEmail)
        {
            var request = new RestRequest("user/userprofiles", Method.Put); // GET: user/UserProfiles/{checkString}?password={password}
            request.AddJsonBody(userProfile);
            request.AddQueryParameter("oldUsername", userProfile.username);
            request.AddQueryParameter("oldEmail", userProfile.email);

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
        [Route("{checkString}")] // checkString passed as parameter can be either email or username
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
