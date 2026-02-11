namespace Settlr.Common.Messages;

public static class Messages
{
    // Auth Messages
    public const string RegistrationSuccessful = "User registered successfully";
    public const string LoginSuccessful = "Login successful";
    public const string InvalidCredentials = "Invalid email or password";
    public const string UserAlreadyExists = "User with this email already exists";
    public const string UserNotFound = "User not found";
    
    // Group Messages
    public const string GroupCreatedSuccessfully = "Group created successfully";
    public const string GroupNotFound = "Group not found";
    public const string MemberAddedSuccessfully = "Member added to group successfully";
    public const string MemberAlreadyExists = "User is already a member of this group";
    public const string UserNotMemberOfGroup = "User is not a member of this group";
    
    // Expense Messages
    public const string ExpenseCreatedSuccessfully = "Expense created successfully";
    public const string ExpenseNotFound = "Expense not found";
    
    // General Messages
    public const string UnauthorizedAccess = "Unauthorized access";
    public const string InternalServerError = "An error occurred while processing your request";
    public const string ValidationFailed = "Validation failed";
}
