// Simple test to debug dish API endpoints
const BASE_URL = 'http://localhost:5236/api';

async function simpleDishTest() {
    try {
        // Test health endpoint first
        console.log('Testing health endpoint...');
        const healthResponse = await fetch(`${BASE_URL}/health`);
        const healthData = await healthResponse.json();
        console.log('Health check:', healthData);

        // Test getting dishes from restaurant 1
        console.log('\nTesting dishes endpoint...');
        const dishesUrl = `${BASE_URL}/DishManagement/restaurant/dishes/1`;
        console.log('Fetching from:', dishesUrl);
        
        const dishesResponse = await fetch(dishesUrl);
        console.log('Response status:', dishesResponse.status);
        console.log('Response headers:', Object.fromEntries(dishesResponse.headers.entries()));
        
        const dishesText = await dishesResponse.text();
        console.log('Raw response:', dishesText);
        
        // Try to parse JSON
        if (dishesText) {
            const dishesData = JSON.parse(dishesText);
            console.log('Parsed dishes data:', JSON.stringify(dishesData, null, 2));
        }
        
    } catch (error) {
        console.error('Test error:', error.message);
        console.error('Stack:', error.stack);
    }
}

simpleDishTest();
