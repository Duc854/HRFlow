using System;

namespace Business.Dtos.AttendanceDtos
{
    public class TimeLogDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? CheckIn { get; set; } // Đổi sang string để FE dễ render "08:00:00"
        public string? CheckOut { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}