using Infrastructure.Repositorios.Base.Interface;
using Dapper;

namespace Infrastructure.Repositorios.Base
{
    public abstract class RepositorioBase<TEntidade> where TEntidade : class
    {
        protected readonly IDbConnectionFactory _connectionFactory;
        protected readonly string _tableName;

        protected RepositorioBase(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _tableName = typeof(TEntidade).Name + "s"; // convenção simples
        }

        public async Task<TEntidade?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = $"SELECT * FROM {_tableName} WHERE Id = @Id";

            return await connection.QueryFirstOrDefaultAsync<TEntidade>(sql, new { Id = id });
        }

        public async Task<IEnumerable<TEntidade>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = $"SELECT * FROM {_tableName}";

            return await connection.QueryAsync<TEntidade>(sql);
        }

        public async Task<int> InsertAsync(TEntidade entity)
        {
            using var connection = _connectionFactory.CreateConnection();

            var properties = typeof(TEntidade).GetProperties()
                .Where(p => p.Name != "Id");

            var columns = string.Join(", ", properties.Select(p => p.Name));
            var values = string.Join(", ", properties.Select(p => "@" + p.Name));

            var sql = $@"
            INSERT INTO {_tableName} ({columns})
            VALUES ({values})
            RETURNING id;
        ";

            return await connection.ExecuteScalarAsync<int>(sql, entity);
        }

        public async Task<bool> UpdateAsync(TEntidade entity)
        {
            using var connection = _connectionFactory.CreateConnection();

            var properties = typeof(TEntidade).GetProperties()
                .Where(p => p.Name != "Id");

            var setClause = string.Join(", ",
                properties.Select(p => $"{p.Name} = @{p.Name}"));

            var sql = $@"
            UPDATE {_tableName}
            SET {setClause}
            WHERE Id = @Id
        ";

            var rows = await connection.ExecuteAsync(sql, entity);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = $"DELETE FROM {_tableName} WHERE Id = @Id";

            var rows = await connection.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
    }
}

