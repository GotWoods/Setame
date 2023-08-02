import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
} from '@mui/material';
import ApplicationSettingsClient from '../../clients/applicationSettingsClient';
//import axios from 'axios';

const ApplicationHistory = () => {
  const { applicationId } = useParams();
  const [history, setHistory] = useState([]);
  
  const fetchApplicationHistory = React.useCallback(async () => {
    let client = new ApplicationSettingsClient();
    let data = await client.getApplicationHistory(applicationId);
    setHistory(data);
  }, [applicationId]);

  useEffect(() => {
    fetchApplicationHistory();
  }, [fetchApplicationHistory]);

  // const fetchApplicationHistory = async () => {
  //   const data = await settingsClient.getApplicationHistory(applicationId);
  //   setHistory(data);
  // };


  return (
    <TableContainer component={Paper}>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell>Date/Time</TableCell>
            <TableCell>Operation Type</TableCell>
            <TableCell>Description of Change</TableCell>
            <TableCell>User</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {history.map((record, index) => (
            <TableRow key={index}>
              <TableCell>{new Date(record.timestamp).toLocaleString()}</TableCell>
              <TableCell>{record.actionType}</TableCell>
              <TableCell>{record.description}</TableCell>
              <TableCell>{record.user}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
};

export default ApplicationHistory;
