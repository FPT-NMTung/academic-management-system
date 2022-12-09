using AcademicManagementSystem.Context.AmsModels;

namespace AcademicManagementSystemTest.MockData;

public static class ModuleMockData
{
    public static readonly List<Module> Modules = new List<Module>()
    {
        new Module()
        {
            Id = 1,
            CenterId = 1,
            SemesterNamePortal = "semester 1",
            ModuleName = "Module 1",
            ModuleExamNamePortal = "Module Exam Portal 1",
            ModuleType = 1,
            MaxTheoryGrade = null,
            MaxPracticalGrade = null,
            Hours = 20,
            Days = 5,
            ExamType = 4, // not take exam
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        },
        new Module()
        {
            Id = 2,
            CenterId = 1,
            SemesterNamePortal = "semester 1",
            ModuleName = "Module 2",
            ModuleExamNamePortal = "Module Exam Portal 2",
            ModuleType = 2,
            MaxTheoryGrade = null,
            MaxPracticalGrade = 100,
            Hours = 20,
            Days = 5,
            ExamType = 1, // practical exam
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        },
        new Module()
        {
            Id = 3,
            CenterId = 1,
            SemesterNamePortal = "semester 2",
            ModuleName = "Module 3",
            ModuleExamNamePortal = "Module Exam Portal 3",
            ModuleType = 1,
            MaxTheoryGrade = 100,
            MaxPracticalGrade = null,
            Hours = 20,
            Days = 5,
            ExamType = 2, // theory exam
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        },
        new Module()
        {
            Id = 4,
            CenterId = 1,
            SemesterNamePortal = "semester 2",
            ModuleName = "Module 4",
            ModuleExamNamePortal = "Module Exam Portal 4",
            ModuleType = 3,
            MaxTheoryGrade = 100,
            MaxPracticalGrade = 100,
            Hours = 20,
            Days = 5,
            ExamType = 3, // both practice and theory exam
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        },
        new Module()
        {
            Id = 5,
            CenterId = 2,
            SemesterNamePortal = "semester 2",
            ModuleName = "Module 5",
            ModuleExamNamePortal = "Module Exam Portal 5",
            ModuleType = 3,
            MaxTheoryGrade = 100,
            MaxPracticalGrade = 100,
            Hours = 20,
            Days = 5,
            ExamType = 3, // both practice and theory exam
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        },
    };
}