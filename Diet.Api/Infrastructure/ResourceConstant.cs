namespace Diet.Api.Infrastructure
{
    /// <summary>
    /// ToDo : Get more expressive messages from product team
    /// </summary>
    public static class ResourceConstant
    {
        public static string Unauthorized = "Invalid email or password";
        public static string TimeFormat = "HH:mm";
        public static string InvalidTime = "Time format is invalid. Please provide a valid(HH:mm) format";
        public static string InvalidGuid = "Invalid Guid expression";
        public static string AlreadyUsedEmail = "Provided email address is already used. Please update it.";
        public static string NotFound = "Requested item not found";
        public static string Forbidden = "You are not authorized to do this action";
    }
}