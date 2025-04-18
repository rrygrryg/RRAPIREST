using System.ComponentModel.DataAnnotations;

namespace RRAPIREST.Model
{
    public class Products
    {
        public int ID { get; set; }
        public string sku { get; set; }

        public string prod_id { get; set; } 
        public string name { get; set; }
        public string ean { get; set; }
        public string producer_name { get; set; }
        public string category  { get; set; }
        public string is_wire { get; set; }
        public string shipping { get; set; }
        public string available { get; set; }
        public string is_vendor { get; set; }
        public string default_image { get; set; }
    }
}
