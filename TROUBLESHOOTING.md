# Troubleshooting Guide for 404 Error with "create-dish" API Endpoint

## Problem Description
The Angular application is encountering a 404 (Not Found) error when attempting to call the API endpoint at `http://localhost:5236/api/cms/create-dish`.

## Possible Causes and Solutions

### 1. Backend Server Not Running
**Check**: Make sure the .NET Core API is running at port 5236.
**Solution**: Start the API by running:
```
cd "c:\Users\Abdulrahman Al Najja\Desktop\.NET Projects\Food Clone Angular + Flutter\Careem-Food-Angular-Flutter\API"
dotnet run
```

### 2. Authentication Issues
The `CmsController` requires Admin role authentication with the `[Authorize(Roles = "Admin")]` attribute.

**Check**: Verify that:
1. You are logged in as a user with the Admin role
2. The JWT token is being included in the HTTP request
3. The token has not expired

**Solution**:
- Log in again with an Admin account
- Check the browser's localStorage to verify the token exists and contains the Admin role
- Use the browser's network tab to confirm the Authorization header is being sent with requests

### 3. CORS Configuration
**Check**: Ensure CORS is properly configured to allow requests from your Angular app's origin.

**Solution**: The CORS policy in Program.cs looks correct, but verify it's being applied correctly.

### 4. Route Configuration
**Check**: Confirm the route in the controller matches what's being called.

**Solution**: The route is configured correctly in the controller as `[HttpPost("create-dish")]` under the base route `[Route("api/cms")]`, which should match the URL being called.

### 5. Port Mismatch
**Check**: Verify that the Angular environment is configured to use the correct API port.

**Solution**: The environment.ts file has `apiUrl: 'http://localhost:5236/api'` which matches the expected port.

## Diagnostic Steps

1. Use the included test-api.js script to test the API directly without involving the Angular app:
   ```
   node test-api.js
   ```
   (Remember to replace the token and IDs with actual values)

2. Try accessing the API via Swagger UI (if enabled) at:
   ```
   http://localhost:5236/swagger/index.html
   ```

3. Check the .NET Core API logs for any errors or exceptions that might be occurring when the endpoint is called.

4. Try using a tool like Postman to make a direct request to the API endpoint with proper authentication headers.

## Additional Debugging Tips

1. Add console logging in the Angular service before making the request:
   ```typescript
   createDish(dish: Dish): Observable<AppResponse<Dish>> {
     console.log('Sending request to:', `${this.baseUrl}/create-dish`);
     console.log('Request payload:', dish);
     console.log('Token:', localStorage.getItem('user') ? JSON.parse(localStorage.getItem('user')!).token : 'No token');
     return this.http.post<AppResponse<Dish>>(`${this.baseUrl}/create-dish`, dish);
   }
   ```

2. Check the API's launchSettings.json to confirm it's configured to listen on the expected port.

3. Ensure there are no network issues or firewalls blocking the communication between the Angular app and the API.
