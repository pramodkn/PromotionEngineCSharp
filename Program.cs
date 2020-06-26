using System;
using System.Collections.Generic;
using System.Linq;

namespace PromotionEngine
{
    class Program
    {


        static void Main(string[] args)
        {
            var SKU_A = new SKU { Price = 50, SKUId = "A" };
            var SKU_B = new SKU { Price = 30, SKUId = "B" };
            var SKU_C = new SKU { Price = 20, SKUId = "C" };
            var SKU_D = new SKU { Price = 15, SKUId = "D" };

            //Use Case 1
            var cart = new Cart
            {
                SKU = new List<SKU> { SKU_A, SKU_B, SKU_C }
            };
            //Use Case 2
            // var cart = new Cart
            // {
            //    SKU = new List<SKU> { SKU_A, SKU_B, SKU_C }
            //};

            //Use Case 3
            //var cart = new Cart
            //{
            //    SKU = new List<SKU> { SKU_A, SKU_B, SKU_C }
            //};
            var cartDistintSKUIDs = cart.SKU.Select(x => x.SKUId).Distinct().ToList();
            HashSet<string> hs = new HashSet<string>();
            for (int i = 1; i <= cartDistintSKUIDs.Count; i++)
            {
                foreach (var item in GetPermutations(cartDistintSKUIDs, i))
                {
                    hs.Add(string.Join("", item));
                }
            }
            var skuIdCombinations = hs.ToList();
            decimal totalDiscount = 0;
            foreach (var comb in skuIdCombinations)
            {
                var splittedSKUIds = comb.Select(x => new String(x, 1)).ToArray();
                var cartList = cart.SKU.Where(x => splittedSKUIds.Any(y => y == x.SKUId)).ToList();
                var skuCount = cartList.GroupBy(x => x.SKUId, (key, group) => new
                {
                    skuId = key,
                    skuCount = group.Count(),
                    price = group.Select(x => x.Price).FirstOrDefault(),
                });

                var promotion = new Promotions().GetPromotion(splittedSKUIds);

                foreach (var sku in skuCount)
                {
                    if (promotion != null)
                    {
                        int? promoUnits = promotion.promotionUnits.Find(x => x.SKUId == sku.skuId)?.Unit;
                        if (sku.skuCount >= promoUnits)
                        {

                            int discountFactor = Math.DivRem(sku.skuCount, promoUnits.Value, out int actualUnits);
                            int discountUnits = sku.skuCount - actualUnits;

                            totalDiscount += (discountFactor * promotion.Discount);

                        }


                    }
                }
                if (promotion != null && skuCount.Count() > 1)
                    totalDiscount -= promotion.Discount * (skuCount.Count() / 2);
            }

            Console.WriteLine("Total Promotion Amount Applied : " + totalDiscount);
            cart.SKU.Add(new SKU { Price = totalDiscount * -1, SKUId = "Promotion" });
            Console.WriteLine("Cart Amount After Discount : " + cart.TotalValue);

            Console.ReadKey();

        }
        static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> items, int count)
        {
            int i = 0;
            foreach (var item in items)
            {
                if (count == 1)
                    yield return new T[] { item };
                else
                {
                    foreach (var result in GetPermutations(items.Skip(i + 1), count - 1))
                        yield return new T[] { item }.Concat(result);
                }

                ++i;
            }
        }

    }

}
