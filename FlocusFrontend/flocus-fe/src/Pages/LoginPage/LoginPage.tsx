import { Box, Grid } from "@mui/material";
import React from "react";
import SignInSide from "./SignIn/SignInSide";
import UnityComponent from "../../Unity";

function LoginPage(){

    return (
      <Grid container spacing={2}>
        <Grid item xs={7}>
          <UnityComponent />
        </Grid>
        <Grid item xs={3}>
          <SignInSide />
        </Grid>
      </Grid>
    )
}

export default LoginPage;