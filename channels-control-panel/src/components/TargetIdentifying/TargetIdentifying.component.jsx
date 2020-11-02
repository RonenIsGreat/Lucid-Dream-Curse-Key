import React, { Component } from 'react';
import './TargetIdentifying.styles.scss';
import socketIOClient from 'socket.io-client';
import { Card, ListGroup } from 'react-bootstrap';

class TargetIdentifying extends Component {
    state = {
        targets: {}
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
            <div className="target-identifying">
                <Card>
                    <Card.Header><h2>Target List</h2></Card.Header>
                    <ListGroup variant="flush">
                        {
                            this.state.targets.systemTargets && this.state.targets.systemTargets.map(target => (
                                <ListGroup.Item className="text-center" key={target.trackID}>
                                    <h4>Target ID: <b>{target.trackID}</b></h4>
                                    <h4>Bearing: <b>{target.relativeBearing}°</b></h4>
                                </ListGroup.Item>
                            ))
                        }
                    </ListGroup>
                </Card>
            </div>
        );

    }
}

export default TargetIdentifying;