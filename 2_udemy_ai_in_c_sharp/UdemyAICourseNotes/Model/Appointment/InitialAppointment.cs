namespace UdemyAICourseNotes.Model.Appointment; 

internal record InitialAppointment(InitialAppointmentStatus InitialAppointmentStatus, string FinalizedRequest, string FurtherQuestionToUser);

internal record InitialAppointmentSlim(InitialAppointmentStatus InitialAppointmentStatus, string FinalizedRequest);

internal enum InitialAppointmentStatus
{
    RequestFinalized, RequestCancelled, CouldNotFinalize, ClarificationsRequired
}
