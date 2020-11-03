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

        var radius = 100;
        var center_x = 200;
        var center_y = 130;

        var c = document.getElementById("myCanvas");
        var ctx = c.getContext("2d");

        function drawCircle() {
            ctx.beginPath();
            ctx.arc(center_x, center_y, radius, 0, 2 * Math.PI);
            ctx.lineWidth = 10;
            ctx.stroke();
        }

        //Execution
        drawCircle();
    }

    componentDidUpdate() {
        
        var radius = 100;
        var point_size = 10;
        var center_x = 200;
        var center_y = 130;
        var font_size = "20px Arial";

        var c = document.getElementById("myCanvas");
        var ctx = c.getContext("2d");
        ctx.clearRect(0,0, c.width, c.height);

        function drawPoint(angle, distance, label) {
            var x = center_x + radius * Math.cos(-angle * Math.PI / 180) * distance;
            var y = center_y + radius * Math.sin(-angle * Math.PI / 180) * distance;

            ctx.beginPath();
            ctx.arc(x, y, point_size, 0, 2 * Math.PI);
            ctx.fill();

            ctx.font = font_size;
            if (angle <= 180.0) {
                ctx.fillText(label, x + 15, y - 10);
            }
            else {
                ctx.fillText(label, x + 15, y + 20);
            }
        }

        function drawCircle() {
            ctx.beginPath();
            ctx.arc(center_x, center_y, radius, 0, 2 * Math.PI);
            ctx.lineWidth = 10;
            ctx.stroke();
        }
        
        // drawPoint(0, 1, "A");
        // drawPoint(45, 1, "B");
        // drawPoint(90, 1, "C");
        // drawPoint(180, 1, "D");
        // drawPoint(150, 1, "E");
        drawCircle();
        if(this.state.targets.systemTargets) {
            this.state.targets.systemTargets.forEach(target => {
                drawPoint(target.relativeBearing, 1, target.trackID)
            })
        }
    }

    render() {

        return (
            <div className="target-identifying">
                <Card>
                    <Card.Header><h2>Target List</h2></Card.Header>
                    <Card.Body className="text-center">
                        <canvas id="myCanvas" width="400" height="260">
                            Your browser does not support the HTML5 canvas tag.
                        </canvas>
                    </Card.Body>
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