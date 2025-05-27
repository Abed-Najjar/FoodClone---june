// Test script to debug dish update issue
// Using built-in fetch (Node.js 18+)

const BASE_URL = 'http://localhost:5236/api';

// Test dish update with minimal payload
async function testDishUpdate() {
    try {
        // First, get all dishes to find one to update
        console.log('1. Getting all dishes from DishManagement...');
        const getDishesResponse = await fetch(`${BASE_URL}/DishManagement/restaurant/dishes/1`);
        const dishesResult = await getDishesResponse.json();
        
        console.log('Dishes response:', JSON.stringify(dishesResult, null, 2));
        
        if (dishesResult.success && dishesResult.data && dishesResult.data.length > 0) {
            const firstDish = dishesResult.data[0];
            console.log('\n2. Found dish to test:', {
                id: firstDish.id,
                name: firstDish.name,
                isAvailable: firstDish.isAvailable,
                restaurantId: firstDish.restaurantId
            });
            
            // Test update with the exact payload structure that frontend sends
            const updatePayload = {
                id: firstDish.id,
                name: firstDish.name,
                description: firstDish.description || '',
                price: firstDish.price,
                imageUrl: firstDish.imageUrl,
                restaurantId: firstDish.restaurantId,
                isAvailable: !firstDish.isAvailable, // Toggle availability
                categoryId: firstDish.categoryId
            };
            
            console.log('\n3. Sending update request...');
            console.log('Update payload:', JSON.stringify(updatePayload, null, 2));
            
            const updateResponse = await fetch(`${BASE_URL}/DishManagement/dishes/${firstDish.id}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(updatePayload)
            });
            
            const updateResult = await updateResponse.json();
            console.log('\n4. Update response:', JSON.stringify(updateResult, null, 2));
            console.log('Response status:', updateResponse.status);
            
            if (!updateResult.success) {
                console.log('\n❌ Update failed!');
                console.log('Error message:', updateResult.errorMessage);
            } else {
                console.log('\n✅ Update successful!');
                console.log('Updated isAvailable:', updateResult.data.isAvailable);
            }
        } else {
            console.log('No dishes found to test with');
        }
        
    } catch (error) {
        console.error('Test error:', error.message);
    }
}

testDishUpdate();
