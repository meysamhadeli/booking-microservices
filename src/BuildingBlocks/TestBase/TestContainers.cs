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
        .WithPortBinding(5432, true)
        .WithCleanUp(true)
        .Build();

    public static PostgreSqlTestcontainer PostgresPersistTestContainer => new TestcontainersBuilder<PostgreSqlTestcontainer>()
        .WithDatabase(
            new PostgreSqlTestcontainerConfiguration
            {
                Database = Guid.NewGuid().ToString("D"),
                Password = Guid.NewGuid().ToString("D"),
                Username = Guid.NewGuid().ToString("D")
            })
        .WithImage("postgres:latest")
        .WithPortBinding(5432, true)
        .WithCleanUp(true)
        .Build();


    public static MongoDbTestcontainer MongoTestContainer => new TestcontainersBuilder<MongoDbTestcontainer>()
        .WithDatabase(new MongoDbTestcontainerConfiguration()
        {
            Database = Guid.NewGuid().ToString("D"),
            Username = Guid.NewGuid().ToString("D"),
            Password = Guid.NewGuid().ToString("D"),
        })
        .WithImage("mongo:5")
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
