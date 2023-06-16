import React, { useState, useEffect } from 'react';
import Button from '@mui/material/Button';
import AddEnvironmentDialog from './AddEnvironmentDialog';
import { SettingGridData, SettingGridItem } from '../SettingGrid/SettingGridData';
import SettingsGrid from '../SettingGrid/SettingGrid';
import Box from '@mui/material/Box';
import SettingsClient from '../../settingsClient';


const EnvironmentSetDetail = ({ enviornmentSet }) => {


    const [transformedSettings, setTransformedSettings] = useState([]);
    const [environmentDialogOpen, setEnvironmentDialogOpen] = useState(false);
    const settingsClient = new SettingsClient();

    const handleAddEnvironmentSetDialogClose = () => {
        setEnvironmentDialogOpen(false);
    };

    useEffect(() => {
        const transformedSettings = loadGrid(enviornmentSet.deploymentEnvironments);
        setTransformedSettings(transformedSettings);
    }, [enviornmentSet]);  // The function will run whenever environmentSet changes

    const loadGrid = (environments) => {
        console.log("loading grid", environments);
        var result = new SettingGridData();
        environments.forEach((env) => {
            result.environments.push(env.name);
            if (env.environmentSettings == undefined)
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
            console.log("final", result);
        });
        return result;
    }
    const handleEnvironmentDetailsClick = (env) => {
        // setSelectedEnvironment(env);
        // setCurrentEnvironment(env);
        // setEnvironmentDetailsDialogOpen(true);
    };

    const handleAddEnvironmentSetting = async (newValue) => {
        if (newValue == "")
            return;
        enviornmentSet.deploymentEnvironments.forEach(env => {
            let obj = {};
            obj[newValue] = "";
            env.environmentSettings[newValue] = "";

        });
        await settingsClient.updateEnvironmentSet(enviornmentSet);
    };

    const handleSettingChange = async (settingName, environment, newValue, updatedSettings) => {
        setTransformedSettings(updatedSettings); // assuming setTransformedSettings is the function to update the state
        var foundEnvironment = enviornmentSet.deploymentEnvironments.find(x=>x.name === environment);
        foundEnvironment.environmentSettings[settingName] = newValue;
        console.log("Settings change", settingName, environment, newValue);
        await settingsClient.updateEnvironmentSet(enviornmentSet);
    };

    return (
        <div>
            <AddEnvironmentDialog
                open={environmentDialogOpen}
                onClose={handleAddEnvironmentSetDialogClose}
                //onAdded={fetchEnvironments}
                environmentSet={enviornmentSet} />
            <h2>{enviornmentSet.name}&nbsp;
            {/* <i className="fa-regular fa-pen-to-square"></i>&nbsp;<i className="fa-solid fa-trash-can"></i> */}
            </h2>
            <Box display="flex" justifyContent="flex-end">
                <Button variant="contained" color="primary" onClick={() => setEnvironmentDialogOpen(true)}>Add Environment</Button>
            </Box>

            {transformedSettings.environments && (
                <SettingsGrid
                    transformedSettings={transformedSettings}
                    onAddSetting={handleAddEnvironmentSetting}
                    onHeaderClick={handleEnvironmentDetailsClick}
                    onSettingChange={handleSettingChange}
                />
            )}
            {/* <Button variant="contained" color="primary"  onClick={() => setEnvironmentDialogOpen(true)}>Add Variable</Button>  */}
        </div>
    );
};

export default EnvironmentSetDetail;
