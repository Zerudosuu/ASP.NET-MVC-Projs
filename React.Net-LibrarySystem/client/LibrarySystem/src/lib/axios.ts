import axios from "axios";
import { getAccessToken, setTokens, clearTokens } from "./token";

const api = axios.create({
  baseURL: "https://localhost:5173/api",
  headers: {
    "Content-Type": "application/json",
  },
  withCredentials: false,
});

api.interceptors.request.use((config) => {
  const token = getAccessToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

let isRefreshing = false;
let faileQueue: any[] = [];

const processQueue = (error: any, token: string | null = null) => {
  faileQueue.forEach((prom) => {
    if (error) prom.reject(error);
    else prom.resolve(token);
  });

  faileQueue = [];
};

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        return new Promise(function (resolve, reject) {
          faileQueue.push({ resolve, reject });
        }).then((token) => {
          originalRequest.headers.Authorization = `Bearer ${token}`;
          return api(originalRequest);
        });
      }
      originalRequest._retry = true;
      isRefreshing = true;

      try {
        const res = await api.post("/auth/refresh", {
          refreshToken: localStorage.getItem("refreshToken"),
        });

        setTokens(res.data.accessToken, res.data.refreshToken);
        processQueue(null, res.data.accessToken);

        originalRequest.headers.Authorization = `Bearer ${res.data.accessToken}`;
        return api(originalRequest);
      } catch (err) {
        processQueue(err, null);
        clearTokens();
        return Promise.reject(err);
      } finally {
        isRefreshing = false;
      }
    }
    return Promise.reject(error);
  }
);

export default api;
