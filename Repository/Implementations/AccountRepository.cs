using Microsoft.Extensions.Configuration;
using Npgsql;
using Repository.Interfaces;
using Repository.Libraries;
using Repository.Models;

namespace Repository.Implementations;
public class AccountRepository : IAccountRepository
{
    private readonly NpgsqlConnection connection;
    public AccountRepository(IConfiguration configuration)
    {
        connection = new(configuration.GetConnectionString("POSTGRESQL_CONNECTION_STRING"));
        connection.Open();
    }

    public void Register(User.Post userdetails)
    {
        if (DoesEmailExist(userdetails.EmailAddress)) throw new UserException("This email address is already registered.");
        NpgsqlCommand registerCommand = new("INSERT INTO t_users(c_image_path, c_first_name, c_last_name, c_phone_number ,c_email, c_password) VALUES (@imagepath, @fname, @lname, @contactnumber, @emailaddress, @password)", connection);
        registerCommand.Parameters.AddWithValue("fname", userdetails.FirstName);
        registerCommand.Parameters.AddWithValue("lname", userdetails.LastName);
        registerCommand.Parameters.AddWithValue("contactnumber", userdetails.ContactNumber);
        registerCommand.Parameters.AddWithValue("emailaddress", userdetails.EmailAddress);
        registerCommand.Parameters.AddWithValue("password", BcryptHasher.HashPassword(userdetails.Password));
        registerCommand.Parameters.AddWithValue("imagepath", "userdata/default.jpeg");
        registerCommand.ExecuteNonQuery();
    }

    public bool DoesEmailExist(string email)
    {
        NpgsqlCommand checkEmailCommand = new("SELECT c_email FROM t_users where c_email = @email", connection);
        checkEmailCommand.Parameters.AddWithValue("email", email);
        using NpgsqlDataReader reader = checkEmailCommand.ExecuteReader();
        return reader.HasRows;
    }

    public void VerifyUser(string email)
    {
        using NpgsqlCommand verifyUserCommand = new("UPDATE t_users SET c_is_verified = true WHERE c_email = @email", connection);
        verifyUserCommand.Parameters.AddWithValue("email", email);
        verifyUserCommand.ExecuteNonQuery();

        using NpgsqlCommand deleteTokenCommand = new("DELETE FROM t_tokens WHERE c_email = @email", connection);
        deleteTokenCommand.Parameters.AddWithValue("email", email);
        deleteTokenCommand.ExecuteNonQuery();
    }

    public bool IsUserVerified(string email)
    {
        using NpgsqlCommand checkUserCommand = new("SELECT * FROM t_users WHERE c_email = @email and c_is_verified", connection);
        checkUserCommand.Parameters.AddWithValue("email", email);
        using NpgsqlDataReader reader = checkUserCommand.ExecuteReader();
        return reader.HasRows;
    }

    public string SendVerificationToken(string email)
    {
        string token = Guid.NewGuid().ToString();
        NpgsqlCommand sendTokenCommand = new("INSERT INTO t_tokens VALUES(@email, @token, @expiry_time)", connection);
        sendTokenCommand.Parameters.AddWithValue("email", email);
        sendTokenCommand.Parameters.AddWithValue("token", token);
        sendTokenCommand.Parameters.AddWithValue("expiry_time", DateTime.Now.AddMinutes(12));
        sendTokenCommand.ExecuteNonQuery();
        return token;
    }

    public bool DoesTokenExist(string email, string token)
    {
        using NpgsqlCommand checkTokenCommand = new("SELECT c_expiry_time FROM t_tokens WHERE c_email = @email and c_token = @token", connection);
        checkTokenCommand.Parameters.AddWithValue("email", email);
        checkTokenCommand.Parameters.AddWithValue("token", token);
        using NpgsqlDataReader reader = checkTokenCommand.ExecuteReader();
        if (!reader.HasRows) throw new Exception("Your verification link is invalid");
        if (reader.Read())
        {
            if (reader.GetDateTime(0) <= DateTime.Now) throw new UserException("Your verification link is expired, please generate a new one");
        }
        return reader.HasRows;
    }

    public User.Get? Login(User.Login credentials)
    {
        if (!DoesEmailExist(credentials.EmailAddress)) throw new UserException("You are not registered, please create a new account.");
        if (!IsUserVerified(credentials.EmailAddress)) throw new UserException("Please complete your account verification");
        NpgsqlCommand loginCommand = new("SELECT c_user_id, c_first_name, c_last_name, c_email, c_password, c_phone_number, c_joined_at, c_image_path, c_user_role FROM t_users WHERE c_email = @emailaddress", connection);
        loginCommand.Parameters.AddWithValue("emailaddress", credentials.EmailAddress);
        loginCommand.Parameters.AddWithValue("password", credentials.Password);
        using NpgsqlDataReader reader = loginCommand.ExecuteReader();
        if (reader.Read())
        {
            if (!BcryptHasher.VerifyPassword(credentials.Password, reader.GetString(4))) throw new UserException("Your password is incorrect, please reenter your password.");
            return new User.Get() { Id = reader.GetInt16(0), FirstName = reader.GetString(1), LastName = reader.GetString(2), EmailAddress = reader.GetString(3), ContactNumber = reader.GetString(5), JoiningDate = reader.GetDateTime(6), ImagePath = reader.GetString(7), UserRole = reader.GetString(8) };
        }
        return null;
    }

    public User.Get? GetUserDetails(string emailaddress)
    {
        NpgsqlCommand loginCommand = new("SELECT c_user_id, c_first_name, c_last_name, c_email, c_password, c_phone_number, c_joined_at, c_image_path, c_user_role FROM t_users WHERE c_email = @emailaddress", connection);
        loginCommand.Parameters.AddWithValue("emailaddress", emailaddress);
        using NpgsqlDataReader reader = loginCommand.ExecuteReader();
        if (reader.Read()) return new User.Get() { Id = reader.GetInt16(0), FirstName = reader.GetString(1), LastName = reader.GetString(2), EmailAddress = reader.GetString(3), ContactNumber = reader.GetString(5), JoiningDate = reader.GetDateTime(6), ImagePath = reader.GetString(7), UserRole = reader.GetString(8) };
        return null;
    }

    public User.GetUpdateProfile? GetProfile(int id)
    {
        User.GetUpdateProfile UserProfile = null;
        using (NpgsqlCommand getProfileCommand = new NpgsqlCommand("select * from t_users where c_user_id = @userID", connection))
        {
            getProfileCommand.Parameters.AddWithValue("@userID", id);
            using (NpgsqlDataReader reader = getProfileCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    UserProfile = new User.GetUpdateProfile
                    {
                        Id = reader.GetInt32(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        EmailAddress = reader.GetString(3),
                        ContactNumber = reader.GetString(5),
                        Password = reader.GetString(4),
                        ImagePath = reader.IsDBNull(9) ? "UserData/default.jpeg" : reader.GetString(9)
                    };
                }
            }
        }
        return UserProfile == null ? throw new Exception("Error") : UserProfile;
    }

    public void UpdateProfile(User.GetUpdateProfile User)
    {
        string? imagePath = null;
        if (User.ImageFile != null)
        {
            string imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UserData");
            if (!Directory.Exists(imagesDirectory))
            {
                Directory.CreateDirectory(imagesDirectory);
            }
            string fileName = $"{User.Id}{Path.GetExtension(User.ImageFile.FileName)}";
            //Console.WriteLine("ImagePath is => " + fileName);
            imagePath = Path.Combine("UserData", fileName);
            string fullImagePath = Path.Combine(imagesDirectory, fileName);
            using (var stream = new FileStream(fullImagePath, FileMode.Create))
            {
                User.ImageFile.CopyTo(stream);
            }
        }

        using (var updateProfileCommand = new NpgsqlCommand("UPDATE t_users SET c_first_name = @firstname, c_last_name = @lastname, c_phone_number = @phone, c_email = @email, c_image_path = COALESCE(@imagepath, c_image_path) WHERE c_user_id = @userID;", connection))
        {
            updateProfileCommand.Parameters.AddWithValue("@firstname", User.FirstName);
            updateProfileCommand.Parameters.AddWithValue("@lastname", User.LastName);
            updateProfileCommand.Parameters.AddWithValue("@phone", User.ContactNumber);
            updateProfileCommand.Parameters.AddWithValue("@email", User.EmailAddress);
            updateProfileCommand.Parameters.AddWithValue("@imagepath", (object?)imagePath ?? DBNull.Value);
            updateProfileCommand.Parameters.AddWithValue("@userID", User.Id);
            updateProfileCommand.ExecuteNonQuery();
        }
    }

    public bool ChangePassword(User.ChangePassword User)
    {
        bool valid = false;
        using (NpgsqlCommand getProfileCommand = new NpgsqlCommand("select c_password from t_users where c_user_id = @userID", connection))
        {
            getProfileCommand.Parameters.AddWithValue("@userID", User.Id);
            using (NpgsqlDataReader reader = getProfileCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    string hashedPassword = reader.GetString(0);
                    reader.Close();
                    if (BcryptHasher.VerifyPassword(User.OldPassword, hashedPassword))
                    {
                        valid = true;
                        if (valid)
                        {
                            string newHashedPassword = BcryptHasher.HashPassword(User.NewPassword);
                            using (NpgsqlCommand updatePasswordCommand = new NpgsqlCommand("update t_users set c_password = @newPassword where c_user_id = @userID", connection))
                            {
                                updatePasswordCommand.Parameters.AddWithValue("@newPassword", newHashedPassword);
                                updatePasswordCommand.Parameters.AddWithValue("@userID", User.Id);
                                updatePasswordCommand.ExecuteNonQuery();
                            }
                        }
                    }
                    else
                    {
                        valid = false;
                    }
                }
            }
        }
        return valid;
    }

    public void Forgotpassword(User.Forgot Useremail)
    {
        if (!DoesEmailExist(Useremail.EmailAddress))
        {
            throw new UserException("The provided email does not exist.");
        }
    }

    public bool Resetpassword(User.Resetpassword Userpassword)
    {
        try
        {
            using NpgsqlCommand ForgotpasswordCommand = new(
                "UPDATE t_users SET c_password = @newPassword WHERE c_email=@email",
                connection
            );

            ForgotpasswordCommand.Parameters.AddWithValue("@newPassword", BcryptHasher.HashPassword(Userpassword.NewPassword));
            ForgotpasswordCommand.Parameters.AddWithValue("@email", Userpassword.EmailAddress);

            int updatepassword = ForgotpasswordCommand.ExecuteNonQuery();
            return updatepassword > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding user: {ex.Message}");
            return false;
        }
    }

    public string GetUserName(string email)
    {
        // throw new NotImplementedException();
        NpgsqlCommand UserNameCmd = new("select concat(c_first_name,' ',c_last_name) from t_users where c_email = @email", connection);
        UserNameCmd.Parameters.AddWithValue("email", email);
        return UserNameCmd.ExecuteScalar()?.ToString() ?? "User";
    }

    public User.GetContectInfo GetContectInfo(int propertie_id)
    {
        // throw new NotImplementedException();
         NpgsqlCommand UserNameCmd = new("SELECT tp.c_property_name, tusr.c_email, tusr.c_phone_number FROM t_properties tp JOIN t_details ts ON tp.c_property_detail_id = ts.c_property_detail_id JOIN t_users tusr ON ts.c_user_id = tusr.c_user_id WHERE tp.c_property_id = @propertie_id", connection);
        UserNameCmd.Parameters.AddWithValue("propertie_id", propertie_id);
        using NpgsqlDataReader reader = UserNameCmd.ExecuteReader();
        if (reader.Read()) return new User.GetContectInfo() { UserName = reader.GetString(0), Email = reader.GetString(1), Phone = reader.GetString(2)};
        return null;
    }
}
