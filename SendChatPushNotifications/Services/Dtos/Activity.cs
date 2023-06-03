using System;
namespace ActivityReport.Services.Dtos
{
  public class Activity
  {
    public int Id { get; set; }
    public string Title { get; set; }
    public int GroupId { get; set; }
    public DateTime Date { get; set; }
  }
}

