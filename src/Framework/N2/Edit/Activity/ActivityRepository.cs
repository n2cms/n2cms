using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using N2.Engine;

namespace N2.Edit.Activity
{
    [Service]
    public class ActivityRepository<TActivity> where TActivity : ActivityBase
    {
        private N2.Web.CacheWrapper cache;

        public ActivityRepository(N2.Web.CacheWrapper cache)
        {
            this.cache = cache;
        }

        public ActivityUser GetUser(string userName)
        {
            var users = GetOrCreateUsers();
            lock (users)
            {
                if (users.ContainsKey(userName))
                    return users[userName];
            }
            return null;
        }

        public IEnumerable<TActivity> GetActivities(DateTime? since = null, string byUser = null) 
        {
            var activities = GetOrCreateActivities();
            lock (activities)
            {
                for (int i = activities.Count - 1; i >= 0; i--)
                {
                    var activity = activities[i];
                    if (since.HasValue && activity.AddedDate < since)
                        break;

                    if (activity is TActivity)
                        yield return activity as TActivity;
                }
            }
        }

        private List<TActivity> GetOrCreateActivities()
        {
            return cache.GetOrCreate<List<TActivity>>("ActivityRepository." + typeof(TActivity).Name, () => new List<TActivity>(), new N2.Web.CacheOptions { Priority = System.Web.Caching.CacheItemPriority.High, SlidingExpiration = TimeSpan.FromHours(1) });
        }

        public virtual Dictionary<string, ActivityUser> GetOrCreateUsers()
        {
            return cache.GetOrCreate<Dictionary<string, ActivityUser>>("ActivityRepository.Users", () => new Dictionary<string, ActivityUser>(), new N2.Web.CacheOptions { Priority = System.Web.Caching.CacheItemPriority.High, SlidingExpiration = TimeSpan.FromHours(2) });
        }

        public void Add(TActivity activity)
        {
            var activities = GetOrCreateActivities();
            Truncate(activities, TimeSpan.FromHours(1));

            activity.AddedDate = Utility.CurrentTime();
            lock (activities)
            {
                activities.Add(activity);
            }

            if (activity.PerformedBy == null)
                return;

            var users = GetOrCreateUsers();
            lock (users)
            {
                ActivityUser user = users.ContainsKey(activity.PerformedBy)
                    ? users[activity.PerformedBy]
                    : users[activity.PerformedBy] = new ActivityUser(activity.PerformedBy);

                user.Activities.Add(activity);
            }
        }

        public void Truncate(TimeSpan activityOlderThan)
        {
            var activities = GetOrCreateActivities();
            Truncate(activities, activityOlderThan);
        }

        private void Truncate(List<TActivity> activities, TimeSpan activityOlderThan)
        {
            var since = Utility.CurrentTime().Subtract(activityOlderThan);
            TActivity[] removedActivities;

            lock (activities)
            {
                int removeCount = GetRemoveCount(activities, since);
                if (removeCount <= 0)
                    return;

                removedActivities = new TActivity[removeCount];
                activities.CopyTo(0, removedActivities, 0, removeCount);
                activities.RemoveRange(0, removeCount);
            }

            var users = GetOrCreateUsers();
            for (int i = 0; i < removedActivities.Length; i++)
            {
                lock (users)
                {
                    var removedActivity = removedActivities[i];
                    if (removedActivity.PerformedBy == null)
                        continue;

                    if (users.ContainsKey(removedActivity.PerformedBy))
                    {
                        var user = users[removedActivity.PerformedBy];
                        if (user.Activities.Contains(removedActivity))
                            user.Activities.Remove(removedActivity);

                        if (user.Activities.Count == 0)
                            users.Remove(removedActivity.PerformedBy);
                    }
                }
            }
        }

        private int GetRemoveCount(List<TActivity> activities, DateTime since)
        {
            for (int i = 0; i < activities.Count; i++)
            {
                var activity = activities[i];
                if (activity.AddedDate > since)
                {
                    if (i > 0)
                    {
                        return i;
                    }
                    break;
                }
            }
            return 0;
        }
    }
}
