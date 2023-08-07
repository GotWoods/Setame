import SettingsClient from "./settingsClient";
import ClientResponse from "./clientResponse";

class EnvironmentSetSettingsClient extends SettingsClient {
    async getEnvironmentSet(name) {
        const response = await this.apiRequest(`${this.apiUrl}/api/environmentSets/${name}`, {
            headers: this.getHeaders(),
        });
        return this.handleResponse(response);
    }

    async getEnvironmentSets() {
        const response = await this.apiRequest(`${this.apiUrl}/api/environmentSets`, {
            headers: this.getHeaders(),
        });

        return this.handleResponse(response);
    }

    async getEnvironmentSetToApplicationAssociation(environmentSetId) {
        const response = await this.apiRequest(`${this.apiUrl}/api/EnvironmentSetApplicationAssociation/${environmentSetId}`, {
            headers: this.getHeaders(),
        });

        return this.handleResponse(response);
    }

    async getHistory(environmentSetId) {
        const response = await this.apiRequest(`${this.apiUrl}/api/environmentSetHistory/${environmentSetId}`, {
            headers: this.getHeaders(),
        });
        return this.handleResponse(response);
    }
    
    async renameEnvironmentSet(environmentSet, newName) {
        const response = await this.apiRequest(`${this.apiUrl}/api/environmentSets/${environmentSet.id}/rename`, {
            method: 'PUT',
            headers: this.getHeaders(environmentSet.version),
            body: JSON.stringify(newName),
        });
        return ClientResponse.fromResponse(response, environmentSet);
    }

    async renameEnvironment(environmentSet, oldValue,newValue) {
        const response = await this.apiRequest(`${this.apiUrl}/api/environmentSets/${environmentSet.id}/environment/${oldValue}/rename`, {
            method: 'PUT',
            headers: this.getHeaders(environmentSet.version),
            body: JSON.stringify(newValue),
        });
        return ClientResponse.fromResponse(response, environmentSet);
    }

    async deleteEnvironmentSet(environmentSet) {
        const response = await this.apiRequest(`${this.apiUrl}/api/environmentSets/${environmentSet.id}`, {
            method: 'DELETE',
            headers: this.getHeaders(),
        });

        return ClientResponse.fromResponse(response);
    }

    async deleteEnvironment(envrironmentSet, environment) {
        const response = await this.apiRequest(`${this.apiUrl}/api/environmentSets/${envrironmentSet.id}/environment/${environment}`, {
            method: 'DELETE',
            headers: this.getHeaders(envrironmentSet.version),
        });

        return ClientResponse.fromResponse(response, envrironmentSet);
    }

    async addEnvironmentSet(name) {
        const response = await this.apiRequest(`${this.apiUrl}/api/environmentSets`, {
            method: 'POST',
            headers: this.getHeaders(),
            body: JSON.stringify(name),
        });
        return ClientResponse.fromResponse(response);
    }

    async addEnvironmentToEnvironmentSet(environmentSet, environmentName) {
        const response = await this.apiRequest(`${this.apiUrl}/api/environmentSets/${environmentSet.id}/environment`, {
            method: 'POST',
            headers: this.getHeaders(environmentSet.version),
            body: JSON.stringify(environmentName),
        });
        return ClientResponse.fromResponse(response, environmentSet);
    }

    async addVariableToEnvironmentSet(environmentSet, variableName) {
        const response = await this.apiRequest(`${this.apiUrl}/api/environmentSets/${environmentSet.id}/variable`, {
            method: 'POST',
            headers: this.getHeaders(environmentSet.version),
            body: JSON.stringify(variableName),
        });
        return ClientResponse.fromResponse(response, environmentSet);
    }

    async updateVariableOnEnvironmentSet(environmentSet, environment, variableName, newValue, ) {
        const response = await this.apiRequest(`${this.apiUrl}/api/environmentSets/${environmentSet.id}/variable/${environment}/${variableName}`, {
            method: 'PUT',
            headers: this.getHeaders(environmentSet.version),
            body: JSON.stringify(newValue),
        });
        return ClientResponse.fromResponse(response, environmentSet);
    }

    async deleteSetting(environmentSet, variableName) {
        const response = await this.apiRequest(`${this.apiUrl}/api/environmentSets/${environmentSet.id}/variable/${variableName}`, {
            method: 'DELETE',
            headers: this.getHeaders(environmentSet.version),
            //body: JSON.stringify(newValue),
        });
        return ClientResponse.fromResponse(response, environmentSet);
    }

    async renameVariableOnEnvironmentSet(environmentSet, originalName, newName, ) {
        const response = await this.apiRequest(`${this.apiUrl}/api/environmentSets/${environmentSet.id}/variable/${originalName}/rename`, {
            method: 'PUT',
            headers: this.getHeaders(environmentSet.version),
            body: JSON.stringify(newName),
        });
        return ClientResponse.fromResponse(response, environmentSet);
    }



//     async updateEnvironmentSet(environmentSet) {
//         const response = await this.apiRequest(`${this.apiUrl}/api/environmentSets/${environmentSet.name}`, {
//             method: 'PUT',
//             headers: this.getHeaders(),
//             body: JSON.stringify(environmentSet),
//         });

//         return this.handleResponse(response);
//     }
}

export default EnvironmentSetSettingsClient;