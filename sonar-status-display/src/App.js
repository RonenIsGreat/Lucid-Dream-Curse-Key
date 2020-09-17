import React, { useState, useEffect } from 'react';
import socketIOClient from 'socket.io-client';
import './App.scss';
import StatusBox from './components/StatusBox/StatusBox.comonent';

function App() {
  const ENDPOINT = "http://127.0.0.1:4000";
  const [response, setResponse] = useState("");

  useEffect(() => {
    const socket = socketIOClient(ENDPOINT);
    socket.on("StatusSocketIO", data => {
      const res = JSON.parse(data);
      setResponse(res);
    });
  }, [])

  return (
    <div className="App">
      <h1>Stream Array Status</h1>
      <StatusBox name = {"Cas Beam Bas"} status = "channel-live"/>  
      <br/> 
      <StatusBox name = {"Cas Stave Bas"} status = {"channel-live"}/>  
      <br/>  
      <StatusBox name = {"Fas/Tas Beam Bas"} status = {"channel-live"}/>  
      <br/>  
      <StatusBox name = {"Fas/Tas Stave Bas"} status = {"channel-live"}/>  
      <br/>  
      <StatusBox name = {"PRS Beam Bas"} status = {"channel-dead"}/>  
      <br/>  
      <StatusBox name = {"ATM IDRS Bas"} status = {"channel-live"}/> 
    </div>  
  );
}

export default App;
