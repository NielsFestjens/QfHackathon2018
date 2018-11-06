import React, { Component } from 'react';

class PlayersList extends Component {
    render() {
        const players = Object.values(this.props.players);

        if (!players || !players.length) {
            return <i>Er zijn nog geen spelers geconnecteerd.</i>
        }

        return (
            <table>
                <thead>
                    <tr>
                        <th style={{ width: 100, textAlign: 'left' }}>Name</th>
                        <th style={{ textAlign: 'left' }} >Connection</th>
                        <th style={{ textAlign: 'left' }}>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {players.map((player, index) => (
                        <tr key={index}>
                            <td>{player.name}</td>
                            <td>{player.connectionId}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        )
    }
}

export default PlayersList;