namespace UdemyAICourseNotes.Model.Appointment;

internal record AppointmentBookingResult(bool AppointmentBooked, Appointment FinalAppointment, string FailureReason,
    AppointmentLead AppointmentLead);

internal record AppointmentBookingResultSlim(bool AppointmentBooked, Appointment FinalAppointment, string FailureReason);
