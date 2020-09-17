import React, { useState, useEffect } from 'react';
import socketIOClient from 'socket.io-client';
import './App.scss';
import StatusBox from './components/StatusBox/StatusBox.comonent';

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
      switch(dataSplit[0]) {
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
    <div className="App">
      <h1>Stream Array Status</h1>
      <StatusBox name = {"Cas Beam Bas"} status = {CasBeam}/>  
      <br/>
      <StatusBox name = {"Cas Stave Bas"} status = {CasStave}/>  
      <br/>
      <StatusBox name = {"Fas/Tas Beam Bas"} status = {FasTasBeam}/>  
      <br/>
      <StatusBox name = {"Fas/Tas Stave Bas"} status = {FasTasStave}/>  
      <br/>
      <StatusBox name = {"PRS Beam Bas"} status = {PRSStave}/>  
      <br/>
      <StatusBox name = {"ATM IDRS Bas"} status = {IDRSBus}/> 
    </div>
  );
}

export default App;
