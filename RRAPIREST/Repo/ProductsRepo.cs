using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using RRAPIREST.Model;
using RRAPIREST.Model.Data;

namespace RRAPIREST.Repo
{
    public class ProductsRepo : IProductsRepo
    {
        private readonly DapperDbContext context;
        public ProductsRepo(DapperDbContext context)
        {
            this.context = context;
        }
        public async Task<string> CreateDatabase(string databaseName)
        {
            string response = string.Empty;
            try
            {
                //Tu nie wiem jednorazowo to robi, jeśli nie trzeba by kasować plik no i skasować dane z tabel w bazie
                DownloadFile("https://rekturacjazadanie.blob.core.windows.net/zadanie/Products.csv", @"C:\Users\louis\Pliki\Products.csv");
                //
                DownloadFile("https://rekturacjazadanie.blob.core.windows.net/zadanie/Inventory.csv", @"C:\Users\louis\Pliki\Inventory.csv");
                //
                DownloadFile("https://rekturacjazadanie.blob.core.windows.net/zadanie/Prices.csv", @"C:\Users\louis\Pliki\Prices.csv");

                if (File.Exists("C:\\Users\\louis\\Pliki\\Products.csv"))
                {
                    foreach (string line in File.ReadAllLines("C:\\Users\\louis\\Pliki\\Products.csv").Where(m => m.Split("\";\"").Length == 19 && m.Split("\";\"").ElementAt<string>(8).Contains("0") && m.Split("\";\"").ElementAt<string>(9).Contains("24h")))
                    {
                        string[] strings = line.Split("\";\"");

                        string queryins = "INSERT INTO dbo.Products (sku,prod_id,name,ean,producer_name,category,is_wire,shipping,available,is_vendor,default_image) VALUES (@sku,@prod_id,@name,@ean,@producer_name,@category,@is_wire,@shipping,@available,@is_vendor,@default_image)";

                        var parameters = new DynamicParameters();
                        parameters.Add("sku", strings[1].Replace("\"",""), DbType.String);
                        parameters.Add("prod_id", strings[0].Replace("\"", ""), DbType.String);
                        parameters.Add("name", strings[2], DbType.String);
                        parameters.Add("ean", strings[4], DbType.String);
                        parameters.Add("producer_name", strings[6], DbType.String);
                        parameters.Add("category", strings[7], DbType.String);
                        parameters.Add("is_wire", strings[8], DbType.String);
                        parameters.Add("shipping", strings[9], DbType.String);
                        parameters.Add("available", strings[11], DbType.String);
                        parameters.Add("is_vendor", strings[16], DbType.String);
                        parameters.Add("default_image", strings[18], DbType.String);

                        using (var connecting = this.context.CreateConnection())
                        {
                            await connecting.ExecuteAsync(queryins, parameters);
                        }
                    }
                }
                if (File.Exists("C:\\Users\\louis\\Pliki\\Inventory.csv"))
                {
                    foreach (string line in File.ReadAllLines("C:\\Users\\louis\\Pliki\\Inventory.csv").Where(m => m.Split(",").Length == 8 && m.Split(',').ElementAt<string>(1).Length > 0 && m.Split(',').ElementAt<string>(6).Contains("24h")))
                    {

                        string[] strings = line.Split(",");
                        // pierwsza wersja jeśli istnieje w produktach spełniającyhc warunek identyczne sku i nie kable i 24h
                        using (var connecting = this.context.CreateConnection())
                        {
                            string quwrydel = "SELECT * FROM Products WHERE sku='"+ strings[1] + "'";
                            var productslist = await connecting.QueryAsync<Products>(quwrydel);
                            int ilosc = productslist.Count();
                            if (ilosc == 0)
                            {
                                continue;
                            }
                        }
                        using (var connecting = this.context.CreateConnection())
                        {
                            string quwrydel = "SELECT * FROM Inventory WHERE sku='" + strings[1] + "'";
                            var inventorylist = await connecting.QueryAsync<Inventory>(quwrydel);
                            int ilosc = inventorylist.Count();
                            if (ilosc > 0)
                            {
                                continue;
                            }
                        }
                        // pierwsza wersja koniec w przypadku drugiej wersji trzeba usunąć
                        string queryins = "INSERT INTO Inventory(product_id,sku,unit,qty,manufacturer,shipping,shipping_cost) VALUES (@product_id,@sku,@unit,@qty,@manufacturer,@shipping,@shipping_cost)";
                        var parameters = new DynamicParameters();
                        parameters.Add("product_id", strings[0], DbType.String);
                        parameters.Add("sku", strings[1], DbType.String);
                        parameters.Add("unit", strings[2], DbType.String);
                        if (decimal.TryParse(strings[3].Replace('.',','),out decimal result))
                        {
                            parameters.Add("qty", result, DbType.Decimal);
                        }
                        else
                        {
                            parameters.Add("qty", 0, DbType.Decimal);
                        }

                        parameters.Add("manufacturer", strings[4], DbType.String);
                        parameters.Add("shipping", strings[6], DbType.String);
                        if (decimal.TryParse(strings[7].Replace('.', ','), out decimal result_s))
                        {
                            parameters.Add("shipping_cost", result_s, DbType.Decimal);
                        }
                        else
                        {
                            parameters.Add("shipping_cost", 0, DbType.Decimal);
                        }
                        using (var connecting = this.context.CreateConnection())
                        {
                            await connecting.ExecuteAsync(queryins, parameters);
                        }
                    }
                    // druga wersja wczytuje wszystkie wiersze, a potem wyrzucam niepotrzebne za pomocą (DELETE
                    // FROM[Test_DB].[dbo].[Inventory] WHERE sku NOT IN (SELECT sku FROM[Test_DB].[dbo].[Products]))
                }

                if (File.Exists("C:\\Users\\louis\\Pliki\\Prices.csv"))
                {
                    foreach (string line in File.ReadAllLines("C:\\Users\\louis\\Pliki\\Prices.csv").Where(m => m.Split("\",\"").Length == 6 && m.Split("\",\"").ElementAt<string>(3).Contains(m.Split("\",\"").ElementAt<string>(5).Replace("\"", ""))))
                    {
                        string[] strings = line.Split("\",\"");
                        // pierwsza wersja jeśli istnieje w produktach spełniającyhc warunek identyczne sku 
                        using (var connecting = this.context.CreateConnection())
                        {
                            string quwrydel = "SELECT * FROM Products WHERE sku='" + strings[1] + "'";
                            var productslist = await connecting.QueryAsync<Products>(quwrydel);
                            int ilosc = productslist.Count();
                            if (ilosc == 0)
                            {
                                continue;
                            }
                        }
                        // pierwsza wersja koniec w przypadku drugiej wersji trzeba usunąć
                        string queryins = "INSERT INTO Prices(unique_id,sku,nett_prod_price,nett_prod_price_disc,VAT,nett_price_logi) VALUES (@unique_id,@sku,@nett_prod_price,@nett_prod_price_disc,@VAT,@nett_price_logi)";
                        var parameters = new DynamicParameters();
                        parameters.Add("unique_id", strings[0].Replace("\"", ""), DbType.String);
                        parameters.Add("sku", strings[1], DbType.String);
                        if (decimal.TryParse(strings[2].Replace('.', ','), out decimal result_n))
                        {
                            parameters.Add("nett_prod_price", result_n, DbType.Decimal);
                        }
                        else
                        {
                            parameters.Add("nett_prod_price", 0, DbType.Decimal);
                        }
                        if (decimal.TryParse(strings[3].Replace('.', ','), out decimal result_d))
                        {
                            parameters.Add("nett_prod_price_disc", result_d, DbType.Decimal);
                        }
                        else
                        {
                            parameters.Add("nett_prod_price_disc", 0, DbType.Decimal);
                        }
                        parameters.Add("VAT", strings[4], DbType.Int64);
                        if (decimal.TryParse(strings[5].Replace("\"", "").Replace('.', ','), out decimal result_l))
                        {
                            parameters.Add("nett_price_logi", result_l, DbType.Decimal);
                        }
                        else
                        {
                            parameters.Add("nett_price_logi", 0, DbType.Decimal);
                        }

                        using (var connecting = this.context.CreateConnection())
                        {
                            await connecting.ExecuteAsync(queryins, parameters);
                        }
                    }
                    // druga wersja wczytuje wszystkie wiersze, a potem wyrzucam niepotrzebne za pomocą (DELETE
                    // FROM[Test_DB].[dbo].[Prices] WHERE sku NOT IN (SELECT sku FROM[Test_DB].[dbo].[Products])) 
                }
                response = "Created";
            }
            catch (Exception ex)
            {
                response = "";
            }

            return await Task.FromResult(response);
        }

        public async Task<DataProduct> GetBySKU(string selectedSKU)
        {
            DataProduct product = new DataProduct();
            using (var connecting = this.context.CreateConnection())
            {
                string quwrydel = "SELECT * FROM Products WHERE sku='" + selectedSKU + "'";
                var productselect = await connecting.QueryFirstAsync<Products>(quwrydel);
                string quwryinw = "SELECT * FROM Inventory WHERE sku='" + selectedSKU + "'";
                var inventoryselect = await connecting.QueryFirstAsync<Inventory>(quwryinw);
                string quwrypri = "SELECT * FROM Prices WHERE sku='" + selectedSKU + "'";
                var priceselect = await connecting.QueryFirstAsync<Prices>(quwrypri);
                if (productselect != null && inventoryselect != null && priceselect != null) { 
                    
                    product.Nazwa_produktu = productselect.name;
                    product.EAN = productselect.ean;
                    product.Nazwa_producenta = productselect.producer_name;
                    product.Kategoria = productselect.category;
                    product.URL_do_zdjecia_produktu = productselect.default_image;
                    product.Stan_magazynowy = inventoryselect.qty;
                    product.Jednostkę_logistyczna_produktu = inventoryselect.unit;
                    product.Cena_netto_zakupu_produktu=priceselect.nett_prod_price;
                    product.Koszt_dostawy = inventoryselect.shipping_cost;

                    return product;
                }
            }

            return null;

        }

        private static void DownloadFile(string url, string filePath)
        {
            // u mnie szybciej działało synchroniczne
            using (var webClient = new WebClient())
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
                webClient.DownloadFile(new Uri(url), filePath);
            }
        }
    }
}
