import React from 'react';
import { Switch, Route, Link, NavLink} from "react-router-dom";
import NotFoundPage from './components/NotFoundPage/NotFoundPage.component';
import SonarStatus from './components/SonarStatus/SonarStatus.component';
import ChannelController from './components/ChannelController/ChannelController.component';
import { Navbar, Nav } from 'react-bootstrap';

import './App.scss';
import 'bootstrap/dist/css/bootstrap.min.css';

function App() {
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
        <Route exact path="/" component={SonarStatus} />
        <Route path="/controller" component={ChannelController}/>
        <Route component={NotFoundPage}/>
      </Switch>
    </div>
  );
}

export default App;
