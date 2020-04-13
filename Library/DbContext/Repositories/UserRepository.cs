using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.Emums;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.Models;
using Library.UnitOfWork;
using Library.ViewModels;

namespace Library.DbContext.Repositories
{
    public class UserRepository : Repository<User>
    {
        public UserRepository(ProjectXContext context) : base(context)
        {
        }

        public Task<User> GetByUserName(string userName)
        {
            return Db.Users.SingleAsync(x => x.UserName.Equals(userName) && !x.IsDelete && x.Status == 1);
        }

        public async Task<int> UserUpdateLoginFailure(long id, bool isLockout, DateTime? lastLockoutDate, DateTime? firstLoginFailureDate,
            byte loginFailureCount, DateTime? lockoutToDate)
        {
            var user = await Db.Users.SingleAsync(x => x.Id == id);

            user.IsLockout = isLockout;
            user.LastLockoutDate = lastLockoutDate;
            user.LockoutToDate = lockoutToDate;
            user.FirstLoginFailureDate = firstLoginFailureDate;
            user.LoginFailureCount = loginFailureCount;

            return await Db.SaveChangesAsync();
        }

        public Task<UserResult> GetUser(int userId)
        {
            return Db.Users.Where(x => !x.IsDelete && x.Id == userId)
                .Join(
                    Db.UserPositions.Where(x => x.IsDefault), user => user.Id,
                    position => position.UserId,
                    (u, p) => new UserResult()
                    {
                        Id = u.Id, // Id (Primary key)
                        UserName = u.UserName, // UserName (length: 50)
                        Password = u.Password, // Password (length: 50)
                        FirstName = u.FirstName, // FirstName (length: 30)
                        MidleName = u.MidleName, // MidleName (length: 30)
                        LastName = u.LastName, // LastName (length: 30)
                        FullName = u.FullName, // FullName (length: 100)
                        Gender = u.Gender, // Gender
                        Email = u.Email, // Email (length: 50)
                        Description = u.Description, // Description (length: 500)
                        Created = u.Created, // Created
                        Updated = u.Updated, // Updated
                        LastUpdateUserId = u.LastUpdateUserId, // LastUpdateUserId
                        Status = u.Status, // Status
                        Birthday = u.Birthday, // Birthday
                        StartDate = u.StartDate, // StartDate
                        Avatar = u.Avatar, // Avatar (length: 2000)

                        TitleId = p.TitleId, // TitleId (Primary key)
                        OfficeId = p.OfficeId, // OfficeId (Primary key)
                        TitleName = p.TitleName, // TitleName (length: 300)
                        OfficeName = p.OfficeName, // OfficeName (length: 300)
                        IsDefault = p.IsDefault, // IsDefault
                        Type = p.Type, // Type
                        OfficeIdPath = p.OfficeIdPath, // OfficeIdPath (length: 500)
                        OfficeNamePath = p.OfficeNamePath, // OfficeNamePath (length: 2000)
                        DirectUserId = p.DirectUserId, // DirectUserId
                        DirectFullName = p.DirectFullName, // DirectFullName (length: 100)
                        DirectTitleId = p.DirectTitleId, // DirectTitleId
                        DirectTitleName = p.DirectTitleName, // DirectTitleName (length: 300)
                        DirectOfficeId = p.DirectOfficeId, // DirectOfficeId
                        DirectOfficeName = p.DirectOfficeName, // DirectOfficeName (length: 300)
                        ApprovalUserId = p.ApprovalUserId, // ApprovalUserId
                        ApprovalFullName = p.ApprovalFullName, // ApprovalFullName (length: 100)
                        ApprovalTitleId = p.ApprovalTitleId, // ApprovalTitleId
                        ApprovalTitleName = p.ApprovalTitleName, // ApprovalTitleName (length: 300)
                        ApprovalOfficeId = p.ApprovalOfficeId, // ApprovalOfficeId
                        ApprovalOfficeName = p.ApprovalOfficeName, // ApprovalOfficeName (length: 300)
                        LevelId = p.LevelId, // LevelId
                        LevelName = p.LevelName, // LevelName (length: 300)
                        GroupPermisionId = p.GroupPermisionId, // GroupPermisionId
                        GroupPermissionName = p.GroupPermissionName, // GroupPermissionName (length: 300)
                    }).SingleOrDefaultAsync();
        }

        public List<T> GetBySpec<T>(Expression<Func<User, T>> projector, Expression<Func<User, bool>> spec)
        {
            return Db.Users.Where(spec).Select(projector).ToList();
        }

        public List<T> GetBySpec<T>(Expression<Func<User, UserPosition, T>> projector, Expression<Func<User, bool>> spec, Expression<Func<UserPosition, bool>> spec1)
        {
            return Db.Users.Where(spec).Join(Db.UserPositions.Where(spec1), u => u.Id, ut => ut.UserId, projector).Distinct().ToList();
        }

        public List<User> GetByExpression(Expression<Func<User, bool>> spec, Expression<Func<UserPosition, bool>> spec1, Expression<Func<Office, bool>> spec2)
        {
            return Db.Users.Where(spec)
                .Join(Db.UserPositions.Where(spec1), u => u.Id, ut => ut.UserId, (u, ut) => new { u, ut })
                .Join(Db.Offices.Where(spec2), g => g.ut.OfficeId, o => o.Id, (arg, o) => arg.u).Distinct().ToList();
        }

        // Lấy ra trưởng đơn vị theo phòng ban
        public Task<UserResult> GetLeaderByOfficeId(int officeId, byte userType)
        {
            return Db.Users.Where(x => !x.IsDelete)
                .Join(Db.UserPositions.Where(x => x.IsDefault && x.OfficeId == officeId && x.Type == userType),
                    user => user.Id,
                    position => position.UserId,
                    (u, p) => new UserResult()
                    {
                        Id = u.Id, // Id (Primary key)
                        UserName = u.UserName, // UserName (length: 50)
                        Password = u.Password, // Password (length: 50)
                        FirstName = u.FirstName, // FirstName (length: 30)
                        MidleName = u.MidleName, // MidleName (length: 30)
                        LastName = u.LastName, // LastName (length: 30)
                        FullName = u.FullName, // FullName (length: 100)
                        Gender = u.Gender, // Gender
                        Email = u.Email, // Email (length: 50)
                        Description = u.Description, // Description (length: 500)
                        Created = u.Created, // Created
                        Updated = u.Updated, // Updated
                        LastUpdateUserId = u.LastUpdateUserId, // LastUpdateUserId
                        Status = u.Status, // Status
                        Birthday = u.Birthday, // Birthday
                        StartDate = u.StartDate, // StartDate
                        Avatar = u.Avatar, // Avatar (length: 2000)
                        TitleId = p.TitleId, // TitleId (Primary key)
                        OfficeId = p.OfficeId, // OfficeId (Primary key)
                        TitleName = p.TitleName, // TitleName (length: 300)
                        OfficeName = p.OfficeName, // OfficeName (length: 300)
                        IsDefault = p.IsDefault, // IsDefault
                        Type = p.Type, // Type
                        OfficeIdPath = p.OfficeIdPath, // OfficeIdPath (length: 500)
                        OfficeNamePath = p.OfficeNamePath, // OfficeNamePath (length: 2000)
                        DirectUserId = p.DirectUserId, // DirectUserId
                        DirectFullName = p.DirectFullName, // DirectFullName (length: 100)
                        DirectTitleId = p.DirectTitleId, // DirectTitleId
                        DirectTitleName = p.DirectTitleName, // DirectTitleName (length: 300)
                        DirectOfficeId = p.DirectOfficeId, // DirectOfficeId
                        DirectOfficeName = p.DirectOfficeName, // DirectOfficeName (length: 300)
                        ApprovalUserId = p.ApprovalUserId, // ApprovalUserId
                        ApprovalFullName = p.ApprovalFullName, // ApprovalFullName (length: 100)
                        ApprovalTitleId = p.ApprovalTitleId, // ApprovalTitleId
                        ApprovalTitleName = p.ApprovalTitleName, // ApprovalTitleName (length: 300)
                        ApprovalOfficeId = p.ApprovalOfficeId, // ApprovalOfficeId
                        ApprovalOfficeName = p.ApprovalOfficeName, // ApprovalOfficeName (length: 300)
                        LevelId = p.LevelId, // LevelId
                        LevelName = p.LevelName, // LevelName (length: 300)
                        GroupPermisionId = p.GroupPermisionId, // GroupPermisionId
                        GroupPermissionName = p.GroupPermissionName, // GroupPermissionName (length: 300)
                    }).SingleOrDefaultAsync();
        }

        public Task<List<UserResult>> Search(UserFilterViewModel filter, int currentPage, int recordPerPage, out int totalRecord)
        {
            //((filter.HasChilds && x.OfficeIdPath.StartsWith(filter.OfficeIdPath + ".")) ||
            var query =
                Db.Users.Where(x => x.IsDelete == false && x.UnsignName.Contains(filter.Keyword))
                    .Join(Db.UserPositions.Where(x => x.IsDefault && (filter.HasChilds && x.OfficeIdPath.StartsWith(filter.OfficeIdPath + ".") || x.OfficeId == filter.OfficeId)
                            && (filter.TitleId == null || x.TitleId == filter.TitleId.Value)), user => user.Id,
                            position => position.UserId,
                            (u, p) => new UserResult()
                            {
                                Id = u.Id, // Id (Primary key)
                                UserName = u.UserName, // UserName (length: 50)
                                Password = u.Password, // Password (length: 50)
                                FirstName = u.FirstName, // FirstName (length: 30)
                                MidleName = u.MidleName, // MidleName (length: 30)
                                LastName = u.LastName, // LastName (length: 30)
                                FullName = u.FullName, // FullName (length: 100)
                                Gender = u.Gender, // Gender
                                Email = u.Email, // Email (length: 50)
                                Description = u.Description, // Description (length: 500)
                                Created = u.Created, // Created
                                Updated = u.Updated, // Updated
                                LastUpdateUserId = u.LastUpdateUserId, // LastUpdateUserId
                                Status = u.Status, // Status
                                Birthday = u.Birthday, // Birthday
                                StartDate = u.StartDate, // StartDate
                                Avatar = u.Avatar, // Avatar (length: 2000)

                                TitleId = p.TitleId, // TitleId (Primary key)
                                OfficeId = p.OfficeId, // OfficeId (Primary key)
                                TitleName = p.TitleName, // TitleName (length: 300)
                                OfficeName = p.OfficeName, // OfficeName (length: 300)
                                IsDefault = p.IsDefault, // IsDefault
                                Type = p.Type, // Type
                                OfficeIdPath = p.OfficeIdPath, // OfficeIdPath (length: 500)
                                OfficeNamePath = p.OfficeNamePath, // OfficeNamePath (length: 2000)
                                DirectUserId = p.DirectUserId, // DirectUserId
                                DirectFullName = p.DirectFullName, // DirectFullName (length: 100)
                                DirectTitleId = p.DirectTitleId, // DirectTitleId
                                DirectTitleName = p.DirectTitleName, // DirectTitleName (length: 300)
                                DirectOfficeId = p.DirectOfficeId, // DirectOfficeId
                                DirectOfficeName = p.DirectOfficeName, // DirectOfficeName (length: 300)
                                ApprovalUserId = p.ApprovalUserId, // ApprovalUserId
                                ApprovalFullName = p.ApprovalFullName, // ApprovalFullName (length: 100)
                                ApprovalTitleId = p.ApprovalTitleId, // ApprovalTitleId
                                ApprovalTitleName = p.ApprovalTitleName, // ApprovalTitleName (length: 300)
                                ApprovalOfficeId = p.ApprovalOfficeId, // ApprovalOfficeId
                                ApprovalOfficeName = p.ApprovalOfficeName, // ApprovalOfficeName (length: 300)
                                LevelId = p.LevelId, // LevelId
                                LevelName = p.LevelName, // LevelName (length: 300)
                                GroupPermisionId = p.GroupPermisionId, // GroupPermisionId
                                GroupPermissionName = p.GroupPermissionName, // GroupPermissionName (length: 300)
                            });

            totalRecord = query.Count();

            return query.OrderBy(x => x.Id).Skip((currentPage - 1) * recordPerPage).Take(recordPerPage).ToListAsync();
        }


        public Task<List<UserSuggetionResult>> Suggetion(string keyword, int currentPage, int recordPerPage, out int totalRecord)
        {
            var query = Db.Users.Where(x => !x.IsDelete && x.UnsignName.Contains(keyword))
                    .Join(Db.UserPositions.Where(x => x.IsDefault), user => user.Id,
                            position => position.UserId,
                            (u, p) => new UserSuggetionResult()
                            {
                                Id = u.Id,
                                UserName = u.UserName,
                                FullName = u.FullName,
                                Gender = u.Gender,
                                Email = u.Email,
                                Status = u.Status,
                                Avatar = u.Avatar,
                                TitleId = p.TitleId,
                                OfficeId = p.OfficeId,
                                TitleName = p.TitleName,
                                OfficeName = p.OfficeName,
                                OfficeIdPath = p.OfficeIdPath,
                                LevelId = p.LevelId,
                                LevelName = p.LevelName
                            });

            totalRecord = query.Count();

            return query.OrderBy(x => x.Id).Skip((currentPage - 1) * recordPerPage).Take(recordPerPage).ToListAsync();
        }

        public Task<List<UserOfficeResult>> GetUserToOffice(int userId, byte type, string officeIdPath, int officeId)
        {
            var query =
                 Db.Users.Where(x => !x.IsDelete && (type != 0 || x.Id == userId))
                     .Join(
                         Db.UserPositions.Where(
                             x => (type == 0 || (x.OfficeIdPath.StartsWith(officeIdPath + ".") || x.OfficeId == officeId))),
                             user => user.Id,
                             position => position.UserId,
                             (u, p) => new UserOfficeResult()
                             {
                                 Id = u.Id, // Id (Primary key)
                                 UserName = u.UserName, // UserName (length: 50)
                                 FullName = u.FullName + (p.IsDefault ? "" : " (Kiêm nhiệm)"), // FullName (length: 100)
                                 Email = u.Email, // Email (length: 50)
                                 Avatar = u.Avatar, // Avatar (length: 2000)
                                 Phone = u.Phone,

                                 TitleId = p.TitleId, // TitleId (Primary key)
                                 OfficeId = p.OfficeId, // OfficeId (Primary key)
                                 TitleName = p.TitleName, // TitleName (length: 300)
                                 OfficeName = p.OfficeName, // OfficeName (length: 300
                                 Type = p.Type, // Type
                                 OfficeIdPath = p.OfficeIdPath, // OfficeIdPath (length: 500)
                                 OfficeNamePath = p.OfficeNamePath, // OfficeNamePath (length: 2000)
                                 LevelId = p.LevelId, // LevelId
                                 LevelName = p.LevelName, // LevelName (length: 300)
                             });

            return query.OrderBy(x => x.Id).ToListAsync();
        }

        /// <summary>
        /// lấy danh sách nhân viên có điều kiện là nhân viên ảo công ty
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Task<List<UserOfficeResult>> GetUserToOfficeTypeCompany(OfficeType type)
        {
            var query =
                 Db.Users.Where(x => !x.IsDelete && x.IsCompany)
                     .Join(
                         Db.UserPositions.Where(x => x.IsDefault),
                             user => user.Id,
                             position => position.UserId,
                         (u, p) => new { u, p })
                         .Join(
                            Db.Offices.Where(x => !x.IsDelete && x.Type == (byte)type),
                            group => group.p.OfficeId,
                            office => office.Id,
                            (g, o) => new UserOfficeResult()
                            {
                                Id = g.u.Id, // Id (Primary key)
                                UserName = g.u.UserName, // UserName (length: 50)
                                FullName = g.u.FullName, // FullName (length: 100)
                                Email = g.u.Email, // Email (length: 50)
                                Avatar = g.u.Avatar, // Avatar (length: 2000)
                                TitleId = g.p.TitleId, // TitleId (Primary key)
                                OfficeId = g.p.OfficeId, // OfficeId (Primary key)
                                TitleName = g.p.TitleName, // TitleName (length: 300)
                                OfficeName = g.p.OfficeName, // OfficeName (length: 300
                                Type = g.p.Type, // Type
                                OfficeIdPath = g.p.OfficeIdPath, // OfficeIdPath (length: 500)
                                OfficeNamePath = g.p.OfficeNamePath, // OfficeNamePath (length: 2000)
                                LevelId = g.p.LevelId, // LevelId
                                LevelName = g.p.LevelName, // LevelName (length: 300)
                            });

            return query.OrderBy(x => x.Id).ToListAsync();
        }

        public Task<UserOfficeResult> GetUserToOfficeOrder(int userId)
        {
            var query =
                 Db.Users.Where(x => !x.IsDelete)
                     .Join(
                         Db.UserPositions.Where(
                             x => (x.UserId == userId) && x.IsDefault),
                             user => user.Id,
                             position => position.UserId,
                             (u, p) => new UserOfficeResult()
                             {
                                 Id = u.Id, // Id (Primary key)
                                 UserName = u.UserName, // UserName (length: 50)
                                 FullName = u.FullName, // FullName (length: 100)
                                 Email = u.Email, // Email (length: 50)
                                 Avatar = u.Avatar, // Avatar (length: 2000)

                                 TitleId = p.TitleId, // TitleId (Primary key)
                                 OfficeId = p.OfficeId, // OfficeId (Primary key)
                                 TitleName = p.TitleName, // TitleName (length: 300)
                                 OfficeName = p.OfficeName, // OfficeName (length: 300
                                 Type = p.Type, // Type
                                 OfficeIdPath = p.OfficeIdPath, // OfficeIdPath (length: 500)
                                 OfficeNamePath = p.OfficeNamePath, // OfficeNamePath (length: 2000)
                                 LevelId = p.LevelId, // LevelId
                                 LevelName = p.LevelName, // LevelName (length: 300)
                             });

            return query.OrderBy(x => x.Id).FirstOrDefaultAsync();
        }

        public List<UserGetShortInfoByFullNameResults> GetShortInfoByFullName(string keyword, int pageIndex, int pageSize, out int totalRecord)
        {
            var query = Db.Users.Join(Db.UserPositions.Where(x => x.IsDefault), u => u.Id, ut => ut.UserId, (u, ut) => new { u, ut }).
                Where(x => x.u.IsDelete == false && x.u.Status < 5 && x.u.UnsignName.Contains(keyword))
                .Select(x => new
                {
                    x.u.Id,
                    x.u.UserName,
                    x.u.FullName,
                    x.ut.TitleName,
                    x.ut.TitleId,
                    x.ut.OfficeName,
                    Image = x.u.Avatar,
                    x.u.FirstName,
                    MiddleName = x.u.MidleName,
                    x.u.LastName
                }).Distinct();

            totalRecord = query.Count();

            return query.Select(x => new UserGetShortInfoByFullNameResults
            {
                Id = x.Id,
                FullName = x.FullName,
                TitleName = x.TitleName,
                TitleId = x.TitleId,
                OfficeName = x.OfficeName,
                Image = x.Image,
                FirstName = x.FirstName,
                MiddleName = x.MiddleName,
                LastName = x.LastName,
                UserName = x.UserName
            }).OrderBy(x => x.FullName).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<UserConnection> GetUserConnectionByUserId(int userId)
        {
            return Db.UserConnections.Where(u => u.UserId == userId).ToList();
        }

        public List<UserConnection> GetUserConnectionByUserIdAndUserType(int userId, byte type)
        {
            return Db.UserConnections.Where(u => u.UserId == userId && u.UserType == type).ToList();
        }

        public List<UserConnection> GetUserConnectionByUserId(List<int> userId)
        {
            return Db.UserConnections.Where(u => userId.Contains(u.UserId)).ToList();
        }

        public List<UserConnection> GetUserConnectionByUserIdAndUserType(List<int> userId, byte type)
        {
            return Db.UserConnections.Where(u => userId.Contains(u.UserId) && u.UserType == type).ToList();
        }

        public List<T> GetUserConnectionByUserId<T>(int userId, Expression<Func<UserConnection, T>> projector)
        {
            return Db.UserConnections.Where(u => u.UserId == userId).Select(projector).ToList();
        }

        public List<T> GetUserConnectionByUserId<T>(List<int> userId, Expression<Func<UserConnection, T>> projector)
        {
            return Db.UserConnections.Where(u => userId.Contains(u.UserId)).Select(projector).ToList();
        }

        public List<T> GetUserConnectionByUserId<T>(Expression<Func<UserConnection, T>> projector, Expression<Func<UserConnection, bool>> spec)
        {
            return Db.UserConnections.Where(spec).Select(projector).ToList();
        }

        public T GetByUserName<T>(string userName, Expression<Func<User, T>> projector)
        {
            return Db.Users.Where(u => u.UserName.Equals(userName) && !u.IsDelete).Select(projector).SingleOrDefault();
        }

        public void InsertUserConnection(int userId, string userName, string fullName, int? officeId, string officeName,
        string titleName, string image, string connectionId, string sessionId, string unsignName, byte userType = 0)
        {
            var connectionInfo = Db.UserConnections.SingleOrDefault(x => x.UserId == userId && x.ConnectionId == connectionId);

            if (connectionInfo == null)
            {
                Db.UserConnections.Add(new UserConnection
                {
                    ConnectionId = connectionId,
                    UserId = userId,
                    SessionId = sessionId,
                    UserName = userName,
                    FullName = fullName,
                    Image = image,
                    OfficeId = officeId,
                    OfficeName = officeName,
                    TitleName = titleName,
                    UnsignName = unsignName,
                    UserType = userType
                });

                Db.SaveChanges();
            }
        }

        public int DeleteUserConnection(string connectionId)
        {
            Db.UserConnections.RemoveRange(Db.UserConnections.Where(x => x.ConnectionId == connectionId));

            return Db.SaveChanges();
        }

        public int DeleteUserConnection(int userId)
        {
            Db.UserConnections.RemoveRange(Db.UserConnections.Where(x => x.UserId == userId));

            return Db.SaveChanges();
        }

        public int DeleteUserConnection(int userId, string sessionId)
        {
            Db.UserConnections.RemoveRange(Db.UserConnections.Where(x => x.UserId == userId && x.SessionId == sessionId));

            return Db.SaveChanges();
        }

        public GroupChatUserInfoMeta GroupChatGetUserByUserName(string userName)
        {
            var userInfo = Db.Users.Where(u => u.UserName.Equals(userName) && u.Status < 5 && !u.IsDelete)
                .Join(Db.UserPositions.Where(ut => ut.IsDefault), u => u.Id, ut => ut.UserId,
                (u, ut) => new { u.Id, u.UserName, u.FullName, u.Avatar, u.IsDelete, ut.OfficeId, ut.OfficeName, u.UnsignName, ut.TitleId, ut.TitleName })
                .FirstOrDefault();

            if (userInfo == null)
                return null;

            var groupChatUserInfo = Db.GroupChatUsers.FirstOrDefault(gcu => gcu.UserId == userInfo.Id);

            if (groupChatUserInfo == null)
                return null;

            var userInfoMeta = new GroupChatUserInfoMeta
            {
                Id = userInfo.Id,
                UserName = userInfo.UserName,
                FullName = userInfo.FullName,
                Image = userInfo.Avatar,
                NotifyUrl = groupChatUserInfo.NotifyUrl,
                TitleName = userInfo.TitleName,
                OfficeId = userInfo.OfficeId,
                OfficeName = userInfo.OfficeName,
                UnsignName = userInfo.UnsignName
            };

            return userInfoMeta;
        }

        /// <summary>
        /// lấy danh sách nhân viên có điều kiện là nhân viên theo phòng
        /// </summary>
        /// <param name="type"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        public Task<List<UserOfficeResult>> GetUserToOfficeType(OfficeType type, UserState userState)
        {
            var query =
                 Db.Users.Where(x => !x.IsDelete && !x.IsCompany)
                     .Join(
                         Db.UserPositions.Where(x=> x.IsDefault),
                             user => user.Id,
                             position => position.UserId,
                         (u, p) => new { u, p })
                         .Join(
                            Db.Offices.Where(x =>
                                !x.IsDelete
                                && x.Type == (byte)type
                                && (x.IdPath == userState.OfficeIdPath || x.IdPath.StartsWith(userState.OfficeIdPath + "."))),
                            group => group.p.OfficeId,
                            office => office.Id,
                            (g, o) => new UserOfficeResult()
                            {
                                Id = g.u.Id, // Id (Primary key)
                                UserName = g.u.UserName, // UserName (length: 50)
                                FullName = g.u.FullName, // FullName (length: 100)
                                Email = g.u.Email, // Email (length: 50)
                                Avatar = g.u.Avatar, // Avatar (length: 2000)
                                TitleId = g.p.TitleId, // TitleId (Primary key)
                                OfficeId = g.p.OfficeId, // OfficeId (Primary key)
                                TitleName = g.p.TitleName, // TitleName (length: 300)
                                OfficeName = g.p.OfficeName, // OfficeName (length: 300
                                Type = g.p.Type, // Type
                                OfficeIdPath = g.p.OfficeIdPath, // OfficeIdPath (length: 500)
                                OfficeNamePath = g.p.OfficeNamePath, // OfficeNamePath (length: 2000)
                                LevelId = g.p.LevelId, // LevelId
                                LevelName = g.p.LevelName,
                                Birthday = g.u.Birthday,
                                TypeId = g.u.TypeId,
                                TypeName = g.u.TypeName,
                                Created = g.u.Created,
                                StartDate = g.u.StartDate,
                                Gender = g.u.Gender
                                // LevelName (length: 300)
                            });

            return query.OrderBy(x => x.Id).ToListAsync();
        }

        public Task<List<UserOfficeResult>> GetUserToOfficeType(out long totalRecord, int page, int pageSize, int? userId, OfficeType type)
        {
            var query =
                 Db.Users.Where(x => !x.IsDelete && !x.IsCompany && (userId == null || x.Id == userId))
                     .Join(
                         Db.UserPositions.Where(x=> x.IsDefault),
                             user => user.Id,
                             position => position.UserId,
                         (u, p) => new { u, p })
                         .Join(
                            Db.Offices.Where(x =>
                                !x.IsDelete
                                && x.Type == (byte)type),
                            group => group.p.OfficeId,
                            office => office.Id,
                            (g, o) => new UserOfficeResult()
                            {
                                Id = g.u.Id, // Id (Primary key)
                                UserName = g.u.UserName, // UserName (length: 50)
                                FullName = g.u.FullName, // FullName (length: 100)
                                Email = g.u.Email, // Email (length: 50)
                                Avatar = g.u.Avatar, // Avatar (length: 2000)
                                TitleId = g.p.TitleId, // TitleId (Primary key)
                                OfficeId = g.p.OfficeId, // OfficeId (Primary key)
                                TitleName = g.p.TitleName, // TitleName (length: 300)
                                OfficeName = g.p.OfficeName, // OfficeName (length: 300
                                Type = g.p.Type, // Type
                                OfficeIdPath = g.p.OfficeIdPath, // OfficeIdPath (length: 500)
                                OfficeNamePath = g.p.OfficeNamePath, // OfficeNamePath (length: 2000)
                                LevelId = g.p.LevelId, // LevelId
                                LevelName = g.p.LevelName,
                                Birthday = g.u.Birthday,
                                TypeId = g.u.TypeId,
                                TypeName = g.u.TypeName,
                                Created = g.u.Created,
                                StartDate = g.u.StartDate,
                                Gender = g.u.Gender,
                                Websites = g.u.Websites
                                // LevelName (length: 300)
                            }).OrderBy(x => x.FullName);

            totalRecord = query.Count();

            return query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public Task<List<UserResult>> RecentSuggetion(int userId, RecentMode mode)
        {
            return Db.Users.Where(x => x.IsDelete == false && x.IsSystem == false && x.IsCompany == false)
                .Join(Db.UserPositions.Where(x=> x.Type == 3), u=> u.Id, up=> up.UserId, (u, up)=> new {u, up})
                .Join(Db.Recents.Where(x => x.Mode == (byte)mode && x.UserId == userId), arg => arg.u.Id,
                    recent => recent.RecordId,
                    (arg, recents) => new UserResult()
                    {
                        Id = arg.u.Id,
                        UserName = arg.u.UserName,
                        Password = arg.u.Password,
                        FirstName = arg.u.FirstName,
                        MidleName = arg.u.MidleName,
                        LastName = arg.u.LastName,
                        FullName = arg.u.FullName,
                        Gender = arg.u.Gender,
                        Email = arg.u.Email,
                        Description = arg.u.Description,
                        Created = arg.u.Created,
                        Updated = arg.u.Updated,
                        LastUpdateUserId = arg.u.LastUpdateUserId,
                        IsDelete = arg.u.IsDelete,
                        Status = arg.u.Status,
                        Birthday = arg.u.Birthday,
                        StartDate = arg.u.StartDate,
                        Avatar = arg.u.Avatar,
                        TitleId = arg.up.TitleId,
                        OfficeId = arg.up.OfficeId,
                        TitleName = arg.up.TitleName,
                        OfficeName = arg.up.OfficeName,
                        IsDefault = arg.up.IsDefault,
                        Type = arg.up.Type,
                        OfficeIdPath = arg.up.OfficeIdPath,
                        OfficeNamePath = arg.up.OfficeNamePath,
                        DirectUserId = arg.up.DirectUserId,
                        DirectFullName = arg.up.DirectFullName,
                        DirectTitleId = arg.up.DirectTitleId,
                        DirectTitleName = arg.up.DirectTitleName,
                        DirectOfficeId = arg.up.DirectOfficeId,
                        DirectOfficeName = arg.up.DirectOfficeName,
                        ApprovalUserId = arg.up.ApprovalUserId,
                        ApprovalFullName = arg.up.ApprovalFullName,
                        ApprovalTitleId = arg.up.ApprovalTitleId,
                        ApprovalTitleName = arg.up.ApprovalTitleName,
                        ApprovalOfficeId = arg.up.ApprovalOfficeId,
                        ApprovalOfficeName = arg.up.ApprovalOfficeName,
                        LevelId = arg.up.LevelId,
                        LevelName = arg.up.LevelName,
                        GroupPermisionId = arg.up.GroupPermisionId,
                        GroupPermissionName = arg.up.GroupPermissionName,
                    })
                .Distinct()
                .ToListAsync();
        }

        public Task<List<UserResult>> SearchShipper(string keyword, int pageIndex, int pageSize, out int totalRecord)
        {
            var query = Db.Users.Where(x => x.IsDelete == false && x.IsSystem == false && x.IsCompany == false
            && x.UnsignName.Contains(keyword))
                .Join(Db.UserPositions.Where(x => x.Type == 3), u => u.Id, up => up.UserId, (u, up) => new UserResult()
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Password = u.Password,
                    FirstName = u.FirstName,
                    MidleName = u.MidleName,
                    LastName = u.LastName,
                    FullName = u.FullName,
                    Gender = u.Gender,
                    Email = u.Email,
                    Description = u.Description,
                    Created = u.Created,
                    Updated = u.Updated,
                    LastUpdateUserId = u.LastUpdateUserId,
                    IsDelete = u.IsDelete,
                    Status = u.Status,
                    Birthday = u.Birthday,
                    StartDate = u.StartDate,
                    Avatar = u.Avatar,
                    TitleId = up.TitleId,
                    OfficeId = up.OfficeId,
                    TitleName = up.TitleName,
                    OfficeName = up.OfficeName,
                    IsDefault = up.IsDefault,
                    Type = up.Type,
                    OfficeIdPath = up.OfficeIdPath,
                    OfficeNamePath = up.OfficeNamePath,
                    DirectUserId = up.DirectUserId,
                    DirectFullName = up.DirectFullName,
                    DirectTitleId = up.DirectTitleId,
                    DirectTitleName = up.DirectTitleName,
                    DirectOfficeId = up.DirectOfficeId,
                    DirectOfficeName = up.DirectOfficeName,
                    ApprovalUserId = up.ApprovalUserId,
                    ApprovalFullName = up.ApprovalFullName,
                    ApprovalTitleId = up.ApprovalTitleId,
                    ApprovalTitleName = up.ApprovalTitleName,
                    ApprovalOfficeId = up.ApprovalOfficeId,
                    ApprovalOfficeName = up.ApprovalOfficeName,
                    LevelId = up.LevelId,
                    LevelName = up.LevelName,
                    GroupPermisionId = up.GroupPermisionId,
                    GroupPermissionName = up.GroupPermissionName,
                });

            totalRecord = query.Count();

            return query.OrderBy(x => x.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public Task<List<UserResult>> SearchUser(string keyword, int pageIndex, int pageSize, out int totalRecord)
        {
            var query = Db.Users.Where(x => x.IsDelete == false && x.IsSystem == false && x.IsCompany == false
            && x.UnsignName.Contains(keyword))
                .Join(Db.UserPositions.Where(x => x.IsDefault), u => u.Id, up => up.UserId, (u, up) => new UserResult()
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Password = u.Password,
                    FirstName = u.FirstName,
                    MidleName = u.MidleName,
                    LastName = u.LastName,
                    FullName = u.FullName,
                    Gender = u.Gender,
                    Email = u.Email,
                    Description = u.Description,
                    Created = u.Created,
                    Updated = u.Updated,
                    LastUpdateUserId = u.LastUpdateUserId,
                    IsDelete = u.IsDelete,
                    Status = u.Status,
                    Birthday = u.Birthday,
                    StartDate = u.StartDate,
                    Avatar = u.Avatar,
                    TitleId = up.TitleId,
                    OfficeId = up.OfficeId,
                    TitleName = up.TitleName,
                    OfficeName = up.OfficeName,
                    IsDefault = up.IsDefault,
                    Type = up.Type,
                    OfficeIdPath = up.OfficeIdPath,
                    OfficeNamePath = up.OfficeNamePath,
                    DirectUserId = up.DirectUserId,
                    DirectFullName = up.DirectFullName,
                    DirectTitleId = up.DirectTitleId,
                    DirectTitleName = up.DirectTitleName,
                    DirectOfficeId = up.DirectOfficeId,
                    DirectOfficeName = up.DirectOfficeName,
                    ApprovalUserId = up.ApprovalUserId,
                    ApprovalFullName = up.ApprovalFullName,
                    ApprovalTitleId = up.ApprovalTitleId,
                    ApprovalTitleName = up.ApprovalTitleName,
                    ApprovalOfficeId = up.ApprovalOfficeId,
                    ApprovalOfficeName = up.ApprovalOfficeName,
                    LevelId = up.LevelId,
                    LevelName = up.LevelName,
                    GroupPermisionId = up.GroupPermisionId,
                    GroupPermissionName = up.GroupPermissionName,
                });

            totalRecord = query.Count();

            return query.OrderBy(x => x.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }
    }
}
