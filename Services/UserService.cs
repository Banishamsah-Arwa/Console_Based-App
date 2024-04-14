using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem
{
    /// <summary>
    /// Service class for managing users .
    /// </summary>
    public class UserService : IUser
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class with the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string used to connect to the PostgreSQL database.</param>
        public UserService(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Creats new account.
        /// </summary>
        /// <param name="user">The user info.</param>
        public async Task CreateNewAccount(User user)
        {
            bool is_admin;
            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password, 13);

           try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    if (user.Role=="admin")
                    {
                        is_admin = true;
                    }
                    else
                    {
                        is_admin = false;
                    }
                    await connection.OpenAsync();

                    using (var cmd = new NpgsqlCommand("INSERT INTO users (username, password, is_admin) VALUES (@Name, @Password, @Is_admin)", connection))
                    {

                        cmd.Parameters.AddWithValue("Name", user.UserName);
                        cmd.Parameters.AddWithValue("Password", passwordHash);
                        cmd.Parameters.AddWithValue("Is_admin", is_admin);

                        await cmd.ExecuteNonQueryAsync();
                        Console.WriteLine("User account created successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating user account: " + ex.Message);
            }
        }

        /// <summary>
        /// Chick user infofor logging in.
        /// </summary>
        /// <param name="username">The name of the user entered.</param>
        /// <param name="password">The password.</param>

        /// <returns>A boolean value indecates if he is exist and his role , (is_admin)?.</returns>
        public async Task<(bool, bool)> LogIn(string username, string password)
        {
            Console.WriteLine(password);

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var cmd = new NpgsqlCommand("SELECT * FROM users WHERE username = @Name", connection))
                {
                    cmd.Parameters.AddWithValue("Name", username);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string hashedPassword = reader.GetString(2);
                           bool auth= BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
                            return (auth, reader.GetBoolean(3));
                        }
                        else
                        {
                            return (false, false);

                        }
                    }
                }
            }

        }


    }
}