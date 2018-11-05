import React, { Component } from 'react';

class PlayersList extends Component {
    render() {
        const { players } = this.props;

        if (!players || !Object.keys(players).length) {
            return <i>Er zijn zijn nog geen spelers geconnecteerd.</i>
        }

        return (
            <div>

            </div>
        )
    }
}

export default PlayersList;