using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers
{
    [ApiController]
    [Route("purchase")]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchase _purchaseService;

        public PurchasesController(IPurchase purchaseService)
        {
            _purchaseService = purchaseService;
        }

        [HttpPost]
        public async Task<ActionResult> AddItemToPurchase([FromBody] purchase request)
        {



            var result = await _purchaseService.AddItemToPurchase(request.ItemName, request.UserName, request.Quantity);
            if (result)
            { return Ok(); }
            else
            { return BadRequest(); }
        }


        [HttpGet("history/{userName}")]
        public async Task<IActionResult> GetPurchaseHistory(string userName)
        {
            var purchases =await _purchaseService.DisplayPurchaseHistory(userName);
            return Ok(purchases);
        }


        [HttpPut("{purchaseId}")]
        public IActionResult UpdateItemQuantityInPurchase(int purchaseId, [FromBody] int newQuantity)
        {

            _purchaseService.UpdateItemQuantityInPurchase(purchaseId, newQuantity);
            return Ok();
        }




        [HttpDelete("{purchaseId}")]
        public IActionResult RemoveItemFromPurchase(int purchaseId)
        {
            _purchaseService.RemoveItemFromPurchase(purchaseId);
            return Ok();
        }

        [HttpGet]
        public async Task  <IActionResult> GetAllPurchases()
        {
           var purchases =await  _purchaseService.DisplayAllPurchases();
            return Ok(purchases);
        }
    }

    
}
