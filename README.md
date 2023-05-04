# DevicesMonitor
Is a test application that simulate temperature Devices that send their readings to a service that monitor these devices and display the readings from them.
The devices generate temperature messages randomlly with numbers between 20 and 30 and the service recieve these messages and display the following data:

- Device number
- TimeStamp
- Last temperature received 
- Average temperatures for the device based on ( sum of all temperatures / count )
- Total messages received from the device until now

The solution uses sockets to send and receive messages between the devices and the monitoring service. 
The service is listening to localhost ( 127.0.0.1 ) on port (12345).
In the beginning of the application I initialize 3 devices in separate tasks to start send messages and start the service on another task to start listening.