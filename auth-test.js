// Authenticated test to debug dish update issue
const BASE_URL = 'http://localhost:5236/api';

async function authenticatedDishTest() {
    try {
        // Step 1: Login to get token
        console.log('1. Logging in as admin...');
        const loginResponse = await fetch(`${BASE_URL}/auth/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                email: 'admin@careemclone.com',
                password: 'Admin@123456'
            })
        });
        
        const loginData = await loginResponse.json();
        console.log('Login response:', loginData);
        
        if (!loginData.success) {
            throw new Error('Login failed: ' + loginData.errorMessage);
        }
        
        const token = loginData.data.token;
        console.log('Got token:', token.substring(0, 20) + '...');
        
        // Step 2: Get dishes with authentication
        console.log('\n2. Getting dishes with authentication...');
        const dishesResponse = await fetch(`${BASE_URL}/DishManagement/restaurant/dishes/1`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });
        
        const dishesData = await dishesResponse.json();
        console.log('Dishes response:', JSON.stringify(dishesData, null, 2));
        
        if (dishesData.success && dishesData.data && dishesData.data.length > 0) {
            const firstDish = dishesData.data[0];
            console.log('\n3. Found dish to test:', {
                id: firstDish.id,
                name: firstDish.name,
                isAvailable: firstDish.isAvailable,
                restaurantId: firstDish.restaurantId
            });
            
            // Step 3: Test update with the exact payload structure that frontend sends
            const updatePayload = {
                id: firstDish.id,
                name: firstDish.name,
                description: firstDish.description || '',
                price: firstDish.price,
                imageUrl: firstDish.imageUrl || '',
                restaurantId: firstDish.restaurantId,
                isAvailable: !firstDish.isAvailable, // Toggle availability
                categoryId: firstDish.categoryId
            };
            
            console.log('\n4. Updating dish with payload:', JSON.stringify(updatePayload, null, 2));
            
            const updateResponse = await fetch(`${BASE_URL}/DishManagement/dishes/${firstDish.id}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(updatePayload)
            });
            
            console.log('Update response status:', updateResponse.status);
            console.log('Update response headers:', Object.fromEntries(updateResponse.headers.entries()));
            
            const updateText = await updateResponse.text();
            console.log('Update raw response:', updateText);
            
            if (updateText) {
                try {
                    const updateData = JSON.parse(updateText);
                    console.log('Update parsed response:', JSON.stringify(updateData, null, 2));
                } catch (e) {
                    console.log('Could not parse update response as JSON');
                }
            }
            
            // Step 4: Verify the change by getting the dish again
            console.log('\n5. Verifying the update...');
            const verifyResponse = await fetch(`${BASE_URL}/DishManagement/restaurant/dishes/1`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            
            const verifyData = await verifyResponse.json();
            const updatedDish = verifyData.data?.find(d => d.id === firstDish.id);
            
            if (updatedDish) {
                console.log('Updated dish:', {
                    id: updatedDish.id,
                    name: updatedDish.name,
                    isAvailable: updatedDish.isAvailable,
                    originalAvailability: firstDish.isAvailable
                });
                
                if (updatedDish.isAvailable !== firstDish.isAvailable) {
                    console.log('✅ SUCCESS: Dish availability was updated successfully!');
                } else {
                    console.log('❌ FAILURE: Dish availability was NOT updated.');
                }
            } else {
                console.log('Could not find updated dish in verification');
            }
        } else {
            console.log('No dishes found or API error');
        }
        
    } catch (error) {
        console.error('Test error:', error.message);
        console.error('Stack:', error.stack);
    }
}

authenticatedDishTest();
