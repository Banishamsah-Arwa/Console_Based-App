using InventoryManagementSystem.Model;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Services
{
   public interface IPurchase
    {
        Task <bool>AddItemToPurchase(string itemName, string userName, int quantity);
        Task<ActionResult<IEnumerable<purchase>>> DisplayPurchaseHistory(string userName);
        Task UpdateItemQuantityInPurchase(int purchaseId, int newQuantity);
        Task RemoveItemFromPurchase(int purchaseId);
        Task<ActionResult<IEnumerable<purchase>>> DisplayAllPurchases();
    }
}
