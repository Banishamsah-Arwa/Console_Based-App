using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemController : ControllerBase
    {
        private readonly Iitem _itemService; 

        public ItemController(Iitem itemService)
        {
            _itemService = itemService;
        }


        

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetAllItems()
        {
            var items = await _itemService.GetAllItems();
            return Ok(items);
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<Item>> GetItemByName(string name)
        {
            var item = await _itemService.GetItemByName(name);
            if (item == null)
                return NotFound();

            return item;
        }

        [HttpPost]
        public async Task<ActionResult<Item>> AddItem([FromBody] Item item) 
        {
          await  _itemService.AddItem(item);
            return CreatedAtAction(nameof(GetItemByName), new { name = item.Name }, item);
        }

        [HttpPut("{name}")]
        public async Task<IActionResult> UpdateItem(string name, [FromBody] Item item) 
        {
            

             await _itemService.UpdateItem(name, item);
            if (item == null)
                return NotFound();

            return Ok("Updated");
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteItem(string name)
        {
            bool deleted = await _itemService.DeleteItem(name);
            if (!deleted)
                return NotFound();

            return Ok("Deleted");
        }
    }
}
