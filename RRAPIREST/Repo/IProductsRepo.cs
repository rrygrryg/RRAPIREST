using RRAPIREST.Model;

namespace RRAPIREST.Repo
{
    public interface IProductsRepo
    {
        Task<Products> getBySKU(string selectedSKU);

        Task<string> CreateDatabase(string databaseName);

    }
}
