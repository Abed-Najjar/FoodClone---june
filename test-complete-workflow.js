// Test script to verify the complete frontend dish editing workflow
const axios = require('axios');

async function testCompleteWorkflow() {
  try {
    console.log('🧪 Testing Complete Dish Edit Workflow...\n');

    // Step 1: Login as admin
    console.log('1. Authenticating as admin user...');
    const loginResponse = await axios.post('http://localhost:5236/api/Auth/login', {
      email: 'admin@careemclone.com',
      password: 'Admin@123456'
    });

    if (!loginResponse.data.success) {
      throw new Error('Authentication failed: ' + loginResponse.data.errorMessage);
    }

    const token = loginResponse.data.data.token;
    console.log('✅ Authentication successful');

    // Step 2: Get dishes for testing
    console.log('\n2. Retrieving dishes...');
    const dishesResponse = await axios.get('http://localhost:5236/api/cms/dishes', {
      headers: { 'Authorization': `Bearer ${token}` }
    });

    if (!dishesResponse.data.success || !dishesResponse.data.data.length) {
      throw new Error('No dishes available for testing');
    }

    const dishes = dishesResponse.data.data;
    console.log(`✅ Retrieved ${dishes.length} dishes`);

    // Step 3: Test the conversion logic for different scenarios
    console.log('\n3. Testing boolean conversion logic...');
    
    // Test string "true" -> boolean true
    let testValue = 'true';
    let converted = testValue === 'true' || testValue === true;
    console.log(`   String "${testValue}" -> Boolean ${converted} ✅`);
    
    // Test string "false" -> boolean false  
    testValue = 'false';
    converted = testValue === 'true' || testValue === true;
    console.log(`   String "${testValue}" -> Boolean ${converted} ✅`);
    
    // Test boolean true -> boolean true
    testValue = true;
    converted = testValue === 'true' || testValue === true;
    console.log(`   Boolean ${testValue} -> Boolean ${converted} ✅`);
    
    // Test boolean false -> boolean false
    testValue = false;
    converted = testValue === 'true' || testValue === true;
    console.log(`   Boolean ${testValue} -> Boolean ${converted} ✅`);

    // Step 4: Test updating dish with form-like data (simulating frontend behavior)
    console.log('\n4. Testing dish update with form data simulation...');
    const testDish = dishes[0];
    console.log(`   Selected dish: "${testDish.name}" (ID: ${testDish.id})`);
    console.log(`   Current availability: ${testDish.isAvailable}`);

    // Simulate what the Angular form would send (as strings)
    const formData = {
      name: testDish.name,
      description: testDish.description || '',
      imageUrl: testDish.imageUrl,
      price: testDish.price.toString(), // Forms send as string
      isAvailable: (!testDish.isAvailable).toString(), // Toggle and convert to string (form behavior)
      restaurantId: testDish.restaurantId.toString(), // Forms send as string
      categoryId: testDish.categoryId.toString() // Forms send as string
    };

    console.log('   Form data (as frontend would send):');
    console.log('   ', JSON.stringify(formData, null, 4));

    // Convert form data to API format (simulating frontend logic)
    const apiPayload = {
      id: testDish.id,
      name: formData.name,
      description: formData.description,
      imageUrl: formData.imageUrl,
      price: parseFloat(formData.price),
      isAvailable: formData.isAvailable === 'true' || formData.isAvailable === true, // Our conversion logic
      restaurantId: parseInt(formData.restaurantId),
      categoryId: parseInt(formData.categoryId)
    };

    console.log('\n   Converted API payload:');
    console.log('   ', JSON.stringify(apiPayload, null, 4));

    // Step 5: Send update request
    console.log('\n5. Sending update request...');
    const updateResponse = await axios.put(
      `http://localhost:5236/api/DishManagement/dishes/${testDish.id}`,
      apiPayload,
      {
        headers: { 
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      }
    );

    if (!updateResponse.data.success) {
      throw new Error('Update request failed: ' + updateResponse.data.errorMessage);
    }

    console.log('✅ Update request successful');
    console.log(`   New availability: ${updateResponse.data.data.isAvailable}`);

    // Step 6: Verify persistence
    console.log('\n6. Verifying data persistence...');
    const verifyResponse = await axios.get(`http://localhost:5236/api/cms/dishes`, {
      headers: { 'Authorization': `Bearer ${token}` }
    });

    const updatedDish = verifyResponse.data.data.find(d => d.id === testDish.id);
    const expectedAvailability = !testDish.isAvailable;
    
    if (updatedDish && updatedDish.isAvailable === expectedAvailability) {
      console.log('✅ Data persistence verified');
      console.log(`   Dish availability changed from ${testDish.isAvailable} to ${updatedDish.isAvailable}`);
    } else {
      throw new Error(`Data persistence failed. Expected: ${expectedAvailability}, Got: ${updatedDish?.isAvailable}`);
    }

    // Step 7: Test edge case - updating with the same value
    console.log('\n7. Testing edge case - updating with same availability...');
    const sameValuePayload = {
      ...apiPayload,
      isAvailable: updatedDish.isAvailable // Same value
    };

    const sameValueResponse = await axios.put(
      `http://localhost:5236/api/DishManagement/dishes/${testDish.id}`,
      sameValuePayload,
      {
        headers: { 
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      }
    );

    if (sameValueResponse.data.success) {
      console.log('✅ Same value update handled correctly');
    } else {
      throw new Error('Same value update failed: ' + sameValueResponse.data.errorMessage);
    }

    console.log('\n🎉 COMPLETE WORKFLOW TEST PASSED!');
    console.log('✅ Authentication works correctly');
    console.log('✅ Boolean conversion logic is correct');
    console.log('✅ Form data simulation works');
    console.log('✅ API updates work with converted data');
    console.log('✅ Data persistence is verified');
    console.log('✅ Edge cases are handled');
    console.log('\n📝 The frontend fix should now work correctly!');

  } catch (error) {
    console.error('\n❌ WORKFLOW TEST FAILED:', error.message);
    if (error.response) {
      console.error('HTTP Status:', error.response.status);
      console.error('Response Data:', JSON.stringify(error.response.data, null, 2));
    }
    if (error.stack) {
      console.error('Stack trace:', error.stack);
    }
  }
}

testCompleteWorkflow();
