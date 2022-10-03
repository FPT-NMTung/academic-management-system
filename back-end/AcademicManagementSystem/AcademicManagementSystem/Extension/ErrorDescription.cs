namespace AcademicManagementSystem.Extension;

public static class ErrorDescription
{
    public static readonly Dictionary<string, ErrorModel> Error = new Dictionary<string, ErrorModel>()
    {
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

        //center
        { "E1001", new ErrorModel() { Message = "This center already exists", Type = "center-error-0001" } },
        { "E1002", new ErrorModel() { Message = "Center name is not match with format", Type = "center-error-0002" } },
        { "E1003", new ErrorModel() { Message = "Center not found", Type = "center-error-0004" } },
        { "E1004", new ErrorModel() { Message = "Center Address not found", Type = "center-error-0005" } },
        { "E1005", new ErrorModel() { Message = "There is no center", Type = "center-error-0006" } },
    };
}

public class ErrorModel
{
    public string Type { get; set; }
    public string Message { get; set; }
}