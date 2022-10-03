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
        { "E0005", new ErrorModel() { Message = "roomTypeId for CREATE room not found", Type = "room-error-0005" } },
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

        //center
        { "E1001", new ErrorModel() { Message = "Center Not Found", Type = "center-error-0001" } },
        { "E1002", new ErrorModel() { Message = "Input cannot be null or white space", Type = "center-error-0002" } },
        { "E1003", new ErrorModel() { Message = "Center name not match with name format", Type = "center-error-0003" } },
        { "E1004", new ErrorModel() { Message = "Center name must be less or equal than 100 characters", Type = "center-error-0004" } },
        { "E1005", new ErrorModel() { Message = "This center already exists", Type = "center-error-0005" } },
        
        // sro
        { "E0016", new ErrorModel() { Message = "centerId for GET SROs not found", Type = "sro-error-0001" } },
        { "E0017", new ErrorModel() { Message = "SRO not found", Type = "sro-error-0002" } },

    };
}

public class ErrorModel
{
    public string Type { get; set; }
    public string Message { get; set; }
}