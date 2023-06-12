import React, { useState, useEffect } from 'react';
import Button from '@mui/material/Button';
import AddEnvironmentDialog from './AddEnvironmentDialog';
import { SettingGridData, SettingGridItem } from '../SettingGrid/SettingGridData';
import SettingsGrid from '../SettingGrid/SettingGrid';

const EnvironmentSetDetail = ({ enviornmentSet }) => {


    const [transformedSettings, setTransformedSettings] = useState([]);
    const [environmentDialogOpen, setEnvironmentDialogOpen] = useState(false);

    const handleAddEnvironmentSetDialogClose = () => {
        setEnvironmentDialogOpen(false);
    };

    useEffect(() => {
        const transformedSettings = loadGrid(enviornmentSet.deploymentEnvironments);
         setTransformedSettings(transformedSettings);
    });  // The function will run whenever environmentSet changes
    
    const loadGrid = (environments) => {
        var result = new SettingGridData();
        environments.forEach((env) => {
            result.environments.push(env.name);
            if (env.settings == undefined)
                return;
            env.settings.forEach((setting) => {
                if (!result.settings[setting.name]) {
                    result.settings[setting.name] = [];
                }

                if (!result.settings[setting.name][env.name]) {
                    result.settings[setting.name][env.name] = "";
                }
                result.settings[setting.name][env.name] = setting.value;
            });

            console.log("final", result);
        });
        return result;
    }
    const handleEnvironmentDetailsClick = (env) => {
        // setSelectedEnvironment(env);
        // setCurrentEnvironment(env);
        // setEnvironmentDetailsDialogOpen(true);
    };

    const handleAddEnvironmentSetting = async (newEnvironmentSettingName, newEnvironmentSettings) => {
        // let keys = Object.keys(newEnvironmentSettings);
        // let allSettings = keys.map(env => {
        //   return {
        //     environment: env,
        //     name: newEnvironmentSettingName,
        //     value: newEnvironmentSettings[env] || '',
        //   }
        // });

        // await settingsClient.addEnvironmentSettings(allSettings);
        // fetchEnvironmentSets();
    };

    const handleSettingChange = async (settingName, environment, newValue) => {
        // Update the API with the new setting value
        //await settingsClient.updateEnvironmentSet(settingName, environment, newValue);
        //fetchEnvironmentSets();
    };

    return (
        <div>
            <AddEnvironmentDialog key={environmentDialogOpen ? 'open' : 'closed'}
                open={environmentDialogOpen}
                onClose={handleAddEnvironmentSetDialogClose}
                //onAdded={fetchEnvironments}
                environmentSet={enviornmentSet} />
            <h2>{enviornmentSet.name}&nbsp;<i className="fa-regular fa-pen-to-square"></i>&nbsp;<i className="fa-solid fa-trash-can"></i></h2>
            <Button variant="contained" color="primary"  onClick={() => setEnvironmentDialogOpen(true)}>Add Environment</Button> 
            TODO: grid here of setting name + environments (e.g. dev/stage/preprod/prod)
            {/* {environments.map((env) => (
      <>
        {env.name}
      </>
    ))} */}


            {transformedSettings.environments && (
                <SettingsGrid
                    transformedSettings={transformedSettings}
                    onAddSetting={handleAddEnvironmentSetting}
                    onHeaderClick={handleEnvironmentDetailsClick}
                    onSettingChange={handleSettingChange}
                />
            )}

        </div>
    );
};

export default EnvironmentSetDetail;
