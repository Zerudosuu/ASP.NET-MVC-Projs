import api from "@/lib/axios";
import { setTokens } from "@/lib/token";

type LoginData = {
  username: string;
  password: string;
};

export async function login(username: string, password: string) {
  const response = await api.post("/auth/login", {
    username: username,
    password: password,
  } as LoginData);

  setTokens(response.data.accessToken, response.data.refreshToken);
  return response;
}
