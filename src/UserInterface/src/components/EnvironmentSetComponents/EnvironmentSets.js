import React, { useEffect, useState } from 'react';
import Button from '@mui/material/Button';
import AddEnvironmentSetDialog from './AddEnvironmentSetDialog';
import '../../App.css';
import EnvironmentSetSettingsClient from '../../clients/environmentSetSettingsClient';
import EnvironmentSetDetail from './EnvironmentSetDetail';
//import ErrorContext from '../../ErrorContext';

const EnvironmentSets = () => {
  const [environments, setEnvironmentSets] = useState([]);
  const [environmentSetDialogOpen, setEnvironmentSetDialogOpen] = useState(false);
  const [loading, setLoading] = useState(false); // Loading state
  //const { setErrorMessage } = useContext(ErrorContext);

  const fetchEnvironmentSets = React.useCallback(async () => {
    setLoading(true); // Start loading
    let settingsClient = new EnvironmentSetSettingsClient();
    try {
      let data = await settingsClient.getEnvironmentSets();
      setEnvironmentSets(data);
    } catch (error) {
      // Handle the error if necessary
      // setErrorMessage(error.message);
    } finally {
      setLoading(false); // Stop loading
    }
  }, []);

  useEffect(() => {
    fetchEnvironmentSets();
  }, [fetchEnvironmentSets]);

  const handleAddEnvironmentSetDialogClose = () => {
    setEnvironmentSetDialogOpen(false);
  };

  return (
    <div className="environment-settings">
      <div>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem', marginTop: '1rem', marginRight: '1rem' }}>
          <h1>Environment Sets</h1>
          <Button variant="contained" color="primary" onClick={() => setEnvironmentSetDialogOpen(true)}>
            Add New Environment Set
          </Button>
        </div>
      </div>

      <AddEnvironmentSetDialog key={environmentSetDialogOpen ? 'open' : 'closed'} open={environmentSetDialogOpen} onClose={handleAddEnvironmentSetDialogClose} onAdded={fetchEnvironmentSets} />

      {loading && <p>Loading environment sets...</p>} {/* Loading message */}

      {!loading && environments.map((env) => (
        <EnvironmentSetDetail key={env.name + Date.now()} environmentSet={env} refreshRequested={fetchEnvironmentSets} />
      ))}
    </div>
  );
};

export default EnvironmentSets;
