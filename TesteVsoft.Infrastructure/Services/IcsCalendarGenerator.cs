using System.Text;
using TesteVsoft.Application.Interfaces.Services;
using TesteVsoft.Domain.Entities;
using TesteVsoft.Infrastructure.Common.Attributes;

namespace TesteVsoft.Infrastructure.Services;

[Scoped]
public class IcsCalendarGenerator : IIcsCalendarGenerator
{
    public char[] GenerateICalendar(UserTask userTask)
    {
        var now = DateTime.UtcNow;
        var sb = new StringBuilder();

        sb.AppendLine("BEGIN:VCALENDAR");
        sb.AppendLine("VERSION:2.0");
        sb.AppendLine("PRODID:-//Vsoft//TaskNotification//EN");
        sb.AppendLine("CALSCALE:GREGORIAN");
        sb.AppendLine("METHOD:PUBLISH");
        sb.AppendLine("BEGIN:VEVENT");
        sb.AppendLine($"UID:{userTask.Id}");
        sb.AppendLine($"DTSTAMP:{now:yyyyMMddTHHmmssZ}");
        sb.AppendLine($"DTSTART:{now:yyyyMMddTHHmmssZ}");
        sb.AppendLine($"DTEND:{userTask.DueDate.AddHours(1):yyyyMMddTHHmmssZ}");
        sb.AppendLine($"SUMMARY:{userTask.Title}");
        sb.AppendLine($"DESCRIPTION:{userTask.Description}");
        sb.AppendLine("STATUS:CONFIRMED");
        sb.AppendLine("END:VEVENT");
        sb.AppendLine("END:VCALENDAR");

        return sb.ToString().ToCharArray();
    }
}