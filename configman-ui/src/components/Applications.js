import React, { useState, useEffect } from 'react';
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid';
import { useNavigate } from 'react-router-dom';
import AddApplicationDialog from './AddApplicationDialog';
import SettingsClient from '../settingsClient';

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

    const handleApplicationAdded = () => {
        fetchApplications();
    };

    const handleApplicationClick = (applicationName) => {
        navigate(`/applicationDetail/${applicationName}`);
    };

    return (
        <div>
            <div style={{ display: 'flex', justifyContent: 'flex-end', marginBottom: '1rem', marginTop: '1rem', marginRight: '1rem' }}>
                <Button variant="contained" color="primary" onClick={handleOpenAddApplicationDialog}>
                    Add Application
                </Button>
            </div>

            <h1>Applications</h1>
            <p>Applications are the core of ConfigMan. Create an application and security token then wire your application up.</p>


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