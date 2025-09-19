namespace RapasIMCEEApi
{
    public static class EnvrionnementVariableHelper
    {
        public static string GetConnectionStringFromEnv()
        {
            var mysqlUrl = Environment.GetEnvironmentVariable("MYSQL_URL");
            if (!string.IsNullOrWhiteSpace(mysqlUrl)) return mysqlUrl;

            var host = Environment.GetEnvironmentVariable("MYSQLHOST");
            var port = Environment.GetEnvironmentVariable("MYSQLPORT") ?? "3306";
            var user = Environment.GetEnvironmentVariable("MYSQLUSER");
            var password = Environment.GetEnvironmentVariable("MYSQLPASSWORD");
            var database = Environment.GetEnvironmentVariable("MYSQLDATABASE");
            return $"Server={host};Port={port};Database={database};User={user};Password={password};";
        }
    }
}
