namespace TelegramMonitor;

public static class SqlSugarSetup
{
    public static void AddSqlSugarSetup(this IServiceCollection services)
    {
        var config = App.GetConfig<DbConnectionOptions>("DbConnection");

        if (!Enum.TryParse<DbType>(config.DbType, true, out var dbType))
        {
            throw new InvalidOperationException($"无效的数据库类型: {config.DbType}");
        }

        var sqlSugar = new SqlSugarScope(
            new ConnectionConfig
            {
                DbType = dbType,
                ConnectionString = config.ConnectionString,
                IsAutoCloseConnection = true
            },
            db => { }
        );

        services.AddSingleton<ISqlSugarClient>(sqlSugar);
        services.AddScoped<SqlSugarScope>(s => sqlSugar);

        InitializeDatabase(sqlSugar);
        services.AddSingleton<SystemCacheServices>();
    }

    private static void InitializeDatabase(ISqlSugarClient db)
    {
        db.DbMaintenance.CreateDatabase();

        InitializeTable<KeywordConfig>(db);
    }

    private static void InitializeTable<T>(ISqlSugarClient db) where T : class, new()
    {
        string tableName = db.EntityMaintenance.GetEntityInfo<T>().DbTableName;

        if (!db.DbMaintenance.IsAnyTable(tableName))
        {
            db.CodeFirst.InitTables<T>();
            Log.Information($"表 {tableName} 已创建");
        }
    }
}