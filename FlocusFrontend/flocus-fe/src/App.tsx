import React, { useState, useEffect } from "react";
import { Unity, useUnityContext } from "react-unity-webgl";
import SignInSide from "./Pages/LoginPage/SignIn/SignInSide";
import LoginPage from "./Pages/LoginPage/LoginPage";
import Box from "@mui/material/Box";

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
      <LoginPage/>
    </Box>
  );
}

export default App;
