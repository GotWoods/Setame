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
import EnvironmentSetSettingsClient from '../../environmentSetSettingsClient';

const EnvironmentSetHistory = () => {
    const { environmentSetId } = useParams();
  const [history, setHistory] = useState([]);
  const settingsClient = new EnvironmentSetSettingsClient();

  useEffect(() => {
      fetchEnvironmentSetHistory();
  }, []);

  const fetchEnvironmentSetHistory = async () => {
      const data = await settingsClient.getHistory(environmentSetId);
      setHistory(data);
  };


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

export default EnvironmentSetHistory;
