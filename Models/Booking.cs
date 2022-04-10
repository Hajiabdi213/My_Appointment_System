
namespace Models;

public class Booking
{
    public int Id { get; set; }
    public int  UserId { get; set; }
    public User User { get; set; }
    public DateTime AppointmentTime { get; set; }
        public int TimeSlotId { get; set; }
        public TimeSlot? TimeSlot { get; set; }
    public Decimal PaidAmount { get; set; }
    public Decimal Commission { get; set; }
    public Decimal  DoctorRevenue { get; set; }
    public string PaymentMethod { get; set; }="";
    public int TransactionId { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }

    
}
