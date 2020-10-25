import React, { Component } from 'react'
import { Button, Form, Row, Col } from 'react-bootstrap'
// import socketIOClient from 'socket.io-client'
import DatePicker from "react-datepicker"

import "react-datepicker/dist/react-datepicker.css"
import './DistributionController.styles.scss'

export default class DistributionController extends Component {
    state = {
        casStave: false,
        fasTasStave: false,
        startDate: new Date(),
        endDate: new Date(),
        startTime: '',
        endTime: '',
        isSending: false,
        socket: this.props.socket
    }

    handleSubmit = (e) => {
        // let {ENDPOINT} = this.props;
        // const socket = socketIOClient(ENDPOINT);
        
        e.preventDefault();
        this.setState({
            isSending: !this.state.isSending
        }, () => {
            if (this.state.isSending) {
                const {casStave, fasTasStave, startDate, endDate,
                        startTime, endTime} = this.state;
                const startTimeArray = startTime.split(":");
                const endTimeArray = endTime.split(":");
                const startDateUnix = startTime === "" ? startDate.getTime() : startDate.setHours(startTimeArray[0], startTimeArray[1], startTimeArray[2]);
                const endDateUnix = endTime === "" ? endDate.getTime() : endDate.setHours(endTimeArray[0], endTimeArray[1], endTimeArray[2]);
                console.log(startDateUnix, endDateUnix);
                this.state.socket.emit("DistributionSocketIO", {
                    date1UnixTime: startDateUnix,
                    date2UnixTime: endDateUnix,
                    channels: {
                        casStave,
                        fasTasStave
                    }
                })
            }
            else {

            }
        })
    }

    handleChange = (e) => {
        this.setState({
            [e.target.id]: e.target.value
        });
    }

    handleToggle = (e) => {
        this.setState({
            [e.target.id]: e.target.checked
        });
    }

    handleDate = (date, type, e) => {
        // console.log(e.target);
        this.setState({
            [type]: date
        });
    }

    render() {
        const CustomDateInput = ({value, onClick}) => (
            <Button className="bg-light text-primary" onClick={onClick}>{value}</Button>
        );

        return (
            <div className="distribution-controller container">
                <div className="form-wrapper bg-light">
                    <h5 className="text-center">Data Distribution</h5>
                    <Form onSubmit={this.handleSubmit}>
                        <Row>
                            <Col xs={3}>
                                <Form.Label>Channels</Form.Label>
                                <Form.Group controlId="casStave">
                                    <Form.Check type="checkbox" label="Cas Stave Bus" onChange={this.handleToggle} />
                                </Form.Group>
                                <Form.Group controlId="fasTasStave">
                                    <Form.Check type="checkbox" label="Fas/Tas Stave Bus" onChange={this.handleToggle} />
                                </Form.Group>
                            </Col>
                            <Col>
                                <Row>
                                    <Col xs={3}>
                                        <Form.Group>
                                            <Form.Label>Start Date</Form.Label>
                                            <DatePicker
                                                selected={this.state.startDate} onChange={(date, e) => this.handleDate(date, "startDate", e)}
                                                dateFormat="dd/MM/yyyy"
                                                customInput={<CustomDateInput />} />
                                        </Form.Group>
                                    </Col>
                                    <Col xs={4}>
                                        <Form.Group>
                                            <Form.Label>Start Time</Form.Label>
                                            <Form.Control id="startTime" type="time" step="1" onChange={this.handleChange}></Form.Control>
                                        </Form.Group>
                                    </Col>
                                    <Col xs={5}>
                                        <Form.Group>
                                            <Form.Label>Speed</Form.Label>
                                            <Form.Control as="select">
                                                <option>X1</option>
                                                <option>X2</option>
                                                <option>X4</option>
                                                <option>X8</option>
                                            </Form.Control>
                                        </Form.Group>
                                    </Col>
                                </Row>
                                <Row>
                                    <Col xs={3}>
                                        <Form.Group>
                                            <Form.Label>End Date</Form.Label>
                                            <DatePicker minDate={this.state.startDate} selected={this.state.endDate >= this.state.startDate ? this.state.endDate : this.state.startDate}
                                                onChange={(date, e) => this.handleDate(date, "endDate", e)}
                                                dateFormat="dd/MM/yyyy"
                                                customInput={<CustomDateInput />} />
                                        </Form.Group>
                                    </Col>
                                    <Col xs={4}>
                                        <Form.Group>
                                            <Form.Label>End Time</Form.Label>
                                            <Form.Control id="endTime" type="time" step="1" onChange={this.handleChange}></Form.Control>
                                        </Form.Group>
                                    </Col>
                                </Row>
                            </Col>
                        </Row>
                        <Button variant="primary" type="submit">
                            {this.state.isSending ? "End" : "Start"}
                        </Button>
                    </Form>
                    
                </div>
            </div>
        )
    }
}
