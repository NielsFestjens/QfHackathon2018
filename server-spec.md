# Server Specification
This document will detail the messages sent between the components of the game system. These components being the **Client**, the **Admin** and the **Server**. The **Client** can be of a **Bot** type or a **Spectator** type.

## Client to Server

#### Connect
This action is **explicit**. The client should invoke this Hub method after it has connected to the SignalR server. Merely connecting to the server using the @aspnet/signalr library is not enough for the server to accept the client.



Javascript invocation:
```js
connection.invoke("Connect", name, apiKey);
```
Hub method:
```csharp
Task Connect(string name, Guid? apiKey);
```

#### Disconnect
This action is **implicit**. When the client disconnects from the SignalR connection, the server will see this as a Client disconnect. The server cannot programmatically disconnect the client, the client needs to stop the connection.

Javascript invocation:
```js
connection.stop();
```
Detected by overriding a Hub method:
```csharp
override Task OnDisconnectedAsync(Exception exception);
```

#### PlayerActions
One of:
- MoveUp
- MoveDown
- MoveLeft
- MoveRight

Javascript invocations:
```js
connection.invoke("MoveUp");
connection.invoke("MoveDown");
connection.invoke("MoveLeft");
connection.invoke("MoveRight");
```
Hub methods:
```csharp
Task MoveUp();
Task MoveDown();
Task MoveLeft();
Task MoveRight();
```

## Server to Client

#### Kicked
Notify a client that is has been kicked. Then the server will force the client to disconnect.

`IHubContext` invocation:
```csharp
_clientsHub.Clients.Client(connectionId).SendAsync("Kicked");
```

Javascript callback:
```js
connection.on("Kicked", () => { ... });
```

#### Disconnect
Force the client to disconnect from the server.

`IHubContext` invocation:
```csharp
_clientsHub.Clients.Client(connectionId).SendAsync("Disconnect");
```

Javascript callback:
```js
connection.on("Disconnect", () => { connection.stop(); });
```

#### ShowSplashScreen (Spectators only)
The spectator app should display a fullscreen splash screen. The screen should show a *funny* waiting meme or gif ([this one for example](https://i.imgur.com/gsM3Lt5.gif))! An optional message parameter is to be shown if it was included in the message.

`IHubContext` invocation:
```csharp
_clientsHub.Clients.Group("Spectators").SendAsync("ShowSplashScreen", message);
```

Javascript callback:
```js
connection.on("ShowSplashScreen", (message) => { ... });
```

#### ShowSpectateScreen (Spectators only)
The spectator app should display a grid of game rooms. The number of game rooms is sent in the message payload. The app should be able to layout any number of rooms from 1 to 8 at the same time.

## Admin to Server

#### KickClient

Javascript invocation:
```js
connection.stop();
```
Detected by overriding a Hub method:
```csharp
override Task OnDisconnectedAsync(Exception exception);
```

## Server to Admin

#### SpectatorConnected

`IHubContext` invocation:
```csharp
_adminsHub.Clients.All.SendAsync("SpectatorConnected", connectionId, name);
```

Javascript callback:
```js
connection.on("SpectatorConnected", (connectionId, name) => { ... });
```

#### SpectatorDisconnected

`IHubContext` invocation:
```csharp
_adminsHub.Clients.All.SendAsync("SpectatorDisconnected", connectionId);
```

Javascript callback:
```js
connection.on("SpectatorDisconnected", (connectionId) => { ... });
```

#### PlayerConnected

`IHubContext` invocation:
```csharp
_adminsHub.Clients.All.SendAsync("PlayerConnected", connectionId, name);
```

Javascript callback:
```js
connection.on("PlayerConnected", (connectionId, name) => { ... });
```

#### PlayerDisconnected

`IHubContext` invocation:
```csharp
_adminsHub.Clients.All.SendAsync("PlayerDisconnected", connectionId);
```

Javascript callback:
```js
connection.on("PlayerDisconnected", (connectionId) => { ... });
```

