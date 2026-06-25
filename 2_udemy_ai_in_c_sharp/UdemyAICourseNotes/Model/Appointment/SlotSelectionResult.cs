namespace UdemyAICourseNotes.Model.Appointment;

internal record SlotSelectionResult(SlotSelectionStatus SlotSelectionStatus, AppointmentLead AppointmentLead, 
    AppointmentSlot SelectedSlot, string AlternativeDateRequestDetails);

internal record SlotSelectionResultSlim(SlotSelectionStatus SlotSelectionStatus, AppointmentSlot SelectedSlot, 
    string AlternativeDateRequestDetails); 

internal enum SlotSelectionStatus
{
    SlotSelected, Cancelled, AlternativeDateProposed
}
