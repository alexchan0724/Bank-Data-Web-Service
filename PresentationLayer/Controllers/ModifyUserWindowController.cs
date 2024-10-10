using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using RestSharp;
using Newtonsoft.Json;
using API_Classes;


namespace PresentationLayer.Controllers
{
    public class ModifyUserWindowController : Controller
    {
        RestClient restClient = new RestClient("http://localhost:5290");

        [HttpGet]
        public IActionResult ModifyUserWindow()
        {
            return View();
        }
        [Route("Home/ModifyUserWindow")]
        [HttpPost]
        public IActionResult Post(string Username, string Email, string Address, string PhoneNo, string UserPassword, IFormFile UserImage)
        {

            Debug.WriteLine("@@@ " + Username +  Email +  Address);
            UserDataIntermed userProfile = new UserDataIntermed();
            if (UserImage.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    UserImage.CopyTo(ms);
                    byte[] fileBytes = ms.ToArray();
                    string s = Convert.ToBase64String(fileBytes);
                    // act on the Base64 data
                    Debug.WriteLine("VVVVVVVVV");
                    userProfile.profilePicture = fileBytes;

                }
            }
            userProfile.username = Username;
            userProfile.email = Email;
            userProfile.address = Address;
            userProfile.password = UserPassword;
            userProfile.phoneNum = PhoneNo;
            userProfile.isAdmin = 0;

            Debug.WriteLine("@$$$$@@ " + userProfile.username + userProfile.email + userProfile.address + userProfile.password + userProfile.phoneNum + userProfile.isAdmin);


            RestRequest request = new RestRequest("api/B_userprofiles", Method.Post);
            request.AddJsonBody(userProfile);
            RestResponse response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                Debug.WriteLine("ACCOUNT SUCCESSFULLY MADE");
            }
            else
            {
                Debug.WriteLine("SSS " + response.Content.ToString() + response.StatusCode);
            }
            return RedirectToAction("Index", "Home"); // Redirecting to the Home action

        }
        [HttpPost]
        public IActionResult Put(string oldUserName, string oldEmail, string Username, string Email, string Address, string PhoneNo, string UserPassword, IFormFile UserImage)
        {

            Debug.WriteLine("@@@ " + Username + Email + Address);
            UserDataIntermed userProfile = new UserDataIntermed();
            if (UserImage.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    UserImage.CopyTo(ms);
                    byte[] fileBytes = ms.ToArray();
                    string s = Convert.ToBase64String(fileBytes);
                    // act on the Base64 data
                    Debug.WriteLine("VVVVVVVVV");
                    userProfile.profilePicture = fileBytes;

                }
            }
            userProfile.username = Username;
            userProfile.email = Email;
            userProfile.address = Address;
            userProfile.password = UserPassword;
            userProfile.phoneNum = PhoneNo;
            userProfile.isAdmin = 0;

            Debug.WriteLine("@$$$$@@ " + userProfile.username + userProfile.email + userProfile.address + userProfile.password + userProfile.phoneNum + userProfile.isAdmin);


            RestRequest request = new RestRequest("api/B_userprofiles", Method.Put);
            request.AddQueryParameter("oldUsername", oldUserName); // Add the old username and email to the request (To find the user to update)
            request.AddQueryParameter("oldEmail", oldEmail);
            request.AddJsonBody(userProfile);
            RestResponse response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                Debug.WriteLine("ACCOUNT SUCCESSFULLY MADE");
            }
            else
            {
                Debug.WriteLine("SSS " + response.Content.ToString() + response.StatusCode);
            }
            return RedirectToAction("Index", "Home"); // Redirecting to the Home action

        }
    }
}
