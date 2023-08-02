import React, { useState } from 'react';
import { useLocation } from 'react-router-dom';
import { TextField, Button, Typography, Box, Paper, Container } from '@mui/material';
import SettingsClient from '../clients/settingsClient';

const ResetPasswordPage = () => {
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);

  const settingsClient = new SettingsClient();
  const location = useLocation();
  const queryParams = new URLSearchParams(location.search);
  const token = queryParams.get('token');

  const handleSubmit = async () => {
    if (password !== confirmPassword) {
      setError('Passwords do not match');
      return;
    }

    const result = await settingsClient.passwordReset(token, password);
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
            <Box mt={2}>
              <Typography variant="body1" align="center">
                Password reset successfully! <a href="/login">Login</a>
              </Typography>
            </Box>
          ) : (
            <>
              <Box mt={2}>
                <TextField
                  fullWidth
                  type="password"
                  label="New Password"
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
                  label="Confirm New Password"
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
                <Button fullWidth variant="contained" color="primary" onClick={() => handleSubmit()}>
                  Reset
                </Button>
              </Box>
            </>
          )}
        </Paper>
      </Box>
    </Container>
  );
};

export default ResetPasswordPage;
