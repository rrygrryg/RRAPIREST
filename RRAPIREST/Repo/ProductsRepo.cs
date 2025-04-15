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
            DownloadFile("https://rekturacjazadanie.blob.core.windows.net/zadanie/Products.csv", @"C:\Users\louis\Pliki\Products.csv");
            //
            DownloadFile("https://rekturacjazadanie.blob.core.windows.net/zadanie/Inventory.csv", @"C:\Users\louis\Pliki\Inventory.csv");
            //
            DownloadFile("https://rekturacjazadanie.blob.core.windows.net/zadanie/Products.csv", @"C:\Users\louis\Pliki\Prices.csv");

            if (File.Exists("https://rekturacjazadanie.blob.core.windows.net/zadanie/Products.csv"))
            {
                foreach (string line in File.ReadAllLines("https://rekturacjazadanie.blob.core.windows.net/zadanie/Products.csv").Where(m => m.Split(',').ElementAt<string>(8).Contains("0") && m.Split(',').ElementAt<string>(9).Contains("24h")))
                {
                    string[] strings = line.Split(",");
                    string queryins = "INSERT INTO Products(sku,name,ean,producer_name,category,is_wire,shipping,available,is_vendor,default_image) VALUES (@sku,@name,@ean,@producer_name,@category,@is_wire,@shipping,@available,@is_vendor,@default_image)";
                    var parameters = new DynamicParameters();
                    parameters.Add("sku", strings[0], DbType.String);
                    parameters.Add("name", strings[0], DbType.String);
                    parameters.Add("ean", strings[0], DbType.String);
                    parameters.Add("producer_name", strings[0], DbType.String);
                    parameters.Add("category", strings[0], DbType.String);
                    parameters.Add("is_wire", strings[0], DbType.String);
                    parameters.Add("shipping", strings[0], DbType.String);
                    parameters.Add("available", strings[0], DbType.String);
                    parameters.Add("is_vendor", strings[0], DbType.String);
                    parameters.Add("default_image", strings[0], DbType.String);
                    using (var connecting = this.context.CreateConnection())
                    {
                        await connecting.ExecuteAsync(queryins, parameters);
                    }
                }
            }
            if (File.Exists("https://rekturacjazadanie.blob.core.windows.net/zadanie/Inventory.csv"))
            {
                string query = "SELECT SKU FROM products";
                using (var connection = context.CreateConnection())
                {
                    var productslist = await connection.QueryAsync<Products>(query);
                    foreach (var product in productslist)
                    {
                        foreach (string line in File.ReadAllLines("https://rekturacjazadanie.blob.core.windows.net/zadanie/Inventory.csv").Where(m => m.Split(',').ElementAt<string>(8).Contains(product.sku)))
                        {
                            string[] strings = line.Split(",");
                            string queryins = "INSERT INTO Inventory(product_id,sku,unit,qty,manufacturer,shipping,shipping_cost) VALUES (@product_id,@sku,unit,@qty,@manufacturer,@shipping,@shipping_cost)";
                            var parameters = new DynamicParameters();
                            parameters.Add("sku", strings[0], DbType.String);
                            parameters.Add("name", strings[0], DbType.String);
                            parameters.Add("ean", strings[0], DbType.String);
                            parameters.Add("producer_name", strings[0], DbType.String);
                            parameters.Add("category", strings[0], DbType.String);
                            parameters.Add("is_wire", strings[0], DbType.String);
                            parameters.Add("shipping", strings[0], DbType.String);
                            parameters.Add("available", strings[0], DbType.String);
                            parameters.Add("is_vendor", strings[0], DbType.String);
                            parameters.Add("default_image", strings[0], DbType.String);
                            using (var connecting = this.context.CreateConnection())
                            {
                                await connecting.ExecuteAsync(queryins, parameters);
                            }
                        }
                    }
                }

            }
            if (File.Exists("https://rekturacjazadanie.blob.core.windows.net/zadanie/Prices.csv"))
            {
                string query = "SELECT SKU FROM products";
                using (var connection = context.CreateConnection())
                {
                    var productslist = await connection.QueryAsync<Products>(query);
                    foreach(var product in productslist)
                    {
                        foreach (string line in File.ReadAllLines("https://rekturacjazadanie.blob.core.windows.net/zadanie/Prices.csv").Where(m => m.Split(',').ElementAt<string>(8).Contains(product.sku)))
                        {
                            string[] strings = line.Split(",");
                            string queryins = "INSERT INTO Products(sku,name,ean,producer_name,category,is_wire,shipping,available,is_vendor,default_image) VALUES (@sku,@name,@ean,@producer_name,@category,@is_wire,@shipping,@available,@is_vendor,@default_image)";
                            var parameters = new DynamicParameters();
                            parameters.Add("sku", strings[0], DbType.String);
                            parameters.Add("name", strings[0], DbType.String);
                            parameters.Add("ean", strings[0], DbType.String);
                            parameters.Add("producer_name", strings[0], DbType.String);
                            parameters.Add("category", strings[0], DbType.String);
                            parameters.Add("is_wire", strings[0], DbType.String);
                            parameters.Add("shipping", strings[0], DbType.String);
                            parameters.Add("available", strings[0], DbType.String);
                            parameters.Add("is_vendor", strings[0], DbType.String);
                            parameters.Add("default_image", strings[0], DbType.String);
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
