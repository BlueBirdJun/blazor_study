namespace blazor_study.Server.Authentication;

public interface IUserAccountService
{
    
}

public class UserAccountService
{
    private List<UserAccount> _userAccountList;
    public UserAccountService()
    {
        _userAccountList = new List<UserAccount>() {
            new UserAccount{UserName="admin",Password="admin",Role="Adminstrator"},
            new UserAccount{UserName="user",Password="user",Role="User"}
        };
    }

    public UserAccount? GetUserAccountByUserName(string userName)
    {
        return _userAccountList.FirstOrDefault(x => x.UserName == userName);
    }

}
