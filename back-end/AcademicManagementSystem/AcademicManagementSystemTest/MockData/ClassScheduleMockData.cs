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
                ClassHourStart = new TimeSpan(8,0,0),
                ClassHourEnd = new TimeSpan(12,0,0),
                Note = null,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                WorkingTimeId = 1
            }
    };
}