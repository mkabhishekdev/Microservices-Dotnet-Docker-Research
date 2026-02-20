# Microservices

In this project which was my M.S Degree Capstone at Rochester Institute of Technology, NY, USA, I have developed a ecommerce web application named 'AbhiShop' to demonstrate the technicalities involved in developing a Microservices model from scratch. If you are curious to understand why should someone move from a traditional design such has Monolithic to Microservices, I have written a detailed blog explaining each and every step. Below is the link to each article. This repo consists of the source code I have developed which I have explained in the article.

Article 1: Introduction (Theory) 
     Link: https://medium.com/@abhikempanna/blog-series-understanding-microservices-using-abhishop-app-3a02b2f9a36f

Article 2: Traditional/Monolithic Design
     Link: https://medium.com/@abhikempanna/blog-series-understanding-microservices-using-abhishop-app-f8a184f4f184

Article 3: Roadmap towards Microservices
     Link: https://medium.com/@abhikempanna/understanding-microservices-using-abhishop-app-68bab7054744

Article 4: Development of AbhiShop application using Microservices approach
     Link: https://medium.com/@abhikempanna/blog-series-understanding-microservices-using-abhishop-app-76c19ad849ee

Article 5: Execution of AbhiShop app as Microservices/Thoughts on Devops/Limitations
     Link: https://medium.com/@abhikempanna/blog-series-understanding-microservices-using-abhishop-app-a03c9265d66a

***************************************************************************************************************************

AbhiShop Microservices Modernization Project (2019 → 2026)
-----------------------------------------------------

SYSTEM-DESIGN PLAN
---------------------

Step 1: Upgrade Framework
---------------------------------------
- Migrate to .NET 8 (or latest LTS)
- Clean deprecated packages
- Update NuGet dependencies
- Document changes.


Step 2: Architecture Review
--------------------------------------
Before coding:
- Redefine service boundaries

Define new services:
- Ordering Service
- Payment Service

Decide communication:
- REST?
- Event-driven?
- Message broker?

Draw architecture diagram first.


Step 3: Scaffold New Microservices (AI Assisted)
-----------------------------------------------------

Use Codex for:
- Project scaffolding
- Controllers
- DTOs
- CRUD operations
- Dockerfiles

Manually:
- Review structure
- Improve validation
- Enforce clean layering


Step 4: Improve Production Readiness
-------------------------------------------------------

Add:
- Health checks
- Swagger/OpenAPI improvements

Logging
- Exception middleware
- API versioning


Step 5: Containerization
------------------------------------------------------

- Dockerize each microservice
- Create docker-compose
- Verify local orchestration


Step 6: CI/CD Pipeline
-------------------------------------------------------

Add GitHub Actions:
- Build all services
- Run tests
- Build Docker images
- (Optional) Push to registry


Step 7: Documentation Section
--------------------------------------------------------

Add in README:
🔄 Modernization Summary (2019 → 2026)
- Migrated to .NET 8
- Added Ordering & Payment services
- Introduced unit testing
- Added CI/CD
- Containerized services
- Refactored architecture for scalability


