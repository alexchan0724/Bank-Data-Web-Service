using API_Classes;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Diagnostics;

namespace PresentationLayer.Controllers
{
    public class ModifyAccountWindowController : Controller
    {
        [HttpPost]
        public IActionResult addAccount(string AccNum, string Pin, string description, string username, string email)
        {
            RestRequest request;
            try
            {
                // Create a BankAccount object with form data
                BankDataIntermed bankAccount = new BankDataIntermed();
                bankAccount.accountNumber = Convert.ToInt32(AccNum);
                bankAccount.pin = Convert.ToInt32(Pin);
                bankAccount.description = description;

                // Ensuring that user entered fields are not too long
                //if (bankAccount.description.Length > 100)
                //{
                //    MessageBox.Show("Ensure that description is under 100 characters");
                //    return;
                //}

                //// Check if the account number is exactly 6 digits long
                //if (AccountBox.Text.Length != 6)
                //{
                //    MessageBox.Show("Account number must be exactly 6 digits long.");
                //    return;
                //}

                //// Check if the PIN is not over 8 digits long
                //if (PinBox.Text.Length > 8 && PinBox.Text.Length == 0)
                //{
                //    MessageBox.Show("PIN must be 8 digits or less.");
                //    return;
                //}

                if (addAccount) // Add account (POST)
                {
                    bankAccount.balance = 0;
                    bankAccount.email = email;
                    bankAccount.username = username;
                    request = new RestRequest("api/B_BankAccounts", Method.Post);
                }
                else // Modify account (PUT)
                {
                    request = new RestRequest("api/B_BankAccounts", Method.Put);
                    request.AddQueryParameter("accountNumber", returnBankAccount.accountNumber);
                    Debug.WriteLine("@@@@@@@@@@@@@@@@@@@");
                }

                // Set the request format to JSON and add the bank account as the body
                request.RequestFormat = RestSharp.DataFormat.Json;
                request.AddJsonBody(bankAccount);
                RestResponse response = restClient.Execute(request);

                // Handle the response and display a message box accordingly
                if (response.IsSuccessful)
                {
                    if (addAccount)
                    {
                        MessageBox.Show("Bank Account added successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Bank Account updated successfully!");
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    MessageBox.Show("Error: " + response.Content); // Display the error message from the Controller
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    MessageBox.Show("Error: " + response.Content); // Display the error message from the Controller
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Ensure that the account number and PIN are numbers.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
    }
}
