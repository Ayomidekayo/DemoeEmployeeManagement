
namespace BaseLibrary.Entities
{
     
      public  class Employee
    {
        public int Id { get; set; }
        public String? Name { get; set; }
        public string? CivilId { get; set; }
        public string? FileNumber { get; set; }
        public string? FullName { get; set; }
        public string? JobName { get; set; }
        public string? Address { get; set; }
        public string? TelephoeNumber { get; set; }
        public string? Phone { get; set; }
        public string? Other { get; set; }
        //Relatonship  many to one may emloyee to 
        public  GeneralDeparment? GeneralDeparment { get; set; }
        public int GeneralDepartmentId { get; set; }
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
        public Branch? Branch { get; set; }
        public int BranchId { get; set; }
        public Town? Town { get; set; }
        public int TownId { get; set; }
    }
}
