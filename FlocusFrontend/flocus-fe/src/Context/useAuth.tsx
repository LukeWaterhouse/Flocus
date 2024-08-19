import { createContext, useEffect, useState } from "react";
import { UserSensitiveInfo } from "../Models/UserSensitiveInfo";
import { useNavigate } from "react-router-dom";
import {
  getAccessTokenAPI,
  getUserAPI,
  registerAPI,
} from "../Services/AuthService";
import React from "react";
import axios from "axios";

type UserContextType = {
  user: UserSensitiveInfo | null;
  token: string | null;
  register: (email: string, username: string, password: string) => void;
  login: (username: string, password: string) => void;
  logout: () => void;
  isLoggedIn: () => boolean;
};

type Props = { children: React.ReactNode };
const UserContext = createContext<UserContextType>({} as UserContextType);

export const UserProvider = ({ children }: Props) => {
  const tokenKey = "token";
  const userKey = "user";

  const navigate = useNavigate();
  const [token, setToken] = useState<string | null>(null);
  const [user, setUser] = useState<UserSensitiveInfo | null>(null);
  const [isReady, setIsReady] = useState(false);

  useEffect(() => {
    const user = localStorage.getItem(userKey);
    const token = localStorage.getItem(tokenKey);

    if (user && token) {
      setUser(JSON.parse(user));
      setToken(token);
    }
    setIsReady(true);
  }, []);

  const register = async (
    email: string,
    username: string,
    password: string
  ) => {
    await registerAPI(username, password, email);
    await login(username, password);
    navigate("/landingpage");
  };

  const login = async (username: string, password: string) => {
    var accessToken = await getAccessTokenAPI(username, password);
    axios.defaults.headers.common["Authorization"] = "Bearer " + accessToken;
    localStorage.setItem(tokenKey, accessToken);
    getUser(accessToken);
  };

  const getUser = async (accessToken: string) => {
    var user = await getUserAPI();
    setUser(user);
    localStorage.setItem(userKey, JSON.stringify(user));
  };

  const isLoggedIn = () => {
    return !!user;
  };

  const logout = () => {
    localStorage.removeItem(tokenKey);
    setToken("");
    navigate("/");
  };

  return (
    <UserContext.Provider
      value={{ user, token, register, login, logout, isLoggedIn }}
    >
      {isReady ? children : null}
    </UserContext.Provider>
  );
};

export const useAuth = () => React.useContext(UserContext);
