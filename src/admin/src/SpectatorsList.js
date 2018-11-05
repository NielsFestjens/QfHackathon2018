import React, { Component } from 'react';

class SpectatorsList extends Component {
    render() {
        const { spectators } = this.props;

        if (!spectators || !spectators.length) {
            return <i>Er zijn zijn nog geen spectators geconnecteerd.</i>
        }

        return (
            <div>

            </div>
        )
    }
}

export default SpectatorsList;