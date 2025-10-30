namespace EmployeeManagementAPI.DTOs
{
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = null!;
        public string Department { get; set; } = null!;
        public decimal Salary { get; set; }
        public string? Email { get; set; }
    }
}