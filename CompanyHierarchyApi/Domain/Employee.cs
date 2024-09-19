namespace CompanyHierarchyApi.Domain
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Title { get; set; }
        public List<Employee> ManagedEmployees { get; set; } = new List<Employee>();
    }
}
