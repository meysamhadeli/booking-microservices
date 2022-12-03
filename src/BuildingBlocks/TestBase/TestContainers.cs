using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

namespace BuildingBlocks.TestBase;

public static class TestContainers
{
    public static PostgreSqlTestcontainer PostgresTestContainer => new TestcontainersBuilder<PostgreSqlTestcontainer>()
        .WithDatabase(
            new PostgreSqlTestcontainerConfiguration
            {
                Database = Guid.NewGuid().ToString("D"),
                Password = Guid.NewGuid().ToString("D"),
                Username = Guid.NewGuid().ToString("D")
            })
        .WithImage("postgres:latest")
        .Build();


    // issue ref: https://github.com/testcontainers/testcontainers-dotnet/discussions/533
    public static MsSqlTestcontainer MsSqlTestContainer = new TestcontainersBuilder<MsSqlTestcontainer>()
        .WithDatabase(new MsSqlTestcontainerConfiguration()
        {
            Password = Guid.NewGuid().ToString("D")
        })
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithExposedPort(1433)
        .WithPortBinding(1433, true) // Add this line for issue in hangup MsSqlTestContainer in docker desktop
        .Build();


    public static MongoDbTestcontainer MongoTestContainer => new TestcontainersBuilder<MongoDbTestcontainer>()
        .WithDatabase(new MongoDbTestcontainerConfiguration()
        {
            Database = Guid.NewGuid().ToString("D"),
            Username = Guid.NewGuid().ToString("D"),
            Password = Guid.NewGuid().ToString("D")
        })
        .WithImage("mongo")
        .Build();


    public static RabbitMqTestcontainer RabbitMqTestContainer => new TestcontainersBuilder<RabbitMqTestcontainer>()
        .WithMessageBroker(new RabbitMqTestcontainerConfiguration() { Password = "guest", Username = "guest" })
        .WithImage("rabbitmq:3-management")
        .Build();
}
