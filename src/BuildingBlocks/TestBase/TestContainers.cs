using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

namespace BuildingBlocks.TestBase;

public static class TestContainers
{
    public static MsSqlTestcontainer SqlTestContainer => new TestcontainersBuilder<MsSqlTestcontainer>()
        .WithDatabase(
            new MsSqlTestcontainerConfiguration
            {
                Database = Guid.NewGuid().ToString("D"),
                Password = Guid.NewGuid().ToString("D")
            })
        .WithImage("mcr.microsoft.com/mssql/server:2017-latest")
        .Build();

    public static MsSqlTestcontainer SqlPersistTestContainer => new TestcontainersBuilder<MsSqlTestcontainer>()
        .WithDatabase(new MsSqlTestcontainerConfiguration
        {
            Database = Guid.NewGuid().ToString("D"), Password = Guid.NewGuid().ToString("D")
        })
        .WithImage("mcr.microsoft.com/mssql/server:2017-latest")
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
        .WithMessageBroker(new RabbitMqTestcontainerConfiguration()
        {
            Password = "guest",
            Username = "guest"
        })
        .WithImage("rabbitmq:3-management")
        .Build();
}
