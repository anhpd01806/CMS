using CMS.Common;
using CMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Bussiness
{
    public class CacheBussiness
    {
        public List<Menus> CacheAllMenus()
        {
            return new InMemoryCache().GetOrSet("ListAllMenu", new MenuBussiness().GetAllMenus);
        }
        public List<Role_User> CacheRoleUser()
        {
            return new InMemoryCache().GetOrSet("ListRoleUser", new RoleUserBussiness().GetAllRoleUser);
        }
        public List<Role_Access> CacheRoleAccess()
        {
            return new InMemoryCache().GetOrSet("ListRoleAccess", new RoleAccessBussiness().GetAllRoleAccess);
        }
        public List<Operation> CacheOperation()
        {
            return new InMemoryCache().GetOrSet("ListOperation", new OperationBussiness().GetAll);
        }

        public List<Menus> UpdateCacheAllMenu()
        {
            System.Runtime.Caching.MemoryCache.Default.Remove("ListAllMenu");
            return CacheAllMenus();
        }

        public List<Role_User> UpdateCacheRoleUser()
        {
            System.Runtime.Caching.MemoryCache.Default.Remove("ListRoleUser");
            return CacheRoleUser();
        }
        public List<Role_Access> UpdateCacheRoleAccess()
        {
            System.Runtime.Caching.MemoryCache.Default.Remove("ListMenuParent");
            return CacheRoleAccess();
        }
        public List<Operation> UpdateCacheOperation()
        {
            System.Runtime.Caching.MemoryCache.Default.Remove("ListOperation");
            return CacheOperation();
        }
    }
}