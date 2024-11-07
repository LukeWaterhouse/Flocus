import React, { useState, useEffect } from "react";
import Box from "@mui/material/Box";
import RegisterPage from "./Pages/AuthenticationPages/RegisterPage/RegisterPage";

function App() {

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
        <RegisterPage />
    </Box>
  );
}

export default App;
