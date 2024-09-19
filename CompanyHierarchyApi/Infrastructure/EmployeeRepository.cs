using Npgsql;
using CompanyHierarchyApi.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompanyHierarchyApi.Infrastructure
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Get an employee by ID along with their managed employees
        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"SELECT employee_id, full_name, title, manager_employee_id 
                              FROM employees WHERE employee_id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            var employee = new Employee
                            {
                                EmployeeId = reader.GetInt32(0),
                                FullName = reader.GetString(1),
                                Title = reader.GetString(2),
                                ManagedEmployees = new List<Employee>()
                            };

                            // Recursively fetch managed employees
                            employee.ManagedEmployees = await GetManagedEmployeesAsync(employee.EmployeeId);
                            return employee;
                        }
                    }
                }
            }

            throw new Exception($"Employee with ID {id} not found.");
        }

        // Get all top-level managers (employees without a manager)
        public async Task<IEnumerable<Employee>> GetTopLevelManagersAsync()
        {
            var employees = new List<Employee>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"SELECT employee_id, full_name, title FROM employees WHERE manager_employee_id IS NULL";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var employee = new Employee
                            {
                                EmployeeId = reader.GetInt32(0),
                                FullName = reader.GetString(1),
                                Title = reader.GetString(2),
                                ManagedEmployees = new List<Employee>()
                            };

                            // Recursively fetch the employees managed by this top-level manager
                            employee.ManagedEmployees = await GetManagedEmployeesAsync(employee.EmployeeId);

                            employees.Add(employee);
                        }
                    }
                }
            }

            return employees;
        }

        // Recursively fetch all employees reporting to a given manager
        private async Task<List<Employee>> GetManagedEmployeesAsync(int managerId)
        {
            var employees = new List<Employee>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"SELECT employee_id, full_name, title 
                              FROM employees WHERE manager_employee_id = @managerId";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@managerId", managerId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var employee = new Employee
                            {
                                EmployeeId = reader.GetInt32(0),
                                FullName = reader.GetString(1),
                                Title = reader.GetString(2),
                                ManagedEmployees = new List<Employee>()
                            };

                            employee.ManagedEmployees = await GetManagedEmployeesAsync(employee.EmployeeId);
                            employees.Add(employee);
                        }
                    }
                }
            }

            return employees;
        }

        public async Task AddOrUpdateEmployeeAsync(EmployeeData employeeData)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Check if the manager exists, if a manager is specified
                if (employeeData.ManagerEmployeeId.HasValue)
                {
                    var managerExistsQuery = @"SELECT COUNT(1) FROM employees WHERE employee_id = @managerId";
                    using (var command = new NpgsqlCommand(managerExistsQuery, connection))
                    {
                        command.Parameters.AddWithValue("@managerId", employeeData.ManagerEmployeeId.Value);

                        var managerExists = (long)await command.ExecuteScalarAsync();

                        if (managerExists == 0)
                        {
                            throw new Exception($"Manager with ID {employeeData.ManagerEmployeeId} does not exist.");
                        }
                    }
                }

                string query;

                if (employeeData.EmployeeId == 0)
                {
                    // Insert new employee
                    query = @"INSERT INTO employees (full_name, title, manager_employee_id) 
                              VALUES (@full_name, @title, @manager_employee_id)";
                }
                else
                {
                    // Update existing employee
                    query = @"UPDATE employees SET full_name = @full_name, title = @title, 
                              manager_employee_id = @manager_employee_id WHERE employee_id = @id";
                }

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@full_name", employeeData.FullName);
                    command.Parameters.AddWithValue("@title", employeeData.Title);
                    command.Parameters.AddWithValue("@manager_employee_id",
                        employeeData.ManagerEmployeeId.HasValue ?
                        (object)employeeData.ManagerEmployeeId.Value : DBNull.Value);

                    if (employeeData.EmployeeId > 0)
                    {
                        command.Parameters.AddWithValue("@id", employeeData.EmployeeId);
                    }
                    var rowsAffected = await command.ExecuteNonQueryAsync();

                    if (rowsAffected == 0)
                    {
                        throw new Exception($"Employee with ID {employeeData.EmployeeId} does not exist.");
                    }
                }
            }
        }

        public async Task DeleteEmployeeAsync(int employeeId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Reassign managed employees
                var reassignQuery = @"UPDATE employees SET manager_employee_id = 
                                      (SELECT manager_employee_id FROM employees WHERE employee_id = @id) 
                                      WHERE manager_employee_id = @id";

                using (var command = new NpgsqlCommand(reassignQuery, connection))
                {
                    command.Parameters.AddWithValue("@id", employeeId);
                    await command.ExecuteNonQueryAsync();
                }

                // Delete the employee
                var deleteQuery = @"DELETE FROM employees WHERE employee_id = @id";

                using (var command = new NpgsqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@id", employeeId);
                    var rowsAffected = await command.ExecuteNonQueryAsync();

                    if (rowsAffected == 0)
                    {
                        throw new Exception($"Employee with ID {employeeId} does not exist.");
                    }
                }
            }
        }
    }
}
