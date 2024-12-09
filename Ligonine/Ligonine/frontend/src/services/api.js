import axios from 'axios';

const API = axios.create({
    baseURL: 'https://ligonine-emcvhrfngsdheshq.canadacentral-01.azurewebsites.net/api', // Ensure this URL is correct and matches your backend
});

// Add token automatically to requests if available
API.interceptors.request.use((config) => {
    const token = localStorage.getItem('authToken');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
}, (error) => Promise.reject(error));

export const login = async (credentials) => {
    try {
        const response = await API.post('/login', credentials);
        return response.data;
    } catch (error) {
        console.error('Login error:', error);
        throw error;
    }
};

export const logout = () => {
    localStorage.removeItem('authToken');
    delete API.defaults.headers['Authorization'];
};

export const register = async (userData) => {
    try {
        const response = await API.post('/accounts', userData);
        return response.data;
    } catch (error) {
        console.error('Error registering user:', error);
        throw error;
    }
};


export default API;
