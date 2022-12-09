using System;
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
        .WithCleanUp(true)
        .Build();


    public static MsSqlTestcontainer MsSqlTestContainer = new TestcontainersBuilder<MsSqlTestcontainer>()
        .WithDatabase(new MsSqlTestcontainerConfiguration()
        {
            Password = Guid.NewGuid().ToString("D")
        })
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPortBinding(1433, true)
        .WithCleanUp(true)
        .Build();

    public static MsSqlTestcontainer MsSqlPersistTestContainer = new TestcontainersBuilder<MsSqlTestcontainer>()
        .WithDatabase(new MsSqlTestcontainerConfiguration()
        {
            Password = Guid.NewGuid().ToString("D")
        })
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPortBinding(1433, true)
        .WithCleanUp(true)
        .Build();


    public static MongoDbTestcontainer MongoTestContainer => new TestcontainersBuilder<MongoDbTestcontainer>()
        .WithDatabase(new MongoDbTestcontainerConfiguration()
        {
            Database = Guid.NewGuid().ToString("D"),
            Username = Guid.NewGuid().ToString("D"),
            Password = Guid.NewGuid().ToString("D"),
        })
        .WithImage("mongo:4")
        .WithCleanUp(true)
        .Build();


    public static RabbitMqTestcontainer RabbitMqTestContainer => new TestcontainersBuilder<RabbitMqTestcontainer>()
        .WithMessageBroker(new RabbitMqTestcontainerConfiguration()
        {
            Password = "guest",
            Username = "guest"
        })
        .WithImage("rabbitmq:3-management")
        .WithPortBinding(15672, true)
        .WithPortBinding(5672, true)
        .WithCleanUp(true)
        .Build();


}
