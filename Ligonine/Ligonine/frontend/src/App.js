import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import DepartmentsPage from './pages/DepartmentsPage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import DoctorsPage from './pages/DoctorsPage';
import OperationsPage from './pages/OperationsPage';
import HomePage from './pages/HomePage'; // Move HomePage to a separate file
import NavBar from './components/NavBar'; // New NavBar component
import Footer from './components/Footer';
import { logout } from './services/api';
import 'bootstrap/dist/css/bootstrap.min.css';



function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [username, setUsername] = useState('');

  useEffect(() => {
      setIsLoggedIn(!!localStorage.getItem('authToken'));
      setUsername(localStorage.getItem('username') || '');
  }, []);

  const handleLogin = (username) => {
      setIsLoggedIn(true);
      setUsername(username);
  };

  const handleLogout = () => {
      logout();
      setIsLoggedIn(false);
      setUsername('');
      localStorage.removeItem('authToken');
      localStorage.removeItem('username');
  };

  return (
      <Router>
          <NavBar isLoggedIn={isLoggedIn} username={username} onLogout={handleLogout} />
          <Routes>
              <Route path="/" element={<HomePage />} />
              <Route path="/departments" element={<DepartmentsPage />} />
              <Route path="/departments/:departmentId/doctors" element={<DoctorsPage />} />
              <Route path="/departments/:departmentId/doctors/:doctorId/operations" element={<OperationsPage />} />
              <Route path="/login" element={<LoginPage onLogin={handleLogin} />} />
              <Route path="/register" element={<RegisterPage />} />
          </Routes>
          <Footer />
      </Router>
  );
}

export default App;
