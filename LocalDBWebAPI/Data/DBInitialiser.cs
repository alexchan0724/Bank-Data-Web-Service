using API_Classes;
using LocalDBWebAPI.Models;
using System.Drawing;
using System.Drawing.Imaging;

namespace LocalDBWebAPI.Data
{
    public class DBInitialiser
    {
        private static Dictionary<string, string> userDictionary = new Dictionary<string, string>(); // Maps username to email and allows both to uniquely identifies users
        private static List<string> usernameList = new List<string>(); // Keeps track of all usernames to ensure they are unique
        private static List<string> emailList = new List<string>(); // Keeps track of all emails to ensure they are unique
        private static List<int> acctNoList = new List<int>(); // Keeps track of all account numbers to ensure they are unique
        private static Random rand = new Random();

        public static void InsertUserProfiles()
        {
            // Generate user profiles for testing
            byte[] randomImage1 = GenerateRandomBitmap();
            UserDataIntermed jin = new UserDataIntermed("123", "Jin", "20 John Street", "cyclonedestroy@gmail.com", randomImage1, "0491019164", 0);
            DBManager.InsertUserProfile(jin);
            byte[] randomImage2 = GenerateRandomBitmap();
            UserDataIntermed alex = new UserDataIntermed("456", "Alex", "30 John Street", "alexchan0724@gmail.com", randomImage2, "0424550558", 0);
            DBManager.InsertUserProfile(alex);
            byte[] randomImage3 = GenerateRandomBitmap();
            UserDataIntermed admin = new UserDataIntermed("789", "Admin", "40 John Street", "admin@gmail.com", randomImage3, "0426201320", 1);
            DBManager.InsertUserProfile(admin);

            // Add Jin and Alex to dictionary and lists
            userDictionary.Add(jin.username, jin.email);
            usernameList.Add(jin.username);
            emailList.Add(jin.email);
            userDictionary.Add(alex.username, alex.email);
            usernameList.Add(alex.username);
            emailList.Add(alex.email);
            userDictionary.Add(admin.username, admin.email);
            usernameList.Add(admin.username);
            emailList.Add(admin.username);

            // Fill database with 97 more user profiles
            for (int i = 0; i < 97; i++)
            {
                byte[] randomImage = GenerateRandomBitmap();
                string randomPassword = GetPassword();
                string randomUsername = GetUsername();
                string randomEmail = GetEmail();
                userDictionary.Add(randomUsername, randomEmail);
                string randomPhoneNum = "04" + rand.Next(10000000, 99999999); // Random phone number from Australia
                string randomAddress = rand.Next(1, 1000) + " John Street"; // Random address on John Street
                UserDataIntermed user = new UserDataIntermed(randomPassword, randomUsername, randomAddress, randomEmail, randomImage, randomPhoneNum, 0);
                DBManager.InsertUserProfile(user);
            }
        }

        private static string GetPassword()
        {
            int password = rand.Next(100000, 999999); // 6-digit password for simplicity
            return password.ToString();
        }

        private static string GetUsername() 
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"; // Only allow letters
            int nameLength = rand.Next(3, 8); // Length of username
            string name;
            do
            {
                name = "";
                for (int i = 0; i < nameLength; i++)
                {
                    int randLetter = rand.Next(chars.Length);
                    name += chars.ElementAt(randLetter);
                }
            } while (usernameList.Contains(name)); // Ensure username is unique
            usernameList.Add(name);
            return name;
        }

        private static string GetEmail()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"; // Only allow letters
            int emailLength = rand.Next(5, 8); // Length of email
            string email;
            do
            {
                email = "";
                for (int i = 0; i < emailLength; i++)
                {
                    int randLetter = rand.Next(chars.Length);
                    email += chars.ElementAt(randLetter);
                }
                email += "@gmail.com";
            }while (emailList.Contains(email)); // Ensure email is unique
            emailList.Add(email);
            return email;
        }

        private static byte[] GenerateRandomBitmap()
        {
            int height = 8, width = 8;
            using (Bitmap profilePic = new Bitmap(width, height))
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int a = rand.Next(256);
                        int r = rand.Next(256);
                        int g = rand.Next(256);
                        int b = rand.Next(256);

                        profilePic.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                    }
                }

                // Convert Bitmap to byte array
                using (MemoryStream ms = new MemoryStream())
                {
                    profilePic.Save(ms, ImageFormat.Bmp);
                    return ms.ToArray(); // Return byte array
                }
            }
        }

        public static void InsertBankAccounts()
        {
            // Initialise bank accounts for testing 
            BankDataIntermed jinSavings = new BankDataIntermed(100000, 1234, 10000, "Jin", "cyclonedestroy@gmail.com", "Savings account");
            BankDataIntermed jinSpendings = new BankDataIntermed(100001, 1234, 1000, "Jin", "cyclonedestroy@gmail.com", "Spendings account");
            BankDataIntermed alexSpendings = new BankDataIntermed(100002, 4567, -2000, "Alex", "alexchan0724@gmail.com", "Spendings account");
            
            // Add bank accounts to list of account numbers
            acctNoList.Add(jinSavings.accountNumber);
            acctNoList.Add(jinSpendings.accountNumber);
            acctNoList.Add(alexSpendings.accountNumber);

            // Insert bank accounts into database
            DBManager.InsertBankAccount(jinSavings);
            DBManager.InsertBankAccount(jinSpendings);
            DBManager.InsertBankAccount(alexSpendings);

            // Fill database with 197 more bank accounts
            for (int i = 0; i < 197; i++)
            {
                string existingUsername = GetExistingUsername();
                string existingEmail = userDictionary[existingUsername];
                int randomAccNo = GetAccNo();
                int randomPin = rand.Next(1000, 9999); // Random 4-digit pin
                int randomBalance = GetBalance();
                string randomDescription = GetDescription();
                BankDataIntermed account = new BankDataIntermed(randomAccNo, randomPin, randomBalance, existingUsername, existingEmail, randomDescription);
                DBManager.InsertBankAccount(account);
            }
        }

        // Get a random username from the list of existing usernames
        private static string GetExistingUsername()
        {
            int index = rand.Next(usernameList.Count);
            return usernameList[index];
        }

        private static int GetAccNo()
        {
            int accNo;
            do
            {
                accNo = rand.Next(100000, 999999);
            } while (acctNoList.Contains(accNo)); // Ensure uniqueness
            acctNoList.Add(accNo);
            return accNo;
        }

        private static int GetBalance()
        {
            int balance = rand.Next(1, 1000000);
            if (rand.Next(2) == 1)
            {
                balance *= -1;
            }
            return balance;
        }

        private static string GetDescription()
        {
            string description = "";
            int typeOfAccount = rand.Next(1, 5);
            if (typeOfAccount == 1)
            {
                description = "Savings account";
            }
            else if (typeOfAccount == 2)
            {
                description = "Spendings account";
            }
            else if (typeOfAccount == 3)
            {
                description = "Investment account";
            }
            else if (typeOfAccount == 4)
            {
                description = "Joint account";
            }
            return description;
        }

        public static void InsertTransactions()
        {
            // Initialise transactions for testing
            TransactionDataIntermed jinSpend = new TransactionDataIntermed(100001, "Bought a new phone", -1000, new DateTime(2004, 9,20));
            TransactionDataIntermed jinSpend2 = new TransactionDataIntermed(100000, "Bought new car", -10000, new DateTime(2012, 10, 14));
            TransactionDataIntermed jinDeposit = new TransactionDataIntermed(100000, "Salary for the month", 6000, new DateTime(2006, 5, 16));
            TransactionDataIntermed alexSpend = new TransactionDataIntermed(100002, "Bought a new laptop", -2000, new DateTime(1989, 12, 13));
            TransactionDataIntermed alexDeposit = new TransactionDataIntermed(100002, "Received money from parents", 5000, new DateTime(2001, 9, 11));

            // Insert transactions into database
            DBManager.WithdrawTransaction(jinSpend);
            DBManager.WithdrawTransaction(jinSpend2);
            DBManager.DepositTransaction(jinDeposit);
            DBManager.WithdrawTransaction(alexSpend);
            DBManager.DepositTransaction(alexDeposit);

            // Fill database with 295 more transactions
            for (int i = 0; i < 295; i++)
            {
                int randomAccNo = GetExistingAccNo();
                int transactionType = rand.Next(1, 9); 
                string randomDescription = GetTransactionDescription(transactionType);
                int randomAmount = rand.Next(1, 10000);
                DateTime randomDate = RandomDay();
                if (transactionType < 5) // Options 1-4 are withdraws, Options 5-8 are deposits
                {
                    randomAmount *= -1;
                }
                TransactionDataIntermed transaction = new TransactionDataIntermed(randomAccNo, randomDescription, randomAmount, randomDate);
                if (randomAmount < 0)
                {
                    DBManager.WithdrawTransaction(transaction);
                }
                else
                {
                    DBManager.DepositTransaction(transaction);
                }
            }
        }

        // Generate random DateTime value between 1/1/1995 and today
        private static DateTime RandomDay()
        {
            DateTime start = new DateTime(1995, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(rand.Next(range));
        }

        private static int GetExistingAccNo()
        {
            int index = rand.Next(acctNoList.Count);
            return acctNoList[index];
        }

        // Method converts transaction type to a description
        private static string GetTransactionDescription(int transactionType)
        {
            string description = "";
            switch (transactionType)
            {
                case 1:
                    description = "Bought a new phone";
                    break;
                case 2:
                    description = "Bought a new laptop";
                    break;
                case 3:
                    description = "Bought new car";
                    break;
                case 4:
                    description = "Bought new clothes";
                    break;
                case 5:
                    description = "Received money from parents";
                    break;
                case 6:
                    description = "Salary for the month";
                    break;
                case 7:
                    description = "Pocket money";
                    break;
                case 8:
                    description = "Did some chores";
                    break;
                default:
                    description = "Unknown transaction";
                    break;
            }
            return description;
        }
    }
}
