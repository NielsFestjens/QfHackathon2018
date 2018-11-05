import React, { Component, Fragment } from 'react';
import * as signalR from '@aspnet/signalr';

class ConnectToSignalR extends Component {

    connection = null;
    reconnectInterval = null;

    constructor(props) {
        super(props);
        this.state = {
            connecting: true,
            connected: false,
            reconnecting: false,
            error: null
        }

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(`${props.server}/hub/admin`)
            .build();

        this.connection.onclose(this.handleConnectionClose);
    }

    async componentDidMount() {
        await this.connect();
    }

    componentWillUnmount() {
        if (this.connection) {
            this.connection.stop();
            this.connection = null;
        }

        if (this.reconnectInterval) {
            clearInterval(this.reconnectInterval);
            this.reconnectInterval = null;
        }
    }

    render() {
        const { server, children } = this.props;
        const { connecting, connected, reconnecting, error } = this.state;

        if (connecting) {
            return <i>Connecting...</i>;
        }

        if (connected) {
            return children(this.connection);
        }

        return (
            <Fragment>
                <p>
                    Could not connect to server: <strong>{server}</strong>

                </p>
                <p>
                    {reconnecting && (<i>Reconnecting in 10s...</i>)}
                    <button onClick={this.handleReconnect}>Reconnect now</button>
                </p>
                <pre>
                    {JSON.stringify(error, null, 4)}
                </pre>
            </Fragment>
        )
    }

    async connect() {
        const onSuccess = () => {
            if (this.reconnectInterval) {
                clearInterval(this.reconnectInterval);
                this.reconnectInterval = null;
            }
            this.setState({ connecting: false, connected: true, reconnecting: false, error: null });
        }

        const onFailed = (error) => {
            this.setState({ connecting: false, error });
        }

        this.setState({ connecting: true });

        await this.connection
            .start()
            .then(onSuccess)
            .catch(onFailed);
    }

    handleConnectionClose = (error) => {
        this.setState({ connected: false, error });

        const { autoReconnect } = this.props;

        if (autoReconnect) {
            this.setState({ reconnecting: true });
            this.reconnectInterval = setInterval(() => this.connect(), 10000);
        }
    }

    handleReconnect = async () => {
        await this.connect();
    }
}

export default ConnectToSignalR;