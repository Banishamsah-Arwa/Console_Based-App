using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Model
{
    public class purchase
    {

        public int Id { get; set; }
        public string ItemName { get; set; }
        public string UserName { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int Quantity { get; set; }


    }
}
