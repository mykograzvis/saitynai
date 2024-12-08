import React, { useState, useEffect } from 'react';
import { register } from '../services/api';
import { useNavigate } from 'react-router-dom';
import '../styles/RegisterPage.css'; // Importing CSS for styling

const RegisterPage = () => {
    const [userName, setUserName] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');
    const navigate = useNavigate();

    useEffect(() => {
        const authToken = localStorage.getItem('authToken');
        if (authToken) {
            navigate('/');
        }
    }, [navigate]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setSuccess('');

        // Validate inputs
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!userName.trim()) {
            setError('Username is required.');
            return;
        }
        if (!emailRegex.test(email)) {
            setError('Invalid email format.');
            return;
        }
        if (password.length < 6) {
            setError('Password must be at least 6 characters long.');
            return;
        }
        
        if (!/[A-Z]/.test(password)) { // Uppercase letter
            setError('Password must contain at least one uppercase letter.');
            return;
        }
        
        if (!/[a-z]/.test(password)) { // Lowercase letter
            setError('Password must contain at least one lowercase letter.');
            return;
        }
        
        if (!/[0-9]/.test(password)) { // Digit
            setError('Password must contain at least one digit.');
            return;
        }
        
        if (!/[!@#$%^&*(),.?":{}|<>]/.test(password)) { // Special character
            setError('Password must contain at least one special character.');
            return;
        }

        try {
            const response = await register({ userName, email, password });
    
            setSuccess('Registration successful!');
            setTimeout(() => navigate('/login'), 3000);

        } catch (err) {
            console.error("Error during registration:", err);
            setError(err.response?.data || 'Registration failed. Please try again.');
        }
    };

    return (
        <div className="register-container">
            <h1 className="register-title">Register</h1>
            <form onSubmit={handleSubmit}>
                <div>
                    <label>Username</label>
                    <input
                        type="text"
                        className="input-field"
                        value={userName}
                        onChange={(e) => setUserName(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label>Email</label>
                    <input
                        type="email"
                        className="input-field"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label>Password</label>
                    <input
                        type="password"
                        className="input-field"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                </div>
                <button type="submit" className="submit-button">Register</button>
            </form>
            {error && <p className="error-message">{error}</p>}
            {success && <p className="success-message">{success}</p>}
        </div>
    );
};

export default RegisterPage;
