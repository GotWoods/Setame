import React from 'react';
import { BrowserRouter as Router, Route, Routes, Navigate, BrowserRouter } from 'react-router-dom';
import LoginPage from './components/LoginPage';
import EnvironmentSets from './components/EnvironmentSetComponents/EnvironmentSets';
import NavigationBar from './components/NavigationBar';
import Applications from './components/ApplicationComponents/Applications';
import ApplicationDetail from './components/ApplicationComponents/ApplicationDetail';
import Users from './components/Users';
import './App.css';
import VariableGroups from './components/VariableGroups';
import Main from './components/Main';
import ProtectedOutlet from './components/ProtectedOutlet';
import EnvironmentGroups from './components/EnvironmentGroups';

function App() {
  return (
    <Router>
      <div className="App">
        <NavigationBar />
        <div className="main-content">
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/" element={<Main />}>
              <Route element={<Main />} />
            </Route>
            <Route path="/environments" element={<ProtectedOutlet />}>
              <Route index element={<EnvironmentSets />} />
            </Route>
            <Route path="/applications" element={<ProtectedOutlet />}>
              <Route index element={<Applications />} />
            </Route>
            <Route path="/applicationDetail/:applicationName" element={<ProtectedOutlet />}>
              <Route index element={<ApplicationDetail />} />
            </Route>
            <Route path="/environmentGroups" element={<ProtectedOutlet />}>
              <Route index element={<EnvironmentGroups />} />
            </Route>
            <Route path="/users" element={<ProtectedOutlet />}>
              <Route index element={<Users />} />
            </Route>
            <Route path="/variableGroups" element={<ProtectedOutlet />}>
              <Route index element={<VariableGroups />} />
            </Route>
            {/* Add other routes here */}
          </Routes>
        </div>
      </div>
    </Router>
  );
}

export default App;
