# Disaster Prediction and Alert System API 


## Scenario
You are tasked with building a Disaster Prediction and Alert System API for a government agency. This API will predict 
potential disaster risks (such as floods, earthquakes, and wildfires) for specified regions and send alerts to affected 
communities. The system integrates with external APIs to gather real-time environmental data and uses a simple scoring 
algorithm to assess risk levels.Disaster Prediction and Alert System API 


## Requirements
1. Build the API: Implement each endpoint with the specified functionality.
2. External API Integration: Set up integration with external environmental data sources for real-time data.
3. Caching with Redis: Use Redis to cache environmental data and risk calculations to minimize redundant external 
API calls.
4. Azure Deployment: Deploy the solution on Azure
5. Error Handling: Manage scenarios like:
   - Failed external API calls.
   - Missing data from external sources.
   - Regions with no available data.  
6. Messaging API Integration: Implement alert-sending functionality via a messaging API to notify people in high risk regions.
7. Logging: Track API usage and alerts for auditing purposes.


# Stack
- Backend: .NET CORE 8
- Database: MSSQL,Redis
- Server: Azure
- Other tool : Github , Azure Key Vault


# Web API Demo
  API Url : https://test-demo-project-befwhacvaugscvaw.southeastasia-01.azurewebsites.net/
  - POST /api/regions: Allows users to add regions with specific location coordinates and types of disasters they want to monitor. 
  - POST /api/alert-settings: Allows users to configure alert settings for each region, including thresholds for disaster risk scores. 
  - GET /api/disaster-risks: Triggers the disaster risk assessment, fetching real-time environmental data for all regions and calculating risk scores. This endpoint should return risk levels for each region and indicate if any alerts should be sent. 
  - POST /api/alerts/send: Sends an alert for regions identified as high-risk and stores the alert in the database. Integrate with a messaging API 
  - GET /api/alerts: Returns a list of recent alerts for each region, stored in a database.

  ## Example
  - api/Regions
    
    ![image](https://github.com/user-attachments/assets/9e245e39-83a2-43c9-b1f8-cd408f354361)

  - api/Alert-settings
    
    ![image](https://github.com/user-attachments/assets/76878810-a1ba-43cd-89da-8e9e17aa5419)

  - api/Disaster-risks
    
    ![image](https://github.com/user-attachments/assets/85bc78af-96b5-46f0-b5d9-db34e7886867) &nbsp; ![image](https://github.com/user-attachments/assets/a05bc398-84f6-4e69-9d17-e8e148c2ea8c)
 
    ![image](https://github.com/user-attachments/assets/70adea8c-c86f-4ae5-8a82-cb7dfd0c7a55)






  - api/Alerts
    
    ![image](https://github.com/user-attachments/assets/fa0a8976-3c13-480d-b462-e749fc2d21bf)



# API Ref
- Weather API : https://openweathermap.org/api
- EarthQuake API : https://earthquake.usgs.gov/fdsnws/event/1/
- Send messages : https://developers.line.biz/en/docs/messaging-api/sending-messages/


## Reference
- [Azure Key Vault] https://medium.com/@ariyakluankloi/azure-key-vault-30cf04006e65
- [ASP.NET Core Web Api and Azure Cache for Redis] https://medium.com/t-t-software-solution/asp-net-core-web-api-and-azure-cache-for-redis-3b81202f11ac
- [CI/CD with GitHub Actions] https://www.youtube.com/watch?v=7LkRipTlTzc&t=1203s
- [Deploy a .NET API to Azure] https://www.youtube.com/watch?v=EKqXAMLsnKQ
- [How to Use Azure Key Vault to Keep Your .NET API] https://www.youtube.com/watch?v=ZXfuxisC0IA
- [Set Up Microsoft Azure SQL Server and SQL Database] https://www.youtube.com/watch?v=6joGkZMVX4o
