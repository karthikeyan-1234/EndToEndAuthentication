# Keycloak Setup with Docker for .NET 9 Web API Authentication

This guide walks you through setting up Keycloak in Docker for securing your .NET 9 Web API using JWT Bearer authentication.

## Prerequisites

- Docker installed on your machine
- Basic understanding of Docker and container management
- The .NET 9 Web API configured as per the provided `program.cs` and `appsettings.json`

---

## Step 1: Run Keycloak Docker Container

Run Keycloak using the official Docker image with these commands:

docker network create keycloak-network

docker run -d
--name keycloak
--network keycloak-network
-p 8091:8080
-e KEYCLOAK_ADMIN=admin
-e KEYCLOAK_ADMIN_PASSWORD=admin
quay.io/keycloak/keycloak:24.0.5 start-dev


- This starts Keycloak in development mode on port `8091` (adjust the port as per your API settings).
- `KEYCLOAK_ADMIN` and `KEYCLOAK_ADMIN_PASSWORD` set the initial admin credentials.
- The `keycloak-network` helps you isolate your container networking if needed.

---

## Step 2: Access the Keycloak Admin Console

- Open a browser and navigate to:  
  `http://ARJUN-SERVER:8091/auth`  
  (Replace `ARJUN-SERVER` with your actual server name or IP)

- Log in with username: `admin` and password: `admin`.

---

## Step 3: Create a Realm

- In the admin console, click **Add Realm**.
- Name it `master` (to match your `.NET appsettings.json`).

---

## Step 4: Create a Client

- Within the `master` realm, go to **Clients** > **Create**.
- Client ID: `api-app` (matches your `ClientId` in the appsettings).
- Client Protocol: `openid-connect`.
- Root URL: Your API URL or leave blank.
- Save the client.

- On the client settings page:
  - Set **Access Type** to `confidential`.
  - Enable **Standard Flow Enabled**.
  - Set **Valid Redirect URIs** to your client redirect URLs if applicable.
  - Note the **Secret** under **Credentials** tab — this should match `ClientSecret` in your appsettings (`ajtH5dc3F7jbcvmNRkEQZllouNkwQI58`).

---

## Step 5: Configure Roles

- Go to **Roles** in the `master` realm.
- Add roles: `manager`, `operator` (matching your `[Authorize(Roles = "manager,operator")]` usage).
- Assign these roles to users as needed under **Users**.

---

## Step 6: Configure .NET API Settings

Your `appsettings.json` snippet to communicate with Keycloak should look like:

"KeyCloak": {
"Authority": "http://ARJUN-SERVER:8091/auth/realms/master",
"ClientId": "api-app",
"ClientSecret": "ajtH5dc3F7jbcvmNRkEQZllouNkwQI58",
"ResponseType": "code",
"Scope": "openid profile email"
}


---

## Step 7: Verify `program.cs` JWT Authentication Configuration

Make sure your `.NET` program uses these settings:

- **Authority** matches `http://ARJUN-SERVER:8091/auth/realms/master`
- **Audience** is `api-app`
- `RoleClaimType = ClaimTypes.Role` (to map roles correctly)
- Handle `OnTokenValidated` event to add role claims for authorization

Your existing setup already matches these recommendations.

---

## Step 8: Testing

- Use a tool like Postman or your frontend client to authenticate users using Keycloak.
- Obtain a JWT token for a user with roles `manager` or `operator`.
- Call your protected API endpoints with the JWT Bearer token in the Authorization header.
- You should receive authorized responses if roles match.

---

## Notes

- In production, **use HTTPS and proper security settings**.
- Adjust `RequireHttpsMetadata` to `true` in production.
- Consider persistent storage for Keycloak data (using volumes) instead of transient containers.
- You can customize users, clients, roles further as per your security model.

---

This completes the Keycloak setup in Docker aligned to your .NET 9 Web API authentication configuration.
