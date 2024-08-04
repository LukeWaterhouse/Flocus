import React from "react";
import { Unity, useUnityContext } from "react-unity-webgl"


export default function UnityComponent() {
    const { unityProvider } = useUnityContext({
      loaderUrl: "buildUnity/Notes.loader.js",
      dataUrl: "buildUnity/webgl.data",
      frameworkUrl: "buildUnity/Notes.framework.js",
      codeUrl: "buildUnity/build.wasm",
    });
  
    return <Unity unityProvider={unityProvider} style={{ width: 800, height: 600 }} />;
  }