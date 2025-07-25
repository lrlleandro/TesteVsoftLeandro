using TesteVsoft.Domain.Entities;

namespace TesteVsoft.Application.Interfaces.Services;

public interface IIcsCalendarGenerator
{
    char[] GenerateICalendar(UserTask userTask);
}
