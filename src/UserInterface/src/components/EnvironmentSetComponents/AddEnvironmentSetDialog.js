import React, { useState, useContext } from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import EnvironmentSetSettingsClient from '../../clients/environmentSetSettingsClient';
import ErrorContext from '../../ErrorContext';

const AddEnvironmentSetDialog = ({ open, onClose, onAdded }) => {
  const [environmentName, setEnvironmentName] = useState('');
  const settingsClient = new EnvironmentSetSettingsClient();
  const { setErrorMessage } = useContext(ErrorContext);
  const handleAddEnvironment = async () => {
    var result = await settingsClient.addEnvironmentSet(environmentName);
    if (!result.wasSuccessful) {
      setErrorMessage(result.errors);
      return;
    }

    onClose();
    if (onAdded) {
      onAdded();
    }
  };

    return (
      <div>
        <Dialog open={open} onClose={onClose} aria-labelledby="form-dialog-title">
          <DialogTitle id="form-dialog-title">Add Environment Set</DialogTitle>
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

  export default AddEnvironmentSetDialog;
