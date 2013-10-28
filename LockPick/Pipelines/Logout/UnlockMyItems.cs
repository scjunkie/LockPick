using System;
using System.Collections.Generic;
using System.Linq;

using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.Logout;

namespace LockPick.Pipelines.Logout
{
    public class UnlockMyItems
    {
        public virtual void Process(LogoutArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            UnlockMyItemsIfAny();
        }

        private void UnlockMyItemsIfAny()
        {
            IEnumerable<Item> lockedItems = GetMyLockedItems();
            if (!CanProcess(lockedItems))
            {
                return;
            }

            foreach (Item lockedItem in lockedItems)
            {
                Unlock(lockedItem);
            }
        }

        private IEnumerable<Item> GetMyLockedItems()
        {
            return Context.ContentDatabase.SelectItems(GetMyLockedItemsQuery());
        }

        protected virtual string GetMyLockedItemsQuery()
        {
            return string.Format("fast://*[@__lock='%owner=\"{0}\"%']", Context.User.Name);
        }

        protected virtual bool CanProcess(IEnumerable<Item> lockedItems)
        {
            return lockedItems != null
                    && lockedItems.Any()
                    && lockedItems.Select(item => item.Locking.HasLock()).Any();
        }

        protected virtual void Unlock(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            if (!item.Locking.HasLock())
            {
                return;
            }
            
            try
            {
                string owner = item.Locking.GetOwner();
                item.Editing.BeginEdit();
                item.Locking.Unlock();
                item.Editing.EndEdit();
                Log.Info(string.Format("Unlocked {0} during logout - was locked by {1}", item.Paths.Path, owner), this);
            }
            catch (Exception ex)
            {
                Log.Error(this.ToString(), ex, this);
            }
        }
    }
}
