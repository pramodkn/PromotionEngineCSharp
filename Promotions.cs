using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PromotionEngine
{
    public class Promotions
    {
        public List<PromotionUnit> promotionUnits { get; set; }
        public int Discount { get; set; }
        public Promotions GetPromotion(string[] SKUIDs)
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
                var promotionUnits = promotion.promotionUnits.Select(x => x.SKUId).ToArray();
                bool promotionExist = (promotionUnits as IStructuralEquatable).Equals(SKUIDs, EqualityComparer<string>.Default);
                if (promotionExist)
                {
                    activePromotion = promotion;
                    break;
                }
            }

            return activePromotion;
        }
    }
    public class PromotionUnit
    {
        public string SKUId { get; set; }
        public int Unit { get; set; }
    }
}
