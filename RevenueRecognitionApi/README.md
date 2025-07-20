# 💲 Revenue Recognition API

A robust, modern API for managing clients, contracts, software products, discounts, payments, and comprehensive revenue recognition - built for SaaS, software vendors, and financial teams.

---

## 🚀 Overview

**Revenue Recognition API** streamlines the entire process of client management, contract lifecycle, and revenue tracking. 
</br>
It supports multi-currency revenue reports, contract and payment management, and secure authentication - all with a clean, RESTful interface.

**Key Features:**
- 🔒 **JWT-based authentication**
- 👥 **Individual & company client management**
- 💾 **Software product & discount catalog**
- 📝 **Contract creation, validation, and payment tracking**
- 💰 **Current & forecasted revenue in any currency (auto exchange rate)**
- 📉 **Automatic discount, loyalty, and price logic**
- 🛡️ **Robust error handling and clear API responses**
- 🧾 **Full audit trail (soft deletes, status tracking)**

---

## 🛠️ Tech Stack

- **.NET 8** (C#)
- **Entity Framework Core**
- **JWT Authentication**
- **SQL Database** (PostgreSQL, SQL Server, or SQLite)
- **Swagger** (OpenAPI) for API documentation

---

## 📦 Main Endpoints

| Endpoint                        | Method | Description                                      |
| ------------------------------- | ------ | ------------------------------------------------ |
| `/api/auth/login`               | POST   | Authenticate and receive JWT                     |
| `/api/auth/refresh`             | POST   | Refresh access token                             |
| `/api/clients/individual`       | CRUD   | Manage individual clients                        |
| `/api/clients/company`          | CRUD   | Manage company clients                           |
| `/api/software`                 | CRUD   | Manage software products                         |
| `/api/discounts`                | CRUD   | Manage software discounts                        |
| `/api/contracts`                | CRUD   | Manage contracts                                 |
| `/api/payments`                 | CRUD   | Track contract payments                          |
| `/api/revenue/current`          | POST   | Get recognized revenue (optional currency filter) |
| `/api/revenue/forecast`         | POST   | Get forecasted revenue (optional currency)        |

> For full details, see interactive Swagger docs at `/swagger`.

---

## 💡 Revenue Recognition Logic

- **Current revenue**: All signed, fully paid contracts
- **Forecasted revenue**: Signed + all active contracts (including unsigned)
- **Currency conversion**: Specify your target currency (e.g., `"currency": "EUR"`); exchange rates fetched automatically
- **Discounts and loyalty**: Best available discount applied; loyal clients get additional bonus

---

## 📚 Example Usage

### Authenticate (get JWT)
```http
POST /api/auth/login
{
  "username": "john",
  "password": "secret"
}
```

### Get Current Revenue in EUR
```http
POST /api/revenue/current
{
  "currency": "EUR"
}
```

### Add Company Client
```http
POST /api/clients/company
{
  "companyName": "Acme Sp. z o.o.",
  "address": "Warsaw, Poland",
  "email": "info@acme.pl",
  "phone": "+48 123 456 789",
  "krs": "1234567890"
}
```

---

## 🔒 Authentication

All endpoints (except `/api/auth/*`) require a valid JWT in the `Authorization` header:
```
Authorization: Bearer <your_token>
```
