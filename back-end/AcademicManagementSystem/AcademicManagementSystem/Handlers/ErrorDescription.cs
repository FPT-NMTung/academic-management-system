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
        { "E1005", new ErrorModel() { Message = "This center already exists", Type = "center-error-0005" } },
        
        // sro
        { "E0016", new ErrorModel() { Message = "centerId for GET SROs not found", Type = "sro-error-0001" } },
        { "E0017", new ErrorModel() { Message = "SRO not found", Type = "sro-error-0002" } },
        { "E0018", new ErrorModel() { Message = "centerId for CREATE/UPDATE SRO not found", Type = "sro-error-0003" } },
        { "E0019", new ErrorModel() { Message = "RoleId for CREATE/UPDATE SRO not found", Type = "sro-error-0004" } },
        { "E0020", new ErrorModel() { Message = "Duplicated MobilePhone for CREATE/UPDATE SRO", Type = "sro-error-0005" } },
        { "E0021", new ErrorModel() { Message = "Duplicated Email for CREATE/UPDATE SRO ", Type = "user-error-0006" } },
        { "E0022", new ErrorModel() { Message = "Duplicated EmailOrganization for CREATE/UPDATE SRO", Type = "sro-error-0007" } },
        { "E0023", new ErrorModel() { Message = "ProvinceId not found for CREATE/UPDATE SRO", Type = "sro-error-0008" } },
        { "E0024", new ErrorModel() { Message = "DistrictId not found for CREATE/UPDATE SRO", Type = "sro-error-0009" } },
        { "E0025", new ErrorModel() { Message = "WardId not found for CREATE/UPDATE SRO", Type = "sro-error-0010" } },
        { "E0026", new ErrorModel() { Message = "GenderId not found for CREATE/UPDATE SRO", Type = "sro-error-0011" } },
        { "E0027", new ErrorModel() { Message = "Duplicated citizenIdentityCardNo for CREATE/UPDATE SRO", Type = "sro-error-0012" } },
        { "E0028", new ErrorModel() { Message = "Birthday must match format yyyy/MM/dd for CREATE/UPDATE SRO", Type = "sro-error-0013" } },
        { "E0029", new ErrorModel() { Message = "CitizenIdentityCardPublishedDate must match format yyyy/MM/dd for CREATE/UPDATE SRO", Type = "sro-error-0014" } },
      
        { "E0030", new ErrorModel() { Message = "MobilePhone 10 digits and starting with 0", Type = "sro-error-0015" } },
        { "E0031", new ErrorModel() { Message = "Email must match with format", Type = "sro-error-0016" } },
        { "E0032", new ErrorModel() { Message = "EmailOrganization must match with format", Type = "sro-error-0017" } },
        { "E0033", new ErrorModel() { Message = "CitizenIdCardNo must be 9 or 12 digits", Type = "sro-error-0018" } },
        { "E0034", new ErrorModel() { Message = "FirstName must match with format", Type = "sro-error-0019" } },
        { "E0035", new ErrorModel() { Message = "LastName must match with format", Type = "sro-error-0020" } },
        { "E0036", new ErrorModel() { Message = "User role SRO Not found", Type = "sro-error-0021" } },
        { "E0037", new ErrorModel() { Message = "Invalid Data", Type = "sro-error-0022" } },

        
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
        
        // teacher
        { "E0038", new ErrorModel() { Message = "Invalid Data", Type = "teacher-error-0001" } },

    };
}

public class ErrorModel
{
    public string Type { get; set; }
    public string Message { get; set; }
}