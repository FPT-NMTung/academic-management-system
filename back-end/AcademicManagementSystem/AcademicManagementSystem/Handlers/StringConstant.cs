namespace AcademicManagementSystem.Handlers;

public struct StringConstant
{
    public const string RegexVietNameseName =
        @"^[a-zA-Z0-9ÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂẾưăạảấầẩẫậắằẳẵặẹẻẽềềểếỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹ\s\W|_]+$";

    public const string RegexVietNameseNameWithDashUnderscoreSpaces =
        @"^[a-zA-Z0-9ÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂẾưăạảấầẩẫậắằẳẵặẹẻẽềềểếệỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹỹý \t](?!.*--)(?!.*__)(?!.*-_)(?!.*_-)[a-zA-Z0-9ÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂẾưăạảấầẩẫậắằẳẵặẹẻẽềềểếệỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹỹý_ \t-]*[a-zA-Z0-9ÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂẾưăạảấầẩẫậắằẳẵặẹẻẽềềểếệỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹỹý \t]$";

    public const string RegexWhiteSpaces = @"\s+";
    
    public const string RegexNameWithDashUnderscoreSpaces =
        @"^[a-zA-Z0-9](?!.*--)(?!.*__)(?!.*-_)(?!.*_-)[a-zA-Z0-9_ \t-]*[a-zA-Z0-9 \t]$";

    public const string RegexNameWithDashUnderscore =
        @"^[a-zA-Z0-9](?!.*--)(?!.*__)(?!.*-_)(?!.*_-)[a-zA-Z0-9_ \t-]*[a-zA-Z0-9]$";

    public const string RegexNameWithUnderscoreSpaces =
        @"^[a-zA-Z0-9](?!.*__)(?!.*_ )[a-zA-Z0-9_ \t]*[a-zA-Z0-9 \t]$";

    public const string RegexNameWithDashSpaces =
        @"^[a-zA-Z0-9](?!.*--)(?!.* -)[a-zA-Z0-9_ \t-]*[a-zA-Z0-9 \t]$";

    public const string RegexNameWithUnderscore =
        @"^[a-zA-Z0-9](?!.*__)[a-zA-Z0-9_]*[a-zA-Z0-9]$";

    public const string RegexNameWithDash =
        @"^[a-zA-Z0-9](?!.*--)[a-zA-Z0-9_ \t-]*[a-zA-Z0-9]$";

    public const string RegexNameWithSpaces =
        @"^[a-zA-Z0-9][a-zA-Z0-9_ \t]*[a-zA-Z0-9 \t]$";

    public const string RegexName =
        @"^[a-zA-Z0-9][a-zA-Z0-9_]*[a-zA-Z0-9]$";

    // regex phone number
    public const string RegexPhoneNumber =
        @"^((\+84|84|0)3[2-9]\d{7})|((\+84|84|0)9[01]\d{7})|((\+84|84|0)7[0|6|7|8|9]\d{7})|((\+84|84|0)8[1-9]\d{7})$";

    // regex email
    public const string RegexEmail =
        @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
    
    // regex special character
    public const string RegexSpecialCharacter = @"[~`!@#$%^&*()+=|\\{}':;.,<>/?[\]""_-]";
    
    public const string RegexSpecialCharacterWithDashUnderscoreSpaces = @"[~`!@#$%^&*()+=|\\{}':;.,<>/?[\]""]";
}