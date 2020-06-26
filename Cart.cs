using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PromotionEngine
{
    class Cart
    {
        public Cart()
        {
            SKU = new List<SKU>();
        }

        public IList<SKU> SKU { get; set; }

        public decimal TotalValue
        {
            get { return SKU.Sum(p => p.Price); }
        }
    }
    public class SKU
    {
        public string SKUId { get; set; }
        public decimal Price { get; set; }
    }
}
