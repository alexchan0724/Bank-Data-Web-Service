using API_Classes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Diagnostics;
using System.Xml.Linq;

namespace PresentationLayer.Controllers
{
    [Route("account/[controller]")]

    public class BankAccountController : Controller
    {

        RestClient restClient = new RestClient("http://localhost:5290");

        [HttpGet("receiverAcct/{receiverAcctNum}")] // This method gets receiver bank account but does not return a view 
        public IActionResult GetReceiverBankAccount(int receiverAcctNum)
        {
            Debug.WriteLine("Account number in BankAccountController: " + receiverAcctNum);
            RestRequest getAccountRequest = new RestRequest($"api/B_BankAccounts/{receiverAcctNum}", Method.Get);
            RestResponse accountResponse = restClient.Execute(getAccountRequest);

            if (accountResponse.IsSuccessful)
            {
                Debug.WriteLine("Response was successful GetReceiverBankAccount");
                BankAccountController account = JsonConvert.DeserializeObject<BankAccountController>(accountResponse.Content);
                return Ok(account);
            }
            return NotFound("Account not found");
        }
    }
}
