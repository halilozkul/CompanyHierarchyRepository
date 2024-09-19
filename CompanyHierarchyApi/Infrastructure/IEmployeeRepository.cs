using System.Collections.Generic;
using System.Threading.Tasks;
using CompanyHierarchyApi.Domain;

namespace CompanyHierarchyApi.Infrastructure
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task<IEnumerable<Employee>> GetTopLevelManagersAsync();
        Task AddOrUpdateEmployeeAsync(EmployeeData employeeData);
        Task DeleteEmployeeAsync(int employeeId);
    }
}
