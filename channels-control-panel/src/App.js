import React, { useState, useEffect } from 'react';
import { Switch, Route, Link, NavLink} from "react-router-dom";
import NotFoundPage from './components/NotFoundPage/NotFoundPage.component';
import SonarStatus from './components/SonarStatus/SonarStatus.component';
import ChannelController from './components/ChannelController/ChannelController.component';
import { Navbar, Nav } from 'react-bootstrap';
import socketIOClient from 'socket.io-client';

import './App.scss';
import 'bootstrap/dist/css/bootstrap.min.css';

function App() {
  const ENDPOINT = "http://127.0.0.1:4000";
  const [CasBeam, setCasBeam] = useState("inactive");
  const [CasStave, setCasStave] = useState("inactive");
  const [FasTasBeam, setFasTasBeam] = useState("inactive");
  const [FasTasStave, setFasTasStave] = useState("inactive");
  const [PRSStave, setPRSStave] = useState("inactive");
  const [IDRSBus, setIDRSBus] = useState("inactive");

  useEffect(() => {
    const socket = socketIOClient(ENDPOINT);
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
  }, [])


  return (
    <div>
      <Navbar bg="light" variant="light">
        <Navbar.Brand as={Link} to='/'>Sonar Control Panel</Navbar.Brand>
        <Nav className="mr-auto">
          <Nav.Link as={NavLink} to='/' exact>Status</Nav.Link>
          <Nav.Link as={NavLink} to='/controller'>Controller</Nav.Link>
        </Nav>
      </Navbar>
      
      <Switch>
        <Route
          exact path="/"
          render={() => <SonarStatus CasBeam={CasBeam} CasStave={CasStave} FasTasBeam={FasTasBeam}
                      FasTasStave={FasTasStave} PRSStave={PRSStave} IDRSBus={IDRSBus} /> }
        />
        <Route
          path="/controller"
          render={() => <ChannelController CasBeam={CasBeam === "active"} CasStave={CasStave === "active"}
                      FasTasBeam={FasTasBeam === "active"} FasTasStave={FasTasStave === "active"}
                      PRSStave={PRSStave === "active"} IDRSBus={IDRSBus === "active"}/>}
        />
        <Route component={NotFoundPage}/>
      </Switch>
    </div>
  );
}

export default App;
