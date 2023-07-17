import React, { useState } from 'react';
import { TextField, Button, Typography, Box, Paper, Container } from '@mui/material';
import SetupClient from '../setupClient';

const SetupPage = () => {
    const isDevelopment = process.env.NODE_ENV === 'development';

    const [email, setEmail] = useState(isDevelopment ? 'admin@admin.com' : '');
    const [password, setPassword] = useState(isDevelopment ? 'admin' : '');
  const [confirmPassword, setConfirmPassword] = useState(isDevelopment ? 'admin' : '');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);

  const setupClient = new SetupClient();

  const handleSubmit = async () => {
   
    console.log("submit was clicked")
    if (!validateEmail(email)) {
      setError('Invalid email address');
      return;
    }

    if (password !== confirmPassword) {
      setError('Passwords do not match');
      return;
    }

    console.log("calling client")
    await setupClient.Setup(email, password);
    console.log("completed!")
    setSuccess(true);
  };

  const validateEmail = (email) => {
    // Email validation regex pattern
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailPattern.test(email);
  };

  return (
    <Container maxWidth="xs">
      <Box mt={8}>
        <Paper elevation={3} sx={{ padding: 3 }}>
          <Typography variant="h5" align="center" gutterBottom>
            Setup
          </Typography>
          {success ? (
            <Box mt={2}>
              <Typography variant="body1" align="center">
                Setup completed successfully! <a href="/login">Login</a>
              </Typography>
            </Box>
          ) : (
           <>
              <Box mt={2}>
                <TextField
                  fullWidth
                  label="Email"
                  variant="outlined"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
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
              <Box mt={2}>
                <TextField
                  fullWidth
                  type="password"
                  label="Confirm Password"
                  variant="outlined"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                  required
                />
              </Box>
              {error && (
                <Box mt={2} color="error.main">
                  <Typography variant="body2">{error}</Typography>
                </Box>
              )}
              <Box mt={3}>
                <Button fullWidth variant="contained" color="primary" onClick={()=>handleSubmit()}>
                  Setup
                </Button>
              </Box>
              </>
          )}
        </Paper>
      </Box>
    </Container>
  );
};

export default SetupPage;
