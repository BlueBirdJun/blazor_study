namespace blazor_study.Shared
{
    public class UserSession
    {
        public UserSession() { }
        public string UserName { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public int ExpiresIn { get; set; }
        public DateTime ExpriyTimeStamp { get; set; }
    }
}
