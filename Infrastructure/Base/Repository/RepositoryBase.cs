﻿using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace Infrastructure.Base.Repository
{
    public abstract class RepositoryBase<T> : IAsyncGenericRepository<T>
    {
        private readonly string _tableName;
        protected string _connectionString;

        public RepositoryBase(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DbConstr") ?? string.Empty;
            _tableName = typeof(T).Name;
        }

        public async Task<int> AddAsync(T entity)
        {
            var columns = GetColumns();
            var stringOfColumns = string.Join(", ", columns);
            var stringOfParameters = string.Join(", ", columns.Select(e => "@" + e));
            var query = $"insert into {_tableName} ({stringOfColumns}) output inserted.Id values ({stringOfParameters})";

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var result = await conn.ExecuteScalarAsync(query, entity);
                return Convert.ToInt32(result);
            }
        }

        public async Task<int> DeleteAsync(int id, int? userId = 0)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var result = await conn.ExecuteAsync($"update {_tableName} set ModifiedBy=@ModifiedBy,ModifiedDate=@ModifiedDate,Status = @Status  WHERE [Id] = @Id", new { Id = id, ModifiedBy = userId, ModifiedDate = DateTime.UtcNow, Status = false });
                return Convert.ToInt32(result);
            }
        }

        public async Task DeletePermanentAsync(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                await conn.ExecuteAsync($"DELETE FROM {_tableName} WHERE [Id] = @Id", new { Id = id });
            }
        }

        public async Task<T> GetByIdAsync(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var data = await conn.QueryAsync<T>($"SELECT * FROM {_tableName} WHERE Id = @Id", new { Id = id });

                return data.FirstOrDefault();
            }
        }

        public async Task<int> UpdateAsync(T entity)
        {
            var columns = GetColumns();
            var stringOfColumns = string.Join(", ", columns.Select(e => $"{e} = @{e}"));
            var query = $"update {_tableName} set {stringOfColumns} where Id = @Id";

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var result = await conn.ExecuteAsync(query, entity);

                return Convert.ToInt32(result);
            }
        }

        public async Task<int> UpdateAsyncByQuery(string query)
        {
            var sqlQuery = $"update {_tableName} set ";

            if (!string.IsNullOrWhiteSpace(query))
                sqlQuery += query;

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var result = await conn.ExecuteAsync(sqlQuery, null);
                return Convert.ToInt32(result);
            }
        }

        public async Task<IEnumerable<T>> GetByQueryAsync(string where)
        {
            var query = $"select * from {_tableName} ";

            if (!string.IsNullOrWhiteSpace(where))
                query += where;

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var data = await conn.QueryAsync<T>(query, null);
                return data;
            }
        }

        public async Task<IEnumerable<T>> GetListByQueryAsync(string where)
        {
            var query = where;

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var data = await conn.QueryAsync<T>(query, null);
                return data;
            }
        }

        public async Task<T> QueryFirstOrDefaultAsync(string sql, object param)
        {
            var query = $"select * from {_tableName} ";

            if (!string.IsNullOrWhiteSpace(sql))
                query += sql;

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                return await conn.QueryFirstOrDefaultAsync<T>(query);
            }
        }

        public async Task<T> QuerySingleAsync(string sql, object param)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                return await conn.QuerySingleAsync<T>(sql, param);
            }
        }

        public async Task<int> DeleteByQueryAsync(int userId, string where)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var result = await conn.ExecuteAsync($"update {_tableName} set ModifiedBy=@ModifiedBy,ModifiedDate=@ModifiedDate,Status = @Status " + where + "", new { ModifiedBy = userId, ModifiedDate = DateTime.UtcNow, Status = 0 });
                return Convert.ToInt32(result);
            }
        }

        public async Task<int> QueryCountAsync(string where)
        {
            var query = $"select count(id) from {_tableName} ";

            if (!string.IsNullOrWhiteSpace(where))
                query += where;

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                int count = await conn.ExecuteScalarAsync<int>(query);
                return count;
            }
        }

        public async Task<int> DeletePermanentByQuery(string where)
        {
            var query = $"delete from {_tableName} ";

            if (!string.IsNullOrWhiteSpace(where))
                query += where;

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var result = await conn.ExecuteAsync(query, null);
                return result;
            }
        }

        private IEnumerable<string> GetColumns()
        {
            return typeof(T)
                    .GetProperties()
                    //.Where(e => e.Name != "Id" && !e.PropertyType.GetTypeInfo().IsGenericType)
                    .Where(e => e.Name != "Id")
                    .Select(e => e.Name);
        }

        public async Task<T> GetByIdAsync(string id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var data = await conn.QueryAsync<T>($"SELECT * FROM {_tableName} WHERE Id = @Id", new { Id = id });
                return data.FirstOrDefault();
            }

        }
    }
}
