import React, { useState, useEffect } from 'react';
import AddEnvironmentDialog from './AddEnvironmentDialog';
import { SettingGridData } from '../SettingGrid/SettingGridData';
import SettingsGrid from '../SettingGrid/SettingGrid';
import Box from '@mui/material/Box';
import EnvironmentSetSettingsClient from '../../environmentSetSettingsClient';
import {
    Button,
} from '@mui/material';
import EnvironmentSetName from './EnvironmentSetName';
import RenameEnvironmentDialog from './RenameEnvironmentDialog';
import DeleteEnvironmentDialog from './DeleteEnvironmentDialog';

const EnvironmentSetDetail = ({ environmentSet, refreshRequested }) => {
    const [transformedSettings, setTransformedSettings] = useState([]);
    const [environmentDialogOpen, setEnvironmentDialogOpen] = useState(false);
    const settingsClient = new EnvironmentSetSettingsClient();

    const [renameEnvironmentDialogOpen, setRenameEnvironmentDialogOpen] = useState(false);
    const [applications, setApplications] = useState([]);
    const [editedEnvironmentName, setEditedEnvironmentName] = useState(null);
    const [originalEnvironmentName, setOriginalEnvironmentName] = useState(null);
    const [deleteEnvironmentDialogOpen, setDeleteEnvironmentDialogOpen] = useState(false);
    const [environmentToDelete, setEnvironmentToDelete] = useState(null);


    const handleAddEnvironmentSetDialogClose = () => {
        setEnvironmentDialogOpen(false);

        if (refreshRequested !== undefined)
            refreshRequested();
    };

    const handleRenameEnvironmentClose = () => {
        setRenameEnvironmentDialogOpen(false);
        setEditedEnvironmentName(null);
    };

    const handleConfirmRenameEnvironment = async () => {
        if (editedEnvironmentName !== null) {
            await settingsClient.renameEnvironment(environmentSet, originalEnvironmentName, editedEnvironmentName);
            setOriginalEnvironmentName(null);
            setEditedEnvironmentName(null);
        }
        setRenameEnvironmentDialogOpen(false);
    };


    const handleDeleteEnvironment = async (environmentName) => {
        const response = await settingsClient.getEnvironmentSetToApplicationAssociation(environmentSet.id);
        setApplications(response.applications);
        setEnvironmentToDelete(environmentName);
        setDeleteEnvironmentDialogOpen(true);
    }

    const handleDeleteEnvironmentDialogClose = () => {
        setDeleteEnvironmentDialogOpen(false);
        setEnvironmentToDelete(null);
    }

    const handleConfirmDeleteEnvironment = async () => {
        if (environmentToDelete !== null) {
            await settingsClient.deleteEnvironment(environmentSet, environmentToDelete);
            setEnvironmentToDelete(null);
        }
        setDeleteEnvironmentDialogOpen(false);

        if (refreshRequested !== undefined)
            refreshRequested();
    }

    useEffect(() => {
        const transformedSettings = loadGrid(environmentSet.deploymentEnvironments);
        setTransformedSettings(transformedSettings);
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

    const handleAddEnvironmentSetting = async (newValue) => {
        if (newValue === "")
            return;
        environmentSet.deploymentEnvironments.forEach(env => {
            let obj = {};
            obj[newValue] = "";
            env.environmentSettings[newValue] = "";

        });
        await settingsClient.addVariableToEnvironmentSet(environmentSet, newValue);
    };


    const handleSettingRename = async (originalName, newName) => {
        if (newName === "")
            return;
        await settingsClient.renameVariableOnEnvironmentSet(environmentSet, originalName, newName);
    };


    const handleSettingChange = async (settingName, environment, newValue) => {
        var foundEnvironment = environmentSet.deploymentEnvironments.find(x => x.name === environment);
        foundEnvironment.environmentSettings[settingName] = newValue;
        await settingsClient.updateVariableOnEnvironmentSet(environmentSet, environment, settingName, newValue);
    };

    const handleEnvironmentRename = async (originalValue, newValue) => {
        setOriginalEnvironmentName(originalValue);
        setEditedEnvironmentName(newValue);
        setRenameEnvironmentDialogOpen(true);
        const response = await settingsClient.getEnvironmentSetToApplicationAssociation(environmentSet.id);
        setApplications(response.applications);

    }
    return (
        <div>
            <AddEnvironmentDialog
                open={environmentDialogOpen}
                onClose={handleAddEnvironmentSetDialogClose}
                environmentSet={environmentSet} />
            <EnvironmentSetName environmentSet={environmentSet} refreshRequested={refreshRequested} />
            {
                transformedSettings.environments?.length > 0 ? (
                    <>
                        <Box display="flex" justifyContent="flex-end">
                            <Button variant="contained" color="primary" onClick={() => setEnvironmentDialogOpen(true)}>Add Environment</Button>
                        </Box>
                        <SettingsGrid
                            transformedSettings={transformedSettings}
                            onAddSetting={handleAddEnvironmentSetting}
                            onSettingRename={handleSettingRename}
                            onSettingChange={handleSettingChange}
                            onEnvironmentRename={handleEnvironmentRename}
                            onDeleteEnvironment={handleDeleteEnvironment}
                            showEditButtons={true}
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

            <RenameEnvironmentDialog
                open={renameEnvironmentDialogOpen}
                handleClose={handleRenameEnvironmentClose}
                handleConfirm={handleConfirmRenameEnvironment}
                applications={applications}
                editedEnvironmentName={editedEnvironmentName}
                originalEnvironmentName={originalEnvironmentName}
            />

            <DeleteEnvironmentDialog
                open={deleteEnvironmentDialogOpen}
                handleClose={handleDeleteEnvironmentDialogClose}
                handleConfirm={handleConfirmDeleteEnvironment}
                applications={applications}
            />
        </div>

    );
};

export default EnvironmentSetDetail;
