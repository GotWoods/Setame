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
            return await response.json();
        }

        // } else {
        //     throw new Error(`Request Failed. Status code: ${response.status}`);
        // }
    }

    getHeaders(version) {
        if (this.isJwtTokenExpired()) {
           window.location.href = '/login';
        }

        const headers = {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${this.authToken}`,
        };
    
        if (version) {
            headers['If-Match'] = `"${version}"`;
        }
        return headers;
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


 

    async getEnvironmentGroups() {
        const response = await this.apiRequest(`${this.apiUrl}/api/environmentGroups`, {
            headers: this.getHeaders(),
        });

        return this.handleResponse(response);
    }

    async addEnvironmentGroup(environmentGroupName) {
        const response = await this.apiRequest(`${this.apiUrl}/api/environmentGroups`, {
            method: 'POST',
            headers: this.getHeaders(),
            body: JSON.stringify({ name: environmentGroupName }),
        });

        return this.handleResponse(response);
    }

    async addEnvironmentSettings(allSettings) {
        const response = await this.apiRequest(`${this.apiUrl}/api/environmentsettings`, {
            method: 'POST',
            headers: this.getHeaders(),
            body: JSON.stringify(allSettings),
        });

        return this.handleResponse(response);
    }

    


   

}


export default SettingsClient;