namespace MTOGO_Customer_System.Model
{
    public static class PasswordHelper
    {
        // Bcrypt installed with command -> Install-Package BCrypt.Net-Next
        public static string HashPassword(string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
            return hashedPassword;
        }

        public static bool VerifyPassword(string enteredPassword, string storedHashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHashedPassword);
        }
    }

}
