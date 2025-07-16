<<<<<<< HEAD
﻿namespace Tourly.Models.BookingModels;
public class HotelModelView
{
    public int Id { get; set; } 
    public string HotelName { get; set; }
    public string HotelDescription { get; set; }
    public string Location { get; set; }
    public decimal HotelPrice { get; set; }
    public DateOnly StartDate { get; set; } 
    public DateOnly EndDate { get; set; }
=======
﻿namespace Tourly.BookingModels;
public class HotelModelView
{
    public string HotelName { get; set; }
    public string HotelDescription { get; set; }
    public string HotelStatus { get; set; }
    public decimal HotelPrice { get; set; }
    public DateTime StartDate { get; set; } 
    public DateTime EndDate { get; set; }
>>>>>>> da2b102d3baeae39fa678fc9dee539ca1b74efbf
}
