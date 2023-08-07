import React, { useState, useEffect, useRef } from 'react';
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

const SettingsGrid = ({ transformedSettings, onAddSetting, onSettingChange, onSettingRename, onSettingDelete, onEnvironmentRename, onDeleteEnvironment, showEditButtons }) => {
    const [settings, setSettings] = useState(transformedSettings);
    const [errors, setErrors] = useState({}); // a map of error states
    const [editingEnvironment, setEditingEnvironment] = useState(null);
    const editingEnvironmentRef = useRef(editingEnvironment);
    const [newRecords, setNewRecords] = useState([]);

    const handleNewSetting = (newSettingName, rowIndex) => {
        if (newSettingName.trim() === "") return;
      
        if (newSettingName in settings.settings) {
          const updatedErrors = Object.keys(settings.settings).reduce((errorsMap, settingName) => ({
            ...errorsMap,
            [settingName]: settingName === newSettingName,
          }), {});
          setErrors(updatedErrors);
        } else {
          if (onAddSetting !== undefined) {
            onAddSetting(newSettingName);
          }
      
          const updatedSettings = { ...settings };
          updatedSettings.settings[newSettingName] = newRecords[rowIndex].values;
          setSettings(updatedSettings);
      
          const updatedNewRecords = [...newRecords];
          updatedNewRecords.splice(rowIndex, 1);
          setNewRecords(updatedNewRecords);
      
          setErrors({});
        }
      }
      
    const handleEnvironmentRename = (newValue) => {
        const originalValue = editingEnvironmentRef.current;
        if (settings.environments.includes(newValue) && newValue !== originalValue) {
          // handle error, environment name already exists
        } else {
          if (onEnvironmentRename !== undefined) {
            onEnvironmentRename(originalValue, newValue);
            // Update the settings state with the new environment name
            const updatedSettings = { ...settings };
            const environmentIndex = updatedSettings.environments.indexOf(originalValue);
            updatedSettings.environments[environmentIndex] = newValue;
            setSettings(updatedSettings);
          }
        }
        setEditingEnvironment(null);
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
            if (onSettingRename !== undefined)
                onSettingRename(originalValue, newValue);
        }
    }

    const handleDeleteSetting = (settingName)  => {
        //TODO: confirmation here
            onSettingDelete(settingName);
    }

    const handleAddRow = () => {
        setNewRecords([...newRecords, { name: '', values: {} }]);
      }

    useEffect(() => {
        setErrors({});
    }, [settings]);

    useEffect(() => {
        editingEnvironmentRef.current = editingEnvironment;
    }, [editingEnvironment]);

    useEffect(() => {
        transformedSettings.settings = sortObjectByKeys(transformedSettings.settings);
        console.log("new settings applied", transformedSettings);
        setSettings(transformedSettings);
    }, [transformedSettings]);

    function sortObjectByKeys(obj) {
        const sortedKeys = Object.keys(obj).sort();
        const sortedObj = {};
        sortedKeys.forEach(key => {
          sortedObj[key] = obj[key];
        });
        return sortedObj;
      }

    return (
        <TableContainer component={Paper}>
            <Table>
                <TableHead>
                    <TableRow>
                        <TableCell></TableCell>
                        {settings.environments.map((env) => {
                            return (
                                <TableCell key={env}>
                                    {/* The rest of your code */}
                                    {editingEnvironment === env ? (
                                        <TextField
                                            key={env + Date.now()}
                                            defaultValue={env}
                                            onBlur={(e) => {
                                                const newValue = e.target.value;
                                                if (newValue !== editingEnvironmentRef.current) {
                                                    handleEnvironmentRename(newValue);
                                                }
                                                setEditingEnvironment(null);
                                            }}
                                        />
                                    ) : env}
                                    {showEditButtons ? (
                                        <>
                                            {editingEnvironment === env ? null : (
                                                <>
                                                    <Button onClick={() => {
                                                        setEditingEnvironment(env)
                                                        setTimeout(() => {
                                                        }, 200);
                                                    }} color="secondary">
                                                        <i className="fa-regular fa-pen-to-square"></i>&nbsp;
                                                    </Button>
                                                    <Button color="secondary" onClick={() => onDeleteEnvironment(env)}>
                                                        <i className="fa-solid fa-trash-can"></i>
                                                    </Button>
                                                </>
                                            )}
                                        </>
                                    ) : null}
                                </TableCell>
                            );
                        })}

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
                                <Button color="secondary" onClick={() => handleDeleteSetting(settingName) }>
                                                        <i className="fa-solid fa-trash-can"></i>
                                                    </Button>
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

                    {newRecords.map((newRecord, i) => (
                        <TableRow key={i}>
                            <TableCell>
                                <Tooltip>
                                {/* <Tooltip title={errors[newEnvironmentSettingName] ? "Variable name already exists" : ""}> */}
                                    <TextField
                                        key={`newRecord${i}`}
                                        label="Name"
                                        value={newRecord.name}
                                        onChange={(e) => {
                                            const updatedNewRecords = [...newRecords];
                                            updatedNewRecords[i].name = e.target.value;
                                            setNewRecords(updatedNewRecords);
                                        
                                            // if (i === newRecords.length - 1 && e.target.value.trim() !== "") {
                                            //     setNewRecords([...updatedNewRecords, { name: '', values: {} }]);
                                            // }
                                        }}
                                        onBlur={(e) => { handleNewSetting(e.target.value, i) }}
                                    />

                                </Tooltip>
                            </TableCell>
                            {settings.environments.map((env) => (
                                <TableCell key={"new" + env}>
                                    <TextField
                                        key={`newRecord${i}-${env}`}
                                        label={env}
                                        value={newRecord.values[env] || ''}
                                        onChange={(e) => {
                                            const updatedNewRecords = [...newRecords];
                                            if (!updatedNewRecords[i].values) {
                                                updatedNewRecords[i].values = {};
                                            }
                                            updatedNewRecords[i].values[env] = e.target.value;
                                            setNewRecords(updatedNewRecords);

                                            if (i === newRecords.length - 1) {
                                                setNewRecords([...updatedNewRecords, { name: '', values: {} }]);
                                            }
                                        }}
                                        onBlur={(e) => { onSettingChange(newRecord.name, env, e.target.value) }}
                                    />

                                </TableCell>
                            ))}
                        </TableRow>
                    ))}

            <TableRow>
            <TableCell colSpan={settings.environments.length + 1}>
              <Button color="primary" onClick={handleAddRow}>New Setting</Button>
            </TableCell>
          </TableRow>

                </TableBody>
            </Table>
        </TableContainer>
    );
};

export default SettingsGrid;
