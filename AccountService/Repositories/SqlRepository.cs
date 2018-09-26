using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace AccountService.Repositories
{
    public abstract class SqlRepository
    {
        private AccountServiceOptions _options;

        public SqlRepository(AccountServiceOptions options)
        {
            _options = options;
        }

        public async Task<SqlConnection> GetConnection()
        {
            var conn = new SqlConnection(_options.AppDBConnection);
            await conn.OpenAsync();
            return conn;
        }
    }
}
