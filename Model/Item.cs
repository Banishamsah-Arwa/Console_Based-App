using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Model
{
    public class Item
    {
        public string Name { set; get; }
        public string Description { set; get; }
        public int Price { set; get; }
        public int Quantity { set; get; }
        public string Category_name { set; get; }

    }
}
