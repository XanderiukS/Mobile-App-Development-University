<div align="center">

# Mobile App Development – University Projects

Projects developed for the **Mobile App Development** course of my **Systems Engineering** degree.

![React Native](https://img.shields.io/badge/React%20Native-0.88-61DAFB?style=flat-square&logo=react&logoColor=black)
![TypeScript](https://img.shields.io/badge/TypeScript-3178C6?style=flat-square&logo=typescript&logoColor=white)
![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?style=flat-square&logo=postgresql&logoColor=white)
![MongoDB](https://img.shields.io/badge/MongoDB-47A248?style=flat-square&logo=mongodb&logoColor=white)

</div>

## About

This repository collects the projects I built during the course: mobile applications with **React Native** and the **REST APIs** with ASP.NET Core that a mobile or web client can consume. Each folder is an independent project, and together they show my progression from simple forms to apps that consume public APIs and back-end services with relational and document databases.

## Projects

Ordered from easiest to most advanced:

| Project | Type | What it does | What I learned |
|---|---|---|---|
| [FirstMobileApp](./FirstMobileApp) | Mobile app | Four-field user form that validates the input and shows the data in an alert. | Components, `useState`, `TextInput` and `Alert`. |
| [EightCharactersMobileApp](./EightCharactersMobileApp) | Mobile app | Extended eight-field user form with keyboard handling. | `KeyboardAvoidingView`, keyboard types and form validation. |
| [Clase2Pokemon](./Clase2Pokemon) | Mobile app | Pokémon search that shows the official artwork and the shiny sprite. | `fetch` with `async`/`await`, error handling and loading indicators. |
| [ProjectMovil](./ProjectMovil) | Mobile app | Menu app with screen navigation and a weather screen that consumes Open-Meteo. | React Navigation, `axios` and `useEffect`. |
| [StoreXander](./StoreXander) | Mobile app | Product catalog with search, retry and clear actions, consuming DummyJSON. | `FlatList`, reusable components and a TypeScript service layer. |
| [Ordenes_API](./Ordenes_API) | REST API | Delivery orders CRUD with an audit log in MongoDB. | Layered ASP.NET Core APIs, Dapper, PostgreSQL and MongoDB. |
| [HotelAPI](./HotelAPI) | REST API | Hotel reservations with room availability, amount calculation and history. | Database transactions, row locking and custom exceptions. |
| [BancoAPI](./BancoAPI) | REST API + web client | Banking API for clients, users and movements, with an Angular front end. | Full-stack integration, CORS and OpenAPI. |

## How to Run

**Mobile apps** (requires [Node.js](https://nodejs.org/) 22.11 or later and Android Studio):

```bash
cd FirstMobileApp
npm install
npm start          # terminal 1: Metro bundler
npm run android    # terminal 2: Android emulator or device
```

**REST APIs** (requires the [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0), PostgreSQL and MongoDB):

```bash
cd Ordenes_API
dotnet run --project Ordenes_API
```

> [!NOTE]
> The application interfaces are in Spanish. Each project's README explains its full setup, including the database configuration.

---

<div align="center">

**Alexander Oliva** · [GitHub](https://github.com/XanderiukS) · [LinkedIn](https://www.linkedin.com/in/alexander-oliva-estrada)

</div>
