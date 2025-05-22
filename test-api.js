x// Script to test API connectivity for the create-dish endpoint
// Save this file as test-api.js and run it with Node.js

const https = require('https');
const http = require('http');

// Configuration
const API_URL = 'http://localhost:5236/api/cms/create-dish';
const TOKEN = 'YOUR_ADMIN_JWT_TOKEN_HERE'; // Replace with an actual token from localStorage

// Sample dish data matching what's sent from the Angular app
const dishData = {
  name: 'Test Dish',
  description: 'A test dish to check API connectivity',
  price: 9.99,
  imageUrl: 'https://example.com/image.jpg',
  isAvailable: true,
  restaurantId: 1, // Replace with an actual restaurant ID from your database
  categoryId: 1,    // Replace with an actual category ID from your database
  id: 0,
  restaurantName: 'Test Restaurant'
};

// Prepare request options
const options = {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${TOKEN}`
  }
};

// Determine if we need http or https
const client = API_URL.startsWith('https') ? https : http;

console.log(`Sending request to: ${API_URL}`);
console.log('Request payload:', JSON.stringify(dishData));

// Make the request
const req = client.request(API_URL, options, (res) => {
  console.log(`Status Code: ${res.statusCode}`);
  console.log(`Status Message: ${res.statusMessage}`);
  console.log('Headers:', res.headers);

  let data = '';

  res.on('data', (chunk) => {
    data += chunk;
  });

  res.on('end', () => {
    console.log('Response body:');
    try {
      const jsonData = JSON.parse(data);
      console.log(JSON.stringify(jsonData, null, 2));
    } catch (e) {
      console.log(data);
      console.log('Error parsing JSON:', e.message);
    }
  });
});

req.on('error', (error) => {
  console.error('Request error:', error.message);
});

// Write data to the request body
req.write(JSON.stringify(dishData));
req.end();

console.log('Request sent, waiting for response...');
