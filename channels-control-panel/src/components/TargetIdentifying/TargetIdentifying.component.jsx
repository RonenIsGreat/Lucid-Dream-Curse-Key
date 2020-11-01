import React, { Component } from 'react';
import './TargetIdentifying.styles.scss';
import socketIOClient from 'socket.io-client';

class TargetIdentifying extends Component {
    state = {
        targets: []
    }

    componentDidMount() {
        const ENDPOINT = "http://127.0.0.1:4000";
        const socket = socketIOClient(ENDPOINT);
        socket.on("TargetStatus", targets => {
            this.setState({
                targets: JSON.parse(targets)
            });
        });
    }

    render() {

        return (
            <div className="targetIdentifying">
                {this.state.targets.map(target => (
                    <div key={target.trackID}>
                        <h1>{target.trackID}</h1>
                        <h4>{target.relativeBearing}</h4>
                    </div>
                ))}
            </div>
        );

    }
}

export default TargetIdentifying;