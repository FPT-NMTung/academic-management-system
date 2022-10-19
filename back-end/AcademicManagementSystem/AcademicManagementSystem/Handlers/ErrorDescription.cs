namespace AcademicManagementSystem.Handlers;

public static class ErrorDescription
{
    public static readonly Dictionary<string, ErrorModel> Error = new Dictionary<string, ErrorModel>()
    {
        // room
        { "E0001", new ErrorModel() { Message = "centerId for GET rooms not found", Type = "room-error-0001" } },
        { "E0002", new ErrorModel() { Message = "Not found any room in this center", Type = "room-error-0002" } },
        { "E0003", new ErrorModel() { Message = "This room already exists for CREATE", Type = "room-error-0003" } },
        { "E0004", new ErrorModel() { Message = "centerId for CREATE room not found", Type = "room-error-0004" } },
        { "E0005", new ErrorModel() { Message = "Invalid Data for CREATE room ", Type = "room-error-0005" } },
        { "E0006", new ErrorModel() { Message = "Capacity for CREATE room must be between 20 and 100", Type = "room-error-0006" } },
        { "E0007", new ErrorModel() { Message = "Name for CREATE room cannot be empty", Type = "room-error-0007" } },
        { "E0008", new ErrorModel() { Message = "Name for CREATE room must match with format", Type = "room-error-0008" } },
        { "E0009", new ErrorModel() { Message = "roomId for UPDATE not found", Type = "room-error-0009" } },
        { "E0010", new ErrorModel() { Message = "Name for UPDATE room cannot be empty", Type = "room-error-0010" } },
        { "E0011", new ErrorModel() { Message = "Name for UPDATE room must match with format", Type = "room-error-0011" } },
        { "E0012", new ErrorModel() { Message = "centerId for UPDATE room not found", Type = "room-error-0012" } },
        { "E0013", new ErrorModel() { Message = "roomTypeId for UPDATE room not found", Type = "room-error-0013" } },
        { "E0014", new ErrorModel() { Message = "Capacity for UPDATE room must be between 20 and 100", Type = "room-error-0014" } },
        { "E0015", new ErrorModel() { Message = "This room already exists for UPDATE", Type = "room-error-0015" } },

        // center
        { "E1001", new ErrorModel() { Message = "Center Not Found", Type = "center-error-0001" } },
        { "E1002", new ErrorModel() { Message = "Input cannot be null or white space", Type = "center-error-0002" } },
        { "E1003", new ErrorModel() { Message = "Center name not match with name format", Type = "center-error-0003" } },
        { "E1004", new ErrorModel() { Message = "Center name must be less or equal than 100 characters", Type = "center-error-0004" } },
        { "E1005", new ErrorModel() { Message = "Center with this address already exists", Type = "center-error-0005" } },
        { "E1047", new ErrorModel() { Message = "Center with this name already exists", Type = "center-error-0006" } },
        { "E1048", new ErrorModel() { Message = "Different Center with this name already exists", Type = "center-error-0007" } },
        { "E1049", new ErrorModel() { Message = "Different Center with this address already exists", Type = "center-error-0008" } },
        
        // sro
        { "E0016", new ErrorModel() { Message = "centerId for GET SROs not found", Type = "sro-error-0001" } },
        { "E0017", new ErrorModel() { Message = "SRO not found", Type = "sro-error-0002" } },
        { "E0018", new ErrorModel() { Message = "centerId for CREATE/UPDATE SRO not found", Type = "sro-error-0003" } },
        { "E0019", new ErrorModel() { Message = "RoleId for CREATE/UPDATE SRO not found", Type = "sro-error-0004" } },
        { "E0020", new ErrorModel() { Message = "Duplicated MobilePhone for CREATE/UPDATE SRO", Type = "sro-error-0005" } },
        { "E0021", new ErrorModel() { Message = "Duplicated Email for CREATE/UPDATE SRO ", Type = "sro-error-0006" } },
        { "E0022", new ErrorModel() { Message = "Duplicated EmailOrganization for CREATE/UPDATE SRO", Type = "sro-error-0007" } },
        { "E0023", new ErrorModel() { Message = "ProvinceId not found for CREATE/UPDATE SRO", Type = "sro-error-0008" } },
        { "E0024", new ErrorModel() { Message = "DistrictId not found for CREATE/UPDATE SRO", Type = "sro-error-0009" } },
        { "E0025", new ErrorModel() { Message = "WardId not found for CREATE/UPDATE SRO", Type = "sro-error-0010" } },
        { "E0026", new ErrorModel() { Message = "GenderId not found for CREATE/UPDATE SRO", Type = "sro-error-0011" } },
        { "E0027", new ErrorModel() { Message = "Duplicated citizenIdentityCardNo for CREATE/UPDATE SRO", Type = "sro-error-0012" } },
        { "E0028", new ErrorModel() { Message = "Please enter at least one search criteria", Type = "sro-error-0013" } },
        { "E0029", new ErrorModel() { Message = "CitizenIdentityCardPublishedDate must match format yyyy/MM/dd for CREATE/UPDATE SRO", Type = "sro-error-0014" } },
      
        { "E0030", new ErrorModel() { Message = "MobilePhone 10 digits and starting with 0", Type = "sro-error-0015" } },
        { "E0031", new ErrorModel() { Message = "Email must match with format", Type = "sro-error-0016" } },
        { "E0032", new ErrorModel() { Message = "EmailOrganization must match with format", Type = "sro-error-0017" } },
        { "E0033", new ErrorModel() { Message = "CitizenIdCardNo must be 9 or 12 digits", Type = "sro-error-0018" } },
        { "E0034", new ErrorModel() { Message = "FirstName must match with format", Type = "sro-error-0019" } },
        { "E0035", new ErrorModel() { Message = "LastName must match with format", Type = "sro-error-0020" } },
        { "E0036", new ErrorModel() { Message = "User role SRO Not found", Type = "sro-error-0021" } },
        { "E0037", new ErrorModel() { Message = "Invalid Data for create/update user", Type = "sro-error-0022" } },
        { "E0038", new ErrorModel() { Message = "Invalid Data for create/update sro", Type = "sro-error-0023" } },

        // address
        { "E1006", new ErrorModel() { Message = "Do not exist provinceId, DistrictId or WardId", Type = "address-error-0001" } },
        
        // course family
        { "E1007", new ErrorModel() { Message = "Input cannot be null or white space", Type = "course-error-0001" } },
        { "E1008", new ErrorModel() { Message = "Course family name not match with name format", Type = "course-error-0002" } },
        { "E1009", new ErrorModel() { Message = "Course family name must be less or equal than 255 characters", Type = "course-error-0003" } },
        { "E1010", new ErrorModel() { Message = "Course family code not match with format", Type = "course-error-0005" } },
        { "E1011", new ErrorModel() { Message = "Course family code must be less or equal than 100 characters", Type = "course-error-0006" } },
        { "E1012", new ErrorModel() { Message = "Course family published year must be larger than 0", Type = "course-error-0007" } },
        { "E1013", new ErrorModel() { Message = "Course family code existed", Type = "course-error-0008" } },
        
        // course
        { "E1014", new ErrorModel() { Message = "Course code existed", Type = "course-error-0009" } },
        { "E1015", new ErrorModel() { Message = "Course family not found", Type = "course-error-0010" } },
        { "E1016", new ErrorModel() { Message = "Course name not match with format", Type = "course-error-0011" } },
        { "E1017", new ErrorModel() { Message = "Course name must be less or equal than 255 characters", Type = "course-error-0012" } },
        { "E1018", new ErrorModel() { Message = "Course code not match with format", Type = "course-error-0013" } },
        { "E1019", new ErrorModel() { Message = "Course code must be less or equal than 100 characters", Type = "course-error-0014" } },
        { "E1020", new ErrorModel() { Message = "Semester count mus be between 1-10", Type = "course-error-0015" } },
        
        // module
        { "E1021", new ErrorModel() { Message = "Fail to SaveChange when Add Module", Type = "module-error-0001" } },
        { "E1022", new ErrorModel() { Message = "Fail to SaveChange when Add data to CourseModuleSemester", Type = "module-error-0002" } },
        { "E1023", new ErrorModel() { Message = "Input cannot be null or white space", Type = "module-error-0003" } },
        { "E1024", new ErrorModel() { Message = "Module name not match with format", Type = "module-error-0004" } },
        { "E1025", new ErrorModel() { Message = "Module name must be less or equal than 255 characters", Type = "module-error-0005" } },
        { "E1026", new ErrorModel() { Message = "Course code not match with format", Type = "module-error-0006" } },
        { "E1027", new ErrorModel() { Message = "Course code must be less or equal than 100 characters", Type = "module-error-0007" } },
        { "E1028", new ErrorModel() { Message = "ModuleExamNamePortal not match with format", Type = "module-error-0008" } },
        { "E1029", new ErrorModel() { Message = "ModuleExamNamePortal be less or equal than 255 characters", Type = "module-error-0009" } },
        { "E1031", new ErrorModel() { Message = "ModuleType must be between 1-3", Type = "module-error-00011" } },
        { "E1033", new ErrorModel() { Message = "ExamType must be between 1-4", Type = "module-error-00013" } },
        { "E1034", new ErrorModel() { Message = "SemesterNamePortal code not match with format", Type = "module-error-00014" } },
        { "E1035", new ErrorModel() { Message = "SemesterNamePortal code must be less or equal than 255 characters", Type = "module-error-00015" } },
        { "E1036", new ErrorModel() { Message = "Days must larger than 0", Type = "module-error-00016" } },
        { "E1037", new ErrorModel() { Message = "Hours must larger than 0", Type = "module-error-00017" } },
        { "E1038", new ErrorModel() { Message = "Course Code is not existed", Type = "module-error-00018" } },
        { "E1039", new ErrorModel() { Message = "Center Id is not existed", Type = "module-error-00019" } },
        { "E1040", new ErrorModel() { Message = "Semester Id is not existed", Type = "module-error-00020" } },
        { "E1041", new ErrorModel() { Message = "Module with course code, semester id existed", Type = "module-error-00021" } },
        { "E1042", new ErrorModel() { Message = "Fail to SaveChange when Update Module", Type = "module-error-00022" } },
        { "E1043", new ErrorModel() { Message = "Max Grade must be ignore or greater than 0", Type = "module-error-00023" } },
        { "E1044", new ErrorModel() { Message = "Update module success but cannot get response", Type = "module-error-00024" } },
        { "E1045", new ErrorModel() { Message = "Create module success but cannot get response", Type = "module-error-00025" } },
        { "E1046", new ErrorModel() { Message = "Set null for max grade by exam type fail", Type = "module-error-00026" } },

        // teacher
        { "E0039", new ErrorModel() { Message = "Please enter at least one search criteria", Type = "teacher-error-0002" } },
        { "E0040", new ErrorModel() { Message = "Invalid Data for Create/Update User", Type = "teacher-error-0003" } },
        { "E0041", new ErrorModel() { Message = "Duplicated Individual Tax Code", Type = "teacher-error-0004" } },
        { "E0042", new ErrorModel() { Message = "MobilePhone must be 10 digits and starting with 0", Type = "teacher-error-0005" } },
        { "E0043", new ErrorModel() { Message = "Email must match with format", Type = "teacher-error-0006" } },
        { "E0044", new ErrorModel() { Message = "EmailOrganization must match with format", Type = "teacher-error-0007" } },
        { "E0045", new ErrorModel() { Message = "CitizenIdCardNo must be 9 or 12 digits", Type = "teacher-error-0008" } },
        { "E0046", new ErrorModel() { Message = "FirstName must match with format", Type = "teacher-error-0009" } },
        { "E0047", new ErrorModel() { Message = "LastName must match with format", Type = "teacher-error-0010" } },
        { "E0048", new ErrorModel() { Message = "User role Teacher Not found", Type = "teacher-error-0011" } },
        { "E0049", new ErrorModel() { Message = "Invalid Data for Create/Update Teacher", Type = "teacher-error-0012" } },
        { "E0050", new ErrorModel() { Message = "Duplicated MobilePhone for CREATE/UPDATE Teacher", Type = "teacher-error-0013" } },
        { "E0051", new ErrorModel() { Message = "Duplicated Email for CREATE/UPDATE Teacher ", Type = "teacher-error-0014" } },
        { "E0052", new ErrorModel() { Message = "Duplicated EmailOrganization for CREATE/UPDATE Teacher", Type = "teacher-error-0015" } },
        { "E0053", new ErrorModel() { Message = "Duplicated citizenIdentityCardNo for CREATE/UPDATE Teacher", Type = "teacher-error-0016" } },
        { "E0054", new ErrorModel() { Message = "Tax Code must be 10 digits", Type = "teacher-error-0017" } },
        { "E0055", new ErrorModel() { Message = "Teacher Not Found", Type = "teacher-error-0018" } },

        //grade
        { "E0056", new ErrorModel() { Message = "Module Not found", Type = "grade-error-0001" } },
        { "E0057", new ErrorModel() { Message = "Total Weight must be 100 for all grade category", Type = "grade-error-0002" } },
        { "E0058", new ErrorModel() { Message = "Invalid data for create grade category details", Type = "grade-error-0003" } },
        { "E0059", new ErrorModel() { Message = "Module Not found", Type = "grade-error-0004" } },
        { "E0060", new ErrorModel() { Message = "Exam must have only 1 item", Type = "grade-error-0005" } },
        { "E0061", new ErrorModel() { Message = "QuantityGradeItem out of range [1-10]", Type = "grade-error-0006" } },
        { "E0062", new ErrorModel() { Message = "Module Not found", Type = "grade-error-0007" } },
        { "E0063", new ErrorModel() { Message = "Can't add PE because Module Exam Type is TE/FE", Type = "grade-error-0008" } },
        { "E0064", new ErrorModel() { Message = "Can't add FE because Module Exam Type is PE", Type = "grade-error-0009" } },
        { "E0065", new ErrorModel() { Message = "This module not take exam", Type = "grade-error-0010" } },
        { "E0066", new ErrorModel() { Message = "Can't add Resit Exam", Type = "grade-error-0011" } },
        { "E0067", new ErrorModel() { Message = "This module must have both PE and FE", Type = "grade-error-0012" } },

    };
}

public class ErrorModel
{
    public string Type { get; set; }
    public string Message { get; set; }
}