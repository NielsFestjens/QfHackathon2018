import React, { Component } from 'react';
import ConnectToSignalR from './ConnectToSignalR';

const server = 'http://localhost:60860';

class App extends Component {

  connection;

  constructor() {
    super();
    this.state = {
    }
  }

  render() {
    return (
      <div>
        <h1>
          This is the admin
        </h1>
        <ConnectToSignalR server={server}>
          {connection => (
            <strong>Connected!</strong>
          )}
        </ConnectToSignalR>
      </div>
    );
  }
}

export default App;
