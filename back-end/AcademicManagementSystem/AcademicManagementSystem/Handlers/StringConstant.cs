namespace AcademicManagementSystem.Handlers;

public struct StringConstant
{
    //Any spaces and tabs
    public const string RegexWhiteSpaces = @"\s+";

    // Digit
    public const string RegexDigits = @"\d+";

    /*
     * Accept characters with tone marks, any spaces, tabs
     * Accept Special characters: hyphens/dash(-), and underscore(_) but not at the beginning or end of the string
     * Special characters must NOT be adjacent (--, __, -_, _-)
     * The string must be at least 2 characters long.
     */
    public const string RegexVietNameseNameWithDashUnderscoreSpaces =
        @"^[a-zA-Z0-9ÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂẾưăạảấầẩẫậắằẳẵặẹẻẽềềểếệỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹỹý \t](?!.*--)(?!.*__)(?!.*-_)(?!.*_-)[a-zA-Z0-9ÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂẾưăạảấầẩẫậắằẳẵặẹẻẽềềểếệỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹỹý_ \t-]*[a-zA-Z0-9ÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂẾưăạảấầẩẫậắằẳẵặẹẻẽềềểếệỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹỹý \t]$";

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

    // regex email
    public const string RegexEmailCopilot =
        @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@" + @"((([0-1]?[0-9]{1,2}\.){3}[0-1]?[0-9]{1,2})|" +
        @"(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,4}))$";

    // regex phone number
    public const string RegexPhoneNumber =
        @"^((\+84|84|0)3[2-9]\d{7})|((\+84|84|0)9[01]\d{7})|((\+84|84|0)7[0|6|7|8|9]\d{7})|((\+84|84|0)8[1-9]\d{7})$";

    // regex special character
    public const string RegexSpecialCharacter = @"[~`!@#$%^&*()+=|\\{}':;.,<>/?[\]""_-]";

    public const string RegexSpecialCharacterWithDashUnderscoreSpaces = @"[~`!@#$%^&*()+=|\\{}':;.,<>/?[\]""]";

    /*
    * Accept characters with tone marks, any spaces, tabs
    * Accept Special characters: apostrophe/single quote(') but not at the beginning or end of the string.
    * Special characters must NOT be adjacent (--)
    * The string must be at least 2 characters long.
    */
    public const string RegexToneMarkUserNameWithApostrophe =
        @"^[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂẾưăạảấầẩẫậắằẳẵặẹẻẽềềểếệỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹỹý \t](?!.*'')[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂẾưăạảấầẩẫậắằẳẵặẹẻẽềềểếệỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹỹý' \t]*[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂẾưăạảấầẩẫậắằẳẵặẹẻẽềềểếệỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹỹý \t]$";

    public const string RegexSpecialCharsNotAllowForPersonName = @"[~`!@#$%^&*()_+=\[{\]};:<>|.""\\/?,-]"; // allow: ' 

    public const string RegexSpecialCharactersNotAllowForClassName = @"[~`!@#$%^&*+=\[{\]};:<>|.""\\/?,]"; // allow: ' -()_

    public const string RegexSpecialCharactersNotAllowForRoomName = RegexSpecialCharactersNotAllowForClassName; // allow: ' -()_

    public const string RegexSpecialCharacterNotAllowForModuleName = @"[~`!@$%^*+=|\\{}':;<>/?[\]]";

    public const string RegexSpecialCharacterNotAllowForSkillName = @"[~`!@$%^&*+=\[{\]};:<>|""\\/?,]"; // allow: ' -()_ . # 
    
    public const string RegexSpecialCharacterForAddress = @"[~!@#$%^&*+=|\\{}':.;<>/?[\]""]";
    
    public const string RegexSpecialCharacterForSchool = @"[~`!@#$%^*()+=|\\{}':;.,<>/?[\]""_]";

    // all special characters 
    public const string RegexSpecialCharacters = @"[~`!@#$%^&*()-_+=""\[{\]};:<>|.'\\/?,";

    // 10 digits starting with 0
    public const string RegexMobilePhone = @"^0[0-9]{9}$";

    // Match all mobile network operators in Vietnam
    public const string RegexVietNamMobilePhone =
        @"^(0|\+84|84)(3[2-9]|5[2|5|6|8|9]|7[0|6-9]|8[1-9]|9[0-4|6-9])[0-9]{7}$";

    // 10 digits
    public const string RegexTenDigits = @"^[0-9]{10}$";

    // 9 or 12 digits
    public const string RegexCitizenIdCardNo = @"^(\d{9}|\d{12})$";

}