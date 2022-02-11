# IoT Dashboard

End-to.end sample to showcase usage of Azure IoT SDK for .NET, best practices for resilency, telemetry consumption, device interaction using PnP capabilities and Digital Twin, etc.

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
dotnet user-secret set "Iot:IotHub" "<YOUR IOT HUB CONNECTION STRING>"
dotnet user-secret set "Iot:StorageAccount" "<YOUR IOT HUB CONNECTION STRING>"
```

### Prerequisites
* Iot Hub
* Message Routes to Built-in endpoint for:
![image](https://user-images.githubusercontent.com/2638875/153589025-2acae73e-bfc5-49d8-bd9b-0599ba7dc7fa.png)
* Consumer group `webapp`
* Any device sending telemetry to IoT Hub, preferable if it is PnP and use Digital Twin

### Quickstart
(Add steps to get up and running quickly)

1. git clone [repository clone url]
2. cd [respository name]
3. ...


## Demo

A demo app is included to show how to use the project.

To run the demo, follow these steps:

(Add steps to start up the demo)

1.
2.
3.

## Resources

(Any additional resources or related projects)

- Link to supporting information
- Link to similar sample
- ...
