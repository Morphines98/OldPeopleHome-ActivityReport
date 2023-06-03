using System;
namespace ActivityReport.Services.Dtos
{
  public class Elders
  {
    public int Id { get; set; }
    public int CarerId { get; set; }
    public int GroupId { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
  }
}

