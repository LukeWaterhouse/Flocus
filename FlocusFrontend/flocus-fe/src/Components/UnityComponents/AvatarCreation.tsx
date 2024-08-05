import Box from "@mui/material/Box";
import React from "react";
import { Unity, useUnityContext } from "react-unity-webgl"

export default function AvatarCreation() {
  const { unityProvider } = useUnityContext({
    loaderUrl: "WebGL/Build/WebGL.loader.js",
    dataUrl: "WebGL/Build/WebGL.data",
    frameworkUrl: "WebGL/Build/WebGL.framework.js",
    codeUrl: "WebGL/Build/WebGL.wasm"});
    
  
    return (
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          width: '100%',
          height: '100vh' // Adjust the height as needed
        }}
      >
        <Box
          sx={{
            width: '80%', // Adjust the width as needed
            height: '80%', // Adjust the height as needed
            borderRadius: '16px', // Add curved borders
            overflow: 'hidden' // Ensures the content inside respects the border radius
          }}
        >
          <Unity unityProvider={unityProvider} style={{ width: "100%", height: "100%" }} />
        </Box>
      </Box>
    );
  }