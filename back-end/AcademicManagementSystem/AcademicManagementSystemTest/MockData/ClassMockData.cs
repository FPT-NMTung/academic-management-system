using AcademicManagementSystem.Context.AmsModels;

namespace AcademicManagementSystemTest.MockData;

public static class ClassMockData
{
    public static readonly List<Class> Classes = new List<Class>()
    {
        new Class()
        {
            Id = 1,
            SroId = 3,
            CenterId = 1,
            CourseFamilyCode = "COURSE FAMILY CODE 1",
            ClassDaysId = 1,
            ClassStatusId = 2,
            Name = "Class 1",
            StartDate = new DateTime(2022, 01, 01),
            CompletionDate = new DateTime(2023, 01, 01),
            GraduationDate = new DateTime(2022, 01, 05),
            ClassHourStart = new TimeSpan(8,0,0),
            ClassHourEnd = new TimeSpan(12,0,0),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        },
        new Class()
        {
            Id = 2,
            SroId = 3,
            CenterId = 1,
            CourseFamilyCode = "COURSE FAMILY CODE 1",
            ClassDaysId = 1,
            ClassStatusId = 2,
            Name = "Class 2",
            StartDate = new DateTime(2022, 01, 01),
            CompletionDate = new DateTime(2023, 01, 01),
            GraduationDate = new DateTime(2022, 01, 05),
            ClassHourStart = new TimeSpan(8,0,0),
            ClassHourEnd = new TimeSpan(12,0,0),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        },
        new Class()
        {
            Id = 3,
            SroId = 3,
            CenterId = 1,
            CourseFamilyCode = "COURSE FAMILY CODE 1",
            ClassDaysId = 1,
            ClassStatusId = 2,
            Name = "Class 3",
            StartDate = new DateTime(2022, 01, 01),
            CompletionDate = new DateTime(2023, 01, 01),
            GraduationDate = new DateTime(2022, 01, 05),
            ClassHourStart = new TimeSpan(8,0,0),
            ClassHourEnd = new TimeSpan(12,0,0),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        },
        new Class()
        {
            Id = 4,
            SroId = 3,
            CenterId = 2,
            CourseFamilyCode = "COURSE FAMILY CODE 1",
            ClassDaysId = 1,
            ClassStatusId = 2,
            Name = "Class 4 center 2",
            StartDate = new DateTime(2022, 01, 01),
            CompletionDate = new DateTime(2023, 01, 01),
            GraduationDate = new DateTime(2022, 01, 05),
            ClassHourStart = new TimeSpan(8,0,0),
            ClassHourEnd = new TimeSpan(12,0,0),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        },
        new Class()
        {
            Id = 100,
            SroId = 3,
            CenterId = 1,
            CourseFamilyCode = "COURSE FAMILY CODE 1",
            ClassDaysId = 1,
            ClassStatusId = 5,
            Name = "Class 100 center 1",
            StartDate = new DateTime(2022, 01, 01),
            CompletionDate = new DateTime(2023, 01, 01),
            GraduationDate = new DateTime(2022, 01, 05),
            ClassHourStart = new TimeSpan(8,0,0),
            ClassHourEnd = new TimeSpan(12,0,0),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        },
    };
}