import React, { Component } from 'react'
import { Button, Form } from 'react-bootstrap'
import socketIOClient from 'socket.io-client'
import './DistributionController.styles.scss'

export default class DistributionController extends Component {
    state = {
        
    }

    handleSubmit = (ENDPOINT) => {
        // let {ENDPOINT} = this.props;
        const socket = socketIOClient(ENDPOINT);
        socket.emit("DistributionSocketIO", {
            date1UnixTime: '1603288680000',
            date2UnixTime: '1603288714000',
            channel: 'CasStave'
        })
    }

    render() {
        return (
            <div className="distribution-controller container">
                <div className="form-wrapper bg-light">
                    <h5 className="text-center">Data Distribution</h5>
                    <Form>
                        <Form.Group controlId="formBasicEmail">
                            <Form.Label>Email address</Form.Label>
                            <Form.Control type="email" placeholder="Enter email" />
                            <Form.Text className="text-muted">
                                We'll never share your email with anyone else.
                        </Form.Text>
                        </Form.Group>

                        <Form.Group controlId="formBasicPassword">
                            <Form.Label>Password</Form.Label>
                            <Form.Control type="password" placeholder="Password" />
                        </Form.Group>
                        <Form.Group controlId="formBasicCheckbox">
                            <Form.Check type="checkbox" label="Check me out" />
                        </Form.Group>
                        <Button variant="primary" type="submit">
                            Submit
                    </Button>
                    </Form>
                </div>
                {/* <Button variant="warning" onClick={() => this.handleSubmit(this.props.ENDPOINT)}>Warning</Button> */}
            </div>
        )
    }
}
