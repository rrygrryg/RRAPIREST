using RRAPIREST.Model;

namespace RRAPIREST.Repo
{
    public interface IProductsRepo
    {
        Task<DataProduct> GetBySKU(string selectedSKU);

        Task<string> CreateDatabase(string databaseName);

    }
}
