using Library.DbContext.Entities;
using Library.UnitOfWork;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity;
using System;
using Library.Models;
namespace Library.DbContext.Repositories
{
    public class FundBillRepository : Repository<FundBill>
    {
        public FundBillRepository(ProjectXContext dbContext) : base(dbContext)
        {
        }
        //Lay so du danh sach quy
        public Task<List<FinanceFund>> FinanceFundBalanceList()
        {
            var list = Db.FinanceFunds.
                GroupJoin(Db.FinanceFunds,
                a => a.Id,
                b => b.ParentId,
                (a, b) => new { a, b }
                ).Select(
                s => new FinanceFund()
                {
                    Id = s.a.Id,
                    IdPath = s.a.IdPath,
                    ParentId = s.a.ParentId,
                    IsParent = s.a.IsParent,
                    Currency = s.a.Currency,
                    Balance = s.a.IsParent == true ? s.b.Sum(x => x.Balance) : s.a.Balance
                }
                ).OrderBy(x => x.IdPath).ToListAsync();
            return list;
        }

        public FundBill FundBillFinanceFund(string curence, DateTime date, DateTime end)
        {

            var fu = new FundBill();
            fu.Increase = 0;
            fu.Diminishe = 0;
            var list = Db.FinanceFunds.Where(s => !s.IsDelete
                                                && s.Currency == curence)
                                                .GroupJoin(Db.FundBill.Where(h => !h.IsDelete
                                                && h.LastUpdated >= date
                                                && h.LastUpdated <= end),
                                                ff => ff.Id,
                                                fb => fb.FinanceFundId.Value,
                                                (ff, fb) => new { ff, fb }).ToList();
            var listFund = list.Select(
                                                x => new FundBill()
                                                {
                                                    FinanceFundId = x.ff.Id,
                                                    FinanceFundName = x.ff.Name,
                                                    Increase = x.fb.Sum(d => d.Increase ?? 0),
                                                    Diminishe = x.fb.Sum(d => d.Diminishe ?? 0),

                                                }).ToList();
            foreach (var item in listFund)
            {
                fu.Increase += item.Increase;
                fu.Diminishe += item.Diminishe;
            }
            return fu;
        }

        public RechargeBill RechargeBillFinanceFund(DateTime date, DateTime end)
        {
            var re = new RechargeBill();
            re.Increase = 0;
            re.Diminishe = 0;
            var list = Db.RechargeBill.Where(s => !s.IsDelete
                                                && s.LastUpdated >= date
                                                && s.LastUpdated <= end);
            foreach (var item in list)
            {
                re.Increase += item.Increase;
                re.Diminishe += item.Diminishe;
            }
            return re;
        }

        #region [Thống kê tình hình thu chi quỹ theo thời gian]
        public List<ProfitDay> GetAccountSituationOnTime(DateTime startDay, DateTime endDay, DateTime now, int status, int? financeFundId, byte increase, byte diminishe)
        {
            var listFund = Db.FundBill.Where(h => !h.IsDelete
                                                 && h.LastUpdated >= startDay
                                                 && h.LastUpdated <= endDay && h.Status == status).ToList();
            var fund = Db.FinanceFunds.FirstOrDefault(s => s.Id == (financeFundId ?? 1));
            decimal balance = 0;


            var list = new List<DateTime>();
            DateTime tmpDate = startDay;
            do
            {
                list.Add(tmpDate);
                tmpDate = tmpDate.AddDays(1);
            } while (tmpDate <= endDay);
            var listPr = new List<ProfitDay>();


            if (fund != null)
            {

                foreach (var item in list)
                {
                    var pr = new ProfitDay();

                    //Lấy ra tổng thu chi trong ngày
                    if (item.Date <= now)
                    {
                        balance = fund.Balance;

                        var fullbill1 = listFund.Where(s => s.FinanceFundId == financeFundId && s.LastUpdated > item);
                        var fullbill2 = listFund.Where(s => s.FinanceFundId == financeFundId && s.LastUpdated.Year == item.Year && s.LastUpdated.Month == item.Month && s.LastUpdated.Day == item.Day);
                        foreach (var item1 in fullbill2)
                        {
                            pr.Diminishe += item1.Diminishe ?? 0;
                            pr.Increase += item1.Increase ?? 0;
                        }

                        //Lấy ra tồn quỹ
                        foreach (var item1 in fullbill1)
                        {
                            if (item1.Type == diminishe)
                            {
                                balance += item1.Diminishe ?? 0;
                            }
                            else
                            {
                                balance -= item1.Increase ?? 0;
                            }
                        }
                        pr.Balance = balance;
                        pr.Created = item.ToShortDateString();
                        listPr.Add(pr);
                    }

                }

            }
            return listPr.ToList();
        }
        #endregion

        //#region [Lay ve danh sach phat sinh giao dich quy]
        //public Task<List<TicketComplain>> GetAllFundBillList(out long totalRecord, int page, int pageSize, string keyword, int userId, int? status,
        //   int systemId, DateTime? dateStart, DateTime? dateEnd, int? financeFundId, int tresure, decimal? currencyFluctuations)
        //{
        //    var listFinanceFundId = Db.FinanceFunds.Where(x => !x.IsDelete && (x.IdPath == searchModal.FinanceFundIdPath || x.IdPath.StartsWith(searchModal.FinanceFundIdPath))).Select(x => x.Id).ToList();
        //    var query = Db.FundBill.Where(
        //            x => (x.Code.Contains(keyword) || x.OrderCode == keyword
        //            || x.FinanceFundUserEmail.Contains(keyword)
        //            || x.SubjectEmail.Contains(keyword))
        //                && !x.IsDelete
        //                && (financeFundId == null || financeFundId == 0 || listFinanceFundId.Contains(x.FinanceFundId ?? 0))
        //                && (searchModal.UserId == null || searchModal.UserId == 0 || x.UserId == searchModal.UserId)
        //                && (searchModal.TreasureId == null || searchModal.TreasureId == 0 || x.TreasureId == searchModal.TreasureId)
        //                && (searchModal.Status == -1 || x.Status == searchModal.Status)
        //                && (searchModal.TypeId == -1 || x.Type == searchModal.TypeId)
        //                && (searchModal.CurrencyFluctuations == null || x.CurrencyFluctuations == searchModal.CurrencyFluctuations.Value)
        //                //&& (searchModal.FinanceFundId == -1 || x.FinanceFundId == searchModal.FinanceFundId)
        //                && (searchModal.AccountantSubjectId == -1 || x.AccountantSubjectId == searchModal.AccountantSubjectId)
        //                && x.LastUpdated >= dateStart && x.LastUpdated <= dateEnd
        //        )
        //        .OrderByDescending(y => y.Created);
        //    totalRecord = query.Count();
        //    return query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        //}
        //#endregion
    }
}
