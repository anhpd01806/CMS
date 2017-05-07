using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.API.Data;

namespace CMS.API.Bussiness
{
    public class InitDB
    {
        #region fields
        private static CmsDataDataContext _instance;
        private static readonly object lockDB = new object();

        #endregion

        #region properties
        public CmsDataDataContext Instance
        {
            get
            {
                return _instance = new CmsDataDataContext();
            }
        }

        #endregion
    }
}