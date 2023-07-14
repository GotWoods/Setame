import React, { useState } from 'react';
import { TextField, Button, Dialog, DialogTitle, DialogContent, DialogContentText, DialogActions } from '@mui/material';
import EnvironmentSetSettingsClient from '../../environmentSetSettingsClient';
import { useNavigate } from 'react-router-dom';

const EnvironmentSetName = ({ environmentSet, refreshRequested }) => {
    const [isEditingName, setIsEditingName] = useState(false);
    const [environmentSetName, setEnvironmentSetName] = useState(environmentSet.name);
    const [deleteConfirmationOpen, setDeleteConfirmationOpen] = useState(false);
    const settingsClient = new EnvironmentSetSettingsClient();
    const navigate = useNavigate();

    const handleRenameEnvironmentClick = () => {
        setIsEditingName(true);
    }

    const handleRenameEnvironmentSet = async () => {
        await settingsClient.renameEnvironmentSet(environmentSet, environmentSetName);
        setIsEditingName(false);
    }

    const handleDeleteEnvironmentClick = () => {
        setDeleteConfirmationOpen(true);
    };

    const handleCloseDeleteConfirmation = () => {
        setDeleteConfirmationOpen(false);
    };

    const handleConfirmDeleteEnvironment = async () => {
        setDeleteConfirmationOpen(false);
        await settingsClient.deleteEnvironmentSet(environmentSet);
        if (refreshRequested !== undefined)
            refreshRequested();
    };

    return (
        <>
            {isEditingName ? (
                <TextField
                    value={environmentSetName}
                    onChange={(e) => setEnvironmentSetName(e.target.value)}
                    onBlur={handleRenameEnvironmentSet}
                    autoFocus
                />
            ) : (
                <h2>
                    {environmentSetName}
                    <Button onClick={handleRenameEnvironmentClick} color="secondary">
                        <i className="fa-regular fa-pen-to-square"></i>&nbsp;
                    </Button>
                    <Button onClick={handleDeleteEnvironmentClick} color="secondary">
                        <i className="fa-solid fa-trash-can"></i>
                    </Button>
                    <Button onClick={() => navigate(`/environmentSetHistory/${environmentSet.id}`)} color="secondary">
                        <i className="fa-solid fa-clock-rotate-left"></i>
                    </Button>
                </h2>
            )}

            {/* Delete confirmation dialog */}
            <Dialog
                open={deleteConfirmationOpen}
                onClose={handleCloseDeleteConfirmation}
                aria-labelledby="delete-confirmation-dialog-title"
                aria-describedby="delete-confirmation-dialog-description"
            >
                <DialogTitle id="delete-confirmation-dialog-title">
                    {'Delete Environment'}
                </DialogTitle>
                <DialogContent>
                    <DialogContentText id="delete-confirmation-dialog-description">
                        Are you sure you want to delete this environment? This action cannot be undone.
                    </DialogContentText>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseDeleteConfirmation} color="primary">
                        Cancel
                    </Button>
                    <Button onClick={handleConfirmDeleteEnvironment} color="secondary">
                        Delete
                    </Button>
                </DialogActions>
            </Dialog>
        </>
    );
};

export default EnvironmentSetName;
