import axios from "axios";
import { getTokenUrl, registerUrl, userControllerSegment } from "./Common/Constants/Constants";
import { UserSensitiveInfo } from "../Models/UserSensitiveInfo";

export const getAccessTokenAPI = async (username: string, password: string): Promise<string> => {
    try {
        const response = await axios.post(getTokenUrl, {
            username: username,
            password: password
        });

        const data = response.data;
        const token = data.token;

        if (token) {
            return token as string;
        } else {
            throw new Error("Token not found in the response.");
        }
    } catch (error) {
        console.error(error);
        throw new Error("Failed to fetch access token.");
    }
};

export const registerAPI = async (username: string, password: string, email: string) => {
    try {
        await axios.post(registerUrl, {
            username: username,
            password: password,
            emailAddress: email,
            isAdmin:false
        })
    } catch (error) {
        console.log(error);
    }
}

export const getUserAPI = async (): Promise<UserSensitiveInfo> => {
    try {
        var response = await axios.get<UserSensitiveInfo>(userControllerSegment);
        return response.data;
    } catch (error) {
        console.log("idk");
        throw new Error("Failed to fetch user");
    }
}
