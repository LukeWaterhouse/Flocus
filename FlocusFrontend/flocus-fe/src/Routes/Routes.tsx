import React from "react";
import { createBrowserRouter } from "react-router-dom";
import LandingPage from "../Pages/UserPages/LandingPage/LandingPage";
import LoginPage from "../Pages/AuthenticationPages/LoginPage/LoginPage";
import RegisterPage from "../Pages/AuthenticationPages/RegisterPage/RegisterPage";
import App from "../App";
import ProtectedRoute from "./ProtectedRoute";

export const router = createBrowserRouter([{
    path: "/",
    element: <App />,
    children: [
        { path: "", element: <ProtectedRoute><LandingPage /></ProtectedRoute>},
        { path: "login", element: <LoginPage />},
        { path: "register", element: <RegisterPage />}
    ]
}])