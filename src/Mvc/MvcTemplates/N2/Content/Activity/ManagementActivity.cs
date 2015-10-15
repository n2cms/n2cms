using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Edit.Activity;
using N2.Engine;
using N2.Web;
using N2.Management.Api;

namespace N2.Management.Activity
{
    public class ManagementActivity : ActivityBase
    {
        public string Operation { get; set; }
        public int ID { get; set; }
        public string Path { get; set; }


        public static IList<ManagementActivity> GetActivity(IEngine engine, ContentItem item)
        {
            var activities = engine.Resolve<ActivityRepository<ManagementActivity>>().GetActivities(since: Utility.CurrentTime().AddHours(-1));
            if (item != null)
                activities = activities.Where(a => a.Path == item.Path);
            return activities.GroupBy(a => new { a.PerformedBy, a.Operation })
                .Select(ag => ag.OrderByDescending(a => a.AddedDate).FirstOrDefault())
                .OrderByDescending(a => a.AddedDate)
                .Take(5)
                .ToList();
        }

        public static string ToJson(IEnumerable<ManagementActivity> activity, List<N2.Edit.Collaboration.CollaborationMessage> messages = null, List<string> flags = null)
        {
            return new
            {
				Messages = messages,
				Flags = new FlagData(flags),
                Activities = activity
                    .Select(a => new { AddedDate = a.AddedDate.ToString(), a.Operation, a.PerformedBy })
                    .OfType<object>(),
                LastChange = activity.Select(a => a.AddedDate).OrderByDescending(a => a).FirstOrDefault()
            }.ToJson();
        }
    }
}
