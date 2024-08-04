import React, { useState, useEffect } from "react";
import { Unity, useUnityContext } from "react-unity-webgl";


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
    <div style={{ position: "relative", width: 800, height: 600 }}>
      {showCover && (
        <div
          style={{
            position: "absolute",
            top: 0,
            left: 0,
            width: "100%",
            height: "100%",
            backgroundColor: "black"
          }}
        />
      )}
      {<Unity unityProvider={unityProvider} style={{ width: "100%", height: "100%" }} />}
    </div>
  );
}

export default App;
