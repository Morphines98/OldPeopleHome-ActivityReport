using System;
using System.Collections.Generic;

namespace ActivityReport.Services.Dtos
{
  public class ActivityReport
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    public int ElderId { get; set; }
    public string FullNameElder { get; set; }

    public List<Activity> Activities { get; set; }
    public List<ActivityPresence> Presence { get; set; }
  }
}

