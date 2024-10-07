using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using RestSharp;
using System.Drawing.Imaging;
using Microsoft.Win32;
using System.Diagnostics;
using API_Classes;

namespace Bank_Web_Client
{

    public partial class ModifyUserWindow : Window
    {
        private RestClient restClient;
        private bool addAccount; // True if adding an account, false if modifying an account
        private UserDataIntermed userProfile;
        private Bitmap generatedProfile;
        private UserDataIntermed newUserProfile;
        public ModifyUserWindow(UserDataIntermed pUserProfile, bool pAddAccount)
        {
            InitializeComponent();
            string URL = "http://localhost:5290";
            restClient = new RestClient(URL);

            addAccount = pAddAccount;
            userProfile = pUserProfile;
            generatedProfile = GetBitmap();

            Title = addAccount ? "Add Account" : "Modify Account";

            if (addAccount) // Changes the text of the button based on whether the user is adding or modifying an account
            {
                RandomImage.Source = ToBitmapImage(generatedProfile); // Display the generated profile picture
                ActionButton.Content = "Add";
            }
            else 
            {
                RandomImage.Source = ByteToBitmapImage(userProfile.profilePicture); // Display the user's profile picture
                ActionButton.Content = "Modify";
            }
        }

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            RestRequest request;
            try
            {
                // Create a UserProfile object with form data
                newUserProfile = new UserDataIntermed();
                newUserProfile.username = UsernameBox.Text;
                newUserProfile.password = PasswordBox.Text;
                newUserProfile.address = AddressBox.Text;
                newUserProfile.email = EmailBox.Text;
                newUserProfile.phoneNum = PhoneNumberBox.Text;

                // Ensuring that user entered fields are not too long
                if (newUserProfile.username.Length > 20 || newUserProfile.password.Length > 20 || newUserProfile.address.Length > 20 || newUserProfile.email.Length > 20)
                {
                    MessageBox.Show("Ensure that username, password, address and email are under 20 characters ");
                    return;
                }

                // Check if the phone number is 10 digits if user attempts to insert a phone number
                if (newUserProfile.phoneNum.Length != 0 && newUserProfile.phoneNum.Length != 10)
                {
                    MessageBox.Show("Phone number set must be 10 digits.");
                    return;
                }

                // Ensuring that user has entered name, email and password
                if (newUserProfile.username.Length == 0 || newUserProfile.email.Length == 0 || newUserProfile.password.Length == 0)
                {
                    MessageBox.Show("Ensure that username, email and password are entered");
                    return;
                }

                // Convert Bitmap to byte array
                byte[] imageAsBytes = ConvertBitmapToByteArray(generatedProfile);
                newUserProfile.profilePicture = imageAsBytes;

                if (addAccount) // Add account (POST)
                {
                    request = new RestRequest("api/B_userprofiles", Method.Post);
                }
                else // Modify account (PUT)
                {
                    request = new RestRequest("api/B_userprofiles", Method.Put);
                    request.AddQueryParameter("oldUsername", userProfile.username); // Add the old username and email to the request (To find the user to update)
                    request.AddQueryParameter("oldEmail", userProfile.email); 
                }

                // Set the request format to JSON and add the user profile as the body
                request.RequestFormat = RestSharp.DataFormat.Json;
                request.AddJsonBody(newUserProfile);
                RestResponse response = restClient.Execute(request);

                Debug.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ " + response.StatusCode.ToString());


                // Handle the response and display a message box accordingly
                if (response.IsSuccessful)
                {
                    if (addAccount)
                    {
                        MessageBox.Show("User profile added successfully!");
                    }
                    else
                    {
                        MessageBox.Show("User profile updated successfully!");
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    MessageBox.Show("Error: " + response.Content); // Display the error message from the Controller
                }
                else
                {
                    MessageBox.Show("Error: creation failed");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.png;*.jpg;*.jpeg"
            };

            bool? response = openFileDialog.ShowDialog();

            if (response == true)
            {
                String filename = openFileDialog.FileName;
                BitmapImage bitmapImage = new BitmapImage(new Uri(filename));
                RandomImage.Source = bitmapImage;
                generatedProfile = BitmapImage2Bitmap(bitmapImage); // Convert BitmapImage to Bitmap so it can be saved in the database
            }
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            if (addAccount) // Return to LoginWindow
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
            else // Return to UserProfileWindow
            {
                //userProfile = ConvertUserProfileToIntermed(newUserProfile); // Convert the UserProfile object to a UserDataIntermed object
                UserProfileWindow userWindow = new UserProfileWindow(userProfile);
                userWindow.Show();
                this.Close();
            }
        }

        // Converts a UserProfile object to a UserDataIntermed object
        private UserDataIntermed ConvertUserProfileToIntermed(UserDataIntermed userProfile)
        {
            UserDataIntermed returnUser = new UserDataIntermed();
            returnUser.username = userProfile.username;
            returnUser.email = userProfile.email;
            returnUser.address = userProfile.address;
            returnUser.phoneNum = userProfile.phoneNum;
            returnUser.profilePicture = userProfile.profilePicture;
            returnUser.password = userProfile.password;

            return returnUser;
        }

        // Converts a Bitmap to a BitmapImage
        private static BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        // Converts a BitmapImage to a Bitmap
        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        // Generate a random profile picture
        private static Bitmap GetBitmap()
        {
            Random rand = new Random();
            int height = 8, width = 8;
            Bitmap profilePic = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int a = rand.Next(256);
                    int r = rand.Next(256);
                    int g = rand.Next(256);
                    int b = rand.Next(256);

                    profilePic.SetPixel(x, y, System.Drawing.Color.FromArgb(a, r, g, b));
                }
            }
            return profilePic;
        }

        // Convert a Bitmap to a byte array
        private byte[] ConvertBitmapToByteArray(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Bmp);
                return ms.ToArray();
            }
        }

        private static BitmapImage ByteToBitmapImage(byte[] imageData)
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
