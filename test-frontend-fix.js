const axios = require('axios');

async function testFrontendFix() {
  try {
    console.log('🧪 Testing Frontend Fix for Dish Status Update...\n');

    // Step 1: Login as admin
    console.log('1. Logging in as admin...');
    const loginResponse = await axios.post('http://localhost:5236/api/Auth/login', {
      email: 'admin@careemclone.com',
      password: 'Admin@123456'
    });

    if (!loginResponse.data.success) {
      throw new Error('Login failed: ' + loginResponse.data.errorMessage);
    }

    const token = loginResponse.data.data.token;
    console.log('✅ Login successful');

    // Step 2: Get a dish to test with
    console.log('\n2. Getting dishes to test with...');
    const dishesResponse = await axios.get('http://localhost:5236/api/cms/dishes', {
      headers: { 'Authorization': `Bearer ${token}` }
    });

    if (!dishesResponse.data.success || !dishesResponse.data.data.length) {
      throw new Error('No dishes found to test with');
    }

    const testDish = dishesResponse.data.data[0];
    console.log(`✅ Found test dish: "${testDish.name}" (ID: ${testDish.id})`);
    console.log(`   Current availability: ${testDish.isAvailable}`);

    // Step 3: Test updating with the correct format (simulating frontend fix)
    console.log('\n3. Testing dish update with corrected boolean value...');
    
    const updatePayload = {
      id: testDish.id,
      name: testDish.name,
      description: testDish.description || '',
      price: testDish.price,
      imageUrl: testDish.imageUrl,
      restaurantId: testDish.restaurantId,
      isAvailable: !testDish.isAvailable, // Toggle the availability (as proper boolean)
      categoryId: testDish.categoryId
    };

    console.log('Request payload:', JSON.stringify(updatePayload, null, 2));

    const updateResponse = await axios.put(
      `http://localhost:5236/api/DishManagement/dishes/${testDish.id}`,
      updatePayload,
      {
        headers: { 
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      }
    );

    if (!updateResponse.data.success) {
      throw new Error('Update failed: ' + updateResponse.data.errorMessage);
    }

    console.log('✅ Update successful!');
    console.log(`   New availability: ${updateResponse.data.data.isAvailable}`);

    // Step 4: Verify the change persisted
    console.log('\n4. Verifying the change persisted...');
    const verifyResponse = await axios.get(`http://localhost:5236/api/cms/dishes`, {
      headers: { 'Authorization': `Bearer ${token}` }
    });

    const updatedDish = verifyResponse.data.data.find(d => d.id === testDish.id);
    if (updatedDish && updatedDish.isAvailable === !testDish.isAvailable) {
      console.log('✅ Change verified in database');
      console.log(`   Dish "${updatedDish.name}" availability is now: ${updatedDish.isAvailable}`);
    } else {
      throw new Error('Change did not persist in database');
    }

    console.log('\n🎉 FRONTEND FIX TEST PASSED!');
    console.log('✅ Boolean values are now handled correctly');
    console.log('✅ Dish availability updates work properly');

  } catch (error) {
    console.error('\n❌ TEST FAILED:', error.message);
    if (error.response) {
      console.error('Status:', error.response.status);
      console.error('Response:', JSON.stringify(error.response.data, null, 2));
    }
  }
}

testFrontendFix();
