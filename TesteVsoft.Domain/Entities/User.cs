using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.Json.Serialization;
using TesteVsoft.Domain.Entities.Common;

namespace TesteVsoft.Domain.Entities;

public class User : BaseEntity<Guid>
{
    public string Name { get; private set; }
    public string UserName { get; private set; }
    [JsonIgnore]
    public string Password { get; private set; }
    public MailAddress? Email { get; private set; }
    public DateTime? LastLoginOn { get; private set; }

    private User(string name, string userName, string password, string? email)
        : base()
    {
        Validate(name, userName, password, email);

        Name = name;
        UserName = userName;
        Password = BCrypt.Net.BCrypt.HashPassword(password);
        Email = string.IsNullOrWhiteSpace(email) ? null : new MailAddress(email);
        LastLoginOn = null;
    }

    private User()
        : base()
    {
    }

    public static User Create(string name, string userName, string password, string email)
    {
        return new User(name, userName, password, email);
    }

    public static User CreateRandom(string mask)
    {
        if (!mask.Contains("{{random}}"))
        {
            throw new ValidationException("A máscara deve conter o marcador {{random}} para gerar valores aleatórios.");
        }

        var random = GenerateRandomString(8);
        var userName = mask.Replace("{{random}}", random);
        return new User($"User {random}", userName, "password", null);
    }

    public void Update(string newName, string newUserName, string oldPassword, string newPassword, string? newEmail)
    {
        if (!string.IsNullOrEmpty(oldPassword) && !string.IsNullOrEmpty(newPassword))
        {
            if (BCrypt.Net.BCrypt.Verify(oldPassword, Password))
            {
                Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            }
        }

        if (!string.IsNullOrEmpty(newName))
        {
            Name = newName;
        }

        if (!string.IsNullOrEmpty(newUserName))
        {
            UserName = newUserName;
        }

        if (!string.IsNullOrEmpty(newEmail))
        {
            Email = new MailAddress(newEmail);
        }
    }

    public bool Login(string password)
    {
        if (BCrypt.Net.BCrypt.Verify(password, Password))
        {
            LastLoginOn = DateTime.UtcNow;
            return true;
        }
        throw new UnauthorizedAccessException("Nome de usuário ou senha incorretas.");
    }

    private static void Validate(string name, string userName, string password, string? email)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("O nome é obrigatório.");
        }
        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new ValidationException("O nome de usuário é obrigatório.");
        }
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ValidationException("A senha é obrigatória.");
        }
        try
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                new MailAddress(email ?? string.Empty);
            }
        }
        catch (FormatException fex)
        {
            throw new ValidationException($"O formato do e-mail é inválido: {fex.Message}");
        }
    }

    private static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
