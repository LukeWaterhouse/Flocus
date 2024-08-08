import Box from "@mui/joy/Box";
import Typography from "@mui/joy/Typography";

export default function CopyrightFooter() {
  return (
    <Box component="footer" sx={{ py: 3 }}>
      <Typography level="body-xs" textAlign="center">
        © Flocus {new Date().getFullYear()}
      </Typography>
    </Box>
  );
}
