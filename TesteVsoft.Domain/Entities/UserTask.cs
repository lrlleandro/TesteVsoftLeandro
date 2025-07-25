using System.ComponentModel.DataAnnotations;
using TesteVsoft.Domain.Entities.Common;
using TesteVsoft.Domain.Enums;

namespace TesteVsoft.Domain.Entities;

public class UserTask : BaseEntity<Guid>
{
    public UserTask() // Somente para EF Core
        : base()
    {
    }

    public UserTask(string title, string description)
    {
        Title = title;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime DueDate { get; private set; } = DateTime.UtcNow;
    public UserTaskStatusTypes Status { get; private set; } = UserTaskStatusTypes.Pending;
    public Guid AssignedUserId { get; private set; }
    public User? AssignedUser { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; }

    public static UserTask Create(string title, string description, DateTime dueDate, User assignedUser)
    {
        var userTask = new UserTask(title, description);

        userTask.ChangeDueDate(dueDate);
        userTask.AssignToUser(assignedUser);

        return userTask;
    }

    public void AssignToUser(User user)
    {
        if (user is null)
        {
            throw new ValidationException("Usuário não pode ser nulo");
        }

        AssignedUserId = user.Id;
        AssignedUser = user;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeStatus(UserTaskStatusTypes status)
    {
        if (!Enum.IsDefined(typeof(UserTaskStatusTypes), status))
        {
            throw new ValidationException("Status inválido");
        }

        if (Status == status)
        {
            return;
        }

        if (Status == UserTaskStatusTypes.Completed)
        {
            throw new ValidationException("Tarefas concluídas não podem ser alteradas");   
        }

        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string title, string description, DateTime dueDate, UserTaskStatusTypes status, User assignedUser)
    {
        Title = title;
        Description = description;

        ChangeDueDate(dueDate);
        ChangeStatus(status);
        AssignToUser(assignedUser);
        UpdatedAt = DateTime.UtcNow;
    }

    private void ChangeDueDate(DateTime dueDate)
    {
        if (dueDate.ToUniversalTime() < DateTime.UtcNow)
        {
            throw new ValidationException("A data de vencimento não deve ser menor que a data atual");
        }

        DueDate = dueDate;
    }
}
