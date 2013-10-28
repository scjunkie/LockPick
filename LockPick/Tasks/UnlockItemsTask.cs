using System;
using System.Collections.Generic;
using System.Web.Security;

using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Security.Accounts;
using Sitecore.Tasks;
using Sitecore.Web.Authentication;

namespace LockPick.Tasks
{
    public class UnlockItemsTask
    {
        private static readonly TimeSpan ElapsedTimeWhenIdle = GetElapsedTimeWhenIdle();

        public virtual void UnlockIdleUserItems(Item[] items, CommandItem command, ScheduleItem schedule)
        {
            if (ElapsedTimeWhenIdle == TimeSpan.Zero)
            {
                return;
            }

            IEnumerable<Item> lockedItems = GetLockedItems(schedule.Database);
            foreach (Item lockedItem in lockedItems)
            {
                UnlockIfApplicable(lockedItem);
            }
        }
        
        protected virtual IEnumerable<Item> GetLockedItems(Database database)
        {
            Assert.ArgumentNotNull(database, "database");
            return database.SelectItems("fast://*[@__lock='%owner=%']");
        }

        private void UnlockIfApplicable(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            if (!ShouldUnlockItem(item))
            {
                return;
            }
            
            Unlock(item);
        }

        protected virtual bool ShouldUnlockItem(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            if(!item.Locking.IsLocked())
            {
                return false;
            }

            string owner = item.Locking.GetOwner();
            return !IsUserAdmin(owner) && IsUserIdle(owner);
        }

        protected virtual bool IsUserAdmin(string username)
        {
            Assert.ArgumentNotNullOrEmpty(username, "username");
            User user = User.FromName(username, false);
            Assert.IsNotNull(user, "User cannot be null -- perhaps the user was deleted.");
            return user.IsAdministrator;
        }

        protected virtual bool IsUserIdle(string username)
        {
            Assert.ArgumentNotNullOrEmpty(username, "username");
            DomainAccessGuard.Session userSession = DomainAccessGuard.Sessions.Find(session => session.UserName == username);
            if(userSession == null)
            {
                return true;
            }

            return userSession.LastRequest.Add(ElapsedTimeWhenIdle) <= DateTime.Now;
        }

        protected virtual void Unlock(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            try
            {
                string owner = item.Locking.GetOwner();
                item.Editing.BeginEdit();
                item.Locking.Unlock();
                item.Editing.EndEdit();
                Log.Info(string.Format("Unlocked {0} due to being idle - was locked by {1}", item.Paths.Path, owner), this);
            }
            catch (Exception ex)
            {
                Log.Error(this.ToString(), ex, this);
            }
        }

        private static TimeSpan GetElapsedTimeWhenIdle()
        {
            TimeSpan elapsedTimeWhenIdle;
            if (TimeSpan.TryParse(Settings.GetSetting("UnlockItems.ElapsedTimeWhenIdle"), out elapsedTimeWhenIdle))
            {
                return elapsedTimeWhenIdle;
            }
            
            return TimeSpan.Zero;
        }
    }
}
