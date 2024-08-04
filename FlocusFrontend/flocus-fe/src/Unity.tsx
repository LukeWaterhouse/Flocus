import Box from "@mui/material/Box";
import React from "react";
import { Unity, useUnityContext } from "react-unity-webgl"


export default function UnityComponent() {
  const { unityProvider } = useUnityContext({
    loaderUrl: "WebGL/Build/WebGL.loader.js",
    dataUrl: "WebGL/Build/WebGL.data",
    frameworkUrl: "WebGL/Build/WebGL.framework.js",
    codeUrl: "WebGL/Build/WebGL.wasm"});
    
  
    return (
    <Box>
      <Unity unityProvider={unityProvider} style={{ width: "100%", height: "100%" }} />
    </Box>)
  }