import React, { useState, useEffect } from 'react';
import socketIOClient from 'socket.io-client';
import './App.scss';

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
      <header className="App-header">
        <p>
          Edit <code>src/App.js</code> and save to reload.
        </p>
      </header>
    </div>
  );
}

export default App;
