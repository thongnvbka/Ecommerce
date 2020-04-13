using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace Library.Jobs
{

    public class PageJob
    {
        public static void UpdatePageName(short pageId, string newName, byte appId, string appName, short moduleId,
            string moduleName)
        {
            using (var unitOfWork = new UnitOfWork.UnitOfWork())
            {
                using (var transaction = unitOfWork.DbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var permissionActions = unitOfWork.PermissionActionRepo.Find(x => !x.IsDelete && x.PageId == pageId).ToList();

                        permissionActions.ForEach(pa =>
                        {
                            pa.PageName = newName;
                            pa.AppId = appId;
                            pa.AppName = appName;
                            pa.ModuleId = moduleId;
                            pa.ModuleName = moduleName;
                        });

                        unitOfWork.PermissionActionRepo.Save();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ExceptionDispatchInfo.Capture(ex).Throw();
                        throw;
                    }
                }
            }
        }
    }
}