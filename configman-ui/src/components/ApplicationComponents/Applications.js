import React, { useState, useEffect } from 'react';
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid';
import { useNavigate } from 'react-router-dom';
import AddApplicationDialog from './AddApplicationDialog';
import SettingsClient from '../../settingsClient';

const Applications = () => {
    const [openAddApplicationDialog, setOpenAddApplicationDialog] = useState(false);
    const [applications, setApplications] = useState([]);
    const navigate = useNavigate();
    const settingsClient = new SettingsClient();

    useEffect(() => {
        fetchApplications();
    }, []);

    const fetchApplications = async () => {
        const data = await settingsClient.getAllApplications();
        setApplications(data);
    };

    const handleDeleteApplication = async (applicationId) => {
        await settingsClient.deleteApplication(applicationId);
        fetchApplications();
    };

    const handleOpenAddApplicationDialog = () => {
        setOpenAddApplicationDialog(true);
    };

    const handleCloseAddApplicationDialog = () => {
        setOpenAddApplicationDialog(false);
    };

    const handleApplicationAdded = async (applicationName, token) => {
        //  await settingsClient.addApplication(applicationName, token)
        fetchApplications();
    };

    const handleApplicationClick = (applicationName) => {
        navigate(`/applicationDetail/${applicationName}`);
    };

    return (
        <div>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem', marginTop: '1rem', marginRight: '1rem' }}>
                <h1>Applications</h1>
                <Button variant="contained" color="primary" onClick={handleOpenAddApplicationDialog}>
                    Add Application
                </Button>
            </div>



            <p>Allow your application to connect and get its own configuration by creating an Application and a Token for authentication. This will need to be provided by your client</p>
            <p>Applications will inherit values from their Environment Set. Setting a variable with the same name as the Environment Set will override it</p>

            <AddApplicationDialog key={openAddApplicationDialog ? 'open' : 'closed'}
                open={openAddApplicationDialog}
                onClose={handleCloseAddApplicationDialog}
                onApplicationAdded={handleApplicationAdded}
            />

            <Grid container spacing={2}>
                {applications.map((app) => (
                    <Grid item xs={12} key={app.name}>
                        <div style={{ display: 'inline-block', width: '150px' }}>
                            <Button onClick={() => handleApplicationClick(app.name)}>{app.name}</Button>
                        </div>
                        <div style={{ display: 'inline-block', width: '100px' }}>
                            <Button color="secondary" variant="contained" onClick={() => handleDeleteApplication(app.name)}>Delete</Button>
                        </div>
                    </Grid>
                ))}
            </Grid>
        </div>
    );
};

export default Applications;