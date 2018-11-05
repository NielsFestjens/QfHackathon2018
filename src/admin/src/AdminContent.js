import React, { Component } from 'react';
import SpectatorsList from './SpectatorsList';
import PlayersList from './PlayersList';

const initialState = {
    spectators: {},
    players: {}
};

class AdminContent extends Component {

    constructor(props) {
        super(props);

        this.state = initialState;

        const { connection } = props;

        connection.on('SpectatorConnected', this.handleSpectatorConnected);
        connection.on('PlayerConnected', this.handlePlayerConnected);
    }

    render() {
        const { spectators, players } = this.state;
        return (
            <div>
                <h3>Spectators</h3>
                <SpectatorsList spectators={spectators} />
                <h3>Players</h3>
                <PlayersList players={players} />
            </div>
        );
    }

    handleSpectatorConnected = (spectator) => {
        this.setState({
            players: {
                ...this.state.players,
                [spectator.connectionId]: spectator
            }
        })
    }

    handlePlayerConnected = (player) => {
        this.setState({
            players: {
                ...this.state.players,
                [player.name]: player
            }
        })
    }
}

export default AdminContent;