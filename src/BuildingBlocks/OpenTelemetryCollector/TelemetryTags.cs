namespace BuildingBlocks.OpenTelemetryCollector;

/// <summary>
/// Telemetry tags use for adding tags to activities as tag name
/// </summary>
public static class TelemetryTags
{
    // https://opentelemetry.io/docs/specs/semconv/general/trace/
    // https://opentelemetry.io/docs/specs/semconv/general/attribute-naming/
    public static class Tracing
    {
        // https://opentelemetry.io/docs/specs/semconv/resource/#service
        // https://opentelemetry.io/docs/specs/semconv/attributes-registry/peer/#peer-attributes
        public static class Service
        {
            public const string PeerService = "peer.service";
            public const string Name = "service.name";
            public const string InstanceId = "service.instance.id";
            public const string Version = "service.version";
            public const string NameSpace = "service.namespace";
        }

        // https://opentelemetry.io/docs/specs/semconv/attributes-registry/messaging/#general-messaging-attributes
        // https://opentelemetry.io/docs/specs/semconv/messaging/messaging-spans/
        public static class Messaging
        {
            // https://opentelemetry.io/docs/specs/semconv/attributes-registry/messaging/#messaging-operation-type
            public static class OperationType
            {
                public const string Key = "messaging.operation.type";
                public const string Receive = "receive";
                public const string Send = "send";
                public const string Process = "process";
            }

            // https://opentelemetry.io/docs/specs/semconv/attributes-registry/messaging/#messaging-system
            public static class System
            {
                public const string Key = "messaging.system";
                public const string ActiveMQ = "activemq";
                public const string RabbitMQ = "rabbitmq";
                public const string AwsSqs = "aws_sqs";
                public const string EventGrid = "eventgrid";
                public const string EventHubs = "eventhubs";
                public const string GcpPubSub = "gcp_pubsub";
                public const string Kafka = "kafka";
                public const string Pulsar = "pulsar";
                public const string ServiceBus = "servicebus";
            }

            public const string Destination = "messaging.destination";
            public const string DestinationKind = "messaging.destination_kind";
            public const string Url = "messaging.url";
            public const string MessageId = "messaging.message_id";
            public const string ConversationId = "messaging.conversation_id";
            public const string CorrelationId = "messaging.correlation_id";
            public const string CausationId = "messaging.causation_id";
            public const string Operation = "messaging.operation";
            public const string OperationName = "messaging.operation.name";
            public const string DestinationName = "messaging.destination.name";
            public const string ConsumerGroup = "messaging.consumer.group.name";
            public const string DestinationPartition = "messaging.destination.partition.id";

            // https://opentelemetry.io/docs/specs/semconv/attributes-registry/messaging/#rabbitmq-attributes
            // https://opentelemetry.io/docs/specs/semconv/messaging/rabbitmq/
            public static class RabbitMQ
            {
                public const string RoutingKey = "messaging.rabbitmq.destination.routing_key";
                public const string DeliveryTag = "messaging.rabbitmq.message.delivery_tag";

                public static IDictionary<string, object?> ProducerTags(
                    string serviceName,
                    string topicName,
                    string routingKey,
                    string? deliveryTag = null
                ) =>
                    new Dictionary<string, object?>
                    {
                        { System.Key, System.Kafka },
                        { DeliveryTag, deliveryTag },
                        { Destination, topicName },
                        { OperationType.Key, OperationType.Send },
                        { Service.Name, serviceName },
                        { RoutingKey, routingKey },
                    };

                public static IDictionary<string, object?> ConsumerTags(
                    string serviceName,
                    string topicName,
                    string routingKey,
                    string? consumerGroup = null
                ) =>
                    new Dictionary<string, object?>
                    {
                        { System.Key, System.Kafka },
                        { Destination, topicName },
                        { OperationType.Key, OperationType.Receive },
                        { Service.Name, serviceName },
                        { ConsumerGroup, consumerGroup },
                        { RoutingKey, routingKey },
                    };
            }

            // https://opentelemetry.io/docs/specs/semconv/attributes-registry/messaging/#kafka-attributes
            // https://opentelemetry.io/docs/specs/semconv/messaging/kafka/
            public static class Kafka
            {
                public const string MessageKey = "messaging.kafka.message.key";
                public const string Tombstone = "messaging.kafka.message.tombstone";
                public const string Offset = "messaging.kafka.offset";

                public static IDictionary<string, object?> ProducerTags(
                    string serviceName,
                    string topicName,
                    string messageKey
                ) =>
                    new Dictionary<string, object?>
                    {
                        { System.Key, System.Kafka },
                        { Destination, topicName },
                        { OperationType.Key, OperationType.Send },
                        { Service.Name, serviceName },
                        { MessageKey, messageKey },
                    };

                public static IDictionary<string, object?> ConsumerTags(
                    string serviceName,
                    string topicName,
                    string messageKey,
                    string partitionName,
                    string consumerGroup
                ) =>
                    new Dictionary<string, object?>
                    {
                        { System.Key, System.Kafka },
                        { Destination, topicName },
                        { OperationType.Key, OperationType.Receive },
                        { Service.Name, serviceName },
                        { MessageKey, messageKey },
                        { DestinationPartition, partitionName },
                        { ConsumerGroup, consumerGroup },
                    };
            }
        }

        // https://opentelemetry.io/docs/specs/semconv/database/database-spans/#common-attributes
        // https://opentelemetry.io/docs/specs/semconv/database/postgresql/#attributes
        public static class Db
        {
            public const string System = "db.system";
            public const string ConnectionString = "db.connection_string";
            public const string User = "db.user";
            public const string MsSqlInstanceName = "db.mssql.instance_name";
            public const string Name = "db.name";
            public const string Statement = "db.statement";
            public const string Operation = "db.operation";
            public const string Instance = "db.instance";
            public const string Url = "db.url";
            public const string CassandraKeyspace = "db.cassandra.keyspace";
            public const string RedisDatabaseIndex = "db.redis.database_index";
            public const string MongoDbCollection = "db.mongodb.collection";
        }

        // https://opentelemetry.io/docs/specs/semconv/exceptions/exceptions-spans/#exception-event
        public static class Exception
        {
            public const string EventName = "exception";
            public const string Type = "exception.type";
            public const string Message = "exception.message";
            public const string Stacktrace = "exception.stacktrace";
        }

        // https://opentelemetry.io/docs/specs/semconv/attributes-registry/otel/#otel-attributes
        public static class Otel
        {
            public const string StatusCode = "otel.status_code";
            public const string StatusDescription = "otel.status_description";
        }

        public static class Message
        {
            public const string Type = "message.type";
            public const string Id = "message.id";
        }

        public static class Application
        {
            public static string AppService = $"{ObservabilityConstant.InstrumentationName}.appservice";
            public static string Consumer = $"{ObservabilityConstant.InstrumentationName}.consumer";
            public static string Producer = $"{ObservabilityConstant.InstrumentationName}.producer";

            public static class Commands
            {
                public static string Command = $"{ObservabilityConstant.InstrumentationName}.command";
                public static string CommandType = $"{Command}.type";
                public static string CommandHandler = $"{Command}.handler";
                public static string CommandHandlerType = $"{CommandHandler}.type";
            }

            public static class Queries
            {
                public static string Query = $"{ObservabilityConstant.InstrumentationName}.query";
                public static string QueryType = $"{Query}.type";
                public static string QueryHandler = $"{Query}.handler";
                public static string QueryHandlerType = $"{QueryHandler}.type";
            }

            public static class Events
            {
                public static string Event = $"{ObservabilityConstant.InstrumentationName}.event";
                public static string EventType = $"{Event}.type";
                public static string EventHandler = $"{Event}.handler";
                public static string EventHandlerType = $"{EventHandler}.type";
            }
        }
    }

    // https://opentelemetry.io/docs/specs/semconv/general/metrics/
    // https://opentelemetry.io/docs/specs/semconv/general/attribute-naming/
    public static class Metrics
    {
        public static class Application
        {
            public static string AppService = $"{ObservabilityConstant.InstrumentationName}.appservice";
            public static string Consumer = $"{ObservabilityConstant.InstrumentationName}.consumer";
            public static string Producer = $"{ObservabilityConstant.InstrumentationName}.producer";

            public static class Commands
            {
                public static string Command = $"{ObservabilityConstant.InstrumentationName}.command";
                public static string CommandType = $"{Command}.type";
                public static string CommandHandler = $"{Command}.handler";
                public static string SuccessCount = $"{CommandHandler}.success.count";
                public static string FaildCount = $"{CommandHandler}.failed.count";
                public static string ActiveCount = $"{CommandHandler}.active.count";
                public static string TotalExecutedCount = $"{CommandHandler}.total.count";
                public static string HandlerDuration = $"{CommandHandler}.duration";
            }

            public static class Queries
            {
                public static string Query = $"{ObservabilityConstant.InstrumentationName}.query";
                public static string QueryType = $"{Query}.type";
                public static string QueryHandler = $"{Query}.handler";
                public static string SuccessCount = $"{QueryHandler}.success.count";
                public static string FaildCount = $"{QueryHandler}.failed.count";
                public static string ActiveCount = $"{QueryHandler}.active.count";
                public static string TotalExecutedCount = $"{QueryHandler}.total.count";
                public static string HandlerDuration = $"{QueryHandler}.duration";
            }

            public static class Events
            {
                public static string Event = $"{ObservabilityConstant.InstrumentationName}.event";
                public static string EventType = $"{Event}.type";
                public static string EventHandler = $"{Event}.handler";
                public static string SuccessCount = $"{EventHandler}.success.count";
                public static string FaildCount = $"{EventHandler}.failed.count";
                public static string ActiveCount = $"{EventHandler}.active.count";
                public static string TotalExecutedCount = $"{EventHandler}.total.count";
                public static string HandlerDuration = $"{EventHandler}.duration";
            }
        }
    }
}