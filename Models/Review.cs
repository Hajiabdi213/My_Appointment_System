
namespace Models;

public class Review
{
    public int Id { get; set; } 
    public int MyProperty { get; set; }   
    public int BookingId { get; set; }
     public Booking Booking { get; set; }=new();
    public int Stars { get; set; }
    public String Remarks { get; set; }="";
    public DateTime CreatedAt { get; set; }
    
}