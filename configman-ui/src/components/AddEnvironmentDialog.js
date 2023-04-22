import React, { useState } from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';


const AddEnvironmentDialog = ({ open, onClose, onEnvironmentAdded }) => {
  const [environmentName, setEnvironmentName] = useState('');

  const handleAddEnvironment = async () => {
    const requestOptions = {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('authToken')}`
      },
      body: JSON.stringify({ name: environmentName })
    };

    try {
      const response = await fetch(`${window.appSettings.apiBaseUrl}/api/environments`, requestOptions);

      if (!response.ok) {
        throw new Error('Failed to add environment');
      }

      onClose();
      if (onEnvironmentAdded) {
        onEnvironmentAdded();
      }
    } catch (error) {
      console.error('Error adding environment:', error);
    }
  };

  return (
    <div>
      <Dialog open={open} onClose={onClose} aria-labelledby="form-dialog-title">
        <DialogTitle id="form-dialog-title">Add Environment</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            id="name"
            label="Environment Name"
            type="text"
            fullWidth
            value={environmentName}
            onChange={(e) => setEnvironmentName(e.target.value)}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={onClose} color="primary">
            Cancel
          </Button>
          <Button onClick={handleAddEnvironment} color="primary">
            Add
          </Button>
        </DialogActions>
      </Dialog>
    </div>
  );
};

export default AddEnvironmentDialog;
