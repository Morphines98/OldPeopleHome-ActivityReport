using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using ActivityReport.Services.Dtos;
using Dapper;
using MySql.Data.MySqlClient;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace SendChatPushNotifications.Services
{
  public class MySqlConnectionDbConnection
  {
    private readonly MySqlConnection _connection;
    private readonly string _sendGridKey = "gjhdkjgfhkdjhkdfjh";
    private readonly string _templateId = "notyourbusiness";

    public MySqlConnectionDbConnection(string connectionString)
    {
      _connection = new MySqlConnection(connectionString);
      _connection.Open();
    }

    public void GetActivityReport()
    {
      string query = $"select Id, Title, GroupId, Date from activities " +
                $"where Date > '"+DateTime.Now.AddDays(-3).ToString("yyyy-dd-MM HH:mm:ss") +"' " +
                "and Date < '" + DateTime.Now.ToString("yyyy-dd-MM HH:mm:ss") +"'";
      var activities = _connection.Query<Activity>(query).ToList();

      var groupIds = activities.Select(a => a.GroupId).Distinct().ToList();

      query = $"select Id, Name, LastName, GroupId, CarerId from elders where groupid in (" +string.Join(',', groupIds)+")";
      var elders = _connection.Query<Elders>(query).ToList();

      var activityIds = activities.Select(a => a.Id).Distinct().ToList();
      query = $"select * from activityElderPresences where Id in("+ string.Join(',', activityIds)+")";
      var presence = _connection.Query<ActivityPresence>(query).ToList();

      var userIds = elders.Select(a => a.CarerId).Distinct().ToList();
      query = $"select * from carers where Id in (" + string.Join(',', userIds) +")";
      var reports = _connection.Query<ActivityReport.Services.Dtos.ActivityReport>(query);

      dynamic usersObject = new ExpandoObject();
      usersObject.users = new List<ExpandoObject>();

      foreach(var user in reports)
      {
        dynamic dynamicObject = new ExpandoObject();
        dynamicObject.name = user.Name;
        dynamicObject.lastName = user.LastName;
        dynamicObject.elders = new List<ExpandoObject>();

        dynamic eldersObject = new List<ExpandoObject>();

        var elderIds = elders.Where(a => a.CarerId == user.Id).ToList();
        foreach(var elder in elderIds)
        {
          
          dynamic elderObject = new ExpandoObject();
          elderObject.name = elder.Name;
          elderObject.lastName = elder.LastName;
          elderObject.activities = new List<ExpandoObject>();

          var activitiesForElder = activities.Where(a => a.GroupId == elder.GroupId).ToList();
          foreach(var activity in activitiesForElder)
          {
            dynamic activityObject = new ExpandoObject();
            activityObject.title = activity.Title;
            activityObject.date = activity.Date.ToString();

            var pres = presence.FirstOrDefault(a => a.ActivityId == activity.Id && a.ElderId == elder.Id);
            if(pres != null)
            {
              activityObject.present = pres.IsPresent;
            }
            else
            {
              activityObject.present = false;
            }
            elderObject.activities.Add(activityObject);
          }
          eldersObject.Add(elderObject);
        }

        dynamicObject.elders = eldersObject;

        SendEmailWithTemplateAsync(user.Email, dynamicObject).Wait();
      }
    }

    public async Task SendEmailWithTemplateAsync(string email, object dynamicTemplateData)
    {
      var client = new SendGridClient(_sendGridKey);
      var msg = new SendGridMessage()
      {
        From = new EmailAddress("andrei.ciornei@outlook.com", "Old People's Home"),
        TemplateId = _templateId
      };

      msg.SetTemplateData(dynamicTemplateData);
      msg.AddTo(new EmailAddress(email));

      await client.SendEmailAsync(msg);
    }

  }
}

