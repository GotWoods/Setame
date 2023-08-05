import React, { useState, useEffect, useContext } from 'react';
import AddEnvironmentDialog from './AddEnvironmentDialog';
import { SettingGridData } from '../SettingGrid/SettingGridData';
import SettingsGrid from '../SettingGrid/SettingGrid';
import Box from '@mui/material/Box';
import EnvironmentSetSettingsClient from '../../clients/environmentSetSettingsClient';
import {
    Button,
} from '@mui/material';
import EnvironmentSetName from './EnvironmentSetName';
import RenameEnvironmentDialog from './RenameEnvironmentDialog';
import DeleteEnvironmentDialog from './DeleteEnvironmentDialog';
import ErrorContext from '../../ErrorContext';

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
    const { setErrorMessage } = useContext(ErrorContext);

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
            var result = await settingsClient.renameEnvironment(environmentSet, originalEnvironmentName, editedEnvironmentName);
            console.log("Rename result", result);
            if (!result.wasSuccessful) {
                
                setErrorMessage(result.errors);
                return;
            }
            setOriginalEnvironmentName(null);
            setEditedEnvironmentName(null);
        }
        setRenameEnvironmentDialogOpen(false);
    };


    const handleDeleteEnvironment = async (environmentName) => {
        const response = await settingsClient.getEnvironmentSetToApplicationAssociation(environmentSet.id);
        // if (!response.wasSuccessful) {
        //     setErrorMessage(response.errors);
        //     return;
        // }
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
            var result = await settingsClient.deleteEnvironment(environmentSet, environmentToDelete);
            if (!result.wasSuccessful) {
                setErrorMessage(result.errors);
                return;
            }
            setEnvironmentToDelete(null);
        }
        setDeleteEnvironmentDialogOpen(false);

        if (refreshRequested !== undefined)
            refreshRequested();
    }

    useEffect(() => {
        const transformedSettings = loadGrid(environmentSet.environments);
        setTransformedSettings(transformedSettings);
    }, [environmentSet]);  // The function will run whenever environmentSet changes

    const loadGrid = (environments) => {
        var result = new SettingGridData();
        environments.forEach((env) => {
            result.environments.push(env.name);
            if (env.settings === undefined)
                return;
            for (let setting in env.settings) {
                if (!result.settings[setting]) {
                    result.settings[setting] = [];
                }

                if (!result.settings[setting][env.name]) {
                    result.settings[setting][env.name] = env.settings[setting];
                }

                result.settings[setting][env.name] = env.settings[setting];
            }
        });
        return result;
    }

    const handleAddEnvironmentSetting = async (newValue) => {
        if (newValue === "")
            return;
        environmentSet.environments.forEach(env => {
            let obj = {};
            obj[newValue] = "";
            env.settings[newValue] = "";

        });
        var result = await settingsClient.addVariableToEnvironmentSet(environmentSet, newValue);
        if (!result.wasSuccessful) {
            setErrorMessage(result.errors);
            return;
        }
    };


    const handleSettingRename = async (originalName, newName) => {
        if (newName === "")
            return;
        var result = await settingsClient.renameVariableOnEnvironmentSet(environmentSet, originalName, newName);
        if (!result.wasSuccessful) {
            setErrorMessage(result.errors);
            return;
        }
        transformedSettings.settings[newName] = transformedSettings.settings[originalName]; //copy children from old to new
        delete transformedSettings.settings[originalName];
        setTransformedSettings({...transformedSettings}); //use the spread operator to create a new reference so React updates
    };


    const handleSettingChange = async (settingName, environment, newValue) => {
        var foundEnvironment = environmentSet.environments.find(x => x.name === environment);
        foundEnvironment.settings[settingName] = newValue;
        var result = await settingsClient.updateVariableOnEnvironmentSet(environmentSet, environment, settingName, newValue);
        if (!result.wasSuccessful) {
            setErrorMessage(result.errors);
            return;
        }
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
