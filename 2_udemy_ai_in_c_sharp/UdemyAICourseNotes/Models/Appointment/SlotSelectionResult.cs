namespace UdemyAICourseNotes.Models.Appointment;

internal record SlotSelectionResult(SlotSelectionStatus SlotSelectionStatus, AppointmentLead AppointmentLead, 
    AppointmentLeadDetail SelectedSlot, string AlternativeDateRequestDetails);

internal record SlotSelectionResultSlim(SlotSelectionStatus SlotSelectionStatus, AppointmentLeadDetail SelectedSlot, 
    string AlternativeDateRequestDetails); 

internal enum SlotSelectionStatus
{
    SlotSelected, Cancelled, AlternativeDateProposed
}
