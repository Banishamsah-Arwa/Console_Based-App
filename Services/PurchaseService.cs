using InventoryManagementSystem.Model;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System;

namespace InventoryManagementSystem.Services
{
    /// <summary>
    /// Service class for managing purchases.
    /// </summary>
    public class PurchaseService: IPurchase
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="PurchaseService"/> class with the specified database connection string.
        /// </summary>
        /// <param name="connectionString">The connection string to the database.</param>
        public PurchaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Adds an item to a purchase.
        /// </summary>
        /// <param name="itemName">The name of the item to add to the purchase.</param>
        /// <param name="userName">The name of the user making the purchase.</param>
        /// <param name="quantity">The quantity of the item to add to the purchase.</param>
        public async Task<bool> AddItemToPurchase(string itemName, string userName, int quantity)
        {
            string connectionString = _connectionString;

            int availableQuantity = await GetAvailableQuantity(itemName);
            if (quantity > availableQuantity)
            {
                Console.WriteLine("Error: Quantity exceeds available quantity.");
                return false ;
            }

            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand("INSERT INTO purchase (item_name, user_name, quantity) VALUES (@ItemName, @UserName, @Quantity)", conn);
            cmd.Parameters.AddWithValue("ItemName", itemName);
            cmd.Parameters.AddWithValue("UserName", userName);
            cmd.Parameters.AddWithValue("Quantity", quantity);
            cmd.ExecuteNonQuery();
            int newQuantity = availableQuantity - quantity;
            using var cmdtwo = new NpgsqlCommand("UPDATE item SET quantity = @NewQuantity WHERE name = @itemName", conn);
            cmdtwo.Parameters.AddWithValue("NewQuantity", newQuantity);
            cmdtwo.Parameters.AddWithValue("itemName", itemName);
            cmdtwo.ExecuteNonQuery();

            Console.WriteLine("Item added to purchase successfully.");
            return true;

        }

        /// <summary>
        /// Retrieves the available quantity of an item.
        /// </summary>
        /// <param name="itemName">The name of the item.</param>
        /// <returns>The available quantity of the item.</returns>
        private async Task<int> GetAvailableQuantity(string itemName)
        {
            string connectionString = _connectionString;
            int availableQuantity = 0;

            using var conn = new NpgsqlConnection(connectionString);
              conn.Open();
            using var cmd = new NpgsqlCommand("SELECT quantity FROM item WHERE name = @ItemName", conn);
            cmd.Parameters.AddWithValue("ItemName", itemName);
            using var reader =  cmd.ExecuteReader();
            if (reader.Read())
            {
                availableQuantity = reader.GetInt32(0);
            }
            return availableQuantity;
        }

        /// <summary>
        /// Displays the purchase history for a specific user.
        /// </summary>
        /// <param name="userName">The name of the user.</param>
        public async Task<ActionResult<IEnumerable<purchase>>> DisplayPurchaseHistory(string userName)
        {
            string connectionString = _connectionString;

            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT id, item_name, purchase_date, quantity FROM purchase WHERE user_name = @UserName", conn);
            cmd.Parameters.AddWithValue("UserName", userName);
            List<purchase> purchases = new List<purchase>();

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                purchase purchase = new purchase
                {
                    Id = reader.GetInt32(0),
                    ItemName = reader.GetString(1),
                    PurchaseDate = reader.GetDateTime(2),
                    Quantity = reader.GetInt32(3)
                };
                purchases.Add(purchase);
            }

            return purchases; 
            //Console.WriteLine($"Purchase history for user: {userName}");
            //Console.WriteLine("ID\tItem name\tPurchase Date\t\tQuantity");
            //while (reader.Read())
            //{
            //    Console.WriteLine($"{reader.GetInt32(4)}\t{reader.GetString(3)}\t{reader.GetDateTime(1)}\t{reader.GetInt32(2)}");
            //}
        }

        /// <summary>
        /// Updates the quantity of an item in a purchase.
        /// </summary>
        /// <param name="purchaseId">The ID of the purchase.</param>
        /// <param name="newQuantity">The new quantity of the item.</param>
        public async Task UpdateItemQuantityInPurchase(int purchaseId, int newQuantity)
        {
            string connectionString = _connectionString;

            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand("UPDATE purchase SET quantity = @NewQuantity WHERE id = @PurchaseId", conn);
            cmd.Parameters.AddWithValue("NewQuantity", newQuantity);
            cmd.Parameters.AddWithValue("PurchaseId", purchaseId);
            cmd.ExecuteNonQuery();
            Console.WriteLine("Item quantity in purchase updated successfully.");
        }

        /// <summary>
        /// Removes an item from a purchase.
        /// </summary>
        /// <param name="purchaseId">The ID of the purchase.</param>
        public async Task RemoveItemFromPurchase(int purchaseId)
        {
            string connectionString = _connectionString;

            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand("DELETE FROM purchase WHERE id = @PurchaseId", conn);
            cmd.Parameters.AddWithValue("PurchaseId", purchaseId);
            cmd.ExecuteNonQuery();
            Console.WriteLine("Item removed from purchase successfully.");
        }

        /// <summary>
        /// Displays all purchases.
        /// </summary>
        public async Task<ActionResult<IEnumerable<purchase>>> DisplayAllPurchases()
        {
            string connectionString = _connectionString;

            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT * FROM purchase", conn);
            List<purchase> purchases = new List<purchase>();

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                purchase purchase = new purchase
                {
                    Id = reader.GetInt32(4),
                    ItemName = reader.GetString(3),
                    UserName = reader.GetString(0),
                    PurchaseDate = reader.GetDateTime(1),
                    Quantity = reader.GetInt32(2)
                };
                purchases.Add(purchase);
            }

            return purchases;
            //Console.WriteLine("All Purchases:");

            //Console.WriteLine("ID\tItem ID\tUser Name\tPurchase Date\t\tQuantity");
            //while (reader.Read())
            //{
            //    Console.WriteLine($"{reader.GetInt32(0)}\t{reader.GetInt32(1)}\t{reader.GetString(2)}\t{reader.GetDateTime(3)}\t{reader.GetInt32(4)}");
            //}
        }
    }
}
