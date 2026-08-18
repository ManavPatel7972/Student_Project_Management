import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5000/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request Interceptor: Attach JWT Token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response Interceptor: Handle Common ApiResponse Envelope & Errors (401, 403, etc.)
api.interceptors.response.use(
  (response) => {
    // If backend returned ApiResponse format
    if (
      response &&
      response.data &&
      typeof response.data === 'object' &&
      'success' in response.data
    ) {
      if (response.data.success) {
        // Return unwrapped data payload if present
        if (response.data.data !== undefined && response.data.data !== null) {
          response.data = response.data.data;
        }
      } else {
        // Backend returned success: false inside a 200 response
        const errorMessage =
          Array.isArray(response.data.errors) && response.data.errors.length > 0
            ? response.data.errors[0]
            : response.data.message || 'Operation failed';
        return Promise.reject(new Error(errorMessage));
      }
    }
    return response;
  },
  (error) => {
    if (error.response) {
      // Normalize error message from ApiResponse errors array if message is blank
      if (
        error.response.data &&
        typeof error.response.data === 'object'
      ) {
        if (
          (!error.response.data.message || error.response.data.message === '') &&
          Array.isArray(error.response.data.errors) &&
          error.response.data.errors.length > 0
        ) {
          error.response.data.message = error.response.data.errors[0];
        }
      }

      if (error.response.status === 401) {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        if (window.location.pathname !== '/login') {
          window.location.href = '/login';
        }
      }
    }
    return Promise.reject(error);
  }
);

export default api;
