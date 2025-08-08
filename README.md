# OnlineRetailer.Products

A backend service for an Amazon-style online retailer, developed for my university Cloud Computing DevOps module.

⚠️ This project was part of a now-retired backend integration. All credentials and endpoints are decommissioned and non-functional.

---

## Overview

This repository contains backend components for managing products and orders in an online retail environment. It is structured to support modular development and demonstrates core DevOps and microservices principles.



<img width="500" height="500" alt="466093642-c74e04eb-8271-4427-9cef-df6c521c195d" src="https://github.com/user-attachments/assets/61c94f65-81a4-43c2-bc14-d37a8db6dc72" />




---

## Project Structure

```
ThAmCo.Services.ProductsAPI/         # Main backend API for products and orders
  ├── Controllers/                   # REST API endpoints
  ├── Data/                          # Data context and access logic
  ├── OrderRepo/                     # Order repository logic
  ├── ProductsRepo/                  # Product repository logic
  ├── UnderCutters/                  # Price comparison/undercutting logic (stub/mock)
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
- (Optional) [Docker](https://www.docker.com/) for containerized development

### Setup & Run

```bash
git clone https://github.com/Meowmixforme/OnlineRetailer.Products.git
cd OnlineRetailer.Products/ThAmCo.Services.ProductsAPI
dotnet restore
dotnet run
```
The API runs by default on [http://localhost:5000](http://localhost:5000) (see `appsettings.json` for configuration).

---

## Running Tests

```bash
dotnet test ThAmCo.Order.Tests
dotnet test ThAmCo.Services.ProductsAPI.Tests
```

---

## Features

- Product listing and retrieval
- Order creation and management
- Modular repository structure
- Automated unit and integration tests

---

## DevOps & CI/CD

- **GitHub Actions** under `/workflows` for automated build and test on every push and pull request
- **Configuration management** using `appsettings.json` and environment variables

---

## System Architecture & Technology Choices

- **Tech Stack:** ASP.NET Core 6, Entity Framework Core, SQL Server (or SQLite for development/testing)
- **Architecture Diagrams:**  
  See the [Design Diagrams/](./Design%20Diagrams) directory for architecture diagrams and design documentation

---

## Licence

This project is licensed under the terms of the MIT Licence.
