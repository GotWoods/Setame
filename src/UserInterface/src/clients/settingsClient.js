import ClientResponse from "./clientResponse";

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

    async handleResponse(response, versionedObject) {
        const etag = response.headers.get('etag');

        if (response.status >= 200 && response.status < 300) {
            if (versionedObject) {
                versionedObject.version = this.extractNumericValue(etag);
                console.log("version is now", versionedObject.version)
            }
            if (response.status === 200) {
                return await response.json();
            }
            return;
        }
        throw new Error(`Request Failed. Status code: ${response.status}`);
    }


    extractNumericValue(etag) {
        const regex = /(\d+)/;
        const matches = regex.exec(etag);
        if (matches && matches.length > 1) {
            return matches[1];
        }
        throw new Error(`Unable to extract numeric value from etag: ${etag}`);
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
            const payloadBase64Url = this.authToken.split('.')[1];
            const payloadJson = atob(payloadBase64Url.replace('-', '+').replace('_', '/')); // Decode the payload from Base64Url to a JSON string
            const payload = JSON.parse(payloadJson);
            if (payload.exp) {
                const currentTime = Math.floor(Date.now() / 1000);  // Get the current date in seconds
                return payload.exp <= currentTime;
            } else {
                return false; // If there is no "exp" field, we can't determine if the token has expired or not
            }
        } catch (error) {
            console.error('Error checking JWT token expiration:', error);
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

    async forgotPassword(username) {
        const response = await this.apiRequest(`${this.apiUrl}/api/Authentication/ForgotPassword`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(username),
        });
        return ClientResponse.fromResponse(response);
    }

    async passwordReset(token, newPassword) {
        const response = await this.apiRequest(`${this.apiUrl}/api/Authentication/${token}/ForgotPasswordReset`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(newPassword),
        });
        return ClientResponse.fromResponse(response);
    }
}

export default SettingsClient;