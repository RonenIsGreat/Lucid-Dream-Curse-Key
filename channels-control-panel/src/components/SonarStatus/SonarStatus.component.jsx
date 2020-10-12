import React, { useState, useEffect } from 'react';
import socketIOClient from 'socket.io-client';
import StatusBox from '../StatusBox/StatusBox.component';

import './SonarStatus.styles.scss'

function SonarStatus() {
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
    <div className="sonar-status">
      <h1>Stream Array Status</h1>
      <StatusBox name = {"Cas Beam Bus"} status = {CasBeam}/>  
      <br/>
      <StatusBox name = {"Cas Stave Bus"} status = {CasStave}/>  
      <br/>
      <StatusBox name = {"Fas/Tas Beam Bus"} status = {FasTasBeam}/>  
      <br/>
      <StatusBox name = {"Fas/Tas Stave Bus"} status = {FasTasStave}/>  
      <br/>
      <StatusBox name = {"PRS Stave Bus"} status = {PRSStave}/>  
      <br/>
      <StatusBox name = {"ATM IDRS Bus"} status = {IDRSBus}/> 
    </div>
  );
}

export default SonarStatus;
