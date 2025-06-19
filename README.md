# OnlineRetailer.Products

A backend service for an Amazon-style online retailer, developed as part of my university Cloud Computing DevOps module.

---

## Overview

This repository contains the core backend components for managing products and orders in an online retail environment. It is designed using microservices principles, supporting modular development, extensibility, and integration with additional system containers.

- **Implements:** Product listing/search, order management, basic user management (if applicable).
- **Assessment:** This service forms part of the solution for the ThAmCo System scenario, addressing requirements for product and order handling.

---

## Project Structure

```
ThAmCo.Services.ProductsAPI/         # Main backend API for products/orders
  ├── Controllers/                   # REST API endpoints (products, orders)
  ├── Data/                          # Data context and access logic
  ├── OrderRepo/                     # Order repository logic
  ├── ProductsRepo/                  # Product repository logic
  ├── UnderCutters/                  # Price comparison/undercutting logic
  ├── Program.cs                     # Application entry/configuration
  └── appsettings.json               # Service configuration

ThAmCo.Order.Tests/                  # Tests for order processing logic
ThAmCo.Services.ProductsAPI.Tests/   # Tests for Products API logic
Design Diagrams/                     # Architecture and design docs
workflows/                           # CI/CD (GitHub Actions) configs
```

---

## Getting Started

### Prerequisites

- [.NET 6.0 SDK or later](https://dotnet.microsoft.com/download)
- (Optional) [Docker](https://www.docker.com/) for containerised development

### Setup & Run

```bash
git clone https://github.com/Meowmixforme/OnlineRetailer.Products.git
cd OnlineRetailer.Products/ThAmCo.Services.ProductsAPI
dotnet restore
dotnet run
```
The API runs by default on [http://localhost:5000](http://localhost:5000) (see `appsettings.json` for config).

---

## Running Tests

```bash
dotnet test ThAmCo.Order.Tests
dotnet test ThAmCo.Services.ProductsAPI.Tests
```

---

## Features

- Product listing and search (including "loose" search in name/descriptor)
- Order creation and management (with stock/funds logic)
- Extensible repository structure for future containers and integrations
- Designed as a microservice, suitable for cloud-native deployment
- Examples of secure endpoints and basic error handling
- Automated tests (unit and integration)

---

## DevOps, CI/CD & Resilience

- **GitHub Actions** under `/workflows` provide:
  - Automated build and test on every push/PR
  - Sample deployment workflow (customizable)
- **Configuration Management:** Uses `appsettings.json` and environment variables for flexible deployment (test/live).
- **Resilience:** Basic error handling and retry logic included (expand as needed for assessment).
- **Security:** Demonstrates secure endpoints (JWT/auth can be mocked for demo).

---

## System Architecture & Technology Choices

- **Tech Stack:** ASP.NET Core 6, Entity Framework Core, (optionally) SQL Server or SQLite.
- **Architecture Diagrams & Design Rationale:**  
  See [Design Diagrams/](./Design%20Diagrams) for:
  - C4 Container and deployment diagrams
  - System boundaries and interfaces
  - Narrative on technology and design decisions, explaining security/resilience and DevOps choices

---

## Mocked/External Dependencies

- Interfaces to other system containers (e.g., payments, user management) are **mocked or stubbed** to allow for individual container development/testing, as per assignment instructions.

---

## Licence

This project is licensed under the terms of the MIT Licence.
