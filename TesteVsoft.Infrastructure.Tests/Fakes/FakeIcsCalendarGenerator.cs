using TesteVsoft.Application.Interfaces.Services;
using TesteVsoft.Domain.Entities;

namespace TesteVsoft.Tests.Unit.Services;

public partial class EmailTemplateServiceUnitTests
{
    private class FakeIcsCalendarGenerator : IIcsCalendarGenerator
    {
        public char[] GenerateICalendar(UserTask userTask)
        {
            return "BEGIN:VCALENDAR...END:VCALENDAR".ToCharArray();
        }
    }
}
