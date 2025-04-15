using System.Data;
using Microsoft.Data.SqlClient;

namespace RRAPIREST.Model.Data
{
    public class DapperDbContext
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionstring;
        public DapperDbContext(IConfiguration configuration) { 
            this._configuration = configuration;
            this.connectionstring = _configuration.GetConnectionString("connection");
        }
        public IDbConnection CreateConnection()=>new SqlConnection(connectionstring);   
        
    }
}
