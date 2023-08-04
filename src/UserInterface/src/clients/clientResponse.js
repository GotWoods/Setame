class ClientResponse {
    constructor(wasSuccessful, errors, data) {
        this.wasSuccessful = wasSuccessful;
        this.errors = errors;
        this.data = data;
    }

    static async fromResponse(response, versionedObject) {
        if (response.status >= 200 && response.status < 300) {
            
            if (versionedObject) {
                versionedObject.version = ClientResponse.extractNumericValue(response.headers.get('etag'));
                console.log("version is now", versionedObject.version)
            }

            if (response.status === 200) {
                return new ClientResponse(true, null, response.json());
            }
            
            return new ClientResponse(true, null, null);
        }

        if (response.status === 400) {
            try {
                const body = await response.json();
                return new ClientResponse(false, body.errors, null);
            } catch (e) {
                return new ClientResponse(false, ["An unexpected error occurred: " + e], null);
            }
        }

        return new ClientResponse(false, ["An unexpected status code was returned from the server: " + response.status], null);
    }

    static extractNumericValue(etag) {
        const regex = /(\d+)/;
        const matches = regex.exec(etag);
        if (matches && matches.length > 1) {
            return matches[1];
        }
        throw new Error(`Unable to extract numeric value from etag: ${etag}`);
    }
}

export default ClientResponse;