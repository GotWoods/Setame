import SettingsClient from "./settingsClient";
import ClientResponse from "./clientResponse";

class ApplicationSettingsClient extends SettingsClient {
    

    async getAllApplications() {
        const response = await this.apiRequest(`${this.apiUrl}/api/applications`, {
            method: 'GET',
            headers: this.getHeaders(),
        });

        return this.handleResponse(response);
    }

    async getApplication(applicationId) {
        const response = await this.apiRequest(`${this.apiUrl}/api/applications/${applicationId}`, {
            method: 'GET',
            headers: this.getHeaders(),
        });

        const etag = response.headers.get('etag');
        var application = this.handleResponse(response);
        application.version = this.extractNumericValue(etag);
        return application;
    }

    async getApplicationHistory(applicationId) {
        const response = await this.apiRequest(`${this.apiUrl}/api/applicationHistory/${applicationId}`, {
            method: 'GET',
            headers: this.getHeaders(),
        });

        return this.handleResponse(response);
    }

    async deleteApplication(applicationId) {
        const response = await this.apiRequest(`${this.apiUrl}/api/applications/${applicationId}`, {
            method: 'DELETE',
            headers: this.getHeaders(),
        });

        return this.handleResponse(response);
    }

    async renameApplication(application, newName) {
        const response = await this.apiRequest(`${this.apiUrl}/api/applications/${application.id}/rename`, {
            method: 'PUT',
            headers: this.getHeaders(application.version),
            body: JSON.stringify(newName)
        });

        return ClientResponse.fromResponse(response, application);
    }



    async addApplicationSetting(application, environment, variable) {
        const response = await this.apiRequest(`${this.apiUrl}/api/ApplicationSettings/${application.id}/${environment}`, {
            method: 'POST',
            headers: this.getHeaders(application.version),
            body: JSON.stringify(variable)
            //     applicationId: applicationName,
            //     settings: allSettings
            // }),
        });

        return this.handleResponse(response, application);
    }

    async updateApplicationSetting(application, environment, variable, value) {
        const response = await this.apiRequest(`${this.apiUrl}/api/ApplicationSettings/${application.id}/${environment}/${variable}`, {
            method: 'PUT',
            headers: this.getHeaders(application.version),
            body: JSON.stringify(value),
        });

        return ClientResponse.fromResponse(response, application);
    }

    async renameApplicationSetting(application, oldName, newName) {
        const response = await this.apiRequest(`${this.apiUrl}/api/ApplicationSettings/${application.id}/${oldName}/rename`, {
            method: 'POST',
            headers: this.getHeaders(application.version),
            body: JSON.stringify(newName),
        });

        return ClientResponse.fromResponse(response, application);
    }

    async renameGlobalApplicationSetting(application, oldName, newName) {
        const response = await this.apiRequest(`${this.apiUrl}/api/ApplicationSettings/${application.id}/${oldName}/renameDefault`, {
            method: 'POST',
            headers: this.getHeaders(application.version),
            body: JSON.stringify(newName),
        });

        return ClientResponse.fromResponse(response, application);
    }

    async addGlobalApplicationSetting(application, newSettingName) {
        const response = await this.apiRequest(`${this.apiUrl}/api/ApplicationSettings/${application.id}/default`, {
            method: 'POST',
            headers: this.getHeaders(application.version),
            body: JSON.stringify(newSettingName),
        });

        return this.handleResponse(response, application);
    }

    
    async updateGlobalApplicationSetting(application, settingName, value) {
        console.log("Updating global", application)
        const response = await this.apiRequest(`${this.apiUrl}/api/ApplicationSettings/${application.id}/default/${settingName}`, {
            method: 'PUT',
            headers: this.getHeaders(application.version),
            body: JSON.stringify(value),
        });

        return ClientResponse.fromResponse(response, application);
    }

    async addApplication(applicationName, environmentSetId, token) {
        const response = await this.apiRequest(`${this.apiUrl}/api/applications`, {
            method: 'POST',
            headers: this.getHeaders(),
            body: JSON.stringify({ name: applicationName, environmentSetId, token }),
        });

        return this.handleResponse(response);
    }


    async deleteSetting(application, variableName) {
        const response = await this.apiRequest(`${this.apiUrl}/api/ApplicationSettings/${application.id}/${variableName}`, {
            method: 'DELETE',
            headers: this.getHeaders(application.version),
            //body: JSON.stringify(newValue),
        });
        return ClientResponse.fromResponse(response, application);
    }

}

export default ApplicationSettingsClient;