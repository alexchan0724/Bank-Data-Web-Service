using RestSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using API_Classes;
using System.Diagnostics;

namespace Bank_Web_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private RestClient restClient;
        public LoginWindow()
        {
            InitializeComponent();
            String URL = "http://localhost:5290";
            restClient = new RestClient(URL);
        }

        private void AuditButton_Click(object sender, RoutedEventArgs e)
        {
            AuditWindow auditWindow = new AuditWindow(null, true); // true to view entire audit log, false to view user-specific audit log
            auditWindow.Show();
            this.Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string checkString = InputTextBox.Text; // checkString can be either email or username
            string password = PasswordTextBox.Text;

            if(!checkString.Equals("") && !password.Equals(""))
            {
                var request = new RestRequest("api/B_UserProfiles/{checkString}", Method.Get); // GET: user/UserProfiles/{checkString}?password={password}
                request.AddUrlSegment("checkString", checkString);
                request.AddParameter("password", password);

                var response = restClient.Execute(request);

                if (response.IsSuccessful)
                {
                    Debug.WriteLine("Response was successful");
                    var userProfile = JsonConvert.DeserializeObject<UserDataIntermed>(response.Content);
                    UserProfileWindow userProfileWindow = new UserProfileWindow(userProfile); // Pass in checkString to display user's profile
                    userProfileWindow.Show();
                    this.Close();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    MessageBox.Show("Error: " + "Password is required."); // Display the error message from the Controller
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    MessageBox.Show("Error: " + "Parameter does not match any emails or usernames in the database.");
                }
                else
                {
                    MessageBox.Show("Error: Login failed");
                }
            }
            else
            {
                MessageBox.Show("USERNAME OR PASSWORD CANNOT BE EMPTY");
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            ModifyUserWindow registerWindow = new ModifyUserWindow(null, true); // Parameters for ModifyUserWindow: UserDataIntermed, addAccount
            registerWindow.Show();
            this.Close();
        }
    }
}
