const axios = require('axios');

const API_BASE_URL = 'http://localhost:5236';

// Test data
const loginData = {
    email: 'admin@careemclone.com',
    password: 'Admin@123456'
};

async function testDishUpdate() {
    try {
        console.log('🔐 Step 1: Logging in...');        // Login to get token
        const loginResponse = await axios.post(`${API_BASE_URL}/api/auth/login`, loginData, {
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            }
        });        if (!loginResponse.data.success) {
            throw new Error(`Login failed: ${loginResponse.data.errorMessage}`);
        }

        const token = loginResponse.data.data.token;
        console.log('✅ Login successful');

        console.log('\n📋 Step 2: Getting dishes list...');        // Get dishes to find one to update
        const dishesResponse = await axios.get(`${API_BASE_URL}/api/cms/dishes`, {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });        if (!dishesResponse.data.success || !dishesResponse.data.data.length) {
            throw new Error('No dishes found');
        }

        const testDish = dishesResponse.data.data[0];
        console.log(`✅ Found test dish: "${testDish.name}" (ID: ${testDish.id})`);
        console.log(`   Current availability: ${testDish.isAvailable}`);
        console.log(`   Restaurant ID: ${testDish.restaurantId}`);

        console.log('\n🔄 Step 3: Updating dish availability...');
        
        // Create proper update payload matching the frontend format
        const updateDishDto = {
            id: testDish.id,
            name: testDish.name,
            description: testDish.description,
            price: testDish.price,
            imageUrl: testDish.imageUrl,
            restaurantId: testDish.restaurantId,  // Keep the same restaurant ID
            isAvailable: !testDish.isAvailable,   // Toggle availability
            categoryId: testDish.categoryId
        };

        console.log('📤 Update payload:');
        console.log(JSON.stringify(updateDishDto, null, 2));        // Update the dish
        const updateResponse = await axios.put(
            `${API_BASE_URL}/api/DishManagement/dishes/${testDish.id}`,
            updateDishDto,
            {
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                }
            }
        );        console.log('\n📥 Update response:');
        console.log('Status:', updateResponse.status);
        console.log('Success:', updateResponse.data.success);
        console.log('Message:', updateResponse.data.errorMessage);

        if (updateResponse.data.success) {
            console.log('✅ Dish updated successfully!');
            console.log(`   New availability: ${updateResponse.data.data.isAvailable}`);
            
            // Verify the update by fetching the dish again            console.log('\n🔍 Step 4: Verifying update...');
            const verifyResponse = await axios.get(`${API_BASE_URL}/api/cms/dishes`, {
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            const updatedDish = verifyResponse.data.data.find(d => d.id === testDish.id);
            if (updatedDish) {
                console.log(`✅ Verification successful!`);
                console.log(`   Updated availability: ${updatedDish.isAvailable}`);
                console.log(`   Expected availability: ${!testDish.isAvailable}`);
                
                if (updatedDish.isAvailable === !testDish.isAvailable) {
                    console.log('🎉 TEST PASSED: Availability was successfully updated!');
                } else {
                    console.log('❌ TEST FAILED: Availability was not updated correctly');
                }
            } else {
                console.log('❌ Could not find updated dish for verification');
            }        } else {
            console.error('❌ Update failed:', updateResponse.data.errorMessage);
        }

    } catch (error) {
        console.error('❌ Error during test:', error.message);
        if (error.response) {
            console.error('   Status:', error.response.status);
            console.error('   Status Text:', error.response.statusText);
            console.error('   Response Data:', JSON.stringify(error.response.data, null, 2));
        }
    }
}

console.log('🧪 Testing Dish Update Fix...\n');
testDishUpdate();
