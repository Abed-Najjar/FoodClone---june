const axios = require('axios');

async function testDishUpdate() {
    try {
        console.log('Testing dish update fix...\n');

        // First, login to get authentication token
        const loginResponse = await axios.post('http://localhost:5236/api/auth/login', {
            username: 'admin',
            password: 'admin123'
        });

        const token = loginResponse.data.data.token;
        console.log('✅ Successfully authenticated as admin');

        // Set up headers with authentication
        const headers = {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        };

        // Get a dish to update (let's get first dish from restaurant 1)
        const dishesResponse = await axios.get('http://localhost:5236/api/DishManagement/restaurant/dishes/1', {
            headers
        });

        if (!dishesResponse.data.success || !dishesResponse.data.data.length) {
            console.log('❌ No dishes found to test with');
            return;
        }

        const dish = dishesResponse.data.data[0];
        console.log(`✅ Found dish to test: "${dish.name}" (ID: ${dish.id})`);
        console.log(`   Current availability: ${dish.isAvailable}`);

        // Test the update - toggle availability
        const updatePayload = {
            id: dish.id,
            name: dish.name,
            description: dish.description || '',
            price: dish.price,
            imageUrl: dish.imageUrl,
            restaurantId: dish.restaurantId,
            isAvailable: !dish.isAvailable,  // Toggle availability
            categoryId: dish.categoryId
        };

        console.log('\n📤 Sending update request...');
        console.log('Payload:', JSON.stringify(updatePayload, null, 2));

        const updateResponse = await axios.put(
            `http://localhost:5236/api/DishManagement/dishes/${dish.id}`,
            updatePayload,
            { headers }
        );

        if (updateResponse.data.success) {
            console.log('✅ Dish update successful!');
            console.log(`   New availability: ${updateResponse.data.data.isAvailable}`);
            console.log(`   Status: ${updateResponse.status}`);
            console.log(`   Message: ${updateResponse.data.errorMessage || 'Success'}`);
        } else {
            console.log('❌ Dish update failed');
            console.log('Response:', updateResponse.data);
        }

    } catch (error) {
        console.log('❌ Error during test:');
        console.log('Status:', error.response?.status);
        console.log('Message:', error.response?.data?.errorMessage || error.message);
        console.log('Full error:', error.response?.data);
    }
}

// Install axios if not already installed
async function ensureAxios() {
    try {
        require('axios');
    } catch (e) {
        console.log('Installing axios...');
        const { execSync } = require('child_process');
        execSync('npm install axios', { stdio: 'inherit' });
    }
}

ensureAxios().then(() => testDishUpdate());
