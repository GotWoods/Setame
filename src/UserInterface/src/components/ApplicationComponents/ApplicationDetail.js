import React, { useState, useEffect } from 'react';
import {
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Paper,
    TextField,
} from '@mui/material';
import { SettingGridData } from '../SettingGrid/SettingGridData';
import SettingsGrid from '../SettingGrid/SettingGrid';
import ApplicationSettingsClient from '../../applicationSettingsClient';
import EnvironmentSetSettingsClient from '../../environmentSetSettingsClient';

const ApplicationDetail = ({applicationId, updateVersion}) => {
    const [application, setApplication] = useState(null);
    const [newSettingName, setNewSettingName] = useState('');
    const [newSettingValue, setNewSettingValue] = useState('');
    const [transformedSettings, setTransformedSettings] = useState([]);
    const settingsClient = new ApplicationSettingsClient();
    
    const fetchEnvironments = React.useCallback(async () => {
        let  settingsClient = new ApplicationSettingsClient();
        let  environmentSetSettingsClient = new EnvironmentSetSettingsClient();
        const application = await settingsClient.getApplication(applicationId);
        setApplication(application);
        const environmentSet = await environmentSetSettingsClient.getEnvironmentSet(application.environmentSetId);

        const transformedSettings = loadGrid(application.environmentSettings, environmentSet.deploymentEnvironments);
        setTransformedSettings(transformedSettings);

        let uniqueKeys = new Set();
        environmentSet.deploymentEnvironments.forEach(env => {
            Object.keys(env.environmentSettings).forEach(key => {
                uniqueKeys.add(key);
            });
        });
      }, [applicationId, ]);

    useEffect(() => {
        fetchEnvironments();
    }, [fetchEnvironments]);

    const loadGrid = (environmentSettings, environments) => {
        var result = new SettingGridData();

        environments.forEach((env) => {
            result.environments.push(env.name);
        });

        environmentSettings.forEach((environment) => {
            environment.settings.forEach(setting => {
                if (!result.settings[setting.name]) {
                    result.settings[setting.name] = [];
                }

                if (!result.settings[setting.name][environment.name]) {
                    result.settings[setting.name][environment.name] = "";
                }

                result.settings[setting.name][environment.name] = setting.value;
            });
        })
        return result;
    }

    const handleAddEnvironmentSettings = async () => {
        console.log("Adding global setting", application)
        if (newSettingName.trim() === "") return;
        await settingsClient.addGlobalApplicationSetting(application, newSettingName, newSettingValue);
        setApplication(prevApplication => {
            return {
                ...prevApplication,
                applicationDefaults: [...prevApplication.applicationDefaults, { name: newSettingName, value: newSettingValue }]
            }
        });
        setNewSettingName('');
        setNewSettingValue('');
        updateVersion(application.id, application.version);
    }

    const handleUpdateEnvironmentSettings = async (name, value) => {
        console.log("handleUpdateEnvironmentSettings", name, value);
        await settingsClient.updateGlobalApplicationSetting(application, name, value);
        updateVersion(application.id, application.version);
        // update the application state with the updated setting
        // setApplication(prevApplication => {
        //     return {
        //         ...prevApplication,
        //         applicationDefaults: prevApplication.applicationDefaults.map(s => 
        //             s.id === id ? {...s, name, value} : s
        //         )
        //     }
        // });
    }
    const handleSettingChange = async (settingName, environment, newValue) => {
        await settingsClient.updateApplicationSetting(application, environment, settingName, newValue);
        updateVersion(application.id, application.version);
        //setTransformedSettings(updatedSettings);
        //var foundEnvironment = enviornmentSet.deploymentEnvironments.find(x=>x.name === environment);
        //foundEnvironment.environmentSettings[settingName] = newValue;
        //await settingsClient.updateEnvironmentSet(enviornmentSet);
    };

    const handleSettingRename = async (oldSettingName, newSettingName) => {
        await settingsClient.renameApplicationSetting(application, oldSettingName, newSettingName);
        updateVersion(application.id, application.version);
        //await settingsClient.updateApplicationSetting(applicationName, environment, settingName, newValue);
        //setTransformedSettings(updatedSettings);
        //var foundEnvironment = enviornmentSet.deploymentEnvironments.find(x=>x.name === environment);
        //foundEnvironment.environmentSettings[settingName] = newValue;
        //await settingsClient.updateEnvironmentSet(enviornmentSet);
    };

    // const handleAddSetting = async () => {
    //     await settingsClient.addGlobalApplicationSetting(applicationId, newSettingName, newSettingValue);
    //     setNewSettingName('');
    //     setNewSettingValue('');
    //     fetchApplication();
    // };

    const handleAddEnvironmentSetting = async (newValue) => {
        if (newValue.trim() === "")
            return;

        await settingsClient.addApplicationSetting(application, "ALL", newValue);
        updateVersion(application.id, application.version);
        // environments.deploymentEnvironments.forEach(async (env) => {


        //     // if (application.environmentSettings[env.name] == null) {
        //     //     let newEnv = {};
        //     //     newEnv[env.name] = {};
        //     //     application.environmentSettings[env.name] = newEnv;
        //     // }

        //     // let obj = {};
        //     // obj[newValue] = "";
        //     // application.environmentSettings[env.name] = obj;
        // });


        //await settingsClient.updateEnvironmentSet(enviornmentSet);
    };


    if (!application) {
        return <div>Loading...</div>;
    }

    if (!transformedSettings) {
        return <div>Loading...</div>;
    }


    return (
        <div>


                    {/* <Button variant="contained" color="primary">View Applied Settings</Button> &nbsp;&nbsp; */}
              
            <h2>Default Settings</h2>
            <TableContainer component={Paper}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>Setting Name</TableCell>
                            <TableCell>Setting Value</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {application.applicationDefaults?.map((setting) => (
                            <TableRow key={setting.name}>
                                <TableCell>
                                    <TextField
                                        defaultValue={setting.name}
                                        fullWidth
                                        onBlur={(e) => handleUpdateEnvironmentSettings(e.target.value, setting.value)}
                                    />
                                </TableCell>
                                <TableCell>
                                    <TextField
                                        defaultValue={setting.value}
                                        fullWidth
                                        onBlur={(e) => handleUpdateEnvironmentSettings(setting.name, e.target.value)}
                                    />
                                </TableCell>
                            </TableRow>
                        ))}
                        <TableRow>
                            <TableCell>
                                <TextField
                                    value={newSettingName} fullWidth
                                    onChange={(e) => setNewSettingName(e.target.value)}
                                    onBlur={handleAddEnvironmentSettings}
                                    placeholder="Setting Name"
                                />
                            </TableCell>
                            <TableCell>
                                <TextField
                                    value={newSettingValue} fullWidth
                                    onChange={(e) => setNewSettingValue(e.target.value)}
                                    placeholder="Setting Value"
                                />
                            </TableCell>
                        </TableRow>
                    </TableBody>
                </Table>
            </TableContainer>

            <h2>Environment Specific Settings</h2>
            {/* <div>TODO: ability to use your own environments just for this application and ignore the global environments</div> */}
            {/* <TableContainer component={Paper}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>Setting Name</TableCell>
                            {environments?.deploymentEnvironments?.map((e, index) => (
                                <TableCell key={index}>{e.name}</TableCell>
                            ))}
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {environmentSetVariableNames.map((e, index) => (
                            <TableRow key={index}>
                                <TableCell>{e}</TableCell>
                                {environments?.deploymentEnvironments?.map((env, index2) => (
                                    <TableCell key={index2}>{env.environmentSettings[e]}</TableCell>
                                ))}
                            </TableRow>
                        ))}

                    </TableBody>
                </Table>
            </TableContainer> */}

            {transformedSettings.environments && (
                <SettingsGrid
                    transformedSettings={transformedSettings}
                    onAddSetting={handleAddEnvironmentSetting}
                    onSettingChange={handleSettingChange}
                    onSettingRename={handleSettingRename}
                    showEditButtons={false}
                />
            )}

        </div>
    );
};

export default ApplicationDetail;
