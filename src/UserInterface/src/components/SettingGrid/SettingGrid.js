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

const SettingsGrid = ({ transformedSettings, onAddSetting, onSettingChange, onSettingRename, onEnvironmentRename, onDeleteEnvironment, showEditButtons }) => {
    const [settings, setSettings] = useState(transformedSettings);
    //const [newEnvironmentSettingName, setNewEnvironmentSettingName] = useState('');
    const [errors, setErrors] = useState({}); // a map of error states
    //const [newSettingError, setNewSettingError] = useState(false);
    const [editingEnvironment, setEditingEnvironment] = useState(null);
    const editingEnvironmentRef = useRef(editingEnvironment);
    const inputRefs = useRef({});
    const [newRecords, setNewRecords] = useState([{ name: '', values: {} }]);
    const [focusNew, setFocusNew] = useState(false);

    const handleNewSetting = (newSettingName, rowIndex) => {
        if (newSettingName in settings.settings) {
            const updatedErrors = Object.keys(settings.settings).reduce((errorsMap, settingName) => ({
                ...errorsMap,
                [settingName]: settingName === newSettingName, // set error for all settings with the same name
            }), {});
            setErrors(updatedErrors);
            //setNewSettingError(true);
        } else {
            if (onAddSetting !== undefined) {
                onAddSetting(newSettingName);
            }

            // Add the new setting to the settings state
            const updatedSettings = { ...settings };
            updatedSettings.settings[newSettingName] = newRecords[rowIndex].values;
            setSettings(updatedSettings);

            const updatedNewRecords = [...newRecords];
            updatedNewRecords.splice(rowIndex, 1);
            setNewRecords(updatedNewRecords);

            setErrors({}); // Reset errors state when new valid setting is added
            //setNewSettingError(false);
        }

        setFocusNew(true);
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

    useEffect(() => {
        setErrors({});
    }, [settings]);

    useEffect(() => {
        editingEnvironmentRef.current = editingEnvironment;
    }, [editingEnvironment]);

    useEffect(() => {
        if (focusNew) {
            // focus on the new input only after it is rendered and ref is assigned to it
            const lastIndex = newRecords.length - 1;
            if (inputRefs.current[`newRecord${lastIndex}`]) {
                inputRefs.current[`newRecord${lastIndex}`].focus();
            }
            setFocusNew(false); // reset it back to false
        }
    }, [focusNew, newRecords.length]); // execute this block whenever focusNew or newRecords.length changes

    return (
        <TableContainer component={Paper}>
            <Table>
                <TableHead>
                    <TableRow>
                        <TableCell></TableCell>
                        {settings.environments.map((env) => {
                            if (!inputRefs.current[env]) {
                                inputRefs.current[env] = React.createRef();
                            }
                            return (
                                <TableCell key={env}>
                                    {/* The rest of your code */}
                                    {editingEnvironment === env ? (
                                        <TextField
                                            inputRef={inputRefs.current[env]}
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
                                                            inputRefs.current[env].current.focus();
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
                                        inputRef={el => inputRefs.current[`newRecord${i}`] = el}
                                        key={`newRecord${i}`}
                                        label="Name"
                                        value={newRecord.name}
                                        inputProps={{ tabIndex: "1" }}
                                        onChange={(e) => {
                                            const updatedNewRecords = [...newRecords];
                                            updatedNewRecords[i].name = e.target.value;
                                            setNewRecords(updatedNewRecords);

                                            if (i === newRecords.length - 1) {
                                                setNewRecords([...updatedNewRecords, { name: '', values: {} }]);
                                                setTimeout(() => {
                                                    if (inputRefs.current[`newRecord${i}`]) {
                                                        inputRefs.current[`newRecord${i}`].focus();
                                                    }
                                                }, 0);
                                            }
                                        }}
                                        onBlur={(e) => { handleNewSetting(e.target.value, i) }}
                                    />

                                </Tooltip>
                            </TableCell>
                            {settings.environments.map((env) => (
                                <TableCell key={"new" + env}>
                                    <TextField
                                        inputRef={el => inputRefs.current[`newRecord${i}-${env}`] = el}
                                        key={`newRecord${i}-${env}`}
                                        label={env}
                                        value={newRecord.values[env] || ''}
                                        inputProps={{ tabIndex: i * settings.environments.length + settings.environments.indexOf(env) + 2 }}
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

                </TableBody>
            </Table>
        </TableContainer>
    );
};

export default SettingsGrid;
