import React from "react";
import { Grid } from "@mui/material";
import AvatarCreation from "../../../Components/UnityComponents/AvatarCreation";
import SignUp from "./SignUp/SignUp";

function RegisterPage() {
  return (
    <Grid container spacing={3}>
      <Grid item xs={7}>
        <AvatarCreation />
      </Grid>
      <Grid item xs={5}>
        <SignUp />
      </Grid>
    </Grid>
  );
}

export default RegisterPage;
