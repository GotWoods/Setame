import React, { useState, useEffect, useContext } from 'react';
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid';
import { TextField } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import AddApplicationDialog from './AddApplicationDialog';
import ApplicationSettingsClient from '../../clients/applicationSettingsClient';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import ExpandLessIcon from '@mui/icons-material/ExpandLess';
import ApplicationDetail from './ApplicationDetail';
import ErrorContext from '../../ErrorContext';

const Applications = () => {
  const [openAddApplicationDialog, setOpenAddApplicationDialog] = useState(false);
  const [applications, setApplications] = useState([]);
  const navigate = useNavigate();
  const settingsClient = new ApplicationSettingsClient();
  const [selectedApplicationId, setSelectedApplicationId] = useState(null);
  const [editingApplicationId, setEditingApplicationId] = useState(null);
  const { setErrorMessage } = useContext(ErrorContext);

  const fetchApplications = React.useCallback(async () => {
    let client = new ApplicationSettingsClient();
    let data = await client.getAllApplications();
    setApplications(data);
  }, []);


  const updateVersion = (applicationId, newVersion) => {
    setApplications((prevApps) =>
      prevApps.map((app) =>
        app.id === applicationId ? { ...app, version: newVersion } : app
      )
    );
  }

  useEffect(() => {
    fetchApplications();
  }, [fetchApplications]);

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

  const handleEditApplicationName = (application) => {
    setEditingApplicationId(application.id);
  };

  const handleUpdateApplicationName = async (application, newName) => {
    setEditingApplicationId(null);
    var result = await settingsClient.renameApplication(application, newName);
    console.log("Application rename result ", result);
    if (!result.wasSuccessful)
    {
        console.log("There were errors", result.errors)
        setErrorMessage(result.errors);
    }
    fetchApplications();
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
              {editingApplicationId === app.id ? (
                <TextField
                  value={app.name}
                  onChange={(e) => {
                    const newName = e.target.value;
                    setApplications((prevApps) =>
                      prevApps.map((prevApp) =>
                        prevApp.id === app.id ? { ...prevApp, name: newName } : prevApp
                      )
                    );
                  }}
                  onBlur={() => handleUpdateApplicationName(app, app.name)}
                  autoFocus
                />

              ) : (
                <Button
                  onClick={() => handleApplicationClick(app.id)}
                  startIcon={selectedApplicationId === app.id ? <ExpandLessIcon /> : <ExpandMoreIcon />}
                >
                  {app.name}
                </Button>
              )}
              {selectedApplicationId === app.id && (
                <>
                  <Button color="secondary" onClick={() => handleEditApplicationName(app)}>
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
            {selectedApplicationId === app.id && <ApplicationDetail applicationId={app.id} updateVersion={updateVersion} />}
            {/* Conditional rendering of ApplicationDetail */}
          </Grid>
        ))}
      </Grid>
    </div>
  );
};

export default Applications;
