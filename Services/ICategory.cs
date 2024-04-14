using InventoryManagementSystem.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Services
{
     public interface ICategory
    {
        Task AddCategory(Category category);
        Task<IEnumerable<Category>> GetAllCategories();
        Task UpdateCategory(string name, Category category);
        Task DeleteCategory(string name);
    }
}
