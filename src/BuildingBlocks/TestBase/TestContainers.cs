using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

namespace BuildingBlocks.TestBase;

public static class TestContainers
{
    public static MsSqlTestcontainer SqlTestContainer => new TestcontainersBuilder<MsSqlTestcontainer>()
        .WithDatabase(
            new MsSqlTestcontainerConfiguration {Database = "sql_test_db", Password = "localpassword#123uuuuu"})
        .WithImage("mcr.microsoft.com/mssql/server:2017-latest")
        .WithCleanUp(true)
        .Build();

    public static MsSqlTestcontainer SqlPersistTestContainer => new TestcontainersBuilder<MsSqlTestcontainer>()
        .WithDatabase(new MsSqlTestcontainerConfiguration
        {
            Database = "sql_test_persist_db", Password = "localpassword#123oooo"
        })
        .WithImage("mcr.microsoft.com/mssql/server:2017-latest")
        .WithCleanUp(true)
        .Build();

    public static MongoDbTestcontainer MongoTestContainer => new TestcontainersBuilder<MongoDbTestcontainer>()
        .WithDatabase(new MongoDbTestcontainerConfiguration()
        {
            Database = "mongo_test_db", Username = "mongo_db", Password = "mongo_db_pass"
        })
        .WithImage("mongo")
        .WithCleanUp(true)
        .Build();
}
