using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using Common.Emums;
using Common.Helper;
using Library.DbContext.Entities;
using Library.Models;

namespace Library.Jobs
{
    public class PackageJob
    {
        ///// <summary>
        ///// Cập nhật lại tổng tiền của các kiện trong đơn hàng
        ///// </summary>
        ///// <param name="orderIds"></param>
        //public static void UpdateTotalPrice(List<int> orderIds)
        //{
        //    using (var unitOfWork = new UnitOfWork.UnitOfWork())
        //    {
        //        using (var transaction = unitOfWork.DbContext.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                foreach (var orderId in orderIds)
        //                {
        //                    var orderWeigthActual =
        //                        unitOfWork.OrderPackageRepo.Any(
        //                            x => x.IsDelete == false && x.OrderId == orderId && x.WeightActual != null)
        //                            ? unitOfWork.OrderPackageRepo.Entities
        //                                .Where(x => x.IsDelete == false && x.OrderId == orderId && x.WeightActual != null)
        //                                .Sum(x => x.WeightActual)
        //                            : 0;

        //                    var packages = unitOfWork.OrderPackageRepo.Find(
        //                        x => x.IsDelete == false && x.OrderId == orderId && x.WeightActual != null);

        //                    foreach (var p in packages)
        //                    {
        //                        p.WeightActualPercent = p.WeightActual * 100 / orderWeigthActual;
        //                    }
        //                }

        //                unitOfWork.PermissionActionRepo.Save();

        //                transaction.Commit();
        //            }
        //            catch (Exception ex)
        //            {
        //                transaction.Rollback();
        //                ExceptionDispatchInfo.Capture(ex).Throw();
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Xử lý trùng mã vận đơn
        /// </summary>
        /// <param name="transportCode">Mã vận đơn bị trùng</param>
        /// <param name="packageCode">Mã kiện hàng</param>
        public static void UpdateSameTransportCode(string transportCode, string packageCode, UnitOfWork.UnitOfWork unitOfWork, UserState userState)
        {
            //var unitOfWork = new UnitOfWork.UnitOfWork();
                var packages = unitOfWork.OrderPackageRepo.Find(
                        x => x.IsDelete == false && x.TransportCode == transportCode &&
                            x.OrderId > 0)
                    .ToList();

                if (packages.Count <= 1)
                    return;

                var mode = new List<byte>();

                // Trùng khách khách
                if (packages.Select(x => x.CustomerId).Distinct().Count() > 1)
                {
                    mode.Add(1);
                }

                if (packages.Select(x => x.CustomerWarehouseId).Distinct().Count() > 1) // Trùng mã khác kho
                {
                    mode.Add(0);
                }

                var currentPackage = packages.First(x => x.Code == packageCode);
                var orderDiff =
                    packages.Where(x => x.OrderId != currentPackage.OrderId)
                        .Select(x => x.OrderCode)
                        .Distinct()
                        .Select(MyCommon.ReturnCode)
                        .ToList();

                string note = $"[TM] {string.Join(", ", orderDiff)} - {string.Join(", ", packages.Select(x => "P" + x.Code).ToList())} [TM]";
                string process;

                // Tìm phương án xử lý
                // Trùng mã cùng khách, cùng kho
                if (!mode.Any())
                {
                    process = "[XL] Tạo kiện ảo [XL]";
                }
                else
                {
                    // Lấy ra các mã bao gỗ của kiện hàng.
                    process = packages.Any(x => x.Status >= (byte)OrderPackageStatus.ChinaExport) ?
                        "[XL] Tách kiện, Chỉnh cân kiện, Chỉnh cân bao gỗ, bao tải  [XL]" : "[XL] Tách kiện, Chỉnh cân nặng kiện [XL]";
                }

                // Cập nhật thông tin các kiện hàng bị trùng mã
                foreach (var p in packages)
                {
                    p.Mode = mode.Any() ? $";{string.Join(";", mode)};" : string.Empty;
                    p.Note = MyCommon.RemoveHash(MyCommon.RemoveHash(p.Note, "[TM]"), "[XL]");
                    p.Note = $"{p.Note} {note} {process}";
                    p.SameCodeStatus = 0;


                //Ghi chú toàn hệ thống cho kiện hàng, đơn hàng
                    var packageNote = unitOfWork.PackageNoteRepo.SingleOrDefault(
                        x =>
                            x.PackageId == p.Id && x.OrderId == p.OrderId && x.ObjectId == null &&
                            x.Mode == (byte) PackageNoteMode.Order);

                if (packageNote == null && !string.IsNullOrWhiteSpace(p.Note))
                {
                    unitOfWork.PackageNoteRepo.Add(new PackageNote()
                    {
                        OrderId = p.OrderId,
                        OrderCode = p.OrderCode,
                        PackageId = p.Id,
                        PackageCode = p.Code,
                        UserId = userState.UserId,
                        UserFullName = userState.FullName,
                        Time = DateTime.Now,
                        ObjectId = null,
                        ObjectCode = string.Empty,
                        Mode = (byte)PackageNoteMode.Order,
                        Content = p.Note
                    });
                }
                else if (packageNote != null && !string.IsNullOrWhiteSpace(p.Note))
                {
                    packageNote.Content = p.Note;
                }
                else if (packageNote != null && string.IsNullOrWhiteSpace(p.Note))
                {
                    unitOfWork.PackageNoteRepo.Remove(packageNote);
                }
            }

            unitOfWork.OrderPackageRepo.Save();
        }

        /// <summary>
        /// Cập nhật thông tin đươn hàng của kiện hàng (Wallet, WalletDetail, PutAway, PutAwayDetail, Transfer, TransferDetail)
        /// </summary>
        /// <param name="packageId">Id kiện hàng</param>
        public static void UpdateOrder(int packageId)
        {
            using (var unitOfWork = new UnitOfWork.UnitOfWork())
            {
                using (var transaction = unitOfWork.DbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var package = unitOfWork.OrderPackageRepo.SingleOrDefault(x => x.Id == packageId);

                        if (package == null)
                            return;


                        #region Cập nhật Nhập kho

                        // Cập nhật chi tiết nhập kho
                        var importWarehouseDetails = unitOfWork.ImportWarehouseDetailRepo.Find(x => x.IsDelete == false && x.PackageId == package.Id).ToList();

                        foreach (var import in importWarehouseDetails)
                        {
                            import.OrderId = package.OrderId;
                            import.OrderCode = package.OrderCode;
                            import.OrderType = package.OrderType;
                            import.OrderServices = package.OrderServices;
                            import.OrderPackageNo = package.PackageNo;
                            import.CustomerId = package.CustomerId;
                            import.CustomerName = package.CustomerName;
                            import.CustomerUserName = package.CustomerUserName;
                        }

                        unitOfWork.ImportWarehouseDetailRepo.Save();

                        #endregion

                        #region Cập nhật lịch sử kiện

                        // Cập nhật lịch sử kiện
                        var packageHistores = unitOfWork.PackageHistoryRepo.Find(x => x.PackageId == package.Id).ToList();

                        foreach (var history in packageHistores)
                        {
                            history.OrderId = package.OrderId;
                            history.OrderCode = package.OrderCode;
                            history.CustomerId = package.CustomerId;
                            history.CustomerName = package.CustomerName;
                        }

                        unitOfWork.PackageHistoryRepo.Save();

                        #endregion

                        #region Cập ghi chú kiện

                        // Cập nhật ghi chú kiện hàng
                        var packageNotes = unitOfWork.PackageNoteRepo.Find(x => x.PackageId == package.Id).ToList();

                        foreach (var pNote in packageNotes)
                        {
                            pNote.OrderId = package.OrderId;
                            pNote.OrderCode = package.OrderCode;
                        }

                        unitOfWork.PackageNoteRepo.Save();

                        #endregion


                        #region Cập nhật theo dõi nợ

                        // Cập nhật theo dõi nợ
                        var depitReports = unitOfWork.DebitReportRepo.Find(x =>  x.PackageId != null && x.PackageId == package.Id).ToList();

                        foreach (var debit in depitReports)
                        {
                            debit.OrderId = package.OrderId;
                            debit.OrderCode = package.OrderCode;
                            debit.CustomerId = package.CustomerId;
                        }

                        unitOfWork.ImportWarehouseDetailRepo.Save();

                        #endregion

                        #region Cập nhật PutAway

                        // Cập nhật PutAway Detial
                        var putAwayDetail = unitOfWork.PutAwayDetailRepo.Find(x => x.IsDelete == false && x.PackageId == package.Id).ToList();

                        foreach (var putDetail in putAwayDetail)
                        {
                            putDetail.OrderId = package.OrderId;
                            putDetail.OrderCode = package.OrderCode;
                            putDetail.OrderType = package.OrderType;
                            putDetail.OrderServices = package.OrderServices;
                            putDetail.OrderPackageNo = package.PackageNo;
                            putDetail.CustomerId = package.CustomerId;
                            putDetail.CustomerName = package.CustomerName;
                            putDetail.CustomerUserName = package.CustomerUserName;
                        }

                        unitOfWork.PutAwayDetailRepo.Save();

                        #endregion

                        #region Cập nhật Wallet

                        // Cập nhật WalletDetail
                        var walletDetails = unitOfWork.WalletDetailRepo.Find(x => x.IsDelete == false && x.PackageId == package.Id).ToList();

                        foreach (var walletDetail in walletDetails)
                        {
                            walletDetail.OrderId = package.OrderId;
                            walletDetail.OrderCode = package.OrderCode;
                            walletDetail.OrderType = package.OrderType;
                            walletDetail.OrderServices = package.OrderServices;
                            walletDetail.OrderPackageNo = package.PackageNo;
                        }

                        unitOfWork.WalletRepo.Save();
                        #endregion

                        #region Cập nhật Transfer

                        // Cập nhật TransferDetail
                        var transferDetails = unitOfWork.TransferDetailRepo.Find(x => x.IsDelete == false && x.PackageId == package.Id).ToList();

                        foreach (var transferDetail in transferDetails)
                        {
                            transferDetail.OrderId = package.OrderId;
                            transferDetail.OrderCode = package.OrderCode;
                            transferDetail.OrderType = package.OrderType;
                            transferDetail.OrderServices = package.OrderServices;
                            transferDetail.OrderPackageNo = package.PackageNo;
                        }
                        unitOfWork.TransferRepo.Save();

                        #endregion

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


        /// <summary>
        /// Cập nhật lại cân nặng của kiện trong (Wallet, WalletDetail, PutAway, PutAwayDetail, Transfer, TransferDetail)
        /// </summary>
        /// <param name="packageId">Id kiện hàng</param>
        public static void UpdateWeight(int packageId)
        {
            using (var unitOfWork = new UnitOfWork.UnitOfWork())
            {
                using (var transaction = unitOfWork.DbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var package = unitOfWork.OrderPackageRepo.SingleOrDefault(x => x.Id == packageId);

                        if (package == null)
                            return;

                        #region Cập nhật PutAway

                        // Cập nhật PutAway Detial
                        var putAwayDetail = unitOfWork.PutAwayDetailRepo.Find(x => x.IsDelete == false && x.PackageId == package.Id).ToList();

                        foreach (var putDetail in putAwayDetail)
                        {
                            putDetail.Weight = package.Weight ?? 0;
                            putDetail.Height = package.Height ?? 0;
                            putDetail.Length = package.Length ?? 0;
                            putDetail.Width = package.Width ?? 0;
                            putDetail.ActualWeight = package.WeightActual ?? 0;
                            putDetail.Size = package.Size;
                            putDetail.ConvertedWeight = package.WeightConverted ?? 0;
                        }

                        // Cập nhật Putaway
                        var putAwayCodes =
                            $";{string.Join(";", putAwayDetail.Select(x => x.PutAwayCode).Distinct().ToList())};";

                        var putAways =
                            unitOfWork.PutAwayRepo.Find(
                                x => x.IsDelete == false && putAwayCodes.Contains(";" + x.Code + ";"));

                        foreach (var putway in putAways)
                        {
                            var details =
                                unitOfWork.PutAwayDetailRepo.Find(x => x.IsDelete == false && x.PutAwayId == putway.Id)
                                    .ToList();

                            putway.TotalWeight = details.Sum(x => x.Weight);
                            putway.TotalActualWeight = details.Sum(x => x.ActualWeight);
                            putway.TotalConversionWeight = details.Sum(x => x.ConvertedWeight);
                        }

                        unitOfWork.PutAwayRepo.Save();

                        #endregion

                        #region Cập nhật Wallet

                        // Cập nhật WalletDetail
                        var walletDetails = unitOfWork.WalletDetailRepo.Find(x => x.IsDelete == false && x.PackageId == package.Id).ToList();

                        foreach (var walletDetail in walletDetails)
                        {
                            walletDetail.Weight = package.Weight ?? 0;
                            walletDetail.ActualWeight = package.WeightActual ?? 0;
                            walletDetail.Volume = package.Volume;
                            walletDetail.ConvertedWeight = package.WeightConverted ?? 0;
                        }

                        // Cập nhật Wallet
                        var walletCodes =
                            $";{string.Join(";", walletDetails.Select(x => x.WalletCode).Distinct().ToList())};";

                        var wallets =
                            unitOfWork.WalletRepo.Find(
                                x => x.IsDelete == false && walletCodes.Contains(";" + x.Code + ";"));

                        foreach (var wallet in wallets)
                        {
                            var details =
                                unitOfWork.WalletDetailRepo.Find(x => x.IsDelete == false && x.WalletId == wallet.Id)
                                    .ToList();

                            wallet.TotalWeight = details.Sum(x => x.Weight ?? 0);
                            wallet.TotalWeightActual = details.Sum(x => x.ActualWeight ?? 0);
                            wallet.TotalWeightConverted = details.Sum(x => x.ConvertedWeight ?? 0);
                            wallet.TotalVolume = details.Sum(x => x.Volume ?? 0);
                        }

                        unitOfWork.WalletRepo.Save();

                        #endregion

                        #region Cập nhật Transfer

                        // Cập nhật TransferDetail
                        var transferDetails = unitOfWork.TransferDetailRepo.Find(x => x.IsDelete == false && x.PackageId == package.Id).ToList();

                        foreach (var transferDetail in transferDetails)
                        {
                            transferDetail.Weight = package.Weight ?? 0;
                            transferDetail.WeightActual= package.WeightActual ?? 0;
                            transferDetail.WeightConverted = package.WeightConverted ?? 0;
                        }

                        // Cập nhật Transfers
                        var transferCodes =
                            $";{string.Join(";", transferDetails.Select(x => x.TransferCode).Distinct().ToList())};";

                        var transfers =
                            unitOfWork.TransferRepo.Find(
                                x => x.IsDelete == false && transferCodes.Contains(";" + x.Code + ";"));

                        foreach (var transfer in transfers)
                        {
                            var details =
                                unitOfWork.TransferDetailRepo.Find(x => x.IsDelete == false && x.TransferId == transfer.Id)
                                    .ToList();

                            transfer.TotalWeight = details.Sum(x => x.Weight ?? 0);
                            transfer.TotalWeightActual = details.Sum(x => x.WeightActual ?? 0);
                            transfer.TotalWeightConverted = details.Sum(x => x.WeightConverted ?? 0);
                        }

                        unitOfWork.TransferRepo.Save();

                        #endregion

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

        /// <summary>
        /// Cập nhật lại % cân nặng của các kiện hàng trong đơn hàng
        /// </summary>
        /// <param name="orderIds"></param>
        public static void UpdateWeightActualPercent(List<int> orderIds)
        {
            using (var unitOfWork = new UnitOfWork.UnitOfWork())
            {
                using (var transaction = unitOfWork.DbContext.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var orderId in orderIds)
                        {
                            var orderWeigthActual = unitOfWork.OrderPackageRepo.Find(
                                    x => x.IsDelete == false && x.OrderId == orderId && x.WeightActual != null)
                                .Sum(x => x.WeightActual);

                            var packages = unitOfWork.OrderPackageRepo.Find(
                                x => x.IsDelete == false && x.OrderId == orderId && x.WeightActual != null).ToList();


                            var index = 1;
                            var totalWeightActualPercent = 0M;
                            foreach (var p in packages)
                            {
                                // Không phải kiện cuối cùng
                                if (index != packages.Count())
                                {
                                    p.WeightActualPercent = Math.Round((p.WeightActual ?? 0) * 100 / (orderWeigthActual ?? 1), 4);
                                    totalWeightActualPercent += p.WeightActualPercent.Value;
                                }
                                else
                                {
                                    p.WeightActualPercent = 100 - totalWeightActualPercent;
                                }
                                index++;
                            }
                        }

                        unitOfWork.OrderPackageRepo.Save();

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

        /// <summary>
        /// Cập nhật lại dịch vụ phát sinh của các kiện hàng trong đơn hàng
        /// </summary>
        /// <param name="orderIds"></param>
        public static void UpdateOtherServiceOfPackage(List<int> orderIds)
        {
            using (var unitOfWork = new UnitOfWork.UnitOfWork())
            {
                using (var transaction = unitOfWork.DbContext.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var orderId in orderIds)
                        {
                            var serviceOthers = unitOfWork.OrderServiceOtherRepo.Find(
                                x => x.OrderId == orderId && x.Type == 2);

                            var packageFirst = new List<int>();

                            // Cập nhật lại tổng cân nặng của đơn
                            foreach (var serviceOther in serviceOthers)
                            {
                                var packagesServiceOther = serviceOther.Type == 2
                                    ? unitOfWork.OrderPackageRepo.GetByOrderIdAndWalletId(orderId,
                                        serviceOther.ObjectId)
                                    : unitOfWork.OrderPackageRepo.GetByOrderIdAndImportWarehouseId(orderId,
                                        serviceOther.ObjectId);

                                unitOfWork.OrderPackageRepo.GetByOrderIdAndWalletId(orderId,
                                    serviceOther.ObjectId);

                                serviceOther.TotalWeightActual = packagesServiceOther.Sum(x => x.WeightActual);

                                var totalPercent = 0M;
                                var index = 1;

                                // Tính lại cân nặng trong kiên hàng
                                foreach (var p in packagesServiceOther)
                                {
                                    decimal percent;
                                    // Không phải kiện cuối cùng
                                    if (index != packagesServiceOther.Count)
                                    {
                                        percent = Math.Round((p.WeightActual ?? 0) * 100 / (serviceOther.TotalWeightActual ?? 1), 4);
                                        totalPercent += percent;
                                    }
                                    else
                                    {
                                        percent = 100 - totalPercent;
                                    }

                                    if (packageFirst.Any(x => x == p.Id))
                                    {
                                        p.OtherService += percent * serviceOther.TotalPrice / 100;
                                    }
                                    else
                                    {
                                        p.OtherService = percent * serviceOther.TotalPrice / 100;
                                        packageFirst.Add(p.Id);
                                    }
                                    index++;
                                }
                            }
                        }

                        unitOfWork.OrderPackageRepo.Save();

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