import React from 'react';
import './App.scss';
import StatusBox from './components/StatusBox/StatusBox.comonent';

function App() {
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
