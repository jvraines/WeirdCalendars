WeirdCalendars provides dozens of unusual calendars for use in the .NET environment, emphasizing compatibility, as far as possible, with the Globalization and DateTime APIs. These are proposed, reformed, antique and archaic, fictional, conjectural, or just plain weird calendars for marking the passage of days, and, in many cases, the time of day.  
  
The base class of this library is **WeirdCalendar**, which inherits [System.Globalization.Calendar](https://learn.microsoft.com/en-us/dotnet/api/system.globalization.calendar?view=netstandard-2.0). Its descendants may be instantiated and manipulated like any other Calendar, particularly in conjunction with the replacement class **WeirdCalendars.CultureInfoWC** and the extension method **System.DateTime.ToStringWC**. They are described in more detail below.  
  
Since these calendars interact with the DateTime API, they are constrained to the range 1 January 1 to 31 December 9999 in the proleptic Gregorian calendar. In many cases, they are constrained further by provisions of the calendar design or limits of astronomical computation. Every effort is made to support standard methods in a way that makes sense under the rules of each calendar. Frequently, additional methods and format codes are available to obtain values that are unique to a particular design. Note that DateTime arguments are always assumed to be in Universal Time.  
  
The handling of intercalary days beyond the ordinary leap day or leap month presents challenges under the Globalization API. When these days are designated as additions to a month, or as a separate month, they are so handled. Otherwise, they are considered part of the month they are said to precede or follow and will have day-of-month numbers. Calendars which mandate blank days (days which do not belong to the flow of the week) employ the special enumeration **DayOfWeekWC**, which includes the constant Blank. Calendars which have more than seven days in the week will also use this special enumeration.  
  
Since these calendars employ unusual ranges of values, calls to **GetDayOfWeek** and **ToString** will often throw exceptions or fail to display appropriate formatting. Instead, a **CultureInfoWC** object should be instantiated with the desired calendar, then set as the current thread culture or supplied in method calls to ensure proper handling. Likewise, **GetDayOfWeekWC** should be called in calendars that use blank days or more than seven weekdays. Finally, the extension method **ToStringWC** will ensure correct formatting without exceptions and with rendering of calendar-specific custom format specifiers. (Those specifiers, if any, are enumerated in the **CustomFormats** property.) Of course, a WeirdCalendar's methods may also be called in standalone fashion.  
  
Example:

    // Add the namespace.
    using WeirdCalendars;
	
    // Three different ways to create a custom culture information object.
    // If a region name is supplied, its conventions will be honored as far as possible.
    CultureInfoWC customCulture = new(DiscordianCalendar, "en-US"); // or
    CultureInfoWC customCulture = new("DiscordianCalendar", "en-US"); // or
    DiscordianCalendar discordian = new();
    CultureInfoWC customCulture = new(discordian, "en-US");
	
    // Create a date value.
    DateTime date = new DateTime(2024, 2, 19);
	
    // Print long date format using the calendar of the instantiated CultureInfoWC object.
    Console.WriteLine(date.ToStringWC("D"), customCulture);
    // "Setting Orange, Chaos 50, 3190"
	
    // Make it the current thread culture.
    CultureInfo.CurrentCulture = customCulture;
	
    // Print short date format using the current culture's calendar.
    Console.WriteLine(date.ToStringWC("d"));
    // "1/50/3190"
	
    // Print the short date and Holyday using a custom format specifier.
    Console.WriteLine(${date.ToStringWC("d")} {date.ToStringWC("n")});
    // "1/50/3190 Chaoflux"
	
    // Or get its value using a custom method.
    string holy = discordian.GetHolyDay(new DateTime(2024, 3, 19));
    // Returns "Mojoday".
