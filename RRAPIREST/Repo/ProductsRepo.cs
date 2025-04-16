using System.ComponentModel;
using System.Data;
using System.Net;
using Dapper;
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
            //
            //DownloadFile("https://rekturacjazadanie.blob.core.windows.net/zadanie/Products.csv", @"C:\Users\louis\Pliki\Products.csv");
            //
            //DownloadFile("https://rekturacjazadanie.blob.core.windows.net/zadanie/Inventory.csv", @"C:\Users\louis\Pliki\Inventory.csv");
            //
            //DownloadFile("https://rekturacjazadanie.blob.core.windows.net/zadanie/Products.csv", @"C:\Users\louis\Pliki\Prices.csv");

            if (File.Exists("C:\\Users\\louis\\Pliki\\Products.csv"))
            {
                foreach (string line in File.ReadAllLines("C:\\Users\\louis\\Pliki\\Products.csv").Where(m => m.Split("\";\"").ElementAt<string>(8).Contains("0") && m.Split("\";\"").ElementAt<string>(9).Contains("24h")))
                {
                    string[] strings = line.Split("\";\"");
                    string queryins = "INSERT INTO dbo.Products (sku,name,ean,producer_name,category,is_wire,shipping,available,is_vendor,default_image) VALUES (@sku,@name,@ean,@producer_name,@category,@is_wire,@shipping,@available,@is_vendor,@default_image)";
                    
                    var parameters = new DynamicParameters();
                    parameters.Add("sku", strings[1].Replace("\"",""), DbType.String);
                    parameters.Add("name", strings[2], DbType.String);
                    parameters.Add("ean", strings[4], DbType.String);
                    parameters.Add("producer_name", strings[6], DbType.String);
                    parameters.Add("category", strings[7], DbType.String);
                    parameters.Add("is_wire", strings[8], DbType.String);
                    parameters.Add("shipping", strings[9], DbType.String);
                    parameters.Add("available", strings[11], DbType.String);
                    parameters.Add("is_vendor", strings[16], DbType.String);
                    parameters.Add("default_image", strings[18], DbType.String);
                    /*
                    Products produkt = new Products();
                    produkt.sku = strings[1].Replace("\"", "");
                    produkt.name = strings[2];
                    produkt.ean = strings[4];
                    produkt.producer_name = strings[6];
                    produkt.category = strings[7];
                    produkt.is_wire = strings[8];
                    produkt.shipping = strings[9];
                    produkt.available = strings[11];
                    produkt.is_vendor = strings[16];
                    produkt.default_image = strings[18];
                    */
                    using (var connecting = this.context.CreateConnection())
                    {
                        await connecting.ExecuteAsync(queryins, parameters);
                    }
                }
            }
            if (File.Exists("C:\\Users\\louis\\Pliki\\Inventory.csv"))
            {
                string query = "SELECT SKU FROM products";
                using (var connection = context.CreateConnection())
                {
                    var productslist = await connection.QueryAsync<Products>(query);
                    foreach (var product in productslist)
                    {
                        foreach (string line in File.ReadAllLines("C:\\Users\\louis\\Pliki\\Inventory.csv").Where(m => m.Split(',').ElementAt<string>(8).Contains(product.sku)))
                        {
                            string[] strings = line.Split(",");
                            string queryins = "INSERT INTO Inventory(product_id,sku,unit,qty,manufacturer,shipping,shipping_cost) VALUES (@product_id,@sku,unit,@qty,@manufacturer,@shipping,@shipping_cost)";
                            var parameters = new DynamicParameters();
                            parameters.Add("product_id", strings[0], DbType.String);
                            parameters.Add("sku", strings[0], DbType.String);
                            parameters.Add("unit", strings[0], DbType.String);
                            parameters.Add("qty", strings[0], DbType.VarNumeric);
                            parameters.Add("manufacturer", strings[0], DbType.String);
                            parameters.Add("shipping", strings[0], DbType.String);
                            parameters.Add("shipping_cost", strings[0], DbType.VarNumeric);
                            using (var connecting = this.context.CreateConnection())
                            {
                                await connecting.ExecuteAsync(queryins, parameters);
                            }
                        }
                    }
                }

            }
            if (File.Exists("C:\\Users\\louis\\Pliki\\Prices.csv"))
            {
                string query = "SELECT SKU FROM products";
                using (var connection = context.CreateConnection())
                {
                    var productslist = await connection.QueryAsync<Products>(query);
                    foreach(var product in productslist)
                    {
                        foreach (string line in File.ReadAllLines("C:\\Users\\louis\\Pliki\\Prices.csv").Where(m => m.Split(',').ElementAt<string>(8).Contains(product.sku)))
                        {
                            string[] strings = line.Split(",");
                            string queryins = "INSERT INTO Prices(unique_id,product_sku,nett_prod_price,nett_prod_price_disc,VAT,nett_price_logi) VALUES (@unique_id,@product_sku,@nett_prod_price,@nett_prod_price_disc,@VAT,@nett_price_logi)";
                            var parameters = new DynamicParameters();
                            parameters.Add("unique_id", strings[0], DbType.String);
                            parameters.Add("product_sku", strings[0], DbType.String);
                            parameters.Add("nett_prod_price", strings[0], DbType.VarNumeric);
                            parameters.Add("nett_prod_price_disc", strings[0], DbType.VarNumeric);
                            parameters.Add("VAT", strings[0], DbType.String);
                            parameters.Add("nett_price_logi", strings[0], DbType.VarNumeric);

                            using (var connecting = this.context.CreateConnection())
                            {
                                await connecting.ExecuteAsync(queryins, parameters);
                            }
                        }
                    }
                }
            }

            return await Task.FromResult(response);
        }

        public Task<Products> getBySKU(string selectedSKU)
        {
            Products products = new Products();
            string query = "SELECT SKU FROM ";
            using (var connection = context.CreateConnection())
            {

            }
            return Task.FromResult(products);
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
