using System;
namespace ActivityReport.Services.Dtos
{
  public class ActivityPresence
  {
    public int ActivityId { get; set; }
    public int Id { get; set; }
    public int ElderId { get; set; }
    public bool IsPresent { get; set; }
  }
}

