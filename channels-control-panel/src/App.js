import React from 'react';
import { Switch, Route, Link} from "react-router-dom";
import NotFoundPage from './components/NotFoundPage/NotFoundPage.component';
import Home from './components/Home/Home.component';
import { Navbar } from 'react-bootstrap';

import './App.scss';
import 'bootstrap/dist/css/bootstrap.min.css';

function App() {

  return (
    <div>
      <Navbar bg="light" variant="light">
        <Navbar.Brand as={Link} to='/'>Sonar Control Panel</Navbar.Brand>
        {/* <Nav className="mr-auto">
          <Nav.Link as={NavLink} to='/' exact>Status</Nav.Link>
        </Nav> */}
      </Navbar>
      
      <Switch>
        <Route exact path="/" component={Home} />
        <Route component={NotFoundPage}/>
      </Switch>
    </div>
  );
}

export default App;
