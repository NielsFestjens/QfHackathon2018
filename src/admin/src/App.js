import React, { Component } from 'react';
import * as signalR from '@aspnet/signalr';

const server = 'http://localhost:60860';

class App extends Component {

  connection;

  constructor() {
    super();
    this.state = {
      loading: false,
      connected: false
    }
  }

  async componentDidMount() {
    this.setState({ loading: true });
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`${server}/hub/client`)
      .build();

    await this.connection.start();
    await new Promise(resolve => setTimeout(resolve, 3000));

    this.setState({ loading: false, connected: true });
  }
  render() {
    return (
      <div>
        <h1>
          This is the admin
        </h1>
        {this.state.loading && <em>loading...</em>}
        {this.state.connected && <strong>Connected!</strong>}
      </div>
    );
  }
}

export default App;
