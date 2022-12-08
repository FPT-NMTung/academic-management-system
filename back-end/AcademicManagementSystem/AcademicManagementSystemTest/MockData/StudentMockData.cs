using AcademicManagementSystem.Context.AmsModels;

namespace AcademicManagementSystemTest.MockData;

public static class StudentMockData
{
    public static readonly List<Student> Students = new List<Student>
    {
        new Student // ACTIVE
        {
            UserId = 6,
            EnrollNumber = "enrollNo 1",
            Status = 1,
            StatusDate = DateTime.Today.AddYears(-1),
            HomePhone = "0981498620",
            ContactPhone = "0981498620",
            ParentalName = "Pa",
            ParentalRelationship = "Father",
            ContactAddress = "768 ABC",
            ParentalPhone = "0981498620",
            ApplicationDate = DateTime.Today.AddYears(-1),
            ApplicationDocument = null,
            HighSchool = null,
            University = null,
            FacebookUrl = null,
            PortfolioUrl = null,
            WorkingCompany = null,
            CompanySalary = null,
            CompanyPosition = null,
            CompanyAddress = null,
            FeePlan = 0,
            Promotion = 0,
            IsDraft = false,
            CourseCode = "COURSE CODE 1"
        },
        new Student // NOT ACTIVE
        {
            UserId = 7,
            EnrollNumber = "enrollNo 2",
            Status = 1,
            StatusDate = DateTime.Today.AddYears(-1),
            HomePhone = "0981498620",
            ContactPhone = "0981498620",
            ParentalName = "Pa",
            ParentalRelationship = "Father",
            ContactAddress = "768 ABC",
            ParentalPhone = "0981498620",
            ApplicationDate = DateTime.Today.AddYears(-1),
            ApplicationDocument = null,
            HighSchool = null,
            University = null,
            FacebookUrl = null,
            PortfolioUrl = null,
            WorkingCompany = null,
            CompanySalary = null,
            CompanyPosition = null,
            CompanyAddress = null,
            FeePlan = 0,
            Promotion = 0,
            IsDraft = false,
            CourseCode = "COURSE CODE 1"
        },
        new Student // ACTIVE
        {
            UserId = 8,
            EnrollNumber = "enrollNo 3",
            Status = 1,
            StatusDate = DateTime.Today.AddYears(-1),
            HomePhone = "0981498620",
            ContactPhone = "0981498620",
            ParentalName = "Pa",
            ParentalRelationship = "Father",
            ContactAddress = "768 ABC",
            ParentalPhone = "0981498620",
            ApplicationDate = DateTime.Today.AddYears(-1),
            ApplicationDocument = null,
            HighSchool = null,
            University = null,
            FacebookUrl = null,
            PortfolioUrl = null,
            WorkingCompany = null,
            CompanySalary = null,
            CompanyPosition = null,
            CompanyAddress = null,
            FeePlan = 0,
            Promotion = 0,
            IsDraft = false,
            CourseCode = "COURSE CODE 1"
        },
    };
}