﻿using Library.DbContext.Entities;
using Library.UnitOfWork;
using Library.ViewModels.Account;
using Common.Items;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Library.ViewModels.Items;
using Library.DbContext.Results;
using System.Collections.Generic;
using System.Threading.Tasks;
using Library.ViewModels.Complains;
using System;
using System.Data.Entity;
using Library.Models;
using Common.Emums;
using Common.Helper;
using System.Globalization;


namespace Library.DbContext.Repositories
{
    public class ComplainHistoryRepository : Repository<ComplainHistory>
    {
        public ComplainHistoryRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
