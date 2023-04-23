// api.js
class SettingsClient {
    get apiUrl() {
        return window.appSettings.apiBaseUrl;
      }

    async apiRequest(url, options) {
        const response = await fetch(url, options);
        if (response.status === 401) {
            localStorage.removeItem('token');
            window.location.href = '/login';
        }
        return response;
    }


    async login(username, password) {
        const response = await this.apiRequest(`${this.apiUrl}/api/Authentication/login`, {
          method: 'POST',
        headers: {
                'Content-Type': 'application/json',
            },
          body: JSON.stringify({ username, password }),
        });
    
        return this.handleResponse(response);
      }
    
      async getEnvironments() {
        const response = await this.apiRequest(`${this.apiUrl}/api/environments`, {
          headers: this.getAuthHeaders(),
        });
    
        return this.handleResponse(response);
      }

    async login(username, password) {
        const response = await fetch(`${window.appSettings.apiBaseUrl}/api/Authentication/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ username, password }),
        });

        if (response.ok) {
            return await response.json();
        } else {
            throw new Error(`Failed to fetch data. Status code: ${response.status}`);
        }
    }

    async getEnvironments() {
        const response = await fetch(`${window.appSettings.apiBaseUrl}/api/environments`, {
            headers: {
                Authorization: `Bearer ${localStorage.getItem('authToken')}`,
            },
        });

        if (response.ok) {
            return await response.json();
        }
        else {
            throw new Error(`Failed to fetch data. Status code: ${response.status}`);
        }
    }

    async deleteEnvironment(environment) {
        const response = await fetch(`${window.appSettings.apiBaseUrl}/api/environments/${environment}`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
            },
        });

        if (response.ok) {
            return;
        } else {
            throw new Error(`Failed to delete data. Status code: ${response.status}`);
        }
    }

    async updateEnvironmentSettings(allSettings) {
        const response = await fetch(`${window.appSettings.apiBaseUrl}/api/environmentsettings`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
            },
            body: JSON.stringify(allSettings),
        });

        if (response.ok) {
            return;
        } else {
            throw new Error(`Failed to update data. Status code: ${response.status}`);
        }
    }

    async updateEnvironmentSettings2(settingName, environment, newValue) {
        const response = await fetch(`${window.appSettings.apiBaseUrl}/api/environmentsettings`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
            },
            body: JSON.stringify({ settingName, environment, value: newValue }),
        });

        if (response.ok) {
            return;
        } else {
            throw new Error(`Failed to update data. Status code: ${response.status}`);
        }
    }

    async getAllApplications() {
        const requestOptions = {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`
            },
        };
        const response = await fetch(`${window.appSettings.apiBaseUrl}/api/applications`, requestOptions);
        if (response.ok)
            return await response.json();
        else
            throw new Error(`Failed to get applications. Status code: ${response.status}`);
    }

    async deleteApplication(applicationName) {
        const requestOptions = {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`
            },
        };

        const response = await fetch(`${window.appSettings.apiBaseUrl}/api/applications/${applicationName}`, requestOptions);

        if (!response.ok) {
            throw new Error('Failed to delete application');
        }


    }

    async getApplication(applicationName) {
        const requestOptions = {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
            },
        };


        const response = await fetch(`${window.appSettings.apiBaseUrl}/api/applications/${applicationName}`, requestOptions);

        if (!response.ok) {
            throw new Error('Failed to fetch application');
        }

        return await response.json();

    }

    async addApplicationSetting(applicationName, allSettings) {
        const requestOptions = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
            },
            body: JSON.stringify({
                applicationId: applicationName,
                settings: allSettings
            }),
        };

        const response = await fetch(
            `${window.appSettings.apiBaseUrl}/api/ApplicationSettings`,
            requestOptions
        );

        if (!response.ok) {
            throw new Error('Failed to add setting');
        }
    }

    async addGlobalApplicationSetting(applicationName, newSettingName, newSettingValue) {
        const requestOptions = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
            },
            body: JSON.stringify({
                applicationId: applicationName,
                settings: [
                    {
                        environment: "Default",
                        name: newSettingName,
                        value: newSettingValue,
                    }
                ]
            }),
        };


        const response = await fetch(
            `${window.appSettings.apiBaseUrl}/api/ApplicationSettings`,
            requestOptions
        );

        if (!response.ok) {
            throw new Error('Failed to add setting');
        }
    }

    async addApplicaiton(applicationName, token) {
        const requestOptions = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
            },
            body: JSON.stringify({ name: applicationName, token }),
        };

        const response = await fetch(
            `${window.appSettings.apiBaseUrl}/api/applications`,
            requestOptions
        );

        if (!response.ok) {
            throw new Error('Failed to add application');
        }


    }

    async addEnvironment(environmentName) {
        const requestOptions = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`
            },
            body: JSON.stringify({ name: environmentName })
        };

        const response = await fetch(`${window.appSettings.apiBaseUrl}/api/environments`, requestOptions);

        if (!response.ok) {
            throw new Error('Failed to add environment');
        }

    };

}


export default SettingsClient;