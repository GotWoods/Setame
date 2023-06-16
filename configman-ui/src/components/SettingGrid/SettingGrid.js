import React, { useState } from 'react';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';

const SettingsGrid = ({ transformedSettings, onAddSetting, onSettingChange, onHeaderClick }) => {
    const [newEnvironmentSettingName, setNewEnvironmentSettingName] = useState('');
    const [newEnvironmentSettings, setNewEnvironmentSettings] = useState({});
//    const [duplicateSettingError, setDuplicateSettingError] = useState('');

    // const handleNewSettingChange = (e) => {
    //     let updatedSettings = { ...newEnvironmentSettings }; // Create a copy of the state
    //     //set(updatedSettings, env, e.target.value); // Set the value
    //     setNewEnvironmentSettings(updatedSettings); // Update the state
    // }

    return (
        <TableContainer component={Paper}>
            <Table>
                <TableHead>
                    <TableRow>
                        <TableCell></TableCell>
                        {transformedSettings.environments.map((env) => (
                            <TableCell key={env}>
                                {onHeaderClick ? (
                                    <a href="#" onClick={() => onHeaderClick(env)}>
                                        {env}
                                    </a>
                                ) : (
                                    env
                                )}
                            </TableCell>
                        ))}
                    </TableRow>
                </TableHead>
                <TableBody>
                    {Object.keys(transformedSettings.settings).map((settingName) => (
                        <TableRow key={settingName}>
                            <TableCell>{settingName}</TableCell>
                            {transformedSettings.environments.map((env) => (
                                <TableCell key={settingName + env}>
                                    <TextField
                                        label={env}
                                        defaultValue={transformedSettings.settings[settingName][env]}
                                        onBlur={(e) => {
                                            let updatedSettings = { ...transformedSettings }; // make a copy of the state
                                            updatedSettings.settings[settingName][env] = e.target.value; // update the specific field
                                            onSettingChange(settingName, env, e.target.value, updatedSettings); // pass the entire updated object to parent component
                                        }}
                                    />
                                </TableCell>
                            ))}
                        </TableRow>
                    ))}

                    <TableRow key="new">
                        <TableCell>
                            <TextField
                                label="Name"
                                defaultValue={newEnvironmentSettingName || ''}
                                onBlur={(e) => { onAddSetting(e.target.value) }}
                            // onChange={e => setNewEnvironmentSettingName(e.target.value)}
                            /></TableCell>
                        {transformedSettings.environments.map((env) => (
                            <TableCell key={"new" + env}>
                                <TextField
                                    label={env}
                                    value=""
                                // onBlur={(e) => {
                                //     onSettingChange("new", env, e.target.value);
                                // }}
                                />
                            </TableCell>
                        ))}
                    </TableRow>

                </TableBody>
            </Table>
        </TableContainer>
    );
};

export default SettingsGrid;
