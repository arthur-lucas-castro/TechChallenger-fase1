using Dapper;
using Compartilhado.Infrastructure.Repositories.Interface;

namespace Compartilhado.Infrastructure.Repositories
{
    public abstract class RepositorioBase<TEntidade> where TEntidade : class
    {
        protected readonly IDbConnectionFactory _connectionFactory;
        protected readonly string _tableName;

        protected RepositorioBase(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _tableName = typeof(TEntidade).Name + "s";
        }

        public async Task<TEntidade?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<TEntidade>(
                $"SELECT * FROM {_tableName} WHERE Id = @Id", new { Id = id });
        }

        public async Task<IEnumerable<TEntidade>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<TEntidade>($"SELECT * FROM {_tableName}");
        }

        public async Task<int> InsertAsync(TEntidade entity)
        {
            using var connection = _connectionFactory.CreateConnection();

            var properties = typeof(TEntidade).GetProperties().Where(p => p.Name != "Id");
            var columns = string.Join(", ", properties.Select(p => p.Name));
            var values  = string.Join(", ", properties.Select(p => "@" + p.Name));

            return await connection.ExecuteScalarAsync<int>(
                $"INSERT INTO {_tableName} ({columns}) VALUES ({values}) RETURNING id;", entity);
        }

        public async Task<bool> UpdateAsync(TEntidade entity)
        {
            using var connection = _connectionFactory.CreateConnection();

            var properties = typeof(TEntidade).GetProperties().Where(p => p.Name != "Id");
            var setClause  = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));

            var rows = await connection.ExecuteAsync(
                $"UPDATE {_tableName} SET {setClause} WHERE Id = @Id", entity);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var rows = await connection.ExecuteAsync(
                $"DELETE FROM {_tableName} WHERE Id = @Id", new { Id = id });
            return rows > 0;
        }
    }
}
