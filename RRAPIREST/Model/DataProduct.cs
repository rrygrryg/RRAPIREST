using Microsoft.Extensions.Hosting;

namespace RRAPIREST.Model
{
    public class DataProduct
    {
        public string Nazwa_produktu { get; set; }
        public string EAN { get; set; }
        public string Nazwa_producenta { get; set; }
        public string Kategoria { get; set; }
        public string URL_do_zdjecia_produktu { get; set; }
        public string Jednostkę_logistyczna_produktu { get; set; }
        public decimal Stan_magazynowy { get; set; }
        public decimal Koszt_dostawy { get; set; }
        public decimal Cena_netto_zakupu_produktu { get; set; }

    }
}
