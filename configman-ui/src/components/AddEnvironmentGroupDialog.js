import React, { useState } from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import SettingsClient from '../settingsClient';


const AddEnvironmentGroupDialog = ({ open, onClose, onAdded }) => {
  const [environmentGroupName, setEnvironmentGroupName] = useState('');
  const settingsClient = new SettingsClient();

  const handleAddEnvironmentGroup = async () => {
    await settingsClient.addEnvironmentGroup(environmentGroupName);
    onClose();
    if (onAdded) {
      onAdded();
    }
  };

    return (
      <div>
        <Dialog open={open} onClose={onClose} aria-labelledby="form-dialog-title">
          <DialogTitle id="form-dialog-title">Add Environment Group</DialogTitle>
          <DialogContent>
            <TextField
              autoFocus
              margin="dense"
              id="name"
              label="Environment Group Name"
              type="text"
              fullWidth
              value={environmentGroupName}
              onChange={(e) => setEnvironmentGroupName(e.target.value)}
            />
          </DialogContent>
          <DialogActions>
            <Button onClick={onClose} color="primary">
              Cancel
            </Button>
            <Button onClick={handleAddEnvironmentGroup} color="primary">
              Add
            </Button>
          </DialogActions>
        </Dialog>
      </div>
    );
  };

  export default AddEnvironmentGroupDialog;
