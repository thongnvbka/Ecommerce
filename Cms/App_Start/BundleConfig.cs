using System.Web;
using System.Web.Optimization;
using Library.Models;

namespace Cms
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/order").Include(
                    "~/Content/plugins/tabdrop/css/tabdrop.css",
                    "~/Content/plugins/datepicker/datepicker3.css",
                    "~/Scripts/bootstrap3-editable-1.5.1/bootstrap3-editable/css/bootstrap-editable.css"
            ));

            bundles.Add(new StyleBundle("~/Content/package").Include(
                     "~/Scripts/bootstrap3-editable-1.5.1/bootstrap3-editable/css/bootstrap-editable.css",
                     "~/Scripts/viewmodels/suggettion/PickUserModal.css",
                     "~/Content/plugins/HenrySlider/henry-slider.css",
                     "~/Content/plugins/webui-popover/jquery.webui-popover.min.css",
                     "~/Content/plugins/wysiwyg/src/wysiwyg-editor.css"
            ));



            #region Bundles Layout

            bundles.Add(new StyleBundle("~/Content/plugin").Include(
                    //Bootstrap 3.3.6
                    "~/Content/bootstrap/css/bootstrap.min.css",
                    //Font Awesome
                    "~/Content/font-awesome.min.css",
                    "~/Content/clip-font.css",
                    //daterange picker
                    "~/Content/plugins/daterangepicker/daterangepicker.css",
                    //bootstrap datepicker
                    "~/Content/plugins/datepicker/datepicker3.css",
                    //Bootstrap time Picker
                    "~/Content/plugins/timepicker/bootstrap-timepicker.min.css",
                    //iCheck for checkboxes and radio inputs
                    "~/Content/plugins/iCheck/all.css",
                    //DATEPICKER
                    "~/Content/plugins/datepicker/datepicker3.css",
                    //DataTables
                    "~/Content/plugins/datatables/dataTables.bootstrap.css",
                    "~/Content/plugins/bootstrap-toastr/toastr.min.css",
                    "~/Content/plugins/sweetalert2/sweetalert2.min.css",
                    "~/Content/plugins/jstree/dist/themes/default/style.css",
                    "~/Content/plugins/select2/select2.min.css",
                    "~/Content/themes/base/jquery-ui.min.css",
                    "~/Content/plugins/tag-editor/jquery.tag-editor.css",
                    "~/Content/plugins/bootstrap-modal/css/bootstrap-modal-bs3patch.css",
                    "~/Content/plugins/bootstrap-modal/css/bootstrap-modal.css",
                    //Notification
                    "~/Content/NotificationCenter.css"
                    ));

            bundles.Add(new StyleBundle("~/Content/theme").Include(
                    //Theme style
                    "~/Content/dist/css/AdminLTE.css",
                    "~/Content/custom.css",
                    "~/Content/dist/css/skins/_all-skins.css"
                    ));

            bundles.Add(new ScriptBundle("~/Content/script").Include(
                    //jQuery 2.2.3
                    "~/Content/plugins/jQuery/jquery-2.2.3.min.js",
                    "~/Scripts/jquery-ui-1.12.0.min.js",
                    "~/Scripts/jquery.redirect.js",
                    //Bootstrap 3.3.6
                    "~/Content/bootstrap/js/bootstrap.min.js",
                    "~/Scripts/jquery.validate.min.js",
                    "~/Scripts/messages_vi.js",
                    "~/Client Scripts/MvcFoolproofJQueryValidation.min.js",
                    "~/Scripts/jquery.validate.unobtrusive.js",
                    "~/Client Scripts/mvcfoolproof.unobtrusive.min.js",
                    "~/Content/plugins/tabdrop/js/bootstrap-tabdrop.js",

                    "~/Scripts/modernizr-*",
                    "~/Scripts/autosize.min.js",

                    //DataTables
                    "~/Content/plugins/datatables/jquery.dataTables.min.js",
                    "~/Content/plugins/datatables/dataTables.bootstrap.min.js",

                    //globalize
                    "~/Content/plugins/globinfo/globalize.js",
                    "~/Content/plugins/globinfo/globalize.culture.vi-VN.js",

                    //Moment
                    "~/Scripts/moment-with-locales.min.js",
                    //Lodash
                    "~/Scripts/lodash.min.js",

                    //InputMask
                    "~/Content/plugins/input-mask/jquery.inputmask.js",
                    "~/Content/plugins/input-mask/jquery.inputmask.extensions.js",
                    "~/Content/plugins/input-mask/jquery.inputmask.date.extensions.js",
                    "~/Content/plugins/input-mask/jquery.inputmask.numeric.extensions.js",

                    //date - range - picker
                    "~/Content/plugins/daterangepicker/daterangepicker.js",

                    //bootstrap datepicker
                    "~/Content/plugins/datepicker/bootstrap-datepicker.js",
                    //bootstrap time picker
                    "~/Content/plugins/timepicker/bootstrap-timepicker.min.js",

                    //SlimScroll
                    "~/Content/plugins/slimScroll/jquery.slimscroll.min.js",

                    //iCheck 1.0.1
                    "~/Content/plugins/iCheck/icheck.min.js",

                    //FastClick
                    "~/Content/plugins/fastclick/fastclick.js",

                    "~/Content/plugins/bootstrap-toastr/toastr.min.js",
                    "~/Content/plugins/sweetalert2/sweetalert2.min.js",
                    "~/Content/plugins/jstree/dist/jstree.min.js",
                    "~/Content/plugins/jstree/dist/jquery.dropdownjstree.js",

                    "~/Scripts/common/common.js",
                    "~/Scripts/viewmodels/chat/chatViewModel.js",
                    "~/Content/plugins/amplify/amplify.min.js",
                    "~/Content/plugins/bootstrap-modal/js/bootstrap-modalmanager.js",
                    "~/Content/plugins/bootstrap-modal/js/bootstrap-modal.js",
                    "~/Scripts/knockout-3.4.0.js",
                    "~/Scripts/knockout.mapping-latest.js",

                    //Begin Notify center
                    "~/Content/plugins/jQueryFileUpload/vendor/jquery.ui.widget.js",
                    "~/Content/plugins/jQueryFileUpload/jquery.iframe-transport.js",
                    "~/Content/plugins/jQueryFileUpload/jquery.fileupload.js",
                    "~/Content/plugins/soundPlugin/ion.sound.js",
                    "~/Scripts/jquery.pager.js",
                    "~/Content/plugins/ckeditor/ckeditor_standard/ckeditor.js",
                    "~/Content/plugins/tag-editor/jquery.tag-editor.js",

                    "~/Scripts/models/MyMessage.js",
                    "~/Scripts/models/MyNotifications.js",

                    "~/Scripts/viewmodels/MyMessage.js",
                    "~/Scripts/viewmodels/MyNotifications.js",

                    "~/Scripts/jquery.cookie.js"
            ));
            bundles.Add(new ScriptBundle("~/Content/themescript").Include(
                    //AdminLTE App
                    "~/Content/dist/js/app.min.js",
                    //AdminLTE for demo purposes
                    "~/Content/dist/js/demo.js",
                    "~/Content/plugins/select2/select2.full.min.js",
                    "~/Content/plugins/select2/i18n/vi.js",
                    //page script
                    "~/Content/dist/js/reset.js",
                    "~/Scripts/jquery.signalR-2.2.1.min.js"
            ));

            #endregion

            #region Bundles Module đặt hàng

            bundles.Add(new ScriptBundle("~/bundles/order").Include(
                       "~/Content/plugins/datepicker/locales/bootstrap-datepicker.vi.js",
                       "~/Scripts/bootstrap3-editable-1.5.1/bootstrap3-editable/js/bootstrap-editable.min.js",
                       "~/Scripts/knockout.x-editable.js",
                       "~/Scripts/components/SearchCustomerComponent.js",
                       "~/Scripts/modernizr-2.8.3.js",
                       "~/Scripts/jquery.autogrow-min.js"
           ));


            bundles.Add(new ScriptBundle("~/bundles/orderModel").Include(
                        "~/Scripts/models/accountant/fundBillModel.js",
                        "~/Scripts/viewmodels/supporter/ticketDetailCommom.js",
                        "~/Scripts/models/complain/ClainForReFundModel.js",
                        "~/Scripts/models/complain/complainModel.js",
                        "~/Scripts/models/complain/complainUserModel.js",
                        "~/Scripts/models/complain/complainDetailModel.js",
                        "~/Scripts/models/customer/customerModel.js",
                        "~/Scripts/models/order/orderModel.js",
                        "~/Scripts/models/order/orderDetailModel.js",
                        "~/Scripts/models/order/orderDepositModel.js",
                        "~/Scripts/models/order/orderDepositDetailModel.js",
                        "~/Scripts/viewmodels/chat/chatViewModel.js",
                        "~/Scripts/viewmodels/package/packageDetailModel.js",
                        "~/Scripts/viewmodels/customer/CustomerLookUpViewModel.js",
                        "~/Scripts/viewmodels/order/orderDetailViewModel.js",
                        "~/Scripts/viewmodels/order/depositAddOrEditViewModel.js",
                        "~/Scripts/viewmodels/order/stockQuotesAddOrEditViewModel.js",
                        "~/Scripts/viewmodels/order/depositDetailViewModel.js",
                        "~/Scripts/viewmodels/order/stockQuotesViewModel.js",
                        "~/Scripts/viewmodels/order/orderAddViewModel.js",
                        "~/Scripts/viewmodels/order/orderCommerceViewModel.js",
                        "~/Scripts/viewmodels/order/orderCommerceDetailViewModel.js",
                        "~/Scripts/viewmodels/order/orderViewModel.js"
            ));
            #endregion

            #region Bundles Module kế toán

            bundles.Add(new ScriptBundle("~/bundles/accoutingModal").Include(
                        "~/Scripts/models/accountant/withDrawalModel.js",
                        "~/Scripts/viewmodels/supporter/ticketDetailCommom.js",
                        "~/Scripts/viewmodels/chat/chatViewModel.js",
                        "~/Scripts/viewmodels/package/packageDetailModel.js",
                        "~/Scripts/viewmodels/order/orderDetailViewModel.js",
                        "~/Scripts/viewmodels/order/depositDetailViewModel.js",
                        "~/Scripts/viewmodels/order/orderCommerceDetailViewModel.js",
                        "~/Scripts/models/complain/complainModel.js",
                        "~/Scripts/models/complain/complainDetailModel.js",
                        "~/Scripts/models/order/orderDepositDetailModel.js",
                        "~/Scripts/models/order/orderDepositModel.js",
                        "~/Scripts/models/order/orderDetailModel.js",
                        "~/Scripts/models/order/orderModel.js",
                        "~/Scripts/models/accountant/fundBillModel.js",
                        "~/Scripts/models/accountant/rechargeBillModel.js",
                        "~/Scripts/models/accountant/mustReturnDetailModel.js",
                        "~/Scripts/models/accountant/debitDetailModel.js",
                        "~/Scripts/models/accountant/debitHistoryModel.js",
                        "~/Scripts/models/customer/customerModel.js",
                        "~/Scripts/components/SearchCustomerComponent.js",
                        "~/Scripts/models/complain/complainModel.js",
                        "~/Scripts/models/complain/complainDetailModel.js",
                        "~/Scripts/models/complain/ClainForReFundModel.js",
                        "~/Scripts/models/complain/ClaimForRefundDetailModel.js",
                        "~/Scripts/models/complain/complainDetailModel.js",
                        "~/Scripts/models/complain/complainUserModel.js",
                        "~/Scripts/viewmodels/chat/chatViewModel.js",
                        "~/Scripts/viewmodels/customer/CustomerLookUpViewModel.js",
                        "~/Scripts/viewmodels/order/accountantOrderViewModel.js",
                        "~/Scripts/viewmodels/accountant/accountantViewModel.js"
                        ));

            #endregion

            #region Bundles Module kinh doanh
            bundles.Add(new ScriptBundle("~/bundles/customerscript").Include(
                        "~/Scripts/models/accountant/fundBillModel.js",
                        "~/Scripts/models/customer/customerOrderPending.js",
                        "~/Scripts/viewmodels/supporter/ticketDetailCommom.js",
                        "~/Scripts/models/order/orderModel.js",
                        "~/Scripts/models/complain/complainUserModel.js",
                        "~/Scripts/models/complain/complainModel.js",
                        "~/Scripts/models/customer/customerModel.js",
                        "~/Scripts/models/accountant/rechargeBillModel.js",
                        "~/Scripts/models/user/userModel.js",
                        "~/Scripts/models/customer/PotentialCustomer.js",
                        "~/Scripts/models/complain/complainDetailModel.js",
                        "~/Scripts/viewmodels/customer/CustomerLookUpViewModel.js",
                        "~/Scripts/viewmodels/package/packageDetailModel.js",
                        "~/Scripts/viewmodels/order/orderDetailViewModel.js",
                        "~/Scripts/viewmodels/order/depositDetailViewModel.js",
                        "~/Scripts/viewmodels/order/orderCommerceDetailViewModel.js",
                        "~/Scripts/viewmodels/customer/customerOfStaffViewModel.js"
            ));
            #endregion

            #region Bundles Module CSKH
            bundles.Add(new ScriptBundle("~/bundles/ticketscript").Include(
                        "~/Content/plugins/tabdrop/js/bootstrap-tabdrop.js",
                        "~/Scripts/bootstrap3-editable-1.5.1/bootstrap3-editable/js/bootstrap-editable.min.js",
                        "~/Scripts/knockout.x-editable.js",
                        "~/Scripts/autosize.min.js",
                        "~/Scripts/jquery.redirect.js",
                         "~/Scripts/models/accountant/rechargeBillModel.js",
                        "~/Scripts/viewmodels/package/packageDetailModel.js",
                        "~/Scripts/models/accountant/fundBillModel.js",
                        "~/Scripts/models/complain/ClainForReFundModel.js",
                        "~/Scripts/models/complain/ClaimForRefundDetailModel.js",
                        "~/Scripts/models/order/orderModel.js",
                        "~/Scripts/models/complain/complainModel.js",
                        "~/Scripts/models/complain/complainUserModel.js",
                        "~/Scripts/models/complain/complainDetailModel.js",
                        "~/Scripts/models/customer/customerModel.js",
                        "~/Scripts/viewmodels/customer/CustomerLookUpViewModel.js",
                        "~/Scripts/viewmodels/chat/chatViewModel.js",
                        "~/Scripts/viewmodels/package/packageDetailModel.js",
                        "~/Scripts/viewmodels/order/orderDetailViewModel.js",
                        "~/Scripts/viewmodels/order/depositDetailViewModel.js",
                        "~/Scripts/viewmodels/order/orderCommerceDetailViewModel.js",
                        "~/Scripts/models/order/orderModel.js",
                        "~/Scripts/viewmodels/order/orderWaitViewModel.js",
                        "~/Scripts/viewmodels/supporter/supporterViewModel.js"

            ));
            #endregion

            #region Bundles Module Kho
            bundles.Add(new ScriptBundle("~/bundles/packageNoCodeScript").Include(
                        "~/Scripts/bootstrap3-editable-1.5.1/bootstrap3-editable/js/bootstrap-editable.min.js",
                        "~/Scripts/knockout.x-editable.js",
                        "~/Content/plugins/HenrySlider/henry-slider.js",
                        "~/Content/plugins/webui-popover/jquery.webui-popover.js",
                        "~/Content/plugins/wysiwyg/src/wysiwyg.js",
                        "~/Content/plugins/wysiwyg/src/wysiwyg-editor.js",
                        "~/Content/plugins/wysiwyg/src/henry-editor.js",

                        "~/Scripts/viewmodels/GroupChatHubModal.js",
                        "~/Scripts/viewmodels/package/packageDetailModel.js",
                        "~/Scripts/viewmodels/order/orderDetailViewModel.js",
                        "~/Scripts/viewmodels/order/depositDetailViewModel.js",
                        "~/Scripts/viewmodels/order/orderCommerceDetailViewModel.js",
                        "~/Scripts/viewmodels/suggettion/SuggetionOrderInputComponents.js",
                        "~/Scripts/viewmodels/importWarehouse/viewImageModel.js",
                        "~/Scripts/viewmodels/importWarehouse/addPackageLoseModel.js",
                        "~/Scripts/viewmodels/packageNoCode/assignPackageModel.js",
                        "~/Scripts/viewmodels/packageNoCode/packageNoCodeModel.js"
            ));
            #endregion

            //BundleTable.EnableOptimizations = true;
        }
    }
}
