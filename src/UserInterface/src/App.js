import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import LoginPage from './components/LoginPage';
import EnvironmentSets from './components/EnvironmentSetComponents/EnvironmentSets';
import NavigationBar from './components/NavigationBar';
import Applications from './components/ApplicationComponents/Applications';
import ApplicationDetail from './components/ApplicationComponents/ApplicationDetail';
import ApplicationHistory from './components/ApplicationComponents/ApplicationHistory';
import Users from './components/Users';
import './App.css';
import VariableGroups from './components/VariableGroups';
import Main from './components/Main';
import ProtectedOutlet from './components/ProtectedOutlet';
import Settings from './components/Settings';
import EnvironmentSetHistory from './components/EnvironmentSetComponents/EnvironmentSetHistory';
import SetupPage from './components/SetupPage';
import ForgotPasswordPage from './components/ForgotPasswordPage'
import ResetPasswordPage from './components/ResetPasswordPage';
import ErrorContext from './ErrorContext';

const App = () => {
  const [errorMessage, setErrorMessage] = useState(null);

  const closeErrorDialog = () => {
    setErrorMessage(null);
  };

  useEffect(() => {
    console.log('Error message updated:', errorMessage);
  }, [errorMessage]);

  return (
    <ErrorContext.Provider value={{ errorMessage, setErrorMessage }}>
      <Router>
        <div className="App">
          <NavigationBar />
          {errorMessage && (
            <div className="error-dialog">
              <div className="error-message">The following errors occurred:
                {errorMessage.map((error, index) => (
                  <div key={index}>{error}</div>
                ))}
              </div>
              <button onClick={closeErrorDialog}>Acknowledge</button>
            </div>
          )}

          <div className="main-content">
            <Routes>
              <Route path="/login" element={<LoginPage />} />
              <Route path="/forgotPassword" element={<ForgotPasswordPage />} />
              <Route path="/resetPassword" element={<ResetPasswordPage />} />
              <Route path="/setup" element={<SetupPage />} />
              <Route path="/" element={<ProtectedOutlet />}>
                <Route index element={<Main />} />
              </Route>
              <Route path="/environmentSets" element={<ProtectedOutlet />}>
                <Route index element={<EnvironmentSets />} />
              </Route>
              <Route path="environmentSetHistory/:environmentSetId" element={<ProtectedOutlet />}>
                <Route index element={<EnvironmentSetHistory />} />
              </Route>
              <Route path="/applications" element={<ProtectedOutlet />}>
                <Route index element={<Applications />} />
              </Route>
              <Route path="/applicationDetail/:applicationId" element={<ProtectedOutlet />}>
                <Route index element={<ApplicationDetail />} />
              </Route>
              <Route path="/applicationHistory/:applicationId" element={<ProtectedOutlet />}>
                <Route index element={<ApplicationHistory />} />
              </Route>
              {/* <Route path="/environmentGroups" element={<ProtectedOutlet />}>
              <Route index element={<EnvironmentGroups />} />
            </Route> */}
              <Route path="/settings" element={<ProtectedOutlet />}>
                <Route index element={<Settings />} />
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
    </ErrorContext.Provider>
  );
}

export default App;
