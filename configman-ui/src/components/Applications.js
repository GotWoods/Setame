import React, { useState, useEffect } from 'react';
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid';
import { useNavigate } from 'react-router-dom';
import AddApplicationDialog from './AddApplicationDialog';

const Applications = () => {
    const [openAddApplicationDialog, setOpenAddApplicationDialog] = useState(false);
    const [applications, setApplications] = useState([]);
    const navigate = useNavigate();

    useEffect(() => {
        fetchApplications();
    }, []);

    const fetchApplications = async () => {
        const requestOptions = {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`
            },
        };

        try {
            const response = await fetch(`${window.appSettings.apiBaseUrl}/api/applications`, requestOptions);

            if (!response.ok) {
                throw new Error('Failed to fetch applications');
            }

            const data = await response.json();
            setApplications(data);
        } catch (error) {
            console.error('Error fetching applications:', error);
        }
    };

    const handleDeleteApplication = async (applicationId) => {
        const requestOptions = {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`
            },
        };

        try {
            const response = await fetch(`${window.appSettings.apiBaseUrl}/api/applications/${applicationId}`, requestOptions);

            if (!response.ok) {
                throw new Error('Failed to delete application');
            }

            fetchApplications(); // Refresh the applications list after deleting
        } catch (error) {
            console.error('Error deleting application:', error);
        }
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

    const handleApplicationClick = (applicationId) => {
        navigate(`/applicationDetail/${applicationId}`);
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
                            <Button color="secondary" variant="contained" onClick={() => handleDeleteApplication(app.id)}>Delete</Button>
                        </div>
                    </Grid>
                ))}
            </Grid>
        </div>
    );
};

export default Applications;