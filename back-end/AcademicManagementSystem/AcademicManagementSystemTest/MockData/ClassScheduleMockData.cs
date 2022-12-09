using AcademicManagementSystem.Context.AmsModels;

namespace AcademicManagementSystemTest.MockData;

public static class ClassScheduleMockData
{
    public static readonly List<ClassSchedule> ClassSchedules = new List<ClassSchedule>()
    {
        new ClassSchedule()
        {
            Id = 1,
            ClassId = 1,
            ModuleId = 1,
            TeacherId = 4,
            ClassDaysId = 1,
            ClassStatusId = 1,
            TheoryRoomId = 1,
            LabRoomId = 1,
            ExamRoomId = 1,
            Duration = 5,
            StartDate = new DateTime(2022, 01, 03),
            EndDate = new DateTime(2022, 01, 12),
            TheoryExamDate = null,
            PracticalExamDate = null,
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            WorkingTimeId = 1
        },
        new ClassSchedule()
        {
            Id = 2,
            ClassId = 2,
            ModuleId = 1,
            TeacherId = 4,
            ClassDaysId = 1,
            ClassStatusId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            StartDate = DateTime.Today.AddYears(2).AddDays(-1),
            EndDate = DateTime.Today.AddYears(2).AddDays(15),
            TheoryExamDate = null,
            PracticalExamDate = null,
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = "another class schedule",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            WorkingTimeId = 1
        },
        new ClassSchedule()
        {
            Id = 3,
            ClassId = 2,
            ModuleId = 3,
            TeacherId = 9,
            ClassDaysId = 1,
            ClassStatusId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            StartDate = DateTime.Today.AddYears(3).AddDays(1),
            EndDate = DateTime.Today.AddYears(3).AddDays(15),
            TheoryExamDate = null,
            PracticalExamDate = null,
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = "another class schedule",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            WorkingTimeId = 1
        },
    };
}