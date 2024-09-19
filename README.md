# Company Hierarchy Management API

## Purpose

This project is a Web API built with **.NET Core** to manage and visualize a company's employee hierarchy. It allows you to create, update, delete, and retrieve employee records while maintaining the hierarchical structure of the organization.

## Features

- **Manage Employees:** Create new employees, update existing ones, and delete them if necessary.
- **Hierarchy Management:** Automatically reassign subordinates when a manager is removed, or make them top-level managers.
- **View Hierarchy:** Retrieve an employee's details along with their entire reporting structure, or get the full company hierarchy.

## Getting Started

1. **Clone the Repository**

    ```bash
    git clone https://github.com/halilozkul/CompanyHierarchyRepository
    cd your-repo
    ```

2. **Install Dependencies**

    ```bash
    dotnet restore
    ```

3. **Run the Application**

    ```bash
    dotnet run
    ```

   The API will be accessible at:
   - **HTTP:** `http://localhost:5126`
   - **HTTPS:** `https://localhost:7199`

## API Endpoints

- **[PUT] /employee**: Create or update an employee.
- **[DELETE] /employee/{EmployeeId}**: Delete an employee and manage their subordinates.
- **[GET] /employee/{EmployeeId}**: Retrieve details of a specific employee and their direct/indirect reports.
- **[GET] /employee**: Get the entire company hierarchy, including all top-level managers and their subordinates.

## Models

- **Employee**: Represents an employee with an `EmployeeId`, `FullName`, `Title`, and a list of `ManagedEmployees`.
- **EmployeeData**: Data transfer object for employee creation and updates.

## Notes

- The project follows **SOLID** principles for maintainable code and **CQRS** for separating command and query responsibilities.
- Total development time: approximately **X hours**.
- Ensure you have the appropriate database setup and migrations applied if using a database.

## License

This project is licensed under the MIT License.
