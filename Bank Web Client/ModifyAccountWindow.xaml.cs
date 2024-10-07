using API_Classes;
using Newtonsoft.Json.Converters;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
namespace Bank_Web_Client
{
    /// <summary>
    /// Interaction logic for ModifyAccountWindow.xaml
    /// </summary>
    public partial class ModifyAccountWindow : Window
    {
        private RestClient restClient;
        private bool addAccount; // True if adding an account, false if modifying an account
        private UserDataIntermed userProfile;
        private BankDataIntermed returnBankAccount;
        private BankDataIntermed bankAccount;

        public ModifyAccountWindow(UserDataIntermed pUserProfile, BankDataIntermed pBankAccount, bool pAddAccount)
        {
            InitializeComponent();
            userProfile = pUserProfile;
            addAccount = pAddAccount;
            returnBankAccount = pBankAccount;

            string URL = "http://localhost:5290";
            restClient = new RestClient(URL);

            Title = addAccount ? "Add Account" : "Modify Account";

            if (addAccount) // Changes the text of the button based on whether the user is adding or modifying an account
            {
                ActionButton.Content = "Add";
                BalanceTextBlock.Text = "Balance: 0";
            }
            else
            {
                ActionButton.Content = "Modify";
                BalanceTextBlock.Text = "Balance: " + returnBankAccount.balance.ToString();
            } 
        }

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            RestRequest request;
            try
            {
                // Create a BankAccount object with form data
                bankAccount = new BankDataIntermed();
                bankAccount.accountNumber = Convert.ToInt32(AccountBox.Text);
                bankAccount.pin = Convert.ToInt32(PinBox.Text);
                bankAccount.description = DescriptionBox.Text;

                // Ensuring that user entered fields are not too long
                if (bankAccount.description.Length > 100)
                {
                    MessageBox.Show("Ensure that description is under 100 characters");
                    return;
                }

                // Check if the account number is exactly 6 digits long
                if (AccountBox.Text.Length != 6)
                {
                    MessageBox.Show("Account number must be exactly 6 digits long.");
                    return;
                }

                // Check if the PIN is not over 8 digits long
                if (PinBox.Text.Length > 8 && PinBox.Text.Length == 0)
                {
                    MessageBox.Show("PIN must be 8 digits or less.");
                    return;
                }

                if (addAccount) // Add account (POST)
                {
                    bankAccount.balance = 0;
                    bankAccount.email = userProfile.email;
                    bankAccount.username = userProfile.username;
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

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            if (addAccount) // Return to UserProfileWindow
            {
                UserProfileWindow userWindow = new UserProfileWindow(userProfile);
                userWindow.Show();
                this.Close();
            }
            else // Return to BankAccountWindow
            {
                BankAccountWindow bankWindow = new BankAccountWindow(userProfile, bankAccount);
                bankWindow.Show();
                this.Close();
            }
        }

        private BankDataIntermed ConvertBankAccountToIntermed(BankDataIntermed bankAccount)
        {
            BankDataIntermed newBankAccount = new BankDataIntermed();
            newBankAccount.accountNumber = bankAccount.accountNumber;
            newBankAccount.pin = bankAccount.pin;
            newBankAccount.description = bankAccount.description;
            newBankAccount.balance = returnBankAccount.balance;
            newBankAccount.email = userProfile.email;
            newBankAccount.username = userProfile.username;
            return newBankAccount;
        }
    }
}
