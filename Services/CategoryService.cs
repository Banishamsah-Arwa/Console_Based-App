using InventoryManagementSystem.Model;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Services
{
    /// <summary>
    /// Service class for managing categories.
    /// </summary>
    public class CategoryService : ICategory
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryService"/> class with the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string used to connect to the PostgreSQL database.</param>
        public CategoryService(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Adds a new category to the inventory.
        /// </summary>
        /// <param name="category">The category to add.</param>
        public async Task AddCategory(Category category)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand("INSERT INTO category (name, description) VALUES (@Name, @Description)", conn);
            cmd.Parameters.AddWithValue("Name", category.Name);
            cmd.Parameters.AddWithValue("Description", category.Description);
            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Retrieves all categories from the inventory.
        /// </summary>
        /// <returns> Return a collection of categories.</returns>
        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            List<Category> categories = new List<Category>();

            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand("SELECT * FROM category", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                Category category = new Category
                {
                    Name = reader.GetString(1),
                    Description = reader.GetString(2)
                };
                categories.Add(category);
            }

            Console.WriteLine($"Categories are:");
            Console.WriteLine("Name\tDescription ");

            foreach (var categorytoread in categories)
            {
                Console.WriteLine($"{categorytoread.Name}\t{categorytoread.Description}");
            }
            return categories;
        }

        /// <summary>
        /// Updates an existing category in the inventory.
        /// </summary>
        /// <param name="name">The name of the category to update.</param>
        /// <param name="category">The updated category information.</param>
        public async Task UpdateCategory(string name, Category category)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand("UPDATE category SET name = @NewName, description = @Description WHERE name = @Name", conn);
            cmd.Parameters.AddWithValue("NewName", category.Name);
            cmd.Parameters.AddWithValue("Description", category.Description);
            cmd.Parameters.AddWithValue("Name", name);
            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Deletes a category from the inventory.
        /// </summary>
        /// <param name="name">The name of the category to delete.</param>
        public async Task DeleteCategory(string name)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand("DELETE FROM category WHERE name = @Name", conn);
            cmd.Parameters.AddWithValue("Name", name);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
