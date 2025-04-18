using System.ComponentModel.DataAnnotations;

namespace RRAPIREST.Model
{
    public class Prices
    {
        public int ID { get; set; }
        public string unique_id { get; set; }
        public string sku { get; set; }
        public decimal nett_prod_price { get; set; }
        public decimal nett_prod_price_disc { get; set; }
        public int VAT { get; set; }
        public decimal nett_price_logi { get; set; }

    }
}
