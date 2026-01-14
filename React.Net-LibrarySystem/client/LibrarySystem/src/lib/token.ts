export const getAccessToken = () => localStorage.getItem("accessToken");
export const setAccessToken = () => localStorage.getItem("accessToken");

export const setTokens = (acceess: string, refresh: string) => {
  localStorage.setItem("accessToken", acceess);
  localStorage.setItem("refreshToken", refresh);
};

export const clearTokens = () => {
  localStorage.removeItem("accessToken");
  localStorage.removeItem("refreshToken");
};
