import React from 'react';
import { BrowserRouter as Router, Route, Routes, Navigate, BrowserRouter } from 'react-router-dom';
import LoginPage from './components/LoginPage';
import PrivateRoute from './components/PrivateRoute';
import EnvironmentSettings from './components/EnvironmentSettings';
import NavigationBar from './components/NavigationBar';
import Applications from './components/Applications';
import ApplicationDetail from './components/ApplicationDetail';
import Users from './components/Users';
import './App.css';
import VariableGroups from './components/VariableGroups';
import Main from './components/Main';

function App() {
  return (
    <Router>
      <div className="App">
        <NavigationBar />
        <div className="main-content">
          <Routes>
            {/* <Route exact path="/" render={() => <Navigate to="/login" />} /> */}
            <Route path="/" element={<Main/> } />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/environments" element={<EnvironmentSettings />} />
            <Route path="/applications" element={<Applications />} />
            <Route path="/applicationDetail/:applicationName" element={<ApplicationDetail />} />
            <Route path="/users" element={<Users />} />
            <Route path="/variableGroups" element={<VariableGroups />} />
            {/* Add other routes here */}
          </Routes>
        </div>
      </div>
    </Router>
  );
}

export default App;
