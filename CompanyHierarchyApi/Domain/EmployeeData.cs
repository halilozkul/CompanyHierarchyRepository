namespace CompanyHierarchyApi.Domain
{
    public class EmployeeData
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Title { get; set; }
        public int? ManagerEmployeeId { get; set; }
    }
}
