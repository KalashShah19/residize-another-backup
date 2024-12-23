using Repository.Models;

namespace Repository.Interfaces;
public interface IAccountRepository
{
    public bool DoesEmailExist(string email);
    public bool DoesTokenExist(string email, string token);
    public User.Get? Login(User.Login credentials);
    public User.Get? GetUserDetails(string emailaddress);
    public void Register(User.Post userdetails);
    public string SendVerificationToken(string email);
    public void VerifyUser(string email);
    public string GetUserName(string email);

    // below anvi code 
    public User.GetUpdateProfile? GetProfile(int id);
    public void UpdateProfile(User.GetUpdateProfile User);
    public bool ChangePassword(User.ChangePassword User);
    public void Forgotpassword(User.Forgot Useremail);
    public bool Resetpassword(User.Resetpassword Userpassword);

    public User.GetContectInfo GetContectInfoProject(int project_id);
    public User.GetContectInfo GetContectInfo(int propertie_id);

}
