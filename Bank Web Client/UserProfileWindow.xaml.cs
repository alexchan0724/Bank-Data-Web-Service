using API_Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.VisualBasic;
using RestSharp;
using Newtonsoft.Json;
using System.Drawing;
using System.IO;
using System.Diagnostics;

namespace Bank_Web_Client
{
    /// <summary>
    /// Interaction logic for UserProfileWindow.xaml
    /// </summary>
    public partial class UserProfileWindow : Window
    {
        private UserDataIntermed userProfile;
        private RestClient restClient;

        public UserProfileWindow(UserDataIntermed pUserProfile)
        {
            InitializeComponent();
            userProfile = pUserProfile;
            InitialiseUserInformation();

            string URL = "http://localhost:5290";
            restClient = new RestClient(URL);

            Title = "User Profile";
        }

        private void InitialiseUserInformation()
        {
            UsernameTextBlock.Text = "Welcome: " + userProfile.username;
            EmailTextBlock.Text = "Email: " + userProfile.email;
            AddressTextBlock.Text = "Address: " + userProfile.address;
            PhoneNumberTextBlock.Text = "Phone Number: " + userProfile.phoneNum;
            PasswordTextBlock.Text = "Password: " + userProfile.password;
            RandomImage.Source = ToBitmapImage(userProfile.profilePicture);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            RestResponse response = null;
            try
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete your account?", "Delete Account", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    string username = userProfile.username;
                    RestRequest request = new RestRequest($"api/B_userprofiles/{username}", Method.Delete);
                    response = restClient.Execute(request);
                    MessageBox.Show(response.Content);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) // If the account was successfully deleted
                    {
                        LoginWindow loginWindow = new LoginWindow();
                        loginWindow.Show();
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
            ModifyUserWindow modifyUserWindow = new ModifyUserWindow(userProfile, false);
            modifyUserWindow.Show();
            this.Close();
        }

        // User returns to the login window after clicking the Return button
        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            ModifyAccountWindow modifyAccountWindow = new ModifyAccountWindow(userProfile, null, true);
            modifyAccountWindow.Show();
            this.Close();
        }

        private void RetrieveAccountButton_Click(object sender, RoutedEventArgs e)
        {
            string accountNumber = Microsoft.VisualBasic.Interaction.InputBox("Enter Bank Account Number:", "Retrieve Bank Account Details", "");

            if (!string.IsNullOrWhiteSpace(accountNumber))
            {
                if (int.TryParse(accountNumber, out int validAccountNumber)) // Ensure the user enters integers
                {
                    try
                    {
                        RestRequest request = new RestRequest($"api/B_BankAccounts/{accountNumber}", Method.Get);
                        request.AddUrlSegment("accountNumber", accountNumber);
                        request.AddQueryParameter("username", userProfile.username);
                        RestResponse response = restClient.Execute(request);

                        if (response.IsSuccessful)
                        {
                            BankDataIntermed bankDataIntermed = JsonConvert.DeserializeObject<BankDataIntermed>(response.Content);
                            BankAccountWindow bankAccountWindow = new BankAccountWindow(userProfile, bankDataIntermed);
                            bankAccountWindow.Show();
                            this.Close();
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            MessageBox.Show(response.Content);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error retrieving bank account: " + ex.Message);
                    }
                }
                else // If the user did not enter a valid account number
                {
                    MessageBox.Show("Please enter a valid account number.");
                }
            }
            else
            {
                MessageBox.Show("Account name cannot be empty. Please enter a valid account number");
            }
        }

        private void AuditButton_Click(object sender, RoutedEventArgs e)
        {
            AuditWindow auditWindow = new AuditWindow(userProfile, false); // false to view user-specific audit log
            auditWindow.Show();
            this.Close();
        }

        // Convert byte[] to BitmapImage
        private static BitmapImage ToBitmapImage(byte[] imageData)
        {
            using (var memory = new MemoryStream(imageData))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

    }
}
