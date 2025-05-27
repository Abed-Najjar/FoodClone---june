// Simple test to verify dishes API is working
// Using built-in fetch (Node.js 18+)

async function testDishesAPI() {
    try {
        console.log('Testing API connection...');
        
        // Test 1: Check if API is running
        const healthResponse = await fetch('http://localhost:5236/api/health');
        if (healthResponse.ok) {
            console.log('✅ API Health Check: PASSED');
        } else {
            console.log('❌ API Health Check: FAILED');
            return;
        }

        // Test 2: Get all restaurants first
        console.log('\nFetching restaurants...');
        const restaurantsResponse = await fetch('http://localhost:5236/api/cms/restaurants');
        const restaurantsData = await restaurantsResponse.json();
        
        if (restaurantsData.success && restaurantsData.data.length > 0) {
            console.log(`✅ Found ${restaurantsData.data.length} restaurants`);
            console.log('Restaurant IDs:', restaurantsData.data.map(r => r.id));
            
            // Test 3: Get dishes for first restaurant
            const firstRestaurantId = restaurantsData.data[0].id;
            console.log(`\nFetching dishes for restaurant ${firstRestaurantId}...`);
            
            const dishesResponse = await fetch(`http://localhost:5236/api/DishManagement/restaurant/dishes/${firstRestaurantId}`);
            const dishesData = await dishesResponse.json();
            
            if (dishesData.success) {
                console.log(`✅ Found ${dishesData.data?.length || 0} dishes for restaurant ${firstRestaurantId}`);
                if (dishesData.data && dishesData.data.length > 0) {
                    console.log('Sample dish:', {
                        id: dishesData.data[0].id,
                        name: dishesData.data[0].name,
                        price: dishesData.data[0].price,
                        restaurantId: dishesData.data[0].restaurantId
                    });
                }
            } else {
                console.log('❌ Failed to fetch dishes:', dishesData.errorMessage);
            }
        } else {
            console.log('❌ No restaurants found or failed to fetch restaurants');
            console.log('Response:', restaurantsData);
        }

    } catch (error) {
        console.error('❌ Test failed with error:', error.message);
    }
}

// Run the test
testDishesAPI();
