using System.Net.Sockets;

var builder = DistributedApplication.CreateBuilder(args);

// 1. Database Services
var username = builder.AddParameter("username", "postgres", secret: true);
var password = builder.AddParameter("password", "postgres", secret: true);

var postgres = builder.AddPostgres("postgres", username, password)
    .WithImage("postgres:latest")
    .WithEndpoint(port: 5432, targetPort: 5432, name: "postgres")
    .WithArgs(
        "-c", "wal_level=logical",
        "-c", "max_prepared_transactions=10"
    )
    .WithDataVolume("postgres-data")
    .WithLifetime(ContainerLifetime.Persistent);

var flightDb = postgres.AddDatabase("flight");
var passengerDb = postgres.AddDatabase("passenger");
var identityDb = postgres.AddDatabase("identity");
var persistMessageDb = postgres.AddDatabase("persist-message");

var mongoUsername = builder.AddParameter("mongo-username", "root");
var mongoPassword = builder.AddParameter("mongo-password", "secret", secret: true);

var mongo = builder.AddMongoDB("mongo", userName: mongoUsername, password: mongoPassword)
    .WithImage("mongo:latest")
    .WithEndpoint(port: 27017, targetPort: 27017, name: "mongo")
    .WithDataVolume("mongo-data")
    .WithLifetime(ContainerLifetime.Persistent);

var redis = builder.AddRedis("redis")
    .WithImage("redis:latest")
    .WithEndpoint(port: 6379, targetPort: 6379, name: "redis")
    .WithDataVolume("redis-data")
    .WithLifetime(ContainerLifetime.Persistent);

var eventstore = builder.AddEventStore("eventstore")
    .WithImage("eventstore/eventstore")
    .WithEnvironment("EVENTSTORE_CLUSTER_SIZE", "1")
    .WithEnvironment("EVENTSTORE_RUN_PROJECTIONS", "All")
    .WithEnvironment("EVENTSTORE_START_STANDARD_PROJECTIONS", "True")
    .WithEnvironment("EVENTSTORE_INSECURE", "True")
    .WithEnvironment("EVENTSTORE_ENABLE_ATOM_PUB_OVER_HTTP", "True")
    .WithHttpEndpoint(port: 2113, targetPort: 2113, name: "eventstore-http")
    .WithDataVolume("eventstore-data")
    .WithLifetime(ContainerLifetime.Persistent);

// 2. Messaging Services
var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithImage("rabbitmq:management")
    .WithEndpoint(port: 5672, targetPort: 5672, name: "rabbitmq-amqp")
    .WithEndpoint(port: 15672, targetPort: 15672, name: "rabbitmq-management")
    .WithLifetime(ContainerLifetime.Persistent);

// 3. Observability Services
var jaeger = builder.AddContainer("jaeger-all-in-one", "jaegertracing/all-in-one")
    .WithEndpoint(port: 6831, targetPort: 6831, name: "jaeger-udp", protocol: ProtocolType.Udp)
    .WithEndpoint(port: 16686, targetPort: 16686, name: "jaeger-ui")
    .WithEndpoint(port: 14268, targetPort: 14268, name: "jaeger-api")
    .WithEndpoint(port: 14317, targetPort: 4317, name: "jaeger-otlp-grpc")
    .WithEndpoint(port: 14318, targetPort: 4318, name: "jaeger-otlp-http")
    .WithLifetime(ContainerLifetime.Persistent);

var zipkin = builder.AddContainer("zipkin-all-in-one", "openzipkin/zipkin")
    .WithEndpoint(port: 9411, targetPort: 9411, name: "zipkin-api")
    .WithLifetime(ContainerLifetime.Persistent);

var otelCollector = builder.AddContainer("otel-collector", "otel/opentelemetry-collector-contrib")
    .WithBindMount("../../../../deployments/configs/otel-collector-config.yaml", "/etc/otelcol-contrib/config.yaml", isReadOnly: true)
    .WithArgs("--config=/etc/otelcol-contrib/config.yaml")
    .WithEndpoint(port: 11888, targetPort: 1888, name: "otel-pprof")
    .WithEndpoint(port: 8888, targetPort: 8888, name: "otel-metrics")
    .WithEndpoint(port: 8889, targetPort: 8889, name: "otel-exporter-metrics")
    .WithEndpoint(port: 13133, targetPort: 13133, name: "otel-health")
    .WithEndpoint(port: 4317, targetPort: 4317, name: "otel-grpc")
    .WithEndpoint(port: 4318, targetPort: 4318, name: "otel-http")
    .WithEndpoint(port: 55679, targetPort: 55679, name: "otel-zpages")
    .WithLifetime(ContainerLifetime.Persistent);

var prometheus = builder.AddContainer("prometheus", "prom/prometheus")
    .WithBindMount("../../../../deployments/configs/prometheus.yaml", "/etc/prometheus/prometheus.yml")
    .WithArgs(
        "--config.file=/etc/prometheus/prometheus.yml",
        "--storage.tsdb.path=/prometheus",
        "--web.console.libraries=/usr/share/prometheus/console_libraries",
        "--web.console.templates=/usr/share/prometheus/consoles",
        "--web.enable-remote-write-receiver")
    .WithEndpoint(port: 9090, targetPort: 9090, name: "prometheus-web")
    .WithLifetime(ContainerLifetime.Persistent);

var grafana = builder.AddContainer("grafana", "grafana/grafana")
    .WithEnvironment("GF_INSTALL_PLUGINS", "grafana-clock-panel,grafana-simple-json-datasource")
    .WithEnvironment("GF_SECURITY_ADMIN_USER", "admin")
    .WithEnvironment("GF_SECURITY_ADMIN_PASSWORD", "admin")
    .WithEnvironment("GF_FEATURE_TOGGLES_ENABLE", "traceqlEditor")
    .WithBindMount("../../../../deployments/configs/grafana/provisioning", "/etc/grafana/provisioning")
    .WithBindMount("../../../../deployments/configs/grafana/dashboards", "/var/lib/grafana/dashboards")
    .WithEndpoint(port: 3000, targetPort: 3000, name: "grafana-web")
    .WithLifetime(ContainerLifetime.Persistent);

var nodeExporter = builder.AddContainer("node-exporter", "prom/node-exporter")
    .WithBindMount("/proc", "/host/proc", isReadOnly: true)
    .WithBindMount("/sys", "/host/sys", isReadOnly: true)
    .WithBindMount("/", "/rootfs", isReadOnly: true)
    .WithArgs(
        "--path.procfs=/host/proc",
        "--path.rootfs=/rootfs",
        "--path.sysfs=/host/sys")
    .WithEndpoint(port: 9101, targetPort: 9100, name: "node-exporter")
    .WithLifetime(ContainerLifetime.Persistent);

var tempo = builder.AddContainer("tempo", "grafana/tempo")
    .WithBindMount("../../../../deployments/configs/tempo.yaml", "/etc/tempo.yaml", isReadOnly: true)
    .WithArgs("--config.file=/etc/tempo.yaml")
    .WithEndpoint(port: 3200, targetPort: 3200, name: "tempo")
    .WithEndpoint(port: 24317, targetPort: 4317, name: "tempo-otlp-grpc")
    .WithEndpoint(port: 24318, targetPort: 4318, name: "tempo-otlp-http")
    .WithLifetime(ContainerLifetime.Persistent);

var loki = builder.AddContainer("loki", "grafana/loki")
    .WithBindMount("../../../../deployments/configs/loki-config.yaml", "/etc/loki/local-config.yaml", isReadOnly: true)
    .WithArgs("-config.file=/etc/loki/local-config.yaml")
    .WithEndpoint(port: 3100, targetPort: 3100, name: "loki")
    .WithLifetime(ContainerLifetime.Persistent);

var elasticsearch = builder.AddElasticsearch("elasticsearch")
    .WithImage("docker.elastic.co/elasticsearch/elasticsearch:8.17.0")
    .WithEnvironment("discovery.type", "single-node")
    .WithEnvironment("cluster.name", "docker-cluster")
    .WithEnvironment("node.name", "docker-node")
    .WithEnvironment("ES_JAVA_OPTS", "-Xms512m -Xmx512m")
    .WithEnvironment("xpack.security.enabled", "false")
    .WithEnvironment("xpack.security.http.ssl.enabled", "false")
    .WithEnvironment("xpack.security.transport.ssl.enabled", "false")
    .WithEnvironment("network.host", "0.0.0.0")
    .WithEnvironment("http.port", "9200")
    .WithEnvironment("transport.host", "localhost")
    .WithEnvironment("bootstrap.memory_lock", "true")
    .WithEnvironment("cluster.routing.allocation.disk.threshold_enabled", "false")
    .WithEndpoint(port: 9200, targetPort: 9200, name: "elasticsearch-http")
    .WithEndpoint(port: 9300, targetPort: 9300, name: "elasticsearch-transport")
    .WithDataVolume("elastic-data")
    .WithLifetime(ContainerLifetime.Persistent);

var kibana = builder.AddContainer("kibana", "docker.elastic.co/kibana/kibana:8.17.0")
    .WithEnvironment("ELASTICSEARCH_HOSTS", "http://elasticsearch:9200")
    .WithEndpoint(port: 5601, targetPort: 5601, name: "kibana")
    .WithReference(elasticsearch)
    .WaitFor(elasticsearch)
    .WithLifetime(ContainerLifetime.Persistent);


// 5. Application Services
var identity = builder.AddProject<Projects.Identity_Api>("identity-service")
    .WithReference(persistMessageDb)
    .WaitFor(persistMessageDb)
    .WithReference(identityDb)
    .WaitFor(identityDb)
    .WithReference(mongo)
    .WaitFor(mongo)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithHttpEndpoint(port: 6005, name: "identity-http")
    .WithHttpsEndpoint(port: 5005, name: "identity-https");

var passenger = builder.AddProject<Projects.Passenger_Api>("passenger-service")
    .WithReference(persistMessageDb)
    .WaitFor(persistMessageDb)
    .WithReference(passengerDb)
    .WaitFor(passengerDb)
    .WithReference(mongo)
    .WaitFor(mongo)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithHttpEndpoint(port: 6012, name: "passenger-http")
    .WithHttpsEndpoint(port: 5012, name: "passenger-https");

var flight = builder.AddProject<Projects.Flight_Api>("flight-service")
    .WithReference(persistMessageDb)
    .WaitFor(persistMessageDb)
    .WithReference(flightDb)
    .WaitFor(flightDb)
    .WithReference(mongo)
    .WaitFor(mongo)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithHttpEndpoint(port: 5004, name: "flight-http")
    .WithHttpsEndpoint(port: 5003, name: "flight-https");

var booking = builder.AddProject<Projects.Booking_Api>("booking-service")
    .WithReference(persistMessageDb)
    .WaitFor(persistMessageDb)
    .WithReference(eventstore)
    .WaitFor(eventstore)
    .WithReference(mongo)
    .WaitFor(mongo)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithHttpEndpoint(port: 6010, name: "booking-http")
    .WithHttpsEndpoint(port: 5010, name: "booking-https");

var gateway = builder.AddProject<Projects.ApiGateway>("api-gateway")
    .WithReference(flight)
    .WaitFor(flight)
    .WithReference(passenger)
    .WaitFor(passenger)
    .WithReference(identity)
    .WaitFor(identity)
    .WithReference(booking)
    .WaitFor(booking)
    .WithHttpEndpoint(port: 5001, name: "gateway-http")
    .WithHttpsEndpoint(port: 5000, name: "gateway-https");

builder.Build().Run();
