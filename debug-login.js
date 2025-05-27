const axios = require('axios');

const API_BASE_URL = 'http://localhost:5236';

// Test data
const loginData = {
    email: 'admin@careemclone.com',
    password: 'Admin@123456'
};

async function debugLogin() {
    try {
        console.log('🔐 Testing login endpoint...');
        console.log('Login data:', JSON.stringify(loginData, null, 2));
        
        // Login to get token
        const loginResponse = await axios.post(`${API_BASE_URL}/api/auth/login`, loginData, {
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            }
        });

        console.log('\n📥 Login Response:');
        console.log('Status:', loginResponse.status);
        console.log('Response Data:', JSON.stringify(loginResponse.data, null, 2));

    } catch (error) {
        console.error('❌ Error during login test:', error.message);
        if (error.response) {
            console.error('   Status:', error.response.status);
            console.error('   Status Text:', error.response.statusText);
            console.error('   Response Data:', JSON.stringify(error.response.data, null, 2));
        }
    }
}

console.log('🧪 Debug Login Test...\n');
debugLogin();
