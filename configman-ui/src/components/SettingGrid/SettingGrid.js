import React, { useState, useEffect } from 'react';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import TextField from '@mui/material/TextField';
import Tooltip from '@mui/material/Tooltip';
import Button from '@mui/material/Button';

const SettingsGrid = ({ transformedSettings, onAddSetting, onSettingChange, onSettingRename, onHeaderClick }) => {
    const [settings, setSettings] = useState(transformedSettings);
    const [newEnvironmentSettingName, setNewEnvironmentSettingName] = useState('');
    const [errors, setErrors] = useState({}); // a map of error states
    const [newSettingError, setNewSettingError] = useState(false); // new state for the new setting error


    const handleNewSetting = (newSettingName) => {
        if (newEnvironmentSettingName in settings.settings) {
            const updatedErrors = Object.keys(settings.settings).reduce((errorsMap, settingName) => ({
                ...errorsMap,
                [settingName]: settingName === newEnvironmentSettingName, // set error for all settings with the same name
            }), {});
            setErrors(updatedErrors);
            setNewSettingError(true);
        } else {
            if (onAddSetting != undefined)
                onAddSetting(newSettingName);
            setErrors({}); // Reset errors state when new valid setting is added
            setNewSettingError(false);
        }
    }

    const handleSettingRename = (originalValue, newValue) => {
        if (newValue in settings.settings && newValue !== originalValue) {
            const updatedErrors = Object.keys(settings.settings).reduce((errorsMap, settingName) => ({
                ...errorsMap,
                [settingName]: settingName === newValue || settingName === originalValue,
            }), {});
            setErrors(updatedErrors);
        } else {
            const { [originalValue]: _, ...rest } = errors; // remove the originalValue from the errors object
            setErrors(rest); // otherwise, set its error state to false
            if (onSettingRename != undefined)
                onSettingRename(originalValue, newValue);
        }
    }

    useEffect(() => {
        setErrors({});
    }, [settings]);

    return (
        <TableContainer component={Paper}>
            <Table>
                <TableHead>
                    <TableRow>
                        <TableCell></TableCell>
                        {settings.environments.map((env) => (
                            <TableCell key={env}>
                                {env}
                                <Button onClick={() => onHeaderClick()} color="secondary">
                                    <i className="fa-regular fa-pen-to-square"></i>&nbsp;
                                </Button>
                                <Button onClick={() => onHeaderClick(env)} color="secondary">
                                    <i className="fa-solid fa-trash-can"></i>
                                </Button>
                            </TableCell>
                        ))}
                    </TableRow>
                </TableHead>
                <TableBody>
                    {Object.keys(settings.settings).map((settingName) => (
                        <TableRow key={settingName}>
                            <TableCell>
                                <Tooltip title={errors[settingName] ? "Variable name already exists" : ""}>
                                    <TextField
                                        error={errors[settingName]}
                                        defaultValue={settingName}
                                        onBlur={(e) => {
                                            const newValue = e.target.value;
                                            const originalValue = settingName;
                                            if (newValue !== originalValue) {
                                                handleSettingRename(originalValue, newValue);
                                            }
                                        }}
                                    />
                                </Tooltip>
                            </TableCell>
                            {settings.environments.map((env) => (
                                <TableCell key={settingName + env}>

                                    <TextField
                                        label={env}
                                        defaultValue={settings.settings[settingName][env]}

                                        onBlur={(e) => {
                                            const newValue = e.target.value;
                                            const originalValue = settings.settings[settingName][env];
                                            if (newValue !== originalValue) {
                                                onSettingChange(settingName, env, e.target.value);
                                            }
                                        }}
                                    />
                                </TableCell>
                            ))}
                        </TableRow>
                    ))}

                    <TableRow key="new">
                        <TableCell>
                            <Tooltip title={errors[newEnvironmentSettingName] ? "Variable name already exists" : ""}>
                                <TextField
                                    error={errors[newEnvironmentSettingName]}
                                    label="Name"
                                    defaultValue={newEnvironmentSettingName || ''}
                                    onChange={(e) => setNewEnvironmentSettingName(e.target.value)}
                                    onBlur={(e) => { handleNewSetting(e.target.value) }}
                                />
                            </Tooltip>
                        </TableCell>
                        {settings.environments.map((env) => (
                            <TableCell key={"new" + env}>
                                <TextField
                                    label={env}
                                    defaultValue=""
                                    onBlur={(e) => { onSettingChange(newEnvironmentSettingName, env, e.target.value) }}
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
