namespace UdemyAICourseNotes.Model.Appointment;

internal record AppointmentSlot(DateTime Date, int StartHour)
{
    public override string ToString()
    {
        var date = new DateTime(Date.Year, Date.Month, Date.Day, StartHour, 0, 0); 

        return date.ToString("yyyy MMM-dd hh t");
    }
}
