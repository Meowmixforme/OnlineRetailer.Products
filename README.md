# OnlineRetailer.Products

A backend service for an Amazon-style online retailer, developed as part of my university DevOps module.

## Overview

This repository contains the core backend components for managing products and orders in an online retail environment. It is designed to be modular and extensible, supporting product listing, order management, and integration with additional microservices.

## Project Structure

- **ThAmCo.Services.ProductsAPI/**  
  Main backend API project for products and orders.
  - `Controllers/` – API endpoints for products and orders
  - `Data/` – Data access and context
  - `OrderRepo/` – Order repository logic
  - `ProductsRepo/` – Product repository logic
  - `UnderCutters/` – Likely contains price comparison/undercutting logic
  - `Program.cs` – App start-up and configuration
  - `appsettings.json` – Application configuration

- **ThAmCo.Order.Tests/**  
  Unit and integration tests for order processing.

- **ThAmCo.Services.ProductsAPI.Tests/**  
  Unit and integration tests for the Products API.

- **Design Diagrams/**  
  Architecture and design documentation.

- **workflows/**  
  CI/CD configurations.

## Getting Started

### Prerequisites

- [.NET 6.0 SDK or later](https://dotnet.microsoft.com/download)
- (Optional) Docker for containerised development

### Setup

1. Clone the repository:
    ```bash
    git clone https://github.com/Meowmixforme/OnlineRetailer.Products.git
    cd OnlineRetailer.Products
    ```

2. Navigate to the API project:
    ```bash
    cd ThAmCo.Services.ProductsAPI
    ```

3. Restore dependencies and run the service:
    ```bash
    dotnet restore
    dotnet run
    ```

4. The API should now be running on `http://localhost:5000` (default port, configurable in `appsettings.json`).

### Running Tests

Navigate to the test projects and run:

```bash
dotnet test ThAmCo.Order.Tests
dotnet test ThAmCo.Services.ProductsAPI.Tests
```

## Features

- Product listing and search
- Order creation and management
- Extensible repository structure
- Designed for microservices and DevOps best practices
- Includes sample tests and design diagrams

## DevOps & CI/CD

- GitHub Actions workflows are configured under `/workflows`
- Supports automated build and test on push and pull request

## Documentation

- See the `Design Diagrams/` directory for architecture diagrams and detailed design documentation.

## Licence

This project is licensed under the terms of the [MIT Licence](LICENSE).
