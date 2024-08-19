import * as React from "react";
import Box from "@mui/joy/Box";
import Button from "@mui/joy/Button";
import Divider from "@mui/joy/Divider";
import FormControl from "@mui/joy/FormControl";
import FormLabel from "@mui/joy/FormLabel";
import Link from "@mui/joy/Link";
import Input from "@mui/joy/Input";
import Typography from "@mui/joy/Typography";
import Stack from "@mui/joy/Stack";
import GoogleButton from "../../CommonComponents/GoogleButton";
import AuthenticationStackWrapper from "../../CommonComponents/AuthenticationStackWrapper";
import { useAuth } from "../../../../Context/useAuth";

interface FormElements extends HTMLFormControlsCollection {
  email: HTMLInputElement;
  password: HTMLInputElement;
  username: HTMLInputElement;
}

interface SignInFormElement extends HTMLFormElement {
  readonly elements: FormElements;
}

interface SignUpStackProps {
  register: (email: string, username: string, password: string ) => void;
}

function SignUpStack({ register }: SignUpStackProps) {
  return (
    <Box>
      <Stack gap={4} sx={{ mb: 2 }}>
        <Stack gap={1}>
          <Typography component="h1" level="h3">
            Sign Up
          </Typography>
          <Typography level="body-sm">
            Already have an account?{" "}
            <Link href="#replace-with-a-link" level="title-sm">
              Sign In!
            </Link>
          </Typography>
        </Stack>
        <GoogleButton />
      </Stack>
      <Divider
        sx={(theme) => ({
          [theme.getColorSchemeSelector("light")]: {
            color: { xs: "#FFF", md: "text.tertiary" },
          },
        })}
      >
        or
      </Divider>
      <Stack gap={4} sx={{ mt: 2 }}>
        <form
          onSubmit={(event: React.FormEvent<SignInFormElement>) => {
            event.preventDefault();
            const formElements = event.currentTarget.elements;

            register(
              formElements.email.value,
              formElements.username.value,
              formElements.password.value
            );
          }}
        >
          <FormControl required>
            <FormLabel>Username</FormLabel>
            <Input type="text" name="username" />
          </FormControl>
          <FormControl required>
            <FormLabel>Email</FormLabel>
            <Input type="email" name="email" />
          </FormControl>
          <FormControl required>
            <FormLabel>Password</FormLabel>
            <Input type="password" name="password" />
          </FormControl>
          <FormControl required>
            <FormLabel>Retype Password</FormLabel>
            <Input type="password" name="passwordConfirm" />
          </FormControl>
          <Stack gap={4} sx={{ mt: 2 }}>
            <Button type="submit" fullWidth>
              Sign Up
            </Button>
          </Stack>
        </form>
      </Stack>
    </Box>
  );
}

export default function SignUp() {
  const { register } = useAuth();

  return <AuthenticationStackWrapper StackComp={() => <SignUpStack register={register} />} />;
}
