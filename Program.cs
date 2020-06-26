using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text.RegularExpressions;
namespace PromotionEngine
{
    class Program
    {
        static Promotions GetPromotion(string[] SKUIDs)
        {
            Promotions promotionsOne = new Promotions()
            {
                Discount = 20,
                promotionUnits = new List<PromotionUnit>() {
                    new PromotionUnit(){ Unit=3,SKUId="A"}
                }
            };
            Promotions promotionsTwo = new Promotions()
            {
                Discount = 15,
                promotionUnits = new List<PromotionUnit>() {
                    new PromotionUnit(){ Unit=2,SKUId="B"}
                }
            };
            Promotions promotionsThree = new Promotions()
            {
                Discount = 5,
                promotionUnits = new List<PromotionUnit>() {
                    new PromotionUnit(){ Unit=1,SKUId="C"},
                    new PromotionUnit(){ Unit=1,SKUId="D"}
                }
            };
            List<Promotions> listPromotions = new List<Promotions>
            {
                promotionsOne,
                promotionsTwo,
                promotionsThree
            };

            Promotions activePromotion = default(Promotions);
            foreach (var promotion in listPromotions)
            {
                if (promotion.promotionUnits.Select(x => x.SKUId).All(SKUIDs.Contains))
                {
                    activePromotion = promotion;
                    break;
                }
            }

            return activePromotion;
        }

        static void Main(string[] args)
        {
            var SKU_A = new SKU { Price = 50, SKUId = "A" };
            var SKU_B = new SKU { Price = 30, SKUId = "B" };
            var SKU_C = new SKU { Price = 20, SKUId = "C" };
            var SKU_D = new SKU { Price = 15, SKUId = "D" };

            var cart = new Cart
            {
                SKU = new List<SKU> { SKU_A, SKU_A, SKU_A, SKU_B, SKU_B }
            };
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

                var promotion = GetPromotion(splittedSKUIds);

                foreach (var sku in skuCount)
                {
                    if (promotion != null)
                    {

                        try
                        {
                            int promoUnits = promotion.promotionUnits.Find(x => x.SKUId == sku.skuId).Unit;
                            if (sku.skuCount >= promoUnits)
                            {

                                int discountFactor = Math.DivRem(sku.skuCount, promoUnits, out int actualUnits);
                                int discountUnits = sku.skuCount - actualUnits;

                                totalDiscount += (discountFactor * promotion.Discount);
                            }
                        }
                        catch (Exception)
                        {


                        }

                    }
                }
            }

            Console.WriteLine(totalDiscount);

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
        public class Promotions
        {
            public List<PromotionUnit> promotionUnits { get; set; }
            public int Discount { get; set; }
        }
        public class PromotionUnit
        {
            public string SKUId { get; set; }
            public int Unit { get; set; }
        }

        public class CartQuantity
        {
            public int Quanity { get; set; }
            public decimal UnitPrice { get; set; }
        }

    }

}
