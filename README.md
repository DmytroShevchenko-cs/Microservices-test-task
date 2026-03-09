# Microservices Test Task

Distributed system with three microservices: asynchronous communication via RabbitMQ, synchronous via gRPC.

## Technologies

- **.NET 9.0**
- **MassTransit** - RabbitMQ integration
- **RabbitMQ** - Message broker
- **gRPC** - Synchronous communication
- **Docker & Docker Compose**

## Components

### Service A (Port 5001)
- REST API endpoint `/api/start`
- Publishes messages to RabbitMQ
- Consumes messages from `service-a-queue` and `service-b-queue`
- gRPC client for Service C

### Service B (Port 5002)
- Consumes messages from `service-b-queue`
- Publishes results back to RabbitMQ

### Service C (Port 5003)
- gRPC server for transaction finalization
- Health check at `/health`

### RabbitMQ
- AMQP: 5672
- Management UI: http://localhost:15672 (guest/guest)

## Quick Start

### Docker

```bash
# Create .env file
nano .env

RABBIT_USER=guest
RABBIT_PASS=guest
RABBIT_HOST=rabbitmq
RABBIT_PORT=5672
SERVICE_C_URL=http://servicec:8080

# Start
docker-compose up -d

# Test
curl -X POST http://localhost:5001/api/start
```

## API

**POST** `http://localhost:5001/api/start` - Start transaction


## Configuration

`.env` file:
```bash
RABBIT_USER=guest
RABBIT_PASS=guest
RABBIT_HOST=rabbitmq
RABBIT_PORT=5672

SERVICE_C_URL=http://servicec:8080

ASPNETCORE_URLS=http://0.0.0.0:8080
ASPNETCORE_ENVIRONMENT=Production
```

## Workflow

1. Client → REST POST → Service A
2. Service A → RabbitMQ → Service A + Service B (async)
3. Service A + Service B → Process messages
4. Service A → gRPC → Service C (sync)
5. Service A → Response → Client

## UPD