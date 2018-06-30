import * as signalR from '@aspnet/signalr';

const server = 'http://localhost:60860';

async function start() {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl(`${server}/hub/client`)
        .build();

    connection.on("Kicked", () => {
        connection.stop();
        console.log('Kicked :(')
    });

    try {
        await connection.start();
        console.log('Connection was started');

        await new Promise(resolve => setTimeout(resolve, 3000));
        connection.send("Connect", "KickMe");
        
    } catch(err) {
        console.error('Connection error', err.toString());
    }
}

start();