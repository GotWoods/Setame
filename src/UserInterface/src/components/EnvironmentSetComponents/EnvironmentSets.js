import React, { useEffect, useState, useContext } from 'react';
import Button from '@mui/material/Button';
import AddEnvironmentSetDialog from './AddEnvironmentSetDialog';
import '../../App.css';
import EnvironmentSetSettingsClient from '../../clients/environmentSetSettingsClient';
import EnvironmentSetDetail from './EnvironmentSetDetail';
import ErrorContext from '../../ErrorContext';


const EnvironmentSets = () => {
  const [environments, setEnvironmentSets] = useState([]);
  const [environmentSetDialogOpen, setEnvironmentSetDialogOpen] = useState(false);
  const { setErrorMessage } = useContext(ErrorContext);
  
  const fetchEnvironmentSets = React.useCallback(async () => {
    let settingsClient = new EnvironmentSetSettingsClient();
    let data = await settingsClient.getEnvironmentSets();
    setEnvironmentSets(data);
  }, []);

  useEffect(() => {
    fetchEnvironmentSets();
  }, [fetchEnvironmentSets]);

  // const fetchEnvironmentSets = async () => {
  //   const data = await settingsClient.getEnvironmentSets();
  //   setEnvironmentSets(data);
  // };

  const handleAddEnvironmentSetDialogClose = () => {
    setEnvironmentSetDialogOpen(false);
  };




  return (
    <div className="environment-settings">
      <div>

        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem', marginTop: '1rem', marginRight: '1rem' }}>
          <h1>
            Environment Sets
          </h1>
          <Button variant="contained" color="primary" onClick={() => setEnvironmentSetDialogOpen(true)}>
            Add New Environment Set
          </Button>
        </div>
        <p>
          Create an environment set for each distinct set of environments your application could move through. For example if you have one set of applications that moves through Dev,Test,Prod and another that moved through Dev,Test,UAT,Prod, you would create two different Environment Sets.
        </p><p>
          Variables applied to an Environment Set are inherited by any application tied to that Environment Set. This is a good spot to put global variables used by ALL applications (e.g. the URL of a central logging server)
          {/* TODO: Refresh on adding an env */}
        </p>
      </div>

      <AddEnvironmentSetDialog key={environmentSetDialogOpen ? 'open' : 'closed'} open={environmentSetDialogOpen} onClose={handleAddEnvironmentSetDialogClose} onAdded={fetchEnvironmentSets} />

      {environments.map((env) => (
        <EnvironmentSetDetail key={env.name + Date.now()} environmentSet={env} refreshRequested={fetchEnvironmentSets} />
      ))}
    </div>
  );
};

export default EnvironmentSets;
