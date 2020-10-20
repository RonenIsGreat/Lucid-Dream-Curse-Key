import React from 'react';
import StatusBox from '../StatusBox/StatusBox.component';

import './SonarStatus.styles.scss'

function SonarStatus(props) {
  let {CasBeam, CasStave, FasTasBeam, FasTasStave, PRSStave, IDRSBus} = props;
  
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
