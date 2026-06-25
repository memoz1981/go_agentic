using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemyAICourseNotes.Models.Appointment;

internal record AppointmentBookingResult(bool AppointmentBooked, Appointment Appointment, string FailureReason,
    AppointmentLead AppointmentLead);

internal record AppointmentBookingResultSlim(bool AppointmentBooked, Appointment Appointment, string FailureReason);
