import React, { useState, useEffect } from 'react';
import { TextField, Button, Typography, Box, Paper, Container } from '@mui/material';
import SettingsClient from '../clients/settingsClient';

const ForgotPasswordPage = () => {
  const [username, setUsername] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false); 

  const settingsClient = new SettingsClient();

  useEffect(() => {
    const storedUsername = localStorage.getItem('username');
    if (storedUsername) {
      setUsername(storedUsername);
    }
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();

    const result = await settingsClient.forgotPassword(username);
    if (result.wasSuccessful) {
      setSuccess(true); 
    } else {
      setError(result.errors);
    }
  };

  return (
    <Container maxWidth="xs">
      <Box mt={8}>
        <Paper elevation={3} sx={{ padding: 3 }}>
          <Typography variant="h5" align="center" gutterBottom>
            Reset Password
          </Typography>
          {success ? (
            <Typography variant="body2" align="center" gutterBottom>
              A password reset was requested. Please check your email for how to reset your password.
            </Typography>
          ) : (
            <form onSubmit={handleSubmit}>
              <Box mt={2}>
                <TextField
                  fullWidth
                  label="Email"
                  variant="outlined"
                  value={username}
                  onChange={(e) => setUsername(e.target.value)}
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
                  Reset
                </Button>
              </Box>
            </form>
          )}
        </Paper>
      </Box>
    </Container>
  );
};

export default ForgotPasswordPage;
