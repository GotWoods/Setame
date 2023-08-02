import SettingsClient from "./settingsClient";

class SetupClient extends SettingsClient {
     async Setup(adminEmailAddress, password) {
        console.log('calling setup');
        const response = await this.apiRequest(`${this.apiUrl}/api/Setup`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({AdminEmailAddress: adminEmailAddress, NewPassword: password})
        });
        console.log('response is ', response);
        return this.handleResponse(response);
    }
}

export default SetupClient;