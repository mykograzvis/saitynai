import React, { useState, useEffect } from 'react'; // Add useEffect here
import { login } from '../services/api';
import { useNavigate } from 'react-router-dom';
import '../styles/LoginPage.css'; // Importing CSS for styling
import 'bootstrap/dist/css/bootstrap.min.css';

const LoginPage = ({ onLogin }) => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState(null);

    const navigate = useNavigate();

    useEffect(() => {
        const authToken = localStorage.getItem('authToken');
        if (authToken) {
            navigate('/');
        }
    }, [navigate]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const credentials = { username, password };
            const response = await login(credentials);
            localStorage.setItem('authToken', response.accessToken);
            localStorage.setItem('username', username);
            onLogin(username); // Update App state
            navigate('/'); // Redirect to main page
        } catch (err) {
            setError('Invalid username or password');
        }
    };

    return (
        <div className="login-container">
            <h1 className="login-title">Login</h1>
            {error && <p className="error-message">{error}</p>}
            <form onSubmit={handleSubmit} className="login-form">
                <input
                    type="text"
                    className="input-field"
                    placeholder="Username"
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                />
                <input
                    type="password"
                    className="input-field"
                    placeholder="Password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                />
                <button type="submit" className="submit-button">Login</button>
            </form>
        </div>
    );
};

export default LoginPage;
