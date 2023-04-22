import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import Grid from '@mui/material/Grid';
import Box from '@mui/material/Box';
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
import { SettingGridData, SettingGridItem } from './SettingGrid/SettingGridData';
import SettingsGrid from './SettingGrid/SettingGrid';

const ApplicationDetail = () => {
    const { applicationName } = useParams();
    const [application, setApplication] = useState(null);
    const [newSettingName, setNewSettingName] = useState('');
    const [newSettingValue, setNewSettingValue] = useState('');
    const [environments, setEnvironments] = useState([]);
    const [newEnvironmentSettingName, setNewEnvironmentSettingName] = useState('');
    const [newEnvironmentSettings, setNewEnvironmentSettings] = useState({});
    const [transformedSettings, setTransformedSettings] = useState([]);

    useEffect(() => {
        fetchEnvironments();
    }, []);

    const fetchEnvironments = async () => {
        const requestOptions = {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
            },
        };

        try {
            const response = await fetch(`${window.appSettings.apiBaseUrl}/api/environments/`, requestOptions);

            if (!response.ok) {
                throw new Error('Failed to fetch environments');
            }

            const environments = await response.json();
            setEnvironments(environments);
            fetchApplication(environments);
        } catch (error) {
            console.error('Error fetching environments:', error);
        }
    };

    const fetchApplication = async (environments) => {
        const requestOptions = {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
            },
        };

        try {
            const response = await fetch(`${window.appSettings.apiBaseUrl}/api/applications/${applicationName}`, requestOptions);

            if (!response.ok) {
                throw new Error('Failed to fetch application');
            }

            const data = await response.json();
            setApplication(data);
            const transformedSettings = loadGrid(data.environmentSettings, environments);
            setTransformedSettings(transformedSettings);

            //const transformedSettings = transformSettings(data.environmentSettings);
            //setTransformedSettings(transformedSettings);

        } catch (error) {
            console.error('Error fetching application:', error);
        }
    };

    const loadGrid = (environmentSettings, environments) => {
        console.log("Se", environmentSettings);
        var result = new SettingGridData();

        environments.forEach((env) => {
            result.environments.push(env.name);
        });

        let keys = Object.keys(environmentSettings);
        keys.forEach((env) => {
            environmentSettings[env].forEach((setting) => {
                console.log("Setting", setting)
                if (!result.settings[setting.name]) {
                    result.settings[setting.name] = [];
                }

                if (!result.settings[setting.name][env]) {
                    result.settings[setting.name][env] = "";
                }
                result.settings[setting.name][env] = setting.value;
            });

            console.log("final", result);
        });
        return result;
    }

    const transformSettings = (environmentSettings) => {
        const transformedSettings = [];

        let environments = Object.keys(environmentSettings);
        environments.forEach((env) => {
            console.log("transforming for", env)
            const settings = environmentSettings[env] || [];
            settings.forEach((setting) => {
                if (!transformedSettings[setting.name]) {
                    transformedSettings[setting.name] = [];
                }

                if (!transformedSettings[setting.name][env]) {
                    transformedSettings[setting.name][env] = [];
                }

                console.log("Pushing", setting.name);
                transformedSettings[setting.name][env] = setting.value;
            });
        });

        console.log("trans", transformedSettings)
        return transformedSettings;
    };

    const handleAddEnvironmentSettings = async () => {
        let keys = Object.keys(newEnvironmentSettings);
        let allSettings = keys.map(env => {
            return {
                environment: env,
                name: newEnvironmentSettingName,
                value: newEnvironmentSettings[env] || '',
            }
        });

        const requestOptions = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
            },
            body: JSON.stringify({
                applicationId: applicationName,
                settings: allSettings
            }),
        };

        try {
            const response = await fetch(
                `${window.appSettings.apiBaseUrl}/api/ApplicationSettings`,
                requestOptions
            );

            if (!response.ok) {
                throw new Error('Failed to add setting');
            }
        } catch (error) {
            console.error('Error adding setting:', error);
        }

        fetchApplication();
        // });

    }

    const handleSettingChange = async() => {
        //TOOD: callback for updating setting
    }

    const handleAddSetting = async () => {
        const requestOptions = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
            },
            body: JSON.stringify({
                applicationId: applicationName,
                settings: [
                    {
                        environment: "Default",
                        name: newSettingName,
                        value: newSettingValue,
                    }
                ]
            }),
        };

        try {
            const response = await fetch(
                `${window.appSettings.apiBaseUrl}/api/ApplicationSettings`,
                requestOptions
            );

            if (!response.ok) {
                throw new Error('Failed to add setting');
            }

            setNewSettingName('');
            setNewSettingValue('');
            fetchApplication();
        } catch (error) {
            console.error('Error adding setting:', error);
        }
    };

    if (!application) {
        return <div>Loading...</div>;
    }

    if (!transformedSettings) {
        return <div>Loading...</div>;
    }


    return (
        <div>
            <h1>{application.name}</h1>
            <h2>Settings</h2>
            <div>These settings to the entire application no matter the environment. A setting here can be overriden by an explicit value being set in the environment settings<br /><br />
                TODO: have a way to see applied settings an application would have that merges in environment, app global settings, variable groups, and then environment specific settings</div>
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
                            <TableRow key={setting.id}>
                                <TableCell>{setting.name}</TableCell>
                                <TableCell>{setting.value}</TableCell>
                            </TableRow>
                        ))}
                        <TableRow>
                            <TableCell>
                                <TextField
                                    value={newSettingName} fullWidth
                                    onChange={(e) => setNewSettingName(e.target.value)}
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
                            <TableCell>
                                <Button onClick={handleAddSetting} color="primary">
                                    Add
                                </Button>
                            </TableCell>
                        </TableRow>
                    </TableBody>
                </Table>
            </TableContainer>

            <h2>Environment Specific Settings</h2>
            <div>TODO: ability to use your own environments just for this application and ignore the global environments</div>
            {transformedSettings.environments && (
                <SettingsGrid
                    transformedSettings={transformedSettings}
                    // onAddSetting={handleAddEnvironmentSetting}
                    onSettingChange={handleSettingChange}
                />
            )}

        </div>
    );
};

export default ApplicationDetail;
