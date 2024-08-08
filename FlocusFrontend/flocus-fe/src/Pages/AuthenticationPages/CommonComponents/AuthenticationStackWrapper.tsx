import Box from "@mui/joy/Box";
import CopyrightFooter from "./CopyrightFooter";
import { CssVarsProvider } from "@mui/joy/styles/CssVarsProvider";
import CssBaseline from "@mui/joy/CssBaseline";

interface Props {
  StackComp: React.ComponentType;
}

const AuthenticationStackWrapper: React.FC<Props> = ({ StackComp }) => {
  return (
    <CssVarsProvider defaultMode="light">
      <CssBaseline />
      <Box
        sx={() => ({
          width: { xs: "100%" },
          position: "relative",
          zIndex: 1,
          display: "flex",
          justifyContent: "flex-end",
          backdropFilter: "blur(12px)",
          backgroundColor: "rgba(19 19 24 / 0.4)",
        })}
      >
        <Box
          sx={{
            display: "flex",
            flexDirection: "column",
            minHeight: "100dvh",
            width: "100%",
            px: 2,
          }}
        >
          <Box
            component="main"
            sx={{
              my: "auto",
              py: 2,
              pb: 5,
              display: "flex",
              flexDirection: "column",
              gap: 2,
              width: 400,
              maxWidth: "100%",
              mx: "auto",
              borderRadius: "sm",
              "& form": {
                display: "flex",
                flexDirection: "column",
                gap: 2,
              },
              [`& .MuiFormLabel-asterisk`]: {
                visibility: "hidden",
              },
            }}
          >
            <StackComp />
          </Box>
          <CopyrightFooter />
        </Box>
      </Box>
    </CssVarsProvider>
  );
};

export default AuthenticationStackWrapper;
