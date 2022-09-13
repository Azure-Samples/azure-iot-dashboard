# IoT Dashboard using PnP & Digital Twin Definition Language

End-to-end sample to showcase usage of Azure IoT SDK for .NET, best practices for telemetry consumption, resilency, device interaction using IoT Plug & Play capabilities and Digital Twin Definition Language (DTDL).

## Features

Starting:

* Telemetry consumption
* Message routing capabilities
* Manage Identities? 
* Failover resilency
* Device Registry Interaction?
* Realtime-device Discovery?
* Device Twin Interaction (based on PnP capabilities)
* ...

## Getting Started

Now using dotnet user secrets to store connection strings:

``` cmd
dotnet user-secret init
dotnet user-secret set "Azure:IotHub:ConnectionString" "<Your Azure Iot Hub Connection String>"
dotnet user-secret set "Azure:CheckpointStorageAccount:ConnectionString" "<Your Azure Storage Account -for checkpointing->"
dotnet user-secret set "Azure:SignalR:ConnectionString" "<Your Azure SignalR service for event broadcasting>" 
dotnet user-secret set "Azure:Redis:ConnectionString" "<Your Azure Redis service for online devices broadcasting>" 
```

### Prerequisites
* Iot Hub Account
* Storage Account for Checkpointing
* SignalR Service
* Azure Redis Cache

Configurations:
* Iot Hub
* Message Routes to Built-in endpoint for:
![image](https://user-images.githubusercontent.com/2638875/153589025-2acae73e-bfc5-49d8-bd9b-0599ba7dc7fa.png)
* Consumer group `webapp`
* Any device sending telemetry to IoT Hub, preferable if it is PnP and use Digital Twin
