namespace UdemyAICourseNotes.Model.Appointment;

internal record NoSlotFoundResult(NoSlotFoundStatus NoSlotFoundStatus, string RequestToRescheduleText);

internal enum NoSlotFoundStatus
{
    StopHere, GoBackToDateSelection
}