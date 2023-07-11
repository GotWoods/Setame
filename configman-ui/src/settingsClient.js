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
        for(let entry of response.headers.entries()) {
            console.log(entry);
          }
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

    async handleResponse(response, versionedObject) {
        const etag = response.headers.get('etag');
        console.log("Etag is ", etag, response.headers);
        const headers = response.headers;
        console.log('headers', headers);
        if (response.status === 200) {
            if (versionedObject) {
                versionedObject.version = this.extractNumericValue(etag);
            }
            return await response.json();
        }
        
        if (response.status === 204) { //no content
            if (versionedObject) {
                versionedObject.version = this.extractNumericValue(etag);
            }
            return;
        }

        console.log("version is now", versionedObject.version)

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
}

export default SettingsClient;