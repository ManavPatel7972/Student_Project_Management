import React, { createContext, useContext, useState, useEffect } from "react";
import { loginApi, getCurrentUserApi, registerApi } from "../api/auth";

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(() => {
    const savedUser = localStorage.getItem("user");
    return savedUser ? JSON.parse(savedUser) : null;
  });

  const [token, setToken] = useState(() => localStorage.getItem("token"));
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const initAuth = async () => {
      if (token) {
        try {
          const userData = await getCurrentUserApi();
          setUser(userData);
          localStorage.setItem("user", JSON.stringify(userData));
        } catch (error) {
          console.error("Auth verification failed:", error);
          logout();
        }
      }
      setLoading(false);
    };
    initAuth();
  }, [token]);

  const login = async (credentials) => {
    const data = await loginApi(credentials);
    localStorage.setItem("token", data.token);
    setToken(data.token);

    console.log("Login response data: ====== ", data);

    const userObj = {
      id: data.userId,
      fullName: data.fullName,
      email: data.email,
      role: data.role,
      userTypeId: data.userTypeId,
      userCode: data.userCode,
      profilePicturePath: data.profilePicturePath,
    };

    setUser(userObj);
    localStorage.setItem("user", JSON.stringify(userObj));
    return data;
  };

  const register = async (registerData) => {
    const data = await registerApi(registerData);
    localStorage.setItem("token", data.token);
    setToken(data.token);

    const userObj = {
      id: data.userId,
      fullName: data.fullName,
      email: data.email,
      role: data.role,
      userTypeId: data.userTypeId,
      userCode: data.userCode,
      profilePicturePath: data.profilePicturePath,
    };

    setUser(userObj);
    localStorage.setItem("user", JSON.stringify(userObj));
    return data;
  };

  const logout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    setToken(null);
    setUser(null);
  };

  const hasRole = (roles) => {
    if (!user) return false;
    if (typeof roles === "string") return user.role === roles;
    if (Array.isArray(roles)) return roles.includes(user.role);
    return false;
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        loading,
        login,
        register,
        logout,
        hasRole,
        setUser,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};
