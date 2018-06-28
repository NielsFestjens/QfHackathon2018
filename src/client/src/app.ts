import * as signalR from '@aspnet/signalr';

const server = 'http://localhost:60860';

function start() {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl(`${server}/hub/client`)
        .build();

    connection.on("Kicked", () => {
        connection.stop();
        console.log('Kicked :(')
    });

    connection.start()
        .catch(err => console.error(err.toString()))
        .then(() => {
            connection.send("Connect", "KickMe")
        });
}

start();