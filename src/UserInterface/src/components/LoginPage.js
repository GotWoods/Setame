import React, { useState, useEffect } from 'react';
import { TextField, Button, Typography, Box, Paper, Container } from '@mui/material';
import SettingsClient from '../settingsClient';

const LoginPage = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const settingsClient = new SettingsClient();

  useEffect(() => {
    const storedUsername = localStorage.getItem('username');
    if (storedUsername) {
      setUsername(storedUsername);
    }
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (username === 'admin@admin.com' && password === 'admin') {
      window.location.href = '/setup';
      return;
    }

    try {
      const data = await settingsClient.login(username, password);
      localStorage.setItem('authToken', data.token);
      localStorage.setItem('username', username);
      window.location.href = '/';
    } catch (error) {
      setError('Invalid username or password');
    }
  };

  return (
    <Container maxWidth="xs">
      <Box mt={8}>
        <Paper elevation={3} sx={{ padding: 3 }}>
          <Typography variant="h5" align="center" gutterBottom>
            Login
          </Typography>
          <form onSubmit={handleSubmit}>
            <Box mt={2}>
              <TextField
                fullWidth
                label="Username"
                variant="outlined"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                required
              />
            </Box>
            <Box mt={2}>
              <TextField
                fullWidth
                type="password"
                label="Password"
                variant="outlined"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
              />
            </Box>
            {error && (
              <Box mt={2} color="error.main">
                <Typography variant="body2">{error}</Typography>
              </Box>
            )}
            <Box mt={3}>
              <Button fullWidth variant="contained" color="primary" type="submit">
                Login
              </Button>
            </Box>
          </form>
        </Paper>
      </Box>
    </Container>
  );
};

export default LoginPage;
