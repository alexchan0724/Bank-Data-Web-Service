using API_Classes;
using LocalDBWebAPI.Models;
using System.Data.SQLite;
using System.Diagnostics;

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
                            profilePicture BLOB
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
                            amount INTEGER NOT NULL,
                            FOREIGN KEY(acctNo) REFERENCES BankAccounts(acctNo) ON DELETE CASCADE
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
                        INSERT INTO UserProfile (password, username, address, email, phone, profilePicture)
                        VALUES (@Password, @Username, @Address, @Email, @Phone, @ProfilePicture)";

                        command.Parameters.AddWithValue("@Password", userProfile.password);
                        command.Parameters.AddWithValue("@Username", userProfile.username);
                        command.Parameters.AddWithValue("@Address", userProfile.address);
                        command.Parameters.AddWithValue("@Email", userProfile.email);
                        command.Parameters.AddWithValue("@Phone", userProfile.phoneNum);
                        command.Parameters.Add("@ProfilePicture", System.Data.DbType.Binary).Value = userProfile.profilePicture; // Insert image as binary

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
                            SET password = @Password, username = @Username, address = @Address, email = @Email, profilePicture = @ProfilePicture, phone = @Phone
                            WHERE username = @OldUsername OR email = @OldEmail";
                        command.Parameters.AddWithValue("@Password", userProfile.password);
                        command.Parameters.AddWithValue("@Username", userProfile.username);
                        command.Parameters.AddWithValue("@Address", userProfile.address);
                        command.Parameters.AddWithValue("@Email", userProfile.email);
                        command.Parameters.AddWithValue("@ProfilePicture", userProfile.profilePicture);
                        command.Parameters.AddWithValue("@Phone", userProfile.phoneNum);
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
                            INSERT INTO Transactions (acctNo, transactionDescription, amount)
                            VALUES (@AcctNo, @TransactionDescription, @Amount)";
                        command.Parameters.AddWithValue("@AcctNo", transaction.acctNo);
                        command.Parameters.AddWithValue("@TransactionDescription", transaction.transactionDescription);
                        command.Parameters.AddWithValue("@Amount", transaction.amount);
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
                            INSERT INTO Transactions (acctNo, transactionDescription, amount)
                            VALUES (@AcctNo, @TransactionDescription, @Amount)";
                        command.Parameters.AddWithValue("@AcctNo", transaction.acctNo);
                        command.Parameters.AddWithValue("@TransactionDescription", transaction.transactionDescription);
                        command.Parameters.AddWithValue("@Amount", transaction.amount); // transaction.Amount will already be negative
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
                        command.CommandText = "SELECT * FROM Transactions";

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TransactionDataIntermed transaction = new TransactionDataIntermed();
                                transaction.transactionID = Convert.ToInt32(reader["transactionID"]);
                                transaction.acctNo = Convert.ToInt32(reader["acctNo"]);
                                transaction.transactionDescription = reader["transactionDescription"]?.ToString() ?? string.Empty;
                                transaction.amount = Convert.ToInt32(reader["amount"]);

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
