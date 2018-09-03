using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.Logging;
using Telerik.Sitefinity.Publishing;
using Telerik.Sitefinity.Scheduling;

namespace SitefinityWebApp.Custom.IAFCHandBook
{
	public class IAFCWeeklycheduledTask : ScheduledTask
	{
		public IAFCWeeklycheduledTask()
		{			
			
		}

		public override void ExecuteTask()
		{
			ILog log = LogManager.GetLogger(typeof(IAFCHandBookHelper));
			log.Info("Execute  Weekly Satart: " + this.Key + ": " + DateTime.UtcNow.ToString());
			/*PublishingManager pubManager = PublishingManager.GetManager();
			var point = pubManager.GetPublishingPoints().Where((w) => w.Name == "Weekly Mails").FirstOrDefault();
			if (point != null)
			{
				PublishingManager.InvokeInboundPushPipes(point.Id, null);
			}

			SchedulingManager schedulingManager = SchedulingManager.GetManager();

			IAFCWeeklycheduledTask newTask = new IAFCWeeklycheduledTask()
			{
				Key = this.Key,				
				ExecuteTime = DateTime.UtcNow.AddSeconds(120)
			};

			schedulingManager.AddTask(newTask);
			schedulingManager.SaveChanges();
			*/
			
			log.Info("Execute Weekly Finished: " + this.Key + ": " + DateTime.UtcNow.ToString());

		}

		public override string TaskName
		{
			get
			{
				return this.GetType().FullName;
			}
		}

		
	}
}



