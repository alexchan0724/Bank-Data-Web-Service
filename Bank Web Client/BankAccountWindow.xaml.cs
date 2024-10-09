using API_Classes;
using RestSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace Bank_Web_Client
{
    /// <summary>
    /// Interaction logic for BankAccountWindow.xaml
    /// </summary>
    public partial class BankAccountWindow : Window
    {
        private UserDataIntermed userProfile;
        private RestClient restClient;
        private BankDataIntermed bankAccount;
        private int trackBalance;

        public BankAccountWindow(UserDataIntermed pUserProfile, BankDataIntermed pBankAccount)
        {
            InitializeComponent();
            userProfile = pUserProfile;
            bankAccount = pBankAccount;
            trackBalance = bankAccount.balance;

            string URL = "http://localhost:5290";
            restClient = new RestClient(URL);

            Title = "Bank Details";
            InitialiseBankInformation();
        }

        private void InitialiseBankInformation()
        {
            AccountNumTextBlock.Text = "Account Number: " + bankAccount.accountNumber;
            BalanceTextBlock.Text = "Balance: " + bankAccount.balance;
            PinTextBlock.Text = "Pin: " + bankAccount.pin;
            DescriptionTextBlock.Text = "Description: " + bankAccount.description;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            RestResponse response = null;
            try
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete your account?", "Delete Account", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    string accountNumber = bankAccount.accountNumber.ToString();
                    RestRequest request = new RestRequest($"api/B_BankAccounts/{accountNumber}", Method.Delete);
                    response = restClient.Execute(request);
                    MessageBox.Show("Bank account deleted.");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) // If the account was successfully deleted
                    {
                        UserProfileWindow userProfileWindow = new UserProfileWindow(userProfile);
                        userProfileWindow.Show();
                        this.Close();
                    }
                }
                if (response != null && response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    MessageBox.Show("Error: " + response.Content); // Display the error message from the Controller
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting account: " + ex.Message);
            }
        }
        private void ModifyButton_Click(object sender, RoutedEventArgs e)
        {
            ModifyAccountWindow modifyAccountWindow = new ModifyAccountWindow(userProfile, bankAccount, false);
            modifyAccountWindow.Show();
            this.Close();
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            UserProfileWindow userProfileWindow = new UserProfileWindow(userProfile);
            userProfileWindow.Show();
            this.Close();
        }

        private void TransactionButton_Click(object sender, RoutedEventArgs e)
        {
            RestRequest request;

            string transactionDescription= Microsoft.VisualBasic.Interaction.InputBox("Transaction Description:", "Retrieve Transaction Details", "");
            if (int.TryParse(AmountTextBox.Text, out int valueToDeposit))
            {
                Debug.WriteLine("Parsing transaction was successful.");
                DateTime date = DateTime.Now;
                TransactionDataIntermed newTransaction = new TransactionDataIntermed(bankAccount.accountNumber, transactionDescription, valueToDeposit, date);
                if (valueToDeposit > 0)
                {
                    request = new RestRequest($"api/B_Transactions/Deposit", Method.Post);
                    request.RequestFormat = RestSharp.DataFormat.Json;
                    request.AddJsonBody(newTransaction);
                    RestResponse response = restClient.Execute(request);

                    if (response.IsSuccessful)
                    {
                        MessageBox.Show("Deposit into account was successful.");
                        trackBalance = trackBalance + valueToDeposit;
                        BalanceTextBlock.Text = "Balance: " + (trackBalance);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        MessageBox.Show("Deposit failed.");
                    }
                    else
                    {
                        MessageBox.Show("Error depositing into account.");
                    }
                }
                else if (valueToDeposit < 0)
                {
                    request = new RestRequest($"api/B_Transactions/Withdraw", Method.Post);
                    request.RequestFormat = RestSharp.DataFormat.Json;
                    request.AddJsonBody(newTransaction);
                    RestResponse response = restClient.Execute(request);

                    if (response.IsSuccessful) {
                        MessageBox.Show("Withdrawal from account was successful.");
                        trackBalance = trackBalance + valueToDeposit; // valueToDeposit is negative hence the addition
                        BalanceTextBlock.Text = "Balance: " + (trackBalance);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        MessageBox.Show("Withdrawal failed.");
                    }
                    else
                    {
                        MessageBox.Show("Error withdrawing from account.");
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a non-zero value for a transaction.");
                }
            }
            else
            {
                MessageBox.Show("Please enter an integer value for a transaciton.");
            }
        }
    }
}
