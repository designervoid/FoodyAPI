using Npgsql;

namespace DataSourceFactory {
    public static class DatabaseConnectionRepository {
        public static NpgsqlDataSource GetDataSource(WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration["pgConnection"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'pgConnection' not found.");
            }

            return NpgsqlDataSource.Create(connectionString);
        }
    }
}