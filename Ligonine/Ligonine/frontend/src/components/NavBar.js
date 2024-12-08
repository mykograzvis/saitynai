import React, { useState } from 'react';
import '../styles/NavBar.css';
import { useNavigate } from 'react-router-dom';
import { 
  AppBar, 
  Toolbar, 
  IconButton, 
  Typography, 
  Button, 
  Drawer, 
  List, 
  ListItem, 
  ListItemText,
  useMediaQuery,
  useTheme
} from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';

const NavBar = ({ isLoggedIn, username, onLogout }) => {
    const navigate = useNavigate();
    const [open, setOpen] = useState(false);
    
    // Use MUI's theme and mediaQuery to determine screen size
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('md'));

    const handleLogout = () => {
        onLogout();
        navigate('/login');
    };

    const toggleDrawer = (open) => () => {
        setOpen(open);
    };

    const menuItems = [
        { text: 'Home', link: '/' },
        ...(isLoggedIn
            ? [
                { text: 'Departments', link: '/departments' },
                { text: username, link: '#' },
                { text: 'Logout', link: '#', onClick: handleLogout },
              ]
            : [
                { text: 'Register', link: '/register' },
                { text: 'Login', link: '/login' },
              ])
    ];

    const drawerList = (
        <List>
            {menuItems.map((item, index) => (
                <ListItem 
                    key={index} 
                    onClick={item.onClick 
                        ? () => {
                            item.onClick();
                            toggleDrawer(false)();
                        }
                        : () => {
                            navigate(item.link);
                            toggleDrawer(false)();
                        }
                    }
                >
                    <ListItemText primary={item.text} />
                </ListItem>
            ))}
        </List>
    );

    return (
        <AppBar position="static">
            <Toolbar>
                {/* Burger Menu Icon - Only shown on mobile */}
                {isMobile && (
                    <IconButton 
                        edge="start" 
                        color="inherit" 
                        aria-label="menu" 
                        onClick={toggleDrawer(true)}
                        sx={{ mr: 2 }}
                    >
                        <MenuIcon />
                    </IconButton>
                )}

                <Typography variant="h6" sx={{ flexGrow: 1 }}>
                    Hospital Management System
                </Typography>

                {/* Mobile Drawer */}
                <Drawer 
                    anchor="left" 
                    open={open} 
                    onClose={toggleDrawer(false)}
                >
                    {drawerList}
                </Drawer>

                {/* Desktop/Tablet Menu - Hidden on mobile */}
                {!isMobile && (
                    <div>
                        {menuItems.map((item, index) => (
                            <Button
                                key={index}
                                color="inherit"
                                onClick={item.onClick ? item.onClick : () => navigate(item.link)}
                                sx={{ ml: 1 }}
                            >
                                {item.text}
                            </Button>
                        ))}
                    </div>
                )}
            </Toolbar>
        </AppBar>
    );
};

export default NavBar;