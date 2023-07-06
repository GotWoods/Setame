import React, { useState, useEffect } from 'react';
import AddEnvironmentDialog from './AddEnvironmentDialog';
import { SettingGridData } from '../SettingGrid/SettingGridData';
import SettingsGrid from '../SettingGrid/SettingGrid';
import Box from '@mui/material/Box';
import EnvironmentSetSettingsClient from '../../environmentSetSettingsClient';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import {
    TextField,
    Button,
} from '@mui/material';
import { useNavigate } from 'react-router-dom';

const EnvironmentSetDetail = ({ environmentSet, refreshRequested }) => {
    const [deleteConfirmationOpen, setDeleteConfirmationOpen] = useState(false);
    const [transformedSettings, setTransformedSettings] = useState([]);
    const [environmentDialogOpen, setEnvironmentDialogOpen] = useState(false);
    const [environmentSetName, setEnvironmentSetName] = useState(environmentSet.name);
    const settingsClient = new EnvironmentSetSettingsClient();
    const [isEditingName, setIsEditingName] = useState(false);

    const navigate = useNavigate();

    const handleAddEnvironmentSetDialogClose = () => {
        setEnvironmentDialogOpen(false);

        if (refreshRequested !== undefined)
            refreshRequested();
    };

    const handleRenameEnvironmentClick = () => {
        setIsEditingName(true);
    }

    const handleRenameEnvironment = async () => {
        await settingsClient.renameEnvironmentSet(environmentSet.id, environmentSetName);
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
        await settingsClient.deleteEnvironmentSet(environmentSet.id);
        if (refreshRequested !== undefined)
            refreshRequested();
    };

    useEffect(() => {
        const transformedSettings = loadGrid(environmentSet.deploymentEnvironments);
        setTransformedSettings(transformedSettings);
        console.log('transformed', transformedSettings);
    }, [environmentSet]);  // The function will run whenever environmentSet changes

    const loadGrid = (environments) => {
        var result = new SettingGridData();
        environments.forEach((env) => {
            result.environments.push(env.name);
            if (env.environmentSettings === undefined)
                return;
            for (let setting in env.environmentSettings) {
                if (!result.settings[setting]) {
                    result.settings[setting] = [];
                }

                if (!result.settings[setting][env.name]) {
                    result.settings[setting][env.name] = env.environmentSettings[setting];
                }

                result.settings[setting][env.name] = env.environmentSettings[setting];
            }
        });
        return result;
    }
    const handleEnvironmentDetailsClick = (env) => {
        // setSelectedEnvironment(env);
        // setCurrentEnvironment(env);
        // setEnvironmentDetailsDialogOpen(true);
    };

    const handleAddEnvironmentSetting = async (newValue) => {
        if (newValue === "")
            return;
        environmentSet.deploymentEnvironments.forEach(env => {
            let obj = {};
            obj[newValue] = "";
            env.environmentSettings[newValue] = "";

        });
        await settingsClient.addVariableToEnvironmentSet(newValue, environmentSet.id);
    };


    const handleSettingRename = async (originalName, newName) => {
        if (newName === "")
            return;
        await settingsClient.renameVariableOnEnvironmentSet(originalName, newName, environmentSet.id);
    };


    const handleSettingChange = async (settingName, environment, newValue) => {
        var foundEnvironment = environmentSet.deploymentEnvironments.find(x => x.name === environment);
        foundEnvironment.environmentSettings[settingName] = newValue;
        console.log("Settings change", settingName, environment, newValue);
        await settingsClient.updateVariableOnEnvironmentSet(environment, settingName, newValue, environmentSet.id);
    };

    return (
        <div>
            <AddEnvironmentDialog
                open={environmentDialogOpen}
                onClose={handleAddEnvironmentSetDialogClose}
                //onAdded={fetchEnvironments}
                environmentSet={environmentSet} />
                {
                    isEditingName ? (
                        <TextField
                            value={environmentSetName}
                            onChange={(e) => setEnvironmentSetName(e.target.value)}
                            onBlur={handleRenameEnvironment}
                            autoFocus
                        />
                    ) : (
                        <h2>
                            {environmentSetName}
                            <Button onClick={() => handleRenameEnvironmentClick()} color="secondary">
                                <i className="fa-regular fa-pen-to-square"></i>&nbsp;
                            </Button>
                            <Button onClick={() => handleDeleteEnvironmentClick()} color="secondary">
                                <i className="fa-solid fa-trash-can"></i>
                            </Button>
                            <Button onClick={() => navigate(`/environmentSetHistory/${environmentSet.id}`)} color="secondary">
                                <i className="fa-solid fa-clock-rotate-left"></i>
                            </Button>

                        </h2>
                    )
                }

            {
                transformedSettings.environments?.length > 0 ? (
                    <>
                        <Box display="flex" justifyContent="flex-end">
                            <Button variant="contained" color="primary" onClick={() => setEnvironmentDialogOpen(true)}>Add Environment</Button>
                        </Box>
                        <SettingsGrid
                            transformedSettings={transformedSettings}
                            onAddSetting={handleAddEnvironmentSetting}
                            onHeaderClick={handleEnvironmentDetailsClick}
                            onSettingRename={handleSettingRename}
                            onSettingChange={handleSettingChange}
                        />
                    </>
                ) : (
                    <>
                        <Box
                            display="flex"
                            flexDirection="column"
                            alignItems="center"
                            justifyContent="center"
                            height="10vh" // Or set a specific height you want
                        >
                            <p>To use this Environment Set, you first need to add at least one environment (e.g. Dev/Stage/Prod)</p>
                            <Button variant="contained" color="primary" onClick={() => setEnvironmentDialogOpen(true)}>Add Environment</Button>
                        </Box>
                    </>

                )
            }
            {/* Other components */}
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
        </div>

    );
};

export default EnvironmentSetDetail;
