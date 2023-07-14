import React from 'react';
import { Dialog, DialogTitle, DialogContent, DialogContentText, DialogActions, Button } from '@mui/material';

const RenameEnvironmentDialog = ({ open, handleClose, handleConfirm, applications, editedEnvironmentName, originalEnvironmentName }) => {
    return (
        <Dialog open={open} onClose={handleClose}>
            <DialogTitle>Associated Applications</DialogTitle>
            <DialogContent>
                <DialogContentText>
                    Renaming this environment will rename the environment in these applications:
                    <ul>
                        {applications.map((app, index) => (
                            <li key={index}>{app.name}</li>
                        ))}
                    </ul>
                    Ensure you have updated the applications to use the new environment name or else the application may fail to load its configuration.
                </DialogContentText>
            </DialogContent>
            <DialogActions>
                <Button onClick={handleClose} color="primary">
                    Cancel
                </Button>
                <Button onClick={handleConfirm} color="primary">
                    Confirm
                </Button>
            </DialogActions>
        </Dialog>
    );
};

export default RenameEnvironmentDialog;
