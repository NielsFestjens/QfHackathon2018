import React, { Component, Fragment } from 'react';
import * as signalR from '@aspnet/signalr';

class ConnectToSignalR extends Component {

    connection = null;

    constructor(props) {
        super(props);
        this.state = {
            connecting: true,
            connected: false,
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
    }

    render() {
        const { server, children } = this.props;
        const { connecting, connected, error } = this.state;

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
                    <button onClick={this.handleReconnect}>Reconnect</button>
                </p>
                <pre>
                    {JSON.stringify(error, null, 4)}
                </pre>
            </Fragment>
        )
    }

    async connect() {
        const onSuccess = () => {
            this.setState({ connecting: false, connected: true, error: null });
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
    }

    handleReconnect = async () => {
        await this.connect();
    }
}

export default ConnectToSignalR;