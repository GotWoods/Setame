import React, { useState, useEffect } from 'react';
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid';
import { useNavigate } from 'react-router-dom';
import AddApplicationDialog from './AddApplicationDialog';
import ApplicationSettingsClient from '../../applicationSettingsClient';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import ExpandLessIcon from '@mui/icons-material/ExpandLess';
import ApplicationDetail from './ApplicationDetail';

const Applications = () => {
  const [openAddApplicationDialog, setOpenAddApplicationDialog] = useState(false);
  const [applications, setApplications] = useState([]);
  const navigate = useNavigate();
  const settingsClient = new ApplicationSettingsClient();
  const [selectedApplicationId, setSelectedApplicationId] = useState(null);

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
    fetchApplications();
  };

  const handleApplicationClick = (applicationId) => {
    setSelectedApplicationId((prevId) => (prevId === applicationId ? null : applicationId));
  };

  return (
    <div>
      <div
        style={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          marginBottom: '1rem',
          marginTop: '1rem',
          marginRight: '1rem',
        }}
      >
        <h1>Applications</h1>
        <Button variant="contained" color="primary" onClick={handleOpenAddApplicationDialog}>
          Add Application
        </Button>
      </div>
      <AddApplicationDialog
        key={openAddApplicationDialog ? 'open' : 'closed'}
        open={openAddApplicationDialog}
        onClose={handleCloseAddApplicationDialog}
        onApplicationAdded={handleApplicationAdded}
      />

      <Grid container spacing={2}>
        {applications.map((app) => (
          <Grid item xs={12} key={app.id}>
            <div>
              <Button
                onClick={() => handleApplicationClick(app.id)}
                startIcon={selectedApplicationId === app.id ? <ExpandLessIcon /> : <ExpandMoreIcon />}
              >
                {app.name}
              </Button>
              {selectedApplicationId === app.id && (
              <>
                  <Button color="secondary">
                    <i className="fa-regular fa-pen-to-square"></i>
                  </Button>
                  <Button onClick={() => handleDeleteApplication(app.id)} color="secondary">
                    <i className="fa-solid fa-trash-can"></i>
                  </Button>
                  <Button onClick={() => navigate(`/applicationHistory/${app.id}`)} color="secondary">
                    <i className="fa-solid fa-clock-rotate-left"></i>
                  </Button>
                  </>
              )}
            </div>
            {selectedApplicationId === app.id && <ApplicationDetail applicationId={app.id} />}
            {/* Conditional rendering of ApplicationDetail */}
          </Grid>
        ))}
      </Grid>
    </div>
  );
};

export default Applications;
