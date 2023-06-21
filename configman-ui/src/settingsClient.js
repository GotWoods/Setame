// api.js
class SettingsClient {
    get apiUrl() {
        return window.appSettings.apiBaseUrl;
    }
    get authToken() {
        return localStorage.getItem('authToken');
      }

    async apiRequest(url, options) {
        const response = await fetch(url, options);
        return response;
    }

    async handleResponse(response) {
        if (response.status===200) {
            console.log("resp", response);
            return await response.json();
        }

        // } else {
        //     throw new Error(`Request Failed. Status code: ${response.status}`);
        // }
    }

    getAuthHeaders() {
        if (this.isJwtTokenExpired()) {
           window.location.href = '/login';
        }

        return {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${this.authToken}`,
        };
    }


    isJwtTokenExpired() {
        try {

            // Get the payload from the JWT token
            const payloadBase64Url = this.authToken.split('.')[1];

            // Decode the payload from Base64Url to a JSON string
            const payloadJson = atob(payloadBase64Url.replace('-', '+').replace('_', '/'));

            // Parse the JSON string to an object
            const payload = JSON.parse(payloadJson);
   
            // Check if the "exp" (expiration) field is present in the payload
            if (payload.exp) {
             
                // Get the current date in seconds
                const currentTime = Math.floor(Date.now() / 1000);

                // Check if the token has expired
                return payload.exp <= currentTime;
            } else {
                // If there is no "exp" field, we can't determine if the token has expired or not
                return false;
            }
        } catch (error) {
            console.error('Error checking JWT token expiration:', error);
            // In case of error, assume the token is invalid
            return true;
        }
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


    async getEnvironmentSet(name) {
        const response = await this.apiRequest(`${this.apiUrl}/api/environmentSets/${name}`, {
            headers: this.getAuthHeaders(),
        });
        return this.handleResponse(response);
    }

    async getEnvironmentSets() {
        const response = await this.apiRequest(`${this.apiUrl}/api/environmentSets`, {
            headers: this.getAuthHeaders(),
        });

        return this.handleResponse(response);
    }

    
    async renameEnvironmentSet(oldName, newName) {
        const response = await this.apiRequest(`${this.apiUrl}/api/environmentSets/${oldName}/rename`, {
            method: 'POST',
            headers: this.getAuthHeaders(),
            body: JSON.stringify(newName),
        });
        return this.handleResponse(response);
    }

    async deleteEnvironmentSet(environment) {
        const response = await this.apiRequest(`${this.apiUrl}/api/environmentSets/${environment}`, {
            method: 'DELETE',
            headers: this.getAuthHeaders(),
        });

        return this.handleResponse(response);
    }

    async addEnvironmentSet(environmentName) {
        const response = await this.apiRequest(`${this.apiUrl}/api/environmentSets`, {
            method: 'POST',
            headers: this.getAuthHeaders(),
            body: JSON.stringify({ name: environmentName }),
        });
        return this.handleResponse(response);
    }

    async updateEnvironmentSet(environmentSet) {
        console.log("In the right spot", environmentSet)
        const response = await this.apiRequest(`${this.apiUrl}/api/environmentSets/${environmentSet.name}`, {
            method: 'PUT',
            headers: this.getAuthHeaders(),
            body: JSON.stringify(environmentSet),
        });

        return this.handleResponse(response);
    }

    async getEnvironmentGroups() {
        const response = await this.apiRequest(`${this.apiUrl}/api/environmentGroups`, {
            headers: this.getAuthHeaders(),
        });

        return this.handleResponse(response);
    }

    async addEnvironmentGroup(environmentGroupName) {
        const response = await this.apiRequest(`${this.apiUrl}/api/environmentGroups`, {
            method: 'POST',
            headers: this.getAuthHeaders(),
            body: JSON.stringify({ name: environmentGroupName }),
        });

        return this.handleResponse(response);
    }

    async addEnvironmentSettings(allSettings) {
        const response = await this.apiRequest(`${this.apiUrl}/api/environmentsettings`, {
            method: 'POST',
            headers: this.getAuthHeaders(),
            body: JSON.stringify(allSettings),
        });

        return this.handleResponse(response);
    }

    


    async getAllApplications() {
        const response = await this.apiRequest(`${this.apiUrl}/api/applications`, {
            method: 'GET',
            headers: this.getAuthHeaders(),
        });

        return this.handleResponse(response);
    }

    async deleteApplication(applicationName) {
        const response = await this.apiRequest(`${this.apiUrl}/api/applications/${applicationName}`, {
            method: 'DELETE',
            headers: this.getAuthHeaders(),
        });

        return this.handleResponse(response);
    }

    async getApplication(applicationName) {
        const response = await this.apiRequest(`${this.apiUrl}/api/applications/${applicationName}`, {
            method: 'GET',
            headers: this.getAuthHeaders(),
        });

        return this.handleResponse(response);
    }

    async addApplicationSetting(applicationName, environment, variable) {
        const response = await this.apiRequest(`${this.apiUrl}/api/ApplicationSettings/${applicationName}/${environment}/${variable}`, {
            method: 'POST',
            headers: this.getAuthHeaders(),
            // body: JSON.stringify({
            //     applicationId: applicationName,
            //     settings: allSettings
            // }),
        });

        return this.handleResponse(response);
    }

    async updateApplicationSetting(applicationName, environment, variable, value) {
        const response = await this.apiRequest(`${this.apiUrl}/api/ApplicationSettings/${applicationName}/${environment}/${variable}`, {
            method: 'PUT',
            headers: this.getAuthHeaders(),
            body: JSON.stringify(value),
        });

        return this.handleResponse(response);
    }

    async renameApplicationSetting(applicationName, oldName, newName) {
        const response = await this.apiRequest(`${this.apiUrl}/api/ApplicationSettings/${applicationName}/${oldName}/rename`, {
            method: 'POST',
            headers: this.getAuthHeaders(),
            body: JSON.stringify(newName),
        });

        return this.handleResponse(response);
    }

    async addGlobalApplicationSetting(applicationName, newSettingName, newSettingValue) {
        const response = await this.apiRequest(`${this.apiUrl}/api/ApplicationSettings`, {
            method: 'POST',
            headers: this.getAuthHeaders(),
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
        });

        return this.handleResponse(response);
    }

    async addApplication(applicationName, environmentSet, token) {
        const response = await this.apiRequest(`${this.apiUrl}/api/applications`, {
            method: 'POST',
            headers: this.getAuthHeaders(),
            body: JSON.stringify({ name: applicationName, environmentSet, token }),
        });

        return this.handleResponse(response);
    }

   

}


export default SettingsClient;