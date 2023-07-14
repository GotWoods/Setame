import React from 'react';
import { Dialog, DialogTitle, DialogContent, DialogContentText, DialogActions, Button } from '@mui/material';

const DeleteEnvironmentDialog = ({ open, handleClose, handleConfirm, applications}) => {
    return (
        <Dialog open={open} onClose={handleClose}>
            <DialogTitle>Associated Applications</DialogTitle>
            <DialogContent>
                <DialogContentText>
                    Deleting this environment will delete the environment from these applications:
                    <ul>
                        {applications.map((app, index) => (
                            <li key={index}>{app.name}</li>
                        ))}
                    </ul>
                    Ensure no applications use this environment or else the application may fail to load its configuration.
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

export default DeleteEnvironmentDialog;
