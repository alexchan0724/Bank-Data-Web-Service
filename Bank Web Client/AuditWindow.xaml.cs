using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using API_Classes;
using System.Diagnostics;

namespace Bank_Web_Client
{
    /// <summary>
    /// Interaction logic for AuditWindow.xaml
    /// </summary>
    public partial class AuditWindow : Window
    {
        private ObservableCollection<string> auditLogs = new ObservableCollection<string>();
        private RestClient restClient;
        private bool auditAll;
        private UserDataIntermed userProfile;
        public AuditWindow(UserDataIntermed pUserProfile, bool pAuditAll)
        {
            InitializeComponent();

            string URL = "http://localhost:5290";
            restClient = new RestClient(URL);

            auditAll = pAuditAll;
            userProfile = pUserProfile;
            Title = auditAll ? "Audit Log" : "User Audit Log";
            AuditInfoText.Text = Title;

            if (auditAll)
            {
                LoadAudits();
            }
            else
            {
                LoadUserAudits(userProfile.username);
            }

            AuditListView.ItemsSource = auditLogs;
        }

        private void LoadAudits()
        {
            var request = new RestRequest("api/B_Transactions", Method.Get);
            var response = restClient.Execute(request);

            if (response.IsSuccessful)
            {
                var transactions = JsonConvert.DeserializeObject<List<TransactionDataIntermed>>(response.Content);
                Debug.WriteLine("Transactions found in LoadAudits: " + transactions.Count);
                foreach (var transaction in transactions)
                {
                    string messageToAdd = $"Transaction ID: {transaction.transactionID}, Account Number: {transaction.acctNo}, Description: {transaction.transactionDescription}, Amount: {transaction.amount}";
                    auditLogs.Add(messageToAdd);
                }
            }
            else
            {
                MessageBox.Show("Error " + response.StatusCode + ": " + response.Content.ToString());
            }
        }

        private void LoadUserAudits(string username)
        {

            Debug.WriteLine("BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB");

            Debug.WriteLine("Username in LoadUserAudits: " + username);
            var request = new RestRequest($"api/B_Transactions/{username}", Method.Get); // Get user specific transactions
            request.AddUrlSegment("username", username);
            var response = restClient.Execute(request);

            if (response.IsSuccessful)
            {
                var transactions = JsonConvert.DeserializeObject<List<TransactionDataIntermed>>(response.Content);
                foreach (var transaction in transactions)
                {
                    string messageToAdd = $"Transaction ID: {transaction.transactionID}, Account Number: {transaction.acctNo}, Description: {transaction.transactionDescription}, Amount: {transaction.amount}";
                    auditLogs.Add(messageToAdd);
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                MessageBox.Show("No transactions have been found in the database.");
            }
            else
            {
                MessageBox.Show("Failed to load user audit logs.");
            }
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            if (auditAll)
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
            else // Return to UserProfileWindow
            {
                UserProfileWindow userWindow = new UserProfileWindow(userProfile);
                userWindow.Show();
                this.Close();
            }
        }
    }
}
