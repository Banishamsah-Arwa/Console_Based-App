using InventoryManagementSystem.Model;
using System.Collections.Generic;


namespace InventoryManagementSystem.Services
{
    public  interface Iitem
    {
        Task<IEnumerable<Item>> GetAllItems();
        Task<Item> GetItemByName(string name);
        Task<IEnumerable<Item>> GetItemByCategoryName(string name);
        Task AddItem(Item item);
        Task UpdateItem(string name, Item item);
        Task<bool> DeleteItem(string name);
    }
}
