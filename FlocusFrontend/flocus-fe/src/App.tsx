import React, { useState, useEffect } from "react";
import { Unity, useUnityContext } from "react-unity-webgl";
import Box from "@mui/material/Box";
import RegisterPage from "./Pages/AuthenticationPages/RegisterPage/RegisterPage";
import LoginPage from "./Pages/AuthenticationPages/LoginPage/LoginPage";

function App() {

  const { unityProvider } = useUnityContext({
    loaderUrl: "WebGL/Build/WebGL.loader.js",
    dataUrl: "WebGL/Build/WebGL.data",
    frameworkUrl: "WebGL/Build/WebGL.framework.js",
    codeUrl: "WebGL/Build/WebGL.wasm"});

    const [showCover, setShowCover] = useState(true);

    useEffect(() => {
      // Remove the cover after 4 seconds
      const timeout = setTimeout(() => {
        setShowCover(false);
      }, 4000);
  
      return () => clearTimeout(timeout);
    }, []);

  return (
    <Box>
      <RegisterPage/>
    </Box>
  );
}

export default App;
