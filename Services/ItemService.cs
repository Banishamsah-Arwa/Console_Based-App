using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using Npgsql;
using System;
using System.Threading.Tasks;

namespace InventoryManagementSystem
{
    /// <summary>
    /// Service class for managing items .
    /// </summary>
    public class ItemService : Iitem
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemService"/> class with the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string used to connect to the PostgreSQL database.</param>
        public ItemService(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Retrieves all items from the inventory.
        /// </summary>
        /// <returns>returning a collection of items.</returns>
        /// <summary>
        /// Retrieves all items from the inventory.
        /// </summary>
        /// <returns>returning a collection of items.</returns>
        public async Task<IEnumerable<Item>> GetAllItems()
        {
            List<Item> items = new List<Item>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var cmd = new NpgsqlCommand("SELECT * FROM item", connection))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Item item = new Item
                        {
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            Quantity = reader.GetInt32(4),
                            Price = (int)reader.GetDecimal(5),
                            Category_name = reader.GetString(3)
                        };

                        items.Add(item);
                    }
                }
            }

            return items;
        }

        /// <summary>
        /// Retrieves an item by name from the inventory.
        /// </summary>
        /// <param name="name">The name of the item to retrieve.</param>
        /// <returns> returning the retrieved item details .</returns>
        public async Task<Item> GetItemByName(string name)
        {
            Item item = null;

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var cmd = new NpgsqlCommand("SELECT * FROM item WHERE name = @Name", connection))
                {
                    cmd.Parameters.AddWithValue("Name", name);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            item = new Item
                            {
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                Quantity = reader.GetInt32(4),
                                Price = (int)reader.GetDecimal(5),
                                Category_name = reader.GetString(3)
                            };
                        }
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Retrieves an item by category name from the inventory.
        /// </summary>
        /// <param name="categoryname">The name of the category to retrieve items from</param>
        /// <returns> returning the retrieved items.</returns>
        public async Task<IEnumerable<Item>> GetItemByCategoryName(string categoryname)
        {
            List<Item> items = new List<Item>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var cmd = new NpgsqlCommand("SELECT * FROM item WHERE category_name = @Name", connection))
                {
                    cmd.Parameters.AddWithValue("Name", categoryname);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Item item = new Item
                            {
                                Name = reader.GetString(0),
                                Description = reader.GetString(1),
                                Quantity = reader.GetInt32(2),
                                Price = (int)reader.GetDecimal(3),
                                Category_name = reader.GetString(4)
                            };
                            items.Add(item);
                        }
                    }
                }
            }

            return items;
        }


        /// <summary>
        /// Adds an item to the inventory.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public async Task AddItem(Item item)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var cmd = new NpgsqlCommand("INSERT INTO item (name, description, quantity, price, category_name) VALUES (@Name, @Description, @Quantity, @Price, @CategoryName)", connection))
                {
                    cmd.Parameters.AddWithValue("Name", item.Name);
                    cmd.Parameters.AddWithValue("Description", item.Description);
                    cmd.Parameters.AddWithValue("Quantity", item.Quantity);
                    cmd.Parameters.AddWithValue("Price", item.Price);
                    cmd.Parameters.AddWithValue("CategoryName", item.Category_name);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Updates an item in the inventory.
        /// </summary>
        /// <param name="name">The name of the item to update.</param>
        /// <param name="item">The new information for the item.</param>
        public async Task UpdateItem(string name, Item item)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var cmd = new NpgsqlCommand("UPDATE item SET name = @NewName, description = @Description, quantity = @Quantity, price = @Price, category_name = @CategoryName WHERE name = @Name", connection))
                {
                    cmd.Parameters.AddWithValue("NewName", item.Name);
                    cmd.Parameters.AddWithValue("Description", item.Description);
                    cmd.Parameters.AddWithValue("Quantity", item.Quantity);
                    cmd.Parameters.AddWithValue("Price", item.Price);
                    cmd.Parameters.AddWithValue("CategoryName", item.Category_name);
                    cmd.Parameters.AddWithValue("Name", name);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Deletes an item from the inventory.
        /// </summary>
        /// <param name="name">The name of the item to delete.</param>
        public async Task<bool> DeleteItem(string name)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var cmd = new NpgsqlCommand("DELETE FROM item WHERE name = @Name", connection))
                    {
                        cmd.Parameters.AddWithValue("Name", name);
                        await cmd.ExecuteNonQueryAsync();
                    }

                }
                return true;

            }
            catch (Exception ex)
            {
                return false;

            }
        }
    }
}
