namespace UdemyAICourseNotes.Models.Appointment; 

internal record InitialAppointment(bool IsFinal, bool IsCancelled, string FinalizedRequest, string FurtherQuestionToUser);
