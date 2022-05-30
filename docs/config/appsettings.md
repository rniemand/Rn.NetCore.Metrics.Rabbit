[Home](/README.md) / [Configuration](/docs/configuration/README.md) / appsettings.json

# appsettings.json
More to come...

```json
{
  "Rn.Metrics.Rabbit": {
    "enabled": true,
    "username": "richardn",
    "password": "password",
    "virtualHost": "/",
    "host": "myserver.local",
    "port": 5672,
    "exchange": "amq.topic",
    "routingKey": "rn_core.metrics",
    "backOffTimeSec": 15,
    "coolDownTimeSec": 300,
    "coolDownThreshold": 3,
    "maxCoolDownRuns": 5
  }
}
```

Details on each option is listed below.

| Property | Type | Required | Default | Notes |
| --- | --- | ---- | ---- | --- |
| `enabled` | `bool` | required | `false` | Enables outputting to RabbitMQ. |
| `username` | `string` | optional | `guest` | Connection username. |
| `password` | `string` | optional | `guest` | Connection password. |
| `virtualHost` | `string` | optional | `/` | Connection virtual host. |
| `host` | `string` | optional | `127.0.0.1` | Connection host name. |
| `port` | `int` | optional | `5672` | Connection port. |
| `exchange` | `string` | optional | `amq.topic` | Exchange to connect to. |
| `routingKey` | `string` | optional | `rn_core.metrics` | Metrics routing key. |
| `backOffTimeSec` | `int` | optional | `15` | Back off time when failing to connect. |
| `coolDownTimeSec` | `int` | optional | `300` | Cooldown time when unable to connect. |
| `coolDownThreshold` | `int` | optional | `3` | Total count of cooldowns before disabling. |
| `maxCoolDownRuns` | `int` | optional | `0` | Max number of cooldowns before disabling the output. |
