import React, { useEffect, useState } from "react";
import { getEnvironmentVariableAsync } from "./services/environment-variable.service";
import "./App.css";

function App() {
  const [responseServer01, setResponseServer01] = useState<any>({});
  const [responseServer02, setResponseServer02] = useState<any>({});

  async function handleServer01() {
    setResponseServer01(
      await getEnvironmentVariableAsync(
        process.env.REACT_APP_AZURE_GATEWAY_ENDPOINT_BACKEND01!
      )
    );
  }

  async function handleServer02() {
    setResponseServer02(
      await getEnvironmentVariableAsync(
        process.env.REACT_APP_AZURE_GATEWAY_ENDPOINT_BACKEND02!
      )
    );
  }

  useEffect(() => {
    handleServer01();
    handleServer02();
  }, []);

  return (
    <>
      <div className="main">
        <section className="card">
          <h2>{responseServer01.webAppName ?? "Não identificado"}</h2>
          <div className="response">
            <p>
              <label>Host Name</label>
              <span>{responseServer01.hostName ?? "Não identificado"}</span>
            </p>
            <p>
              <label>IP addrress</label>
              <span>{responseServer01.addrress ?? "Não identificado"}</span>
            </p>
          </div>
        </section>
        <section className="card">
          <h2>{responseServer02.webAppName ?? "Não identificado"}</h2>
          <div className="response">
            <p>
              <label>Host Name</label>
              <span>{responseServer02.hostName ?? "Não identificado"}</span>
            </p>
            <p>
              <label>IP addrress</label>
              <span>{responseServer02.addrress ?? "Não identificado"}</span>
            </p>
          </div>
        </section>
      </div>
    </>
  );
}

export default App;
