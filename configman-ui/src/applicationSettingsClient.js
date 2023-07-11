import SettingsClient from "./settingsClient";


class ApplicationSettingsClient extends SettingsClient {
    

    async getAllApplications() {
        const response = await this.apiRequest(`${this.apiUrl}/api/applications`, {
            method: 'GET',
            headers: this.getHeaders(),
        });

        return this.handleResponse(response);
    }

    async getApplicationHistory(applicationId) {
        const response = await this.apiRequest(`${this.apiUrl}/api/applicationHistory/${applicationId}`, {
            method: 'GET',
            headers: this.getHeaders(),
        });

        return this.handleResponse(response);
    }

    async deleteApplication(applicationId) {
        console.log("Deleting app", applicationId);
        const response = await this.apiRequest(`${this.apiUrl}/api/applications/${applicationId}`, {
            method: 'DELETE',
            headers: this.getHeaders(),
        });

        return this.handleResponse(response);
    }

    async getApplication(applicationId) {
        const response = await this.apiRequest(`${this.apiUrl}/api/applications/${applicationId}`, {
            method: 'GET',
            headers: this.getHeaders(),
        });

        return this.handleResponse(response);
    }

    async addApplicationSetting(applicationId, environment, variable) {
        const response = await this.apiRequest(`${this.apiUrl}/api/ApplicationSettings/${applicationId}/${environment}`, {
            method: 'POST',
            headers: this.getHeaders(),
            body: JSON.stringify(variable)
            //     applicationId: applicationName,
            //     settings: allSettings
            // }),
        });

        return this.handleResponse(response);
    }

    async updateApplicationSetting(applicationName, environment, variable, value) {
        const response = await this.apiRequest(`${this.apiUrl}/api/ApplicationSettings/${applicationName}/${environment}/${variable}`, {
            method: 'PUT',
            headers: this.getHeaders(),
            body: JSON.stringify(value),
        });

        return this.handleResponse(response);
    }

    async renameApplicationSetting(applicationName, oldName, newName) {
        const response = await this.apiRequest(`${this.apiUrl}/api/ApplicationSettings/${applicationName}/${oldName}/rename`, {
            method: 'POST',
            headers: this.getHeaders(),
            body: JSON.stringify(newName),
        });

        return this.handleResponse(response);
    }

    async addGlobalApplicationSetting(applicationId, newSettingName) {
        const response = await this.apiRequest(`${this.apiUrl}/api/ApplicationSettings/${applicationId}/default`, {
            method: 'POST',
            headers: this.getHeaders(),
            body: JSON.stringify(newSettingName),
        });

        return this.handleResponse(response);
    }

    
    async updateGlobalApplicationSetting(applicationId, settingName, value) {
        const response = await this.apiRequest(`${this.apiUrl}/api/ApplicationSettings/${applicationId}/default/${settingName}`, {
            method: 'PUT',
            headers: this.getHeaders(),
            body: JSON.stringify(value),
        });

        return this.handleResponse(response);
    }

    async addApplication(applicationName, environmentSetId, token) {
        const response = await this.apiRequest(`${this.apiUrl}/api/applications`, {
            method: 'POST',
            headers: this.getHeaders(),
            body: JSON.stringify({ name: applicationName, environmentSetId, token }),
        });

        return this.handleResponse(response);
    }
}

export default ApplicationSettingsClient;