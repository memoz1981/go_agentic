namespace UdemyAICourseNotes.Model.Appointment;

internal record FinalAppointment(FinalAppointmentStatus Status, Appointment Appointment, string FollowUpRequest);

internal record FinalAppointmentSlim(FinalAppointmentStatus Status, string FollowUpRequest, string FurtherQuestionsToUser); 

internal enum FinalAppointmentStatus
{
    CompletedStopHere, CompletedWithFollowUp, CancelledStopHere, CancelledWithFollowUp, ClarificationsRequired
}
