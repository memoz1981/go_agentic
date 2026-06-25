namespace UdemyAICourseNotes.Models.Appointment;

internal record AppointmentLead(string Name, string Description, string Phone, AppointmentLeadDetail[] Slots);

internal record AppointmentLeadDetail(DateTime Date, int StartHour); 
