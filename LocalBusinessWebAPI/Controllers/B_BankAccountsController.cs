using Microsoft.AspNetCore.Mvc;
using RestSharp;
using API_Classes;
using Newtonsoft.Json;
using System.Diagnostics;

namespace LocalBusinessWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class B_BankAccountsController : Controller
    {
        RestClient restClient = new RestClient("http://localhost:5012");

        [HttpGet("{accountNumber}")]
        public IActionResult Get(int accountNumber, [FromQuery] string username)
        {
            RestRequest request = new RestRequest($"account/BankAccounts/{accountNumber}", Method.Get);
            request.AddUrlSegment("accountNumber", accountNumber);
            request.AddQueryParameter("username", username);
            RestResponse response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                var result = JsonConvert.DeserializeObject<BankDataIntermed>(response.Content);

                return Ok(result);
            }
            return NotFound(response.Content);

        }


        [HttpPost]
        public IActionResult Post([FromBody] BankDataIntermed account)
        {
            RestRequest request = new RestRequest("account/BankAccounts", Method.Post);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddJsonBody(account);
            RestResponse response = restClient.Execute(request);

            if (response.IsSuccessful)
            {
                return Ok(response.Content);
            }
            return BadRequest(response.Content);
        }

        [HttpPut]
        public IActionResult Put([FromBody] BankDataIntermed account, [FromQuery] int accountNumber)
        {
            Debug.WriteLine("FFFFFFFFFFFFFFFFFF");
            RestRequest request = new RestRequest("account/BankAccounts", Method.Put);
            request.AddQueryParameter("accountNumber", accountNumber);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddJsonBody(account);
            RestResponse response = restClient.Execute(request);

            if (response.IsSuccessful)
            {
                return Ok(response.Content);
            }
            return BadRequest(response.Content);
        }

        [HttpDelete]
        [Route("{accountNumber}")]
        public IActionResult Delete(int accountNumber)
        {
            RestRequest request = new RestRequest($"account/BankAccounts/{accountNumber}", Method.Delete);
            RestResponse response = restClient.Execute(request);


            if (response.IsSuccessful)
            {
                return Ok(response.Content);
            }
            return NotFound(response.Content);
        }
    }
}
