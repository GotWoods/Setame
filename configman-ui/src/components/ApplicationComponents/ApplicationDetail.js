import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Paper,
    TextField,
    Button,
} from '@mui/material';
import { SettingGridData } from '../SettingGrid/SettingGridData';
import SettingsGrid from '../SettingGrid/SettingGrid';
import ApplicationSettingsClient from '../../applicationSettingsClient';
import EnvironmentSetSettingsClient from '../../environmentSetSettingsClient';

const ApplicationDetail = () => {
    const { applicationId } = useParams();
    const [application, setApplication] = useState(null);
    const [newSettingName, setNewSettingName] = useState('');
    const [newSettingValue, setNewSettingValue] = useState('');
    const [environments, setEnvironmentSet] = useState([]);
    const [environmentSetVariableNames, setEnvironmentSetVariableNames] = useState([]);
    // const [newEnvironmentSettingName, setNewEnvironmentSettingName] = useState('');
    // const [newEnvironmentSettings, setNewEnvironmentSettings] = useState({});
    const [transformedSettings, setTransformedSettings] = useState([]);
    const settingsClient = new ApplicationSettingsClient();
    const environmentSetSettingsClient = new EnvironmentSetSettingsClient();
    const navigate = useNavigate();

    useEffect(() => {
        fetchEnvironments();
    }, []);

    const fetchEnvironments = async () => {
        const application = await settingsClient.getApplication(applicationId);
        setApplication(application);
        const environmentSet = await environmentSetSettingsClient.getEnvironmentSet(application.environmentSetId);
        console.log("EnvironmentSet", environmentSet);
        setEnvironmentSet(environmentSet);

        const transformedSettings = loadGrid(application.environmentSettings, environmentSet.deploymentEnvironments);
        setTransformedSettings(transformedSettings);
        // fetchApplication(environments);

        let uniqueKeys = new Set();
        environmentSet.deploymentEnvironments.forEach(env => {
            Object.keys(env.environmentSettings).forEach(key => {
                uniqueKeys.add(key);
            });
        });
        setEnvironmentSetVariableNames([...uniqueKeys]);
    };

    // const fetchApplication = async (environments) => {
    //     try {
    //         const data = await settingsClient.getApplication(applicationId);
    //         setApplication(data);
    //         const transformedSettings = loadGrid(data.environmentSettings, environments);
    //         setTransformedSettings(transformedSettings);
    //     } catch (error) {
    //         console.error('Error fetching application:', error);
    //     }
    // };

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


        // let keys = Object.keys(environmentSettings);
        // keys.forEach((env) => {
        //     environmentSettings[env].forEach((setting) => {
        //         if (!result.settings[setting.name]) {
        //             result.settings[setting.name] = [];
        //         }

        //         if (!result.settings[setting.name][env]) {
        //             result.settings[setting.name][env] = "";
        //         }
        //         result.settings[setting.name][env] = setting.value;
        //     });

        // });
        return result;
    }

    // const transformSettings = (environmentSettings) => {
    //     const transformedSettings = [];

    //     let environments = Object.keys(environmentSettings);
    //     environments.forEach((env) => {
    //         const settings = environmentSettings[env] || [];
    //         settings.forEach((setting) => {
    //             if (!transformedSettings[setting.name]) {
    //                 transformedSettings[setting.name] = [];
    //             }

    //             if (!transformedSettings[setting.name][env]) {
    //                 transformedSettings[setting.name][env] = [];
    //             }

    //             transformedSettings[setting.name][env] = setting.value;
    //         });
    //     });

    //     return transformedSettings;
    // };

    const handleAddEnvironmentSettings = async () => {
        if (newSettingName.trim() === "") return;
        await settingsClient.addGlobalApplicationSetting(applicationId, newSettingName, newSettingValue);
        setApplication(prevApplication => {
            return {
                ...prevApplication,
                applicationDefaults: [...prevApplication.applicationDefaults, { name: newSettingName, value: newSettingValue }]
            }
        });
        setNewSettingName('');
        setNewSettingValue('');
    }

    const handleUpdateEnvironmentSettings = async (name, value) => {
        console.log("Updating Glboal ", name, value);
        await settingsClient.updateGlobalApplicationSetting(applicationId, name, value);

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
        await settingsClient.updateApplicationSetting(applicationId, environment, settingName, newValue);
        //setTransformedSettings(updatedSettings);
        //var foundEnvironment = enviornmentSet.deploymentEnvironments.find(x=>x.name === environment);
        //foundEnvironment.environmentSettings[settingName] = newValue;
        //console.log("Settings change", settingName, environment, newValue);
        //await settingsClient.updateEnvironmentSet(enviornmentSet);
    };

    const handleSettingRename = async (oldSettingName, newSettingName) => {
        await settingsClient.renameApplicationSetting(applicationId, oldSettingName, newSettingName);
        //await settingsClient.updateApplicationSetting(applicationName, environment, settingName, newValue);
        //setTransformedSettings(updatedSettings);
        //var foundEnvironment = enviornmentSet.deploymentEnvironments.find(x=>x.name === environment);
        //foundEnvironment.environmentSettings[settingName] = newValue;
        //await settingsClient.updateEnvironmentSet(enviornmentSet);
    };

    // const handleAddSetting = async () => {
    //     console.log("new setting");
    //     await settingsClient.addGlobalApplicationSetting(applicationId, newSettingName, newSettingValue);
    //     setNewSettingName('');
    //     setNewSettingValue('');
    //     fetchApplication();
    // };

    const handleAddEnvironmentSetting = async (newValue) => {
        if (newValue.trim() === "")
            return;

        await settingsClient.addApplicationSetting(applicationId, "ALL", newValue);
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


            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem', marginTop: '1rem', marginRight: '1rem' }}>
                <h1>{application.name}</h1>
                <div>
                    {/* <Button variant="contained" color="primary">View Applied Settings</Button> &nbsp;&nbsp; */}
                    <Button variant="contained" color="primary" onClick={() => navigate(`/applicationHistory/${applicationId}`)}>History</Button>
                </div>
            </div>

            <h2>Settings</h2>

            <div>These settings to the entire application no matter the environment. A setting here can be overriden by an explicit value being set in the environment settings<br /><br />
                {/* TODO: have a way to see applied settings an application would have that merges in environment, app global settings, variable groups, and then environment specific settings */}
            </div>



            <h2>Default Settings</h2>
            <p>
                Default settings apply to all environments. You can override a default setting by adding a variable with the same name and specifying a value for specific environments
            </p>
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
