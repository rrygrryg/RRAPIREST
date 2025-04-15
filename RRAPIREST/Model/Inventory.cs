using System.ComponentModel.DataAnnotations;

namespace RRAPIREST.Model
{
    public class Inventory
    {
        [Key]
        public int product_id {  get; set; }
        public string sku {  get; set; }
        public string unit { get; set; }
        public decimal qty { get; set; }
        public string manufacturer { get; set; }
        public string shipping { get; set; }
        public decimal shipping_cost { get; set; }

    }
}
