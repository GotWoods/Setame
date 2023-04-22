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
    const [duplicateSettingError, setDuplicateSettingError] = useState('');

    const handleAddEnvironmentSetting = () => {
        if (transformedSettings.settings[newEnvironmentSettingName]) {
            setDuplicateSettingError('A setting with this name already exists.');
            return;
        } else {
            setDuplicateSettingError('');
        }

        onAddSetting(newEnvironmentSettingName, newEnvironmentSettings);
        setNewEnvironmentSettingName('');
        setNewEnvironmentSettings({});
    };

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
                                        value={transformedSettings.settings[settingName][env]}
                                        onBlur={(e) => {
                                            onSettingChange(settingName, env, e.target.value);
                                        }}
                                    />
                                </TableCell>
                            ))}
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </TableContainer>
    );
};

export default SettingsGrid;
