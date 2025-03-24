#Disaster Prediction and Alert System API 

##Scenario
You are tasked with building a Disaster Prediction and Alert System API for a government agency. This API will predict 
potential disaster risks (such as floods, earthquakes, and wildfires) for specified regions and send alerts to affected 
communities. The system integrates with external APIs to gather real-time environmental data and uses a simple scoring 
algorithm to assess risk levels.Disaster Prediction and Alert System API 

##Requirements
1. Build the API: Implement each endpoint with the specified functionality.
2. External API Integration: Set up integration with external environmental data sources for real-time data.
3. Caching with Redis: Use Redis to cache environmental data and risk calculations to minimize redundant external 
API calls.
4. Azure Deployment: Deploy the solution on Azure
5. Error Handling: Manage scenarios like:
   **Failed external API calls.
   **Missing data from external sources.
   **Regions with no available data.  
6. Messaging API Integration: Implement alert-sending functionality via a messaging API to notify people in high risk regions.
7. Logging: Track API usage and alerts for auditing purposes.

#Stack
**Backend:** .NET CORE 8
**Database:** MSSQL,Redis
**Server:** Azure
**Other tool : Github , Azure Key Vault

#API Ref
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
