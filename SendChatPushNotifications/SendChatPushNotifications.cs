using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SendChatPushNotifications.Services;

namespace SendChatPushNotifications
{
  public class SendActivityReport
  {
    private readonly MySqlConnectionDbConnection _dbConnection;

    public SendActivityReport(MySqlConnectionDbConnection dbConnection)
    {
      _dbConnection = dbConnection;
    }

    [FunctionName("SendActivityReport")]
    public async Task Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer)
    {
      _dbConnection.GetActivityReport();    
    }
  }
}