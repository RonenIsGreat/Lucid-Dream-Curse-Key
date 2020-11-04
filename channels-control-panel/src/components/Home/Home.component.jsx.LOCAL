import React, { useState, useEffect } from 'react'
import socketIOClient from 'socket.io-client'
import SonarStatus from '../SonarStatus/SonarStatus.component'
import ChannelController from '../ChannelController/ChannelController.component'
import DistributionController from '../DistributionController/DistributionController.component'
import { Row, Col } from 'react-bootstrap'

export default function Home() {

    const ENDPOINT = "http://127.0.0.1:4000";
    const [CasBeam, setCasBeam] = useState("inactive");
    const [CasStave, setCasStave] = useState("inactive");
    const [FasTasBeam, setFasTasBeam] = useState("inactive");
    const [FasTasStave, setFasTasStave] = useState("inactive");
    const [PRSStave, setPRSStave] = useState("inactive");
    const [IDRSBus, setIDRSBus] = useState("inactive");
    const socket = socketIOClient(ENDPOINT);

    useEffect(() => {
        // socket.on('connect_error', (timeout) => {
        //     socket.close()
        // });
        // if(socket.connected) {
        socket.on("StatusSocketIO", data => {
            let dataSplit = data.split(" ");
            switch (dataSplit[0]) {
                case "CasBeam":
                    setCasBeam(dataSplit[1]);
                    break;
                case "CasStave":
                    setCasStave(dataSplit[1]);
                    break;
                case "FasTasBeam":
                    setFasTasBeam(dataSplit[1]);
                    break;
                case "FasTasStave":
                    setFasTasStave(dataSplit[1]);
                    break;
                case "PRSStave":
                    setPRSStave(dataSplit[1]);
                    break;
                case "IDRSBus":
                    setIDRSBus(dataSplit[1]);
                    break;
                default:
                    break;
            }
        });
        // }
    }, [socket])

    return (
        <div>
            <Row>
                <Col xs={7}>
                    <ChannelController CasBeam={CasBeam === "active"} CasStave={CasStave === "active"}
                        FasTasBeam={FasTasBeam === "active"} FasTasStave={FasTasStave === "active"}
                        PRSStave={PRSStave === "active"} IDRSBus={IDRSBus === "active"} />
                </Col>
                <Col>
                    <SonarStatus CasBeam={CasBeam} CasStave={CasStave} FasTasBeam={FasTasBeam}
                        FasTasStave={FasTasStave} PRSStave={PRSStave} IDRSBus={IDRSBus} ENDPOINT={ENDPOINT} />
                </Col>
            </Row>
            <Row>
                <Col xs={6}>
                    <DistributionController ENDPOINT={ENDPOINT} socket={socket} />
                </Col>
                <Col>
                    <StorageStatus />
                    <PlayAudio />
                </Col>
            </Row>
        </div>
    )
}
