import React from 'react';
import ConnectToSignalR from './ConnectToSignalR';
import AdminContent from './AdminContent';

const server = 'http://localhost:60860';

const App = () => (
  <div>
    <h1>
      New Year's Chapter 2019 - Admin
    </h1>
    <ConnectToSignalR server={server} autoReconnect={true}>
      {connection => <AdminContent connection={connection} />}
    </ConnectToSignalR>
  </div>
);

export default App;
