﻿using API_Classes;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing.Text;

namespace LocalDBWebAPI.Data
{
    public class DBManager
    {
        private static string connectionString = "Data Source=mydatabase.db;Version=3;";

        public static bool CreateTables()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        // SQL command to create a table named "UserProfile"
                        command.CommandText = @"
                        DROP TABLE IF EXISTS UserProfile;
                        CREATE TABLE UserProfile(
                            password TEXT NOT NULL,
                            username TEXT NOT NULL UNIQUE,
                            email TEXT NOT NULL UNIQUE,
                            address TEXT,
                            phone TEXT,
                            profilePicture BLOB,
                            isAdmin INTEGER NOT NULL
                        )";
                        command.ExecuteNonQuery();

                        // SQL command to create a table named "BankAccounts"
                        command.CommandText = @"
                        DROP TABLE IF EXISTS BankAccounts;
                        CREATE TABLE BankAccounts (
                            acctNo INTEGER PRIMARY KEY,
                            pin INTEGER NOT NULL,
                            balance INTEGER NOT NULL,
                            description TEXT,
                            username TEXT NOT NULL,
                            email TEXT NOT NULL,
                            FOREIGN KEY(username, email) REFERENCES UserProfile(username, email) ON DELETE CASCADE
                        )";
                        command.ExecuteNonQuery();

                        // SQL command to create a table named "Transactions"
                        command.CommandText = @"
                        DROP TABLE IF EXISTS Transactions;
                        CREATE TABLE Transactions(
                            transactionID INTEGER PRIMARY KEY AUTOINCREMENT,
                            acctNo INTEGER,
                            transactionDescription TEXT NOT NULL,
                            transactionDate DATETIME NOT NULL,
                            amount INTEGER NOT NULL,
                            FOREIGN KEY(acctNo) REFERENCES BankAccounts(acctNo) ON DELETE CASCADE
                        )";
                        command.ExecuteNonQuery();
                        // SQL command to create a table named "Logs"
                        command.CommandText = @"
                        DROP TABLE IF EXISTS Logs;
                        CREATE TABLE Logs(
                            logID INTEGER PRIMARY KEY AUTOINCREMENT,
                            logDate DATETIME NOT NULL,
                            logUsername TEXT NOT NULL,
                            logDescription TEXT NOT NULL
                        )";
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return false; // Create table failed
        }

        public static bool InsertUserProfile(UserDataIntermed userProfile)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        INSERT INTO UserProfile (password, username, address, email, phone, profilePicture, isAdmin)
                        VALUES (@Password, @Username, @Address, @Email, @Phone, @ProfilePicture, @IsAdmin)";

                        command.Parameters.AddWithValue("@Password", userProfile.password);
                        command.Parameters.AddWithValue("@Username", userProfile.username);
                        command.Parameters.AddWithValue("@Address", userProfile.address);
                        command.Parameters.AddWithValue("@Email", userProfile.email);
                        command.Parameters.AddWithValue("@Phone", userProfile.phoneNum);
                        command.Parameters.Add("@ProfilePicture", System.Data.DbType.Binary).Value = userProfile.profilePicture; // Insert image as binary
                        command.Parameters.AddWithValue("@IsAdmin", userProfile.isAdmin);

                        int rowsInserted = command.ExecuteNonQuery();
                        if (rowsInserted > 0)
                        {
                            return true;
                        }
                    }
                    connection.Close();
                }
            }
            catch (SQLiteException ex) // Catch SQLiteException (unique constraint violation)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return false;
        }

        public static bool DeleteUserProfile(string checkString)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        // Parameterized SQL command to delete a row from the UserProfile table
                        command.CommandText = @"DELETE FROM UserProfile 
                            WHERE (username = @CheckString OR email = @CheckString)";
                        command.Parameters.AddWithValue("@CheckString", checkString); 

                        int rowsDeleted = command.ExecuteNonQuery();
                        if (rowsDeleted > 0)
                        {
                            return true;
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return false;
        }

        // Method to update a user profile return false if there is existing user with the same username or email
        public static bool UpdateUserProfile(UserDataIntermed userProfile, string oldUsername, string oldEmail)
        {
            Debug.WriteLine("Username in UpdateUserProfile: " + userProfile.username);
            Debug.WriteLine("Email in UpdateUserProfile: " + userProfile.email);
            Debug.WriteLine("Password in UpdateUserProfile: " + userProfile.password);
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        // Update UserProfile table using old username and email to find the correct record
                        command.CommandText = @"
                            UPDATE UserProfile
                            SET password = @Password, username = @Username, address = @Address, email = @Email, profilePicture = @ProfilePicture, phone = @Phone, isAdmin = @IsAdmin
                            WHERE username = @OldUsername OR email = @OldEmail";
                        command.Parameters.AddWithValue("@Password", userProfile.password);
                        command.Parameters.AddWithValue("@Username", userProfile.username);
                        command.Parameters.AddWithValue("@Address", userProfile.address);
                        command.Parameters.AddWithValue("@Email", userProfile.email);
                        command.Parameters.AddWithValue("@ProfilePicture", userProfile.profilePicture);
                        command.Parameters.AddWithValue("@Phone", userProfile.phoneNum);
                        command.Parameters.AddWithValue("@IsAdmin", userProfile.isAdmin);
                        command.Parameters.AddWithValue("@OldUsername", oldUsername);
                        command.Parameters.AddWithValue("@OldEmail", oldEmail);
                        command.ExecuteNonQuery();

                        // Update BankAccounts table using old username and email
                        command.CommandText = @"
                            UPDATE BankAccounts
                            SET username = @Username, email = @Email
                            WHERE username = @OldUsername AND email = @OldEmail";
                        command.Parameters.Clear(); // Clear previous parameters
                        command.Parameters.AddWithValue("@Username", userProfile.username);
                        command.Parameters.AddWithValue("@Email", userProfile.email);
                        command.Parameters.AddWithValue("@OldUsername", oldUsername);
                        command.Parameters.AddWithValue("@OldEmail", oldEmail); 
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return false;
        }

        public static UserDataIntermed GetUserProfile(string checkString, string password)
        {
            UserDataIntermed userProfile = null;
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"SELECT * FROM UserProfile 
                            WHERE password = @Password 
                            AND (email = @CheckString OR username = @CheckString)";
                        command.Parameters.AddWithValue("@CheckString", checkString);
                        command.Parameters.AddWithValue("@Password", password);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userProfile = new UserDataIntermed();
                                // Provide an empty string if the column is null
                                userProfile.password = reader["Password"]?.ToString() ?? string.Empty;
                                userProfile.username = reader["Username"]?.ToString() ?? string.Empty;
                                userProfile.address = reader["Address"]?.ToString() ?? string.Empty;
                                userProfile.email = reader["Email"]?.ToString() ?? string.Empty;
                                userProfile.profilePicture = (byte[])reader["profilePicture"]; // Read byte array
                                userProfile.phoneNum = reader["Phone"]?.ToString() ?? string.Empty;
                                userProfile.isAdmin = Convert.ToInt32(reader["isAdmin"]);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return userProfile;
        }

        public static bool InsertBankAccount(BankDataIntermed bankAccount)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            INSERT INTO BankAccounts (acctNo, pin, balance, username, email, description)
                            VALUES (@AcctNo, @Pin, @Balance, @Username, @Email, @Description)";

                        command.Parameters.AddWithValue("@AcctNo", bankAccount.accountNumber);
                        command.Parameters.AddWithValue("@Pin", bankAccount.pin);
                        command.Parameters.AddWithValue("@Balance", bankAccount.balance);
                        command.Parameters.AddWithValue("@Username", bankAccount.username);
                        command.Parameters.AddWithValue("@Email", bankAccount.email);
                        command.Parameters.AddWithValue("@Description", bankAccount.description);

                        int rowsInserted = command.ExecuteNonQuery();

                        if (rowsInserted > 0)
                        {
                            return true;
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return false;
        }

        public static bool DeleteBankAccount(int acctNo)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $"DELETE FROM BankAccounts WHERE acctNo = @AcctNo";
                        command.Parameters.AddWithValue("@AcctNo", acctNo);

                        int rowsDeleted = command.ExecuteNonQuery();
                        if (rowsDeleted > 0)
                        {
                            return true;
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return false;
        }

        // Method to update a bank account return false if there is existing account with a matching account number
        public static bool UpdateBankAccount(BankDataIntermed bankAccount, int oldAccountNumber)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        // Check if the new account number already exists
                        command.CommandText = "SELECT COUNT(*) FROM BankAccounts WHERE acctNo = @NewAcctNo";
                        command.Parameters.AddWithValue("@NewAcctNo", bankAccount.accountNumber);
                        int count = Convert.ToInt32(command.ExecuteScalar());

                        if (count > 0)
                        {
                            // Account number already exists, return false
                            return false;
                        }

                        // Update the bank account based on the old account number
                        command.CommandText = @"
                            UPDATE BankAccounts
                            SET pin = @Pin, description = @Description, acctNo = @NewAcctNo
                            WHERE acctNo = @OldAcctNo";
                        command.Parameters.AddWithValue("@Pin", bankAccount.pin);
                        command.Parameters.AddWithValue("@Description", bankAccount.description);
                        command.Parameters.AddWithValue("@OldAcctNo", oldAccountNumber);

                        int rowsUpdated = command.ExecuteNonQuery();
                        if (rowsUpdated == 0)
                        {
                            return false; // No rows updated, account number might not exist
                        }

                        // Update Transactions if account number changes
                        command.CommandText = @"
                            UPDATE Transactions
                            SET acctNo = @NewAcctNo
                            WHERE acctNo = @OldAcctNo";
                        command.Parameters.Clear(); // Clear previous parameters
                        command.Parameters.AddWithValue("@NewAcctNo", bankAccount.accountNumber);
                        command.Parameters.AddWithValue("@OldAcctNo", oldAccountNumber);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return false;
        }

        // Method to retrieve all bank accounts for a specific user
        public static List<BankDataIntermed> GetUserBankAccounts(string username)
        {
            List<BankDataIntermed> bankAccounts = new List<BankDataIntermed>();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM BankAccounts WHERE username = @Username";
                        command.Parameters.AddWithValue("@Username", username);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                BankDataIntermed bankAccount = new BankDataIntermed();
                                bankAccount.accountNumber = Convert.ToInt32(reader["acctNo"]);
                                bankAccount.pin = Convert.ToInt32(reader["pin"]);
                                bankAccount.balance = Convert.ToInt32(reader["balance"]);
                                bankAccount.username = reader["username"]?.ToString() ?? string.Empty;
                                bankAccount.email = reader["email"]?.ToString() ?? string.Empty;
                                bankAccount.description = reader["description"]?.ToString() ?? string.Empty;

                                bankAccounts.Add(bankAccount);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return bankAccounts;
        }

        public static BankDataIntermed GetBankAccount(int acctNo, string username)
        {
            BankDataIntermed bankAccount = null; // Initialize as null to return if no match is found
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM BankAccounts WHERE acctNo = @AcctNo AND username = @Username";
                        command.Parameters.AddWithValue("@AcctNo", acctNo);
                        command.Parameters.AddWithValue("@Username", username);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                bankAccount = new BankDataIntermed();
                                bankAccount.accountNumber = Convert.ToInt32(reader["acctNo"]);
                                bankAccount.pin = Convert.ToInt32(reader["pin"]);
                                bankAccount.balance = Convert.ToInt32(reader["balance"]);
                                bankAccount.username = reader["username"]?.ToString() ?? string.Empty;
                                bankAccount.email = reader["email"]?.ToString() ?? string.Empty;
                                bankAccount.description = reader["description"]?.ToString() ?? string.Empty;

                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return bankAccount;
        }

        // Method to initiate money transfer between two bank accounts, Assume accounts have been validated prior to calling this method
        public static bool TransferMoney(TransactionDataIntermed sendAccount, TransactionDataIntermed receiveAccount)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        // Withdraw from the sending account
                        command.CommandText = @"
                            INSERT INTO Transactions (acctNo, transactionDescription, amount, transactionDate)
                            VALUES (@AcctNo, @TransactionDescription, @Amount, @TransactionDate)";
                        command.Parameters.AddWithValue("@AcctNo", sendAccount.acctNo);
                        command.Parameters.AddWithValue("@TransactionDescription", sendAccount.transactionDescription);
                        command.Parameters.AddWithValue("@Amount", sendAccount.amount); // Amount will already be negative
                        command.Parameters.AddWithValue("@TransactionDate", sendAccount.transactionDate);
                        command.ExecuteNonQuery();

                        // Update balance in BankAccounts table
                        command.CommandText = @"
                            UPDATE BankAccounts SET balance = balance + @Amount WHERE acctNo = @AcctNo";
                        command.Parameters.Clear(); // Clear previous parameters
                        command.Parameters.AddWithValue("@Amount", sendAccount.amount);
                        command.Parameters.AddWithValue("@AcctNo", sendAccount.acctNo);
                        command.ExecuteNonQuery();
                        
                        // Deposit into the receiving account note that all parameters will be the same except for the amount and acctNo
                        command.CommandText = @"
                            INSERT INTO Transactions (acctNo, transactionDescription, amount, transactionDate)
                            VALUES (@AcctNo, @TransactionDescription, @Amount, @TransactionDate)";
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@AcctNo", receiveAccount.acctNo);
                        command.Parameters.AddWithValue("@TransactionDescription", receiveAccount.transactionDescription);
                        command.Parameters.AddWithValue("@Amount", receiveAccount.amount);
                        command.Parameters.AddWithValue("@TransactionDate", receiveAccount.transactionDate);
                        command.ExecuteNonQuery();

                        command.CommandText = @"
                            UPDATE BankAccounts SET balance = balance + @Amount WHERE acctNo = @AcctNo";
                        command.Parameters.Clear(); // Clear previous parameters
                        command.Parameters.AddWithValue("@Amount", receiveAccount.amount);
                        command.Parameters.AddWithValue("@AcctNo", receiveAccount.acctNo);
                        command.ExecuteNonQuery();

                    }
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return false;
        }

        public static bool DepositTransaction(TransactionDataIntermed transaction)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        // Insert transaction record
                        command.CommandText = @"
                            INSERT INTO Transactions (acctNo, transactionDescription, amount, transactionDate)
                            VALUES (@AcctNo, @TransactionDescription, @Amount, @TransactionDate)";
                        command.Parameters.AddWithValue("@AcctNo", transaction.acctNo);
                        command.Parameters.AddWithValue("@TransactionDescription", transaction.transactionDescription);
                        command.Parameters.AddWithValue("@Amount", transaction.amount);
                        command.Parameters.AddWithValue("@TransactionDate", transaction.transactionDate);
                        command.ExecuteNonQuery();

                        // Update balance in BankAccounts table
                        command.CommandText = @"
                            UPDATE BankAccounts SET balance = balance + @Amount WHERE acctNo = @AcctNo";
                        command.Parameters.Clear(); // Clear previous parameters
                        command.Parameters.AddWithValue("@Amount", transaction.amount);
                        command.Parameters.AddWithValue("@AcctNo", transaction.acctNo);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return false;
        }

        public static bool WithdrawTransaction(TransactionDataIntermed transaction)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        // Insert transaction record
                        command.CommandText = @"
                            INSERT INTO Transactions (acctNo, transactionDescription, amount, transactionDate)
                            VALUES (@AcctNo, @TransactionDescription, @Amount, @TransactionDate)";
                        command.Parameters.AddWithValue("@AcctNo", transaction.acctNo);
                        command.Parameters.AddWithValue("@TransactionDescription", transaction.transactionDescription);
                        command.Parameters.AddWithValue("@Amount", transaction.amount); // transaction.Amount will already be negative
                        command.Parameters.AddWithValue("@TransactionDate", transaction.transactionDate);
                        command.ExecuteNonQuery();

                        // Update balance in BankAccounts table
                        command.CommandText = @"
                            UPDATE BankAccounts SET balance = balance + @Amount WHERE acctNo = @AcctNo";
                        command.Parameters.Clear(); // Clear previous parameters
                        command.Parameters.AddWithValue("@Amount", transaction.amount);
                        command.Parameters.AddWithValue("@AcctNo", transaction.acctNo);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return false;
        }

        // Method to retrieve all transactions for a specific bank account
        public static List<TransactionDataIntermed> GetTransactionByBankAccount(int acctNo)
        {
            List<TransactionDataIntermed> transactions = new List<TransactionDataIntermed>();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM Transactions WHERE acctNo = @AcctNo";
                        command.Parameters.AddWithValue("@AcctNo", acctNo);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TransactionDataIntermed transaction = new TransactionDataIntermed();
                                transaction.transactionID = Convert.ToInt32(reader["transactionID"]);
                                transaction.acctNo = Convert.ToInt32(reader["acctNo"]);
                                transaction.transactionDescription = reader["transactionDescription"]?.ToString() ?? string.Empty;
                                transaction.amount = Convert.ToInt32(reader["amount"]);
                                transaction.transactionDate = Convert.ToDateTime(reader["transactionDate"]);
                                transactions.Add(transaction);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return transactions;
        }

        // Method to retrieve oldest to newest transactions for a specific bank account
        public static List<TransactionDataIntermed> GetTransactionOrderedByBankAccount(int acctNo)
        {
            List<TransactionDataIntermed> transactions = new List<TransactionDataIntermed>();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        // ORDER BY transactionDate ASC to get oldest to newest transactions
                        command.CommandText = "SELECT * FROM Transactions WHERE acctNo = @AcctNo ORDER BY transactionDate ASC";
                        command.Parameters.AddWithValue("@AcctNo", acctNo);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TransactionDataIntermed transaction = new TransactionDataIntermed();
                                transaction.transactionID = Convert.ToInt32(reader["transactionID"]);
                                transaction.acctNo = Convert.ToInt32(reader["acctNo"]);
                                transaction.transactionDescription = reader["transactionDescription"]?.ToString() ?? string.Empty;
                                transaction.amount = Convert.ToInt32(reader["amount"]);
                                transaction.transactionDate = Convert.ToDateTime(reader["transactionDate"]);
                                transactions.Add(transaction);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return transactions;
        }

        // Method to retrieve all transactions for a specific user
        public static List<TransactionDataIntermed> GetTransactionsByUser(string username)
        {
            Debug.WriteLine("Username in GetTransactionsByUser: " + username);
            List<TransactionDataIntermed> transactions = new List<TransactionDataIntermed>();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        // Join the Transactions, BankAccounts, and UserProfile tables to get all transactions for a specific user
                        command.CommandText = @"
                            SELECT T.* FROM Transactions T 
                            JOIN BankAccounts B ON T.acctNo = B.acctNo
                            JOIN UserProfile U ON B.username = U.username AND B.email = U.email
                            WHERE U.username = @Username";
                        command.Parameters.AddWithValue("@Username", username); // Check transactions for this user (usernames are unique)

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TransactionDataIntermed transaction = new TransactionDataIntermed();
                                transaction.transactionID = Convert.ToInt32(reader["transactionID"]);
                                transaction.acctNo = Convert.ToInt32(reader["acctNo"]);
                                transaction.transactionDescription = reader["transactionDescription"]?.ToString() ?? string.Empty;
                                transaction.amount = Convert.ToInt32(reader["amount"]);
                                transaction.transactionDate = Convert.ToDateTime(reader["transactionDate"]);
                                transactions.Add(transaction);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return transactions;
        }

        // Method to retrieve all transactions for auditing purposes (shown on log-in page)
        public static List<TransactionDataIntermed> GetAllTransactions()
        {
            List<TransactionDataIntermed> transactions = new List<TransactionDataIntermed>();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"SELECT * FROM Transactions";

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TransactionDataIntermed transaction = new TransactionDataIntermed();
                                transaction.transactionID = Convert.ToInt32(reader["transactionID"]);
                                transaction.acctNo = Convert.ToInt32(reader["acctNo"]);
                                transaction.transactionDescription = reader["transactionDescription"]?.ToString() ?? string.Empty;
                                transaction.amount = Convert.ToInt32(reader["amount"]);
                                transaction.transactionDate = Convert.ToDateTime(reader["transactionDate"]);
                                transactions.Add(transaction);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return transactions;
        }

        // Checks whether a user is admin or not using the username
        public static int IsAdmin(string username)
        {
            int isAdmin = 0; // 0 is false, 1 is true
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"SELECT isAdmin FROM UserProfile WHERE username = @Username";
                        command.Parameters.AddWithValue("@Username", username);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isAdmin = Convert.ToInt32(reader["isAdmin"]); // Convert isAdmin of UserProfile to boolean
                            }
                            else // If no user is found, return false
                            {
                                isAdmin = -1; // Helps to display error message
                                Console.WriteLine("No user found with username: " + username);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return isAdmin;
        }

        // Method to get a user profile (Same as above except does not require password)
        private static UserDataIntermed AdminGetUserProfile(string username)
        {
            UserDataIntermed userProfile = null;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT * FROM UserProfile WHERE username = @Username";
                    command.Parameters.AddWithValue("@Username", username);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userProfile = new UserDataIntermed();
                            // Provide an empty string if the column is null
                            userProfile.password = reader["Password"]?.ToString() ?? string.Empty;
                            userProfile.username = reader["Username"]?.ToString() ?? string.Empty;
                            userProfile.address = reader["Address"]?.ToString() ?? string.Empty;
                            userProfile.email = reader["Email"]?.ToString() ?? string.Empty;
                            userProfile.profilePicture = (byte[])reader["profilePicture"]; // Read byte array
                            userProfile.phoneNum = reader["Phone"]?.ToString() ?? string.Empty;
                            userProfile.isAdmin = Convert.ToInt32(reader["isAdmin"]); // Convert isAdmin of UserProfile to boolean                        }
                        }
                    }
                }
                connection.Close();
            }
            return userProfile;
        }

        // Method to search for users by username returns a list of users that match the search string
        private static List<UserDataIntermed> SearchUsersByName(string searchString)
        {
            List<UserDataIntermed> users = new List<UserDataIntermed>();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT * FROM UserProfile WHERE username LIKE @SearchString";
                    command.Parameters.AddWithValue("@SearchString", searchString + "%"); // Uses wildcard to search for usernames starting with the search string
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserDataIntermed user = new UserDataIntermed();
                            user.password = reader["password"]?.ToString() ?? string.Empty;
                            user.username = reader["username"]?.ToString() ?? string.Empty;
                            user.address = reader["address"]?.ToString() ?? string.Empty;
                            user.email = reader["email"]?.ToString() ?? string.Empty;
                            user.profilePicture = (byte[])reader["profilePicture"]; // Read byte array
                            user.phoneNum = reader["phone"]?.ToString() ?? string.Empty;
                            user.isAdmin = Convert.ToInt32(reader["isAdmin"]);
                            users.Add(user);
                        }
                    }
                }
                connection.Close();
            }
            return users;
        }

        // Method to search for transactions using filters
        public static List<TransactionDataIntermed> getTransactionsUsingFilter(string username, DateTime startingDate, DateTime endingDate, int minAmount, bool ascending)
        {
            List<TransactionDataIntermed> transactions = new List<TransactionDataIntermed>();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            SELECT T.* FROM Transactions T 
                            JOIN BankAccounts B ON T.acctNo = B.acctNo
                            JOIN UserProfile U ON B.username = U.username AND B.email = U.email
                            WHERE U.username = @Username 
                            AND (transactionDate BETWEEN @StartingDate AND @EndingDate) 
                            AND amount > @MinAmount" ;
                        command.Parameters.AddWithValue("@Username", username + "%"); // Uses wildcard to search for usernames starting with the search string
                        command.Parameters.AddWithValue("@StartingDate", startingDate);
                        command.Parameters.AddWithValue("@EndingDate", endingDate);
                        command.Parameters.AddWithValue("@MinAmount", minAmount);
                        if (ascending) // Order by transaction date ascending or descending
                        {
                            command.CommandText += " ORDER BY transactionDate ASC";
                        }
                        else // If ascending is false, order by transaction date descending
                        {
                            command.CommandText += " ORDER BY transactionDate DESC";
                        }

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TransactionDataIntermed transaction = new TransactionDataIntermed();
                                transaction.transactionID = Convert.ToInt32(reader["transactionID"]);
                                transaction.acctNo = Convert.ToInt32(reader["acctNo"]);
                                transaction.transactionDescription = reader["transactionDescription"]?.ToString() ?? string.Empty;
                                transaction.amount = Convert.ToInt32(reader["amount"]);
                                transaction.transactionDate = Convert.ToDateTime(reader["transactionDate"]);
                                transactions.Add(transaction);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return transactions;
        }

        // Method to add a log entry
        public static bool AddLogEntry(string username, string description)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            INSERT INTO Logs (logDate, logUsername, logDescription)
                            VALUES (@LogDate, @LogUsername, @LogDescription)";
                        command.Parameters.AddWithValue("@LogDate", DateTime.Now);
                        command.Parameters.AddWithValue("@LogUsername", username);
                        command.Parameters.AddWithValue("@LogDescription", description);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return false;
        }

        // Method to initialise the database with tables and initial data
        public static void DBInitialise()
        {
            if (CreateTables())
            {
                Console.WriteLine("Tables created successfully.");
                DBInitialiser.InsertUserProfiles();
                DBInitialiser.InsertBankAccounts();
                DBInitialiser.InsertTransactions();
                Console.WriteLine("Initial data inserted.");
            }
        }
    }
}
