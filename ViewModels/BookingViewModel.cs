namespace ViewModels;

public class BookingViewModel
{
    public int TimeSlotId { get; set; }
    public DateTime AppointmentTime { get; set; }
    public string PaymentMethod { get; set; }="";
}