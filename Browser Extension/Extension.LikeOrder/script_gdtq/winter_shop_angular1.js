 var app = angular.module('myapp', ['ngSanitize','ngCookies']);
   app.filter("sanitize", ['$sce', function($sce) {
     return function(htmlCode){
       return $sce.trustAsHtml(htmlCode);
     };
   }]);
   app.run(['$rootScope',function($rootScope) {
      $rootScope.param = '';
      $rootScope.cart_data = '';
   }]);
   app.directive('toggle', function(){
      return {
         restrict: 'A',
         link: function(scope, element, attrs){
            if (attrs.toggle=="tooltip"){
               $(element).tooltip();
            }
            if (attrs.toggle=="popover"){
               $(element).popover();
            }
         }
     };
   });
   app.factory('myFactory', ['$http','$q','$timeout', function($http, $q, $timeout) {
      return{
         ketdon : function(name1,name,id){
            var datashop = name;
            if(id!=null && id>0){
               datashop = name1;
            }
            $.ajax({
               url: '/san-pham/ket-don',
               method: 'POST',
               data: {
                  "cart_data": datashop,
                  "sid": id,
                  "_token": $('body').data("token")
                  
               },
               dataType: 'json',
               beforeSend: function () {
                  $("#modal1").show();
                  $("#modal2").hide();
               },
               success: function(data) {
                  // console.log(data);
                  if (data.error == 0) {
                     // console.log('success');
                     if(id!=null && id>0){
                        // console.log(name);
                        if(typeof name !== 'undefined' && name === '[]'){
                           // localStorage.removeItem('cart_8b54d81e2e464b085b44ef1b7a8e191a');
                           updateDataShop(null,3,2);
                        }else{
                           // setlocalData('cart_8b54d81e2e464b085b44ef1b7a8e191a',name);
                           updateDataShop(name,3,2);
                        }
                     }else{
                        // localStorage.removeItem('cart_8b54d81e2e464b085b44ef1b7a8e191a');
                        updateDataShop(null,3,2);
                     }
                     $('#modal2 .modal-header button').remove();
                     $timeout(function(){
                        $("#modal1").hide();
                        $("#modal2").show();
                     },2000);
                  } else {
                     $("#modal1").hide();
                     $("#modal2 .modal-body").html('<p><strong class="text-danger">'+data.message+'</strong></p>');
                     $("#modal2").show();
                  }
               },
               error: function(xhr, textStatus, error){
                  $("#modal1").hide();
                  $("#modal2 .modal-body").html('<p><strong class="text-danger">Lỗi hệ thống!</strong></p><p>Bạn hãy <span class="text-blue">F5</span> để tải lại trang, xin lỗi vì sự bất tiện này!</p>');
                  $("#modal2").show();
              }
            });
         },
         datcoc : function(name1,name,id,check){
            var datashop = name;
            if(id!=null && id>0){
               datashop = name1;
            }
            $.ajax({
               url: '/san-pham/ket-don',
               method: 'POST',
               data: {
                  "cart_data": datashop,
                  "sid": id,
                  "_token": $('body').data("token"),
                  "dataCheckout": check
                  
               },
               dataType: 'json',
               beforeSend: function () {
                  $("#modal1").show();
                  $("#modal2").hide();
               },
               success: function(data) {
                  if (data.error == 0) {
                     $('#modal2 .modal-header button').remove();
                     $("#modal1").hide();
                     $("#modal2").show();
                     if(id!=null && id>0){
                        setlocalData('cart_8b54d81e2e464b085b44ef1b7a8e191a',name);
                     }else{
                        localStorage.removeItem('cart_8b54d81e2e464b085b44ef1b7a8e191a');
                     }
                  } else {
                     $("#modal1").hide();
                     $("#modal2 .modal-body").html('<p><strong class="text-danger">Đặt hàng thất bại!</strong></p><p>'+data.message+'</p><p><a href="/danh-muc-bai-viet/chi-tiet-17-huong-dan-nap-tien-vao-tai-khoan-khach-hang#controller">Hướng dẫn nạp tiền vào tài khoản</a></p>');
                     $("#modal2").show();
                  }
               },
               error: function(xhr, textStatus, error){
                  $("#modal1").hide();
                  $("#modal2 .modal-body").html('<p><strong class="text-danger">Lỗi hệ thống!</strong></p><p>Bạn hãy <span class="text-blue">F5</span> để tải lại trang, xin lỗi vì sự bất tiện này!</p>');
                  $("#modal2").show();
              }
            });
         },
         saveLink : function(name, id) {
            return $http({
               url: '/san-pham/ket-don',
               data: {
                  "cart_data": name,
                  "sid": id,
                  "_token": $('body').data("token")
                  
               },
               method: 'POST'
            });
         },
         formatCurrency: function(total) {
            var neg = false;
            if(total < 0) {
               neg = true;
               total = Math.abs(total);
            }
            return parseFloat(total, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();
         },
         getlocalData: function(key) {
            if(typeof(Storage) !== "undefined") {
               return localStorage.getItem(key);
            } else {
               console.log('Sorry! No Web Storage support..');
               return false;
            }
            
         },
         setlocalData: function(key,data) {
            if(typeof(Storage) !== "undefined") {
               localStorage.setItem(key, data);
               return true;
            } else {
               console.log('Sorry! No Web Storage support..');
               return false;
            }
         },
         getDataShop: function(key){
            var tmp = getlocalData(key);
            if(typeof tmp !== 'undefined' && (tmp ==='' || tmp ==='[]')){
               localStorage.removeItem(key);
               tmp = false;
            }
            return tmp;
         },
         syncDataShop: function(){
            var tmp = '';
            $.ajax({
               url: '/san-pham/sync-shopcart',
               method: 'POST',
               crossDomain:true,
               // async: false,
               data: {
                  "type":1,
                  "cart_get": true,
                  "_token": $('body').data("token")
                  
               },
               dataType: 'json',
               beforeSend: function () {
                  
               },
               success: function(data) {
                  if (data.error == 0 && data.data != null) {
                     setlocalData('cart_8b54d81e2e464b085b44ef1b7a8e191a',data.data);
                     // tmp = data.data;
                  } else {
                     
                  }
               },
               error: function(xhr, textStatus, error){
                 
              }
            });
            // return tmp;
         },
         updateDataShop: function(item, type, update){
            var tmp = '';
            $.ajax({
               url: '/san-pham/sync-shopcart',
               method: 'POST',
               // crossDomain:true,
               // async: false,
               data: {
                  "type":type,
                  "cart_update": update,
                  "cart_item": item, 
                  "_token": $('body').data("token")
                  
               },
               dataType: 'json',
               beforeSend: function () {
                  $("#loading").show();
               },
               success: function(data) {
                  tmp = data;
                  if (data.error == 0) {
                     // setlocalData('cart_8b54d81e2e464b085b44ef1b7a8e191a',data.data);
                     // tmp = data.data;
                  } else {
                     
                  }
                  $("#loading").hide();
               },
               error: function(xhr, textStatus, error){
                 
              }
            });
            return tmp;
         },
         $_GET: function(param) {
            var vars = {};
            window.location.href.replace( 
               /[?&]+([^=&]+)=?([^&]*)?/gi,
               function( m, key, value ) {
                  vars[key] = value !== undefined ? value : '';
               }
            );

            if ( param ) {
               return vars[param] ? vars[param] : null;  
            }
            return vars;
         },
         getCookie: function(name){
            var deferred = $q.defer();
            $timeout(function() {
               deferred.resolve($.cookie(name));
            }, 0);

            return deferred.promise;
         },

         getAllCookies: function(){
            return $.cookie();
         },

         setCookie: function(name, value){
            return $.cookie(name, value);
         },

         deleteCookie: function(name){
            return $.removeCookie(name);
         },
         allStorage: function() {
            var values = [],
            keys = Object.keys(localStorage),
            i = keys.length;
            while ( i-- ) {
               values.push( localStorage.getItem(keys[i]) );
            }
            return values;
         },
         //
         isArray: function(data){
            if(data.hasOwnProperty('data')){
               var shop = data.data;
               if(shop.hasOwnProperty('tbex_products')){
                  if(data.data.tbex_products instanceof Array){
                     return data.data.tbex_products;
                  }else{
                     return false;
                  }
               }else{
                  return false;
               }
            }else{
               return false;
            }
         },

         getProduct: function(shop){
            var data = [];
            for (var i = shop.length - 1; i >= 0; i--) {
               data = isInProduct(shop[i],data);
            }
            return data;
         },

         getShop: function(shop){
            var data = [];
            for (var i = shop.length - 1; i >= 0; i--) {
               data = isInShop(shop[i],data);
            }
            return data;
         },

         isInArray: function(value, array) {
            return array.indexOf(value) > -1;
         },

         isInProduct: function(item, array){
            var add = true;
            if(array.length){
               for (var i = array.length - 1; i >= 0; i--) {
                  if(array[i].pro_link == item.pro_link && array[i].color== item.color && array[i].size == item.size && array[i].price == item.price){
                     array[i].shop_nick= decodeURI(item.shop_nick);
                     array[i].amount += item.amount;
                     //
                     item.pro_id = array.length+1;
                     item.url = '<a target="_blank" href="'+item.pro_link+'"><img src="'+item.image+'" alt="" /></a>';
                     item.namepro = '<a href="'+item.pro_link+'" target="_blank" class="text-blue"><span>'+item.name+'</span></a>';
                     item.price_vn = item.rate*item.price;
                     item.price_string = formatCurrency(item.price);
                     item.price_vn_string = formatCurrency(item.rate*item.price);
                     array[i].money_cn = array[i].amount*array[i].price;
                     array[i].money_vn = array[i].amount*array[i].price*array[i].rate;
                     array[i].money_cn_string = formatCurrency(array[i].money_cn);
                     array[i].money_vn_string = formatCurrency(array[i].money_vn);
                     //
                     add = false;
                     break;
                  }
               }
            }
            if(add){
               item.shop_nick= decodeURI(item.shop_nick);
               //
               item.pro_id = array.length+1;
               item.url = '<a target="_blank" href="'+item.pro_link+'"><img src="'+item.image+'" alt="" /></a>';
               item.namepro = '<a href="'+item.pro_link+'" target="_blank" class="text-blue"><span>'+item.name+'</span></a>';
               item.price_vn = item.rate*item.price;
               item.price_string = formatCurrency(item.price);
               item.price_vn_string = formatCurrency(item.rate*item.price);
               item.money_cn = item.amount*item.price;
               item.money_vn = item.amount*item.price*item.rate;
               item.money_cn_string = formatCurrency(item.money_cn);
               item.money_vn_string = formatCurrency(item.money_vn);
               item.is_check = false;
               item.is_wood = false;
               item.is_wet = false;
               item.is_egg = false;
               item.service = 1;
               item.shop_note = '';
               //
               array.push(item);
            }
            return array;
         },

         isInShop: function(item,array){
            var add = true;
            var comment = [];
            if(array.length){
               for (var i = array.length - 1; i >= 0; i--) {
                  if(array[i].shop_link == item.shop_link){
                     add = false;
                     break;
                  }
               }
            }
            //
            if(add){
               array.push({shop_link:item.shop_link,shop_nick:decodeURI(item.shop_nick)});
            }
            return array;
         },
         isProductInShop: function(pro, shop){
            var shopdata = [];
            var data = [];
            var count = 0;
            var money_cn = 0;
            var money_vn = 0;
            var rate = 0;
            var total_count = 0;
            var total_money_cn = 0;
            var total_money_vn = 0;
            var money_cn_string = null;
            var money_vn_string = null;
            var is_check = false;
            var is_wood = false;
            var is_wet = false;
            var is_egg = false;
            var shop_node = '';
            var service = 1;
            for (var i = shop.length - 1; i >= 0; i--) {
               for (var j = pro.length - 1; j >= 0; j--) {
                  if(i==(shop.length - 1)){pro[j].item =  pro.length - 1 - j;}
                  if(shop[i].shop_link == pro[j].shop_link){
                     data.push(pro[j]);
                     count += pro[j].amount;
                     money_cn += pro[j].money_cn; 
                     money_vn += pro[j].money_vn; 
                     rate = pro[j].rate;
                     if(pro[j].is_check){
                        is_check = true;
                     }
                     if(pro[j].is_wood){
                        is_wood = true;
                     }
                     if(pro[j].is_wet){
                        is_wet = true;
                     }
                     if(pro[j].is_egg){
                        is_egg = true;
                     }
                     if(typeof pro[j].service !== 'undefined'){
                        service = pro[j].service;
                     }
                     shop_note = pro[j].shop_note;
                  }
               }
               shop[i].comment = [];
               shop[i].selected = true;
               shop[i].money_cn = money_cn;
               shop[i].money_vn = money_vn;
               shop[i].money_cn_string = formatCurrency(money_cn);
               shop[i].money_vn_string = formatCurrency(money_vn);
               shop[i].product = data;
               shop[i].rate = rate;
               shop[i].total = count;
               shop[i].is_check = is_check;
               shop[i].is_wood = is_wood;
               shop[i].is_wet = is_wet;
               shop[i].is_egg = is_egg;
               // shop[i].service = service;
               shop[i].business = false;
               // console.log(service);
               if(service == 1){
                  shop[i].business = true;
               }
               shop[i].consumer = false;
               if(service == 2){
                  shop[i].consumer = true;
               }
               shop[i].shop_note = shop_note;
               total_count += count;
               total_money_cn += money_cn;
               total_money_vn += money_vn;
               count = 0;
               data = [];
               money_cn = 0;
               money_vn = 0;
               rate = 0;
               is_check = false;
               is_wood = false;
               service = 1;
            }
            shopdata = {shop:shop,total_shop:shop.length,shop_note:shop_note,total:total_count,money_cn:total_money_cn,money_vn:total_money_vn,money_cn_string:formatCurrency(total_money_cn),money_percent_string:formatCurrency(total_money_vn*0.9),money_vn_string:formatCurrency(total_money_vn)};
            return shopdata;
         },
         feeBestBuy: function(total2,service){
            var fee = 0;
            var dis =1;
            var discount = JSON.parse(getlocalData('f98b6997b040c07cc2f550b0c0bada5d'));
            if(typeof discount.discount !== 'undefined'){
               dis = discount.discount;
               // console.log(discount.discount);
            }
            if(total2>0&& total2<=30000000){
               fee = total2*4/100;
            }else if(total2>30000000 && total2<=50000000){
               fee = total2*3.5/100;
            }else if(total2>50000000 && total2<=100000000){
               fee = total2*3/100;
            }else if(total2>100000000 && total2<=300000000){
               fee = total2*2.5/100;
            }else if(total2>300000000){
               fee = total2*2/100;
            }
            if(typeof service !== 'undefined' && service == 1){
               return fee*dis; 
            }
            if(typeof service !== 'undefined' && service == 2){
               if(fee*dis*0.5 > 5000){
                  return fee*dis*0.5;
               }else{
                  return 5000;
               }
            }
            return fee*dis;
         },
         feeBestCheck: function(medium,total){
            var fee = 0;
            if(medium<10){
               if(total>=1 && total<=2){
                  fee = 1500;
               }else if(total>=3 && total<=10){
                  fee = 1000;
               }else if(total>=11 && total<=100){
                  fee = 700;
               }else if(total>=101 && total<=500){
                  fee = 700;
               }else if(total>500){
                  fee = 500;
               }
            }else{
               if(total>=1 && total<=2){
                  fee = 5000;
               }else if(total>=3 && total<=10){
                  fee = 3500;
               }else if(total>=11 && total<=100){
                  fee = 2000;
               }else if(total>=101 && total<=500){
                  fee = 1500;
               }else if(total>500){
                  fee = 1000;
               }
            }
            return fee;
         }
         //
      };
   }]);
   app.controller('mycontroller1', ['$scope', '$timeout', '$rootScope', '$cookies','$cookieStore','myFactory','$window', function($scope, $timeout, $rootScope, $cookies, $cookieStore, myFactory, $window){
      // 
      var old_cart = localStorage.getItem("old_8b54d81e2e464b085b44ef1b7a8e191a");
      if(typeof old_cart === 'undefined' || old_cart === null || old_cart.length === 0){
         var cart = myFactory.getlocalData('cart_8b54d81e2e464b085b44ef1b7a8e191a');
         if(cart){
            myFactory.setlocalData('old_8b54d81e2e464b085b44ef1b7a8e191a',cart );
         }
      }
      // 
      if(myFactory.getCookie('vgacartxyz')===null){
         localStorage.removeItem('cart_8b54d81e2e464b085b44ef1b7a8e191a');
      }
      $scope.cartAlertMessage = false;
      $scope.check = false;
      $scope.GDTQ = 'https://'+window.location.host;
      // $scope.GDTQ = 'https://likeorder.com';
      $timeout(function(){
         window.postMessage({type:"REQUEST_DATA"}, $scope.GDTQ);
         $scope.check = true;
      }, 1000);
      $scope.alert = '';
      $scope.cart = [];
      var shid = null;
      var tmp = [];
      var data = [];
      var msg = '<div class="alert alert-danger" role="alert"><strong>Không</strong> có sản phẩm nào trong giỏ hàng!</div>';
      var dataShop = null;
      var shop = null;
      $timeout(function(){ 
         myFactory.syncDataShop();
         $timeout(function(){
            dataShop = myFactory.getDataShop('cart_8b54d81e2e464b085b44ef1b7a8e191a');
            $scope.cartAlertMessage = true;
            if(dataShop){
               shop = JSON.parse(dataShop);
               tmp = myFactory.isProductInShop(myFactory.getProduct(shop), myFactory.getShop(shop));
               if(myFactory.$_GET('shid')){
                  shid = myFactory.$_GET('shid');
               }
               $scope.cart = tmp;
               $rootScope.$emit('scope.stored', $scope.total());
               $scope.cartAlertMessage = true;
               localStorage.removeItem('cart_8b54d81e2e464b085b44ef1b7a8e191a');
            }else{
               $scope.alert = msg;
            }
         }, 2000);  
      },5000);
        
      $scope.chooseinput = function(key,index){
         var check = true;
         angular.forEach($scope.cart.shop, function(shop, key) {
            // console.log(key,shop.selected);
            if(!shop.selected){
               check = false;
            }
         });
         // console.log(check);
         $rootScope.$emit('scope.selected', check);
         $rootScope.$emit('scope.stored', $scope.total());

      },
      //
      $rootScope.$on('scope.checked', function (event, data) {
         // console.log(data);
         angular.forEach($scope.cart.shop, function(shop, key) {
            shop.selected = data;
         });
         $rootScope.$emit('scope.stored', $scope.total());
      }),
      $rootScope.$on('scope.createOrderAll', function(event,data){
         var tmp = [];
         var tmp1 = [];
         var shopcart = [];
         var shopcart1 = [];
         angular.forEach($scope.cart.shop, function(shop, key2) {
            angular.forEach(shop.product, function(item, key3) {
               if(shop.selected){
                  tmp1.push(item);
               }else{
                  tmp.push(item);
               }
            });

         });
         if(tmp.length){
            for (var i = tmp.length - 1; i >= 0; i--) {
               shopcart.push(tmp[i]);
            }
         } 
         if(tmp1.length){
            for (var i = tmp1.length - 1; i >= 0; i--) {
               shopcart1.push(tmp1[i]);
            }
         }
         myFactory.ketdon(JSON.stringify(shopcart1),JSON.stringify(shopcart), 1);
      }),
      //
      $scope.fee_bestbuy = function(key){
         var service = 1;//goi kinh doanh
         if(typeof $scope.cart.shop[key].business !=='undefined' && $scope.cart.shop[key].busines){
            service = 1;
         }
         if(typeof $scope.cart.shop[key].consumer !=='undefined' && $scope.cart.shop[key].consumer){
            service = 2; //goi tieu dung
         }
         var total2 = 0;
         angular.forEach($scope.cart.shop[key].product, function(item, key2) {
               total2 += item.amount * item.price * item.rate;
            });
         // console.log(service, total2);
         return myFactory.feeBestBuy(total2,service);
      },
      //
      $scope.fee_paycheck = function(key,val){
         var total = 0;
         var fee = 0;
         if(val){
            // console.log('fee_paycheck',$scope.cart.shop[key].is_check);
            total = $scope.total_shop(key).count;
            var it = 1;
            var total2 = 0;
            angular.forEach($scope.cart.shop[key].product, function(item, key2) {
               it = key2;
               total2 += item.price;
            });
            var medium = total2/it;
            fee = myFactory.feeBestCheck(medium,total);
            //
         }
         fee = fee*total;
         return fee;
      },
      $scope.fee_payischeck= function(key,shop){
         var total = 0;
         var fee = 0;
         if(shop.is_check){
            var it = 1;
            var total2 = 0;
            angular.forEach(shop.product, function(item, key2) {
               total += item.amount;
               it = key2;
               total2 += item.price;
            });
            var medium = total2/it;
            fee = myFactory.feeBestCheck(medium,total);
         }
         fee = fee*total;
         return fee;
      },
      $scope.total_shop_estimate = function(key,shop){
         var total = 0;
         // console.log(shop);
         total = $scope.total_shop(key).price + $scope.fee_bestbuy(key) + $scope.fee_paycheck(key,shop.is_check);
         return total;
      },
      //
      $scope.fee_paywood = function(key,val){
         fee = "<span>0</span><sup>đ</sup>";
         if(val){
            fee = "<span class='text-red'>20 tệ/kg đầu tiên, 0.5 tệ/kg sau</span>";
         }
          return fee;
      },
      //
      $scope.fee_transport = function(key,val){
         fee = 0;
         if(val){
            fee = "<span>Đang cập nhật</span>";
         }
          return fee;
      },
      //
      $scope.getShops = function(key,shop) {
         if(myFactory.$_GET('shid')){
            shid = myFactory.$_GET('shid') - 1;
            if(shid == key){
               return $scope.cart;
            }else{
               return $scope.cart;
            }
         }else{
            return $scope.cart;
         }
      },
      //
      $scope.myupdatenote = function(key,key2,value){
         // console.log($scope.cart.shop[key].product[key2]);
         // var res = myFactory.updateDataShop(JSON.stringify($scope.cart.shop[key].product[key2]), 1, 2);
         // if(res.error==0){
         //    return false;
         // }
         var tmp = [];
         var shopcart = [];
         //
         // $scope.cart.shop[key].product[key2].note = value;
         angular.forEach($scope.cart.shop, function(shop, key3) {
            angular.forEach(shop.product, function(item, key4) {
               tmp.push(item);
            });

         });
         if(tmp.length){
            for (var i = tmp.length - 1; i >= 0; i--) {
               shopcart.push(tmp[i]);
            }
         }
         // console.log('shopcart',shopcart);
         // myFactory.setlocalData('cart_8b54d81e2e464b085b44ef1b7a8e191a',JSON.stringify(shopcart));
         return false;
      },
      //
      $scope.myupdate = function(key,key2,value){

         if(value<0){
            value = -1*value;
         }
         if(value>99999){
            value = 99999;
         }
         // var res = myFactory.updateDataShop(JSON.stringify($scope.cart.shop[key].product[key2]),1,2);
         // if(res.error==0){
         //    return false;
         // }
         var tmp = [];
         var shopcart = [];
         var price = $scope.cart.shop[key].product[key2].price;
         var pro_link = $scope.cart.shop[key].product[key2].pro_link;
         var amount = 0;
         var warning = null;
         angular.forEach($scope.cart.shop[key].product, function(it,k1){
            if(pro_link == it.pro_link){amount += it.amount;}
         });
         if($scope.cart.shop[key].product[key2].domain === "1688"){
            angular.forEach($scope.cart.shop[key].product[key2].price_arr, function(item1, key5) {
                  if(item1.end.length > 0){
                     if(amount >= parseInt(item1.begin) && amount <= parseInt(item1.end)){
                        price = parseFloat(item1.price);
                     }
                  }else{
                     if(amount >= parseInt(item1.begin) ){
                        price = parseFloat(item1.price);
                     }
                  }
            });
            if(amount < $scope.cart.shop[key].product[key2].beginAmount){
               warning = '<span data-toggle="tooltip" title="Shop yêu cầu mua tối thiểu '+ $scope.cart.shop[key].product[key2].beginAmount +' sản phẩm"><i class="fa fa-exclamation-triangle fa-lg text-red"></i></span>';              
            }
            angular.forEach($scope.cart.shop[key].product, function(it,k1){
               if(pro_link == it.pro_link){
                  $scope.cart.shop[key].product[k1].warning = warning;
                  $scope.cart.shop[key].product[k1].price = price;
               }
            });
         }else{
            $scope.cart.shop[key].product[key2].price = price;
         }
         //
         $scope.cart.shop[key].product[key2].amount = value;
         //
         angular.forEach($scope.cart.shop, function(shop, key3) {
            angular.forEach(shop.product, function(item, key4) {
               tmp.push(item);
            });

         });
         if(tmp.length){
            for (var i = tmp.length - 1; i >= 0; i--) {
               shopcart.push(tmp[i]);
            }
         }
         // console.log('shopcart',shopcart);
         // myFactory.setlocalData('cart_8b54d81e2e464b085b44ef1b7a8e191a',JSON.stringify(shopcart));
         return false;
      },
      //
      $scope.cancel = function(key) {
         var shopcart = [];
         var tmp = [];
         $scope.cart.shop.splice(key,1);
         //
         angular.forEach($scope.cart.shop, function(shop, key2) {
            angular.forEach(shop.product, function(item, key3) {
               tmp.push(item);
            });

         });

         if(tmp.length){
            for (var i = tmp.length - 1; i >= 0; i--) {
               shopcart.push(tmp[i]);
            }
         }
         // console.log('shopcart',shopcart);
         if(shopcart.length){
            myFactory.setlocalData('cart_8b54d81e2e464b085b44ef1b7a8e191a',JSON.stringify(shopcart));
         }else{
            localStorage.removeItem('cart_8b54d81e2e464b085b44ef1b7a8e191a');
         }
         return false;
      },
      //
      $scope.removeItem = function(key,key2) {
         var shopcart = [];
         var tmp = [];
         var res = myFactory.updateDataShop(JSON.stringify($scope.cart.shop[key].product[key2]),2,3);
         $timeout(function(){
             angular.forEach($scope.cart.shop[key].product, function(item, k) {
               if(k == key2){
                  // console.log(item);
                  $scope.cart.shop[key].product.splice(key2, 1);
                  if($scope.cart.shop[key].product.length==0){
                     $scope.cart.shop.splice(key,1);
                  }
               }  
            });
         },1500);
         //
         // angular.forEach($scope.cart.shop, function(shop, key2) {
         //    angular.forEach(shop.product, function(item, key3) {
         //       tmp.push(item);
         //    });

         // });

         // if(tmp.length){
         //    for (var i = tmp.length - 1; i >= 0; i--) {
         //       shopcart.push(tmp[i]);
         //    }
         // }
         // console.log('shopcart',shopcart);
         // if(shopcart.length){
         //    myFactory.setlocalData('cart_8b54d81e2e464b085b44ef1b7a8e191a',JSON.stringify(shopcart));
         // }else{
         //    localStorage.removeItem('cart_8b54d81e2e464b085b44ef1b7a8e191a');
         // }
         return false;
      },
      //
      $scope.createOrder = function(key){
         $('#modal4').show();
         $('#modal4 .btn-primary').click(function(){
            $('#modal4').hide();
            //
            var tmp = [];
            var tmp1 = [];
            var shopcart = [];
            var shopcart1 = [];
            angular.forEach($scope.cart.shop, function(shop, key2) {
               angular.forEach(shop.product, function(item, key3) {
                  if(key==key2){
                     tmp1.push(item);
                  }else{
                     tmp.push(item);
                  }
               });

            });
            if(tmp.length){
               for (var i = tmp.length - 1; i >= 0; i--) {
                  shopcart.push(tmp[i]);
               }
            } 
            if(tmp1.length){
               for (var j = tmp1.length - 1; j >= 0; j--) {
                  shopcart1.push(tmp1[j]);
               }
            }
            myFactory.ketdon(JSON.stringify(shopcart1),JSON.stringify(shopcart), key+1);
            // 
            return false;
         });
         $('#modal4 .btn-danger').click(function(){
            $('#modal4').hide();
            // $rootScope.$emit('scope.ketdon', check);
            return false;
         });         
      },
       $scope.choose = function(key){
         if(false){
            $('.tooltip').css('opacity',1);
            // alert('Please check shop name before!');
         }else{
            
            key = key+1;
            window.location.href = "/san-pham/chon-dich-vu?shid="+key;
         }
         return false;
      },
      //
      $scope.paycheck = function(key){
         var shopcart = [];
         var tmp = [];
         var is_check = $scope.cart.shop[key].is_check; 
         // console.log(is_check);
         //
         
         angular.forEach($scope.cart.shop, function(shop, key2) {
            angular.forEach(shop.product, function(item, key3) {
               if(key2 == key){
                  item.is_check = is_check;
                  $scope.cart.shop[key2].product[key3].is_check = is_check;
               }
               tmp.push(item);
            });

         });
         // console.log(tmp);
         // if(tmp.length){
         //    for (var i = tmp.length - 1; i >= 0; i--) {
         //       shopcart.push(tmp[i]);
         //    }
         // }

         myFactory.updateDataShop(JSON.stringify($scope.cart.shop[key]), 2,2);
         // myFactory.setlocalData('cart_8b54d81e2e464b085b44ef1b7a8e191a',JSON.stringify(shopcart));
         return false;
      },
      //
      $scope.noteshop = function(key){
         var shopcart = [];
         var tmp = [];
         var shop_note = $scope.cart.shop[key].shop_note;
         // myFactory.updateDataShop(JSON.stringify($scope.cart.shop[key]), 2,2);
         angular.forEach($scope.cart.shop, function(shop, key2) {
            angular.forEach(shop.product, function(item, key3) {
               if(key2 == key){
                  $scope.cart.shop[key].product[key3].shop_note = shop_note;
                  item.shop_note = shop_note;
               }
               // tmp.push(item);
            });

         });
         // if(tmp.length){
         //    for (var i = tmp.length - 1; i >= 0; i--) {
         //       shopcart.push(tmp[i]);
         //    }
         // }
         // myFactory.setlocalData('cart_8b54d81e2e464b085b44ef1b7a8e191a',JSON.stringify(shopcart));
         return false;
      },
      // 
      $scope.switchService = function(name,key){
         var service = 1;//kinh doanh
         if(name == 1){
            var business = $scope.cart.shop[key].business;
            if(business){
               $scope.cart.shop[key].consumer = false;
               service = 1;
            }else{
               $scope.cart.shop[key].business = true;
               return false;
            }
         }else{
            var consumer = $scope.cart.shop[key].consumer;
            if(consumer){
               $scope.cart.shop[key].business = false;
               service = 2;//tieu dung
            }else{
               $scope.cart.shop[key].consumer = true;
               return false;
            }
         }
         // console.log('service',service);
         angular.forEach($scope.cart.shop, function(shop, key2) {
            angular.forEach(shop.product, function(item, key3) {
               if(key2 == key){
                  $scope.cart.shop[key2].product[key3].service = service;
               }
               // tmp.push(item);
            });

         });
         // console.log($scope.cart.shop[key]);
          myFactory.updateDataShop(JSON.stringify($scope.cart.shop[key]), 2,2);
         return false;

      }
      // 
      $scope.paywood = function(name,key){
         var shopcart = [];
         var tmp = [];
         if(name == 3){
            var is_wood = $scope.cart.shop[key].is_wood; 
         }else if(name == 4){
            var is_wood = $scope.cart.shop[key].is_wet; 
         }else if(name == 5){
            var is_wood = $scope.cart.shop[key].is_egg; 
         }
         
         angular.forEach($scope.cart.shop, function(shop, key2) {
            angular.forEach(shop.product, function(item, key3) {
               if(key2 == key){
                  if(name == 3){
                     item.is_wood = is_wood;
                     $scope.cart.shop[key2].product[key3].is_wood = is_wood;
                  }else if(name == 4){
                     item.is_wet = is_wood;
                     $scope.cart.shop[key2].product[key3].is_wet = is_wood;
                  }else if(name == 5){
                     item.is_egg = is_wood;
                     $scope.cart.shop[key2].product[key3].is_egg = is_wood;
                  }
               }
               tmp.push(item);
            });

         });

         // if(tmp.length){
         //    for (var i = tmp.length - 1; i >= 0; i--) {
         //       shopcart.push(tmp[i]);
         //    }
         // }
         myFactory.updateDataShop(JSON.stringify($scope.cart.shop[key]), 2,2);
         // myFactory.setlocalData('cart_8b54d81e2e464b085b44ef1b7a8e191a',JSON.stringify(shopcart));
         return false;
      },
      //
      $scope.checkIfEnterKeyWasPressed = function($event,key){;
         var keyCode = $event.which || $event.keyCode;
         var msg = $('.mygrid-wrapper-div');
         if (keyCode === 13) {
            if (!event.shiftKey) {
               event.preventDefault();              
               if($scope.cart.shop[key].txtcomment !=''){
                  $scope.cart.shop[key].comment.push({date:( new Date() ).getTime(),text:$scope.cart.shop[key].txtcomment});             
                  $scope.cart.shop[key].txtcomment = "";
                  msg.animate({ scrollTop:  msg[0].scrollHeight }, "slow" );
               }
            }
         }

      },
      //
      $scope.total_shop = function(k){
         var shid = myFactory.$_GET('shid');
         if(shid>0){
            shid = shid -1;
         }
         var shid_total = 0;
         var shid_money = 0;
         var count = 0;
         var price = 0;
         var total1 = 0;
         var money1 = 0;
         var total2 = 0;
         var money2 = 0;
         var new_money = 0;
         var shop_link = '';
         var shop_name = '';
         var check_money = 0;
         angular.forEach($scope.cart.shop, function(shop, key) {
            if(shid == key){
               angular.forEach(shop.product, function(item) {
                     shid_total += item.amount;
                     shid_money += item.amount * item.price * item.rate;
               });
               shid_money += $scope.fee_bestbuy(key);
            }
            if(k == key){
               angular.forEach(shop.product, function(item) {
                     count += item.amount;
                     price += item.amount * item.price * item.rate;
                     shop_link = '<a target="_blank" href="'+item.shop_link+'"><img src="'+item.image+'"/></a>';
                     shop_name = '<a target="_blank" href="'+item.shop_link+'">'+item.shop_nick+'</a>';    
               });
            }
            if(shop.selected==true){
               angular.forEach(shop.product, function(item) {
                  total1 += item.amount;
                  money1 += item.amount * item.price * item.rate;
                  new_money += item.amount * item.price * item.rate;
               });
               // console.log(shop);
               new_money += $scope.fee_bestbuy(key) + $scope.fee_payischeck(key, shop);
            }
            angular.forEach(shop.product, function(item) {
               total2 += item.amount;
               money2 += item.amount * item.price * item.rate;
            });
            money2 += $scope.fee_bestbuy(key);
         });
         data = {total_shop:$scope.cart.shop.length,shop_link:shop_link,shop_name:shop_name,new_money1:new_money,count:count,price:price,total1:total1,total_count:total2,money1:money1,total_money_string:money2,precent:money2*0.9,shid_total:shid_total,shid_money:shid_money};
         $rootScope.$emit('scope.stored', data);
         return data;
      },
      //
      $rootScope.$on('scope.ketdon', function (event, data) {
         var shopcart = [];
         var shopcart1 = [];
         var tmp = [];
         var tmp1 = [];
         var shid = null;
         var shid1 = null;
         if(myFactory.$_GET('shid')){
            shid = myFactory.$_GET('shid');
         }
         if(shid!=null && shid>0){
            shid1 = shid -1;
         }
         angular.forEach($scope.cart.shop, function(shop, key2) {
            angular.forEach(shop.product, function(item, key3) {
               if(shid1==key2){
                  tmp1.push(item);
               }else{
                  tmp.push(item);
               }
            });

         });
         if(tmp.length){
            for (var i = tmp.length - 1; i >= 0; i--) {
               shopcart.push(tmp[i]);
            }
         } 
         if(tmp1.length){
            for (var i = tmp1.length - 1; i >= 0; i--) {
               shopcart1.push(tmp1[i]);
            }
         }
         myFactory.ketdon(JSON.stringify(shopcart1),JSON.stringify(shopcart), shid);
      });
      //
      $scope.total = function() {
         var total_count = 0;
         var total_money  = 0;
         var total_money_string = null;
         var total_count1 = 0;
         var total_money1  = 0;
         var total_money_string1 = null;
         var new_money = 0;
         angular.forEach($scope.cart.shop, function(shop, key) {
            total_count1 += shop.total;
            total_money1 += shop.money_vn;
            //
            if(shop.selected){
               total_count += shop.total;
               total_money += shop.money_vn;
               new_money += shop.money_vn;
            }
            new_money += $scope.fee_bestbuy(key);

         });
         data = {total_shop:$scope.cart.shop.length,new_money:new_money,cart_count:total_count1,cart_money:total_money1,cart_money_string:total_money_string1,total_count:total_count, total_money:total_money, total_money_string:myFactory.formatCurrency(total_money) };
         // console.log(data);
         return data;
      };
      //
   }]);
   app.controller('mycontroller2', ['$scope', '$rootScope', '$cookies','$cookieStore', function($scope, $rootScope, $cookies, $cookieStore){
      $rootScope.$on('scope.stored', function (event, data) {
         // $scope.total = data.money1;
         $scope.total = data.new_money1;
         $scope.quantity = data.total1;
         // $scope.param = data.param;
      });
      $scope.selectedAll = true;
      $scope.getParam = function() {
         return $rootScope.param;
      };
      // console.log('param1', $rootScope);
      $scope.chooseAllService = function(){
         if(!$scope.selectedAll) {
            alert('Please check all shop name before!');
            return false;
         }else if(!$scope.quantity){
            console.log('Shop null!');
            return false;
         }else{
            // console.log('/san-pham/chon-dich-vu?shid=' + $scope.getParam());
            if($scope.getParam()){
               // console.log($scope.getParam());
               window.location.href = '/san-pham/chon-dich-vu?shid=' + $scope.getParam();
            }else{
               window.location.href = '/san-pham/chon-dich-vu';
            }
            return false;
         }
      };
      // $scope.chooseAllService = function(){
      //    if(typeof(localStorage.getItem("cart_8b54d81e2e464b085b44ef1b7a8e191a"))=='undefined'){
      //       console.log('gio hang rong');
      //       return false;
      //    };
      //    if (localStorage.getItem("cart_8b54d81e2e464b085b44ef1b7a8e191a") === null) {
      //       // console.log('gio hang rong');
      //       return false;
      //    }
      //    return window.location.href = "/san-pham/chon-dich-vu";
      // };
      $scope.createOrderAll = function(){
         $('#modal3').show();
         $('#modal3 .btn-primary').click(function(){
            $('#modal3').hide();
            $rootScope.$emit('scope.createOrderAll', $scope.selectedAll);
            return false;
         });
         $('#modal3 .btn-danger').click(function(){
            $('#modal3').hide();
            // $rootScope.$emit('scope.ketdon', check);
            return false;
         });
      };

      $scope.checkAll = function () {
         if ($scope.selectedAll) {
            $scope.selectedAll = true;
         } else {
            $scope.selectedAll = false;
         }
            $rootScope.$emit('scope.checked', $scope.selectedAll);
      };
      $rootScope.$on('scope.selected', function (event, data) {
            // console.log('hi',data);
            $scope.selectedAll = data;
         });
   }]);
   app.controller('cartcontroller', ['$scope', '$timeout', '$rootScope', '$cookies','$cookieStore','myFactory','$window', function($scope, $timeout, $rootScope, $cookies, $cookieStore, myFactory, $window){
      //
      $scope.cartquantity = 0;//set default
      $scope.total = function() {
         var total_count = 0;
         var total_money  = 0;
         var total_money_string = null;
         var total_count1 = 0;
         var total_money1  = 0;
         var total_money_string1 = null;
         angular.forEach($scope.cart.shop, function(shop, key) {
            total_count1 += shop.total;
            total_money1 += shop.money_vn;
            //
            if(shop.selected){
               total_count += shop.total;
               total_money += shop.money_vn;
            }
         });
         // console.log($scope.cart.shop.length);
         data = {total_shop:$scope.cart.shop.length,cart_count:total_count1,cart_money:total_money1,cart_money_string:total_money_string1,total_count:total_count, total_money:total_money, total_money_string:myFactory.formatCurrency(total_money) };
         // console.log(data);
         return data;
      };
      var dataShop = myFactory.getDataShop('cart_8b54d81e2e464b085b44ef1b7a8e191a');
      if(dataShop){
         shop = JSON.parse(dataShop);
         tmp = myFactory.isProductInShop(myFactory.getProduct(shop), myFactory.getShop(shop));
         $scope.cart = tmp;
         $rootScope.$emit('scope.stored', $scope.total());
      }
      $rootScope.$on('scope.stored', function (event, data) {
         // $scope.cartmoneny = data.total_money_string;
         // $scope.cartquantity = data.total_count;
         $scope.cartquantity = data.total_shop;
      });
   }]);

   /*app.controller('mycontroller3',['$scope', '$rootScope', '$cookies','$cookieStore','myFactory', function($scope, $rootScope, $cookies, $cookieStore,myFactory){
      $rootScope.$on('scope.stored', function (event, data) {
         if(myFactory.$_GET('shid')){
            $scope.cartmoneny3 = data.shid_money;
            $scope.cartquantity3 = data.shid_total;
            $scope.cartmonenypercent3 = data.shid_money*0.9;
         }else{
            $scope.cartmoneny3 = data.total_money_string;
            $scope.cartquantity3 = data.total_count;
            $scope.cartmonenypercent3 = data.precent;
         }
      });
      //
      var btnConfirm = '';    
      $scope.saveLinkOrder = function(check){
         $('#modal3').show();
         $('#modal3 .btn-primary').click(function(){
            $('#modal3').hide();
            $rootScope.$emit('scope.ketdon', check);
            return false;
         });
         $('#modal3 .btn-danger').click(function(){
            $('#modal3').hide();
            // $rootScope.$emit('scope.ketdon', check);
            return false;
         });
      };
   }]);*/

   
var app1 = angular.module('myapp1', ['ngSanitize','ngCookies']);
   app1.factory('myFactory1', ['$http','$q','$timeout', function($http, $q, $timeout) {
      return{
         getlocalData: function(key) {
            if(typeof(Storage) !== "undefined") {
               // console.log("getlocalData");
               return localStorage.getItem(key);
            } else {
               console.log('Sorry! No Web Storage support..');
               return false;
            }
            
         },
         getDataShop: function(key){
            return getlocalData(key);
         },
         //
         syncDataShop: function(){
            var tmp = '';
            $.ajax({
               url: '/san-pham/sync-shopcart',
               method: 'POST',
               crossDomain:true,
               // async: false,
               data: {
                  "type":1,
                  "cart_get": true,
                  "_token": $('body').data("token")
                  
               },
               dataType: 'json',
               beforeSend: function () {
                  
               },
               success: function(data) {
                  if (data.error == 0 && data.data != null) {
                     setlocalData('cart_8b54d81e2e464b085b44ef1b7a8e191a',data.data);
                     // tmp = data.data;
                  } else {
                     
                  }
               },
               error: function(xhr, textStatus, error){
                 
              }
            });
            // return tmp;
         },
         //
         isArray: function(data){
            if(data.hasOwnProperty('data')){
               var shop = data.data;
               if(shop.hasOwnProperty('tbex_products')){
                  if(data.data.tbex_products instanceof Array){
                     return data.data.tbex_products;
                  }else{
                     return false;
                  }
               }else{
                  return false;
               }
            }else{
               return false;
            }
         },

         getProduct: function(shop){
            var data = [];
            for (var i = shop.length - 1; i >= 0; i--) {
               data = isInProduct(shop[i],data);
            };
            return data;
         },

         getShop: function(shop){
            var data = [];
            for (var i = shop.length - 1; i >= 0; i--) {
               data = isInShop(shop[i],data);
            };
            return data;
         },

         isInArray: function(value, array) {
            return array.indexOf(value) > -1;
         },

         isInProduct: function(item, array){
            var add = true;
            if(array.length){
               for (var i = array.length - 1; i >= 0; i--) {
                  if(array[i].pro_link == item.pro_link && array[i].color== item.color && array[i].size == item.size && array[i].price == item.price){
                     array[i].shop_nick= decodeURI(item.shop_nick);
                     array[i].amount += item.amount;
                     //
                     item.pro_id = array.length+1;
                     item.url = '<a target="_blank" href="'+item.pro_link+'"><img src="'+item.image+'" alt="" /></a>';
                     item.namepro = '<a href="'+item.pro_link+'" target="_blank" class="text-blue"><span>'+item.name+'</span></a>';
                     item.price_vn = item.rate*item.price;
                     item.price_string = formatCurrency(item.price);
                     item.price_vn_string = formatCurrency(item.rate*item.price);
                     array[i].money_cn = array[i].amount*array[i].price;
                     array[i].money_vn = array[i].amount*array[i].price*array[i].rate;
                     array[i].money_cn_string = formatCurrency(array[i].money_cn);
                     array[i].money_vn_string = formatCurrency(array[i].money_vn);
                     //
                     add = false;
                     break;
                  }
               }
            }
            if(add){
               item.shop_nick= decodeURI(item.shop_nick);
               //
               item.pro_id = array.length+1;
               item.url = '<a target="_blank" href="'+item.pro_link+'"><img src="'+item.image+'" alt="" /></a>';
               item.namepro = '<a href="'+item.pro_link+'" target="_blank" class="text-blue"><span>'+item.name+'</span></a>';
               item.price_vn = item.rate*item.price;
               item.price_string = formatCurrency(item.price);
               item.price_vn_string = formatCurrency(item.rate*item.price);
               item.money_cn = item.amount*item.price;
               item.money_vn = item.amount*item.price*item.rate;
               item.money_cn_string = formatCurrency(item.money_cn);
               item.money_vn_string = formatCurrency(item.money_vn);
               item.is_check = false;
               item.is_wood = false;
               item.shop_note = '';
               //
               array.push(item);
            }
            return array;
         },

         isInShop: function(item,array){
            var add = true;
            var comment = [];
            if(array.length){
               for (var i = array.length - 1; i >= 0; i--) {
                  if(array[i].shop_link == item.shop_link){
                     add = false;
                     break;
                  }
               }
            }
            //
            if(add){
               array.push({shop_link:item.shop_link,shop_nick:decodeURI(item.shop_nick)});
            }
            return array;
         },
         isProductInShop: function(pro, shop){
            var shopdata = [];
            var data = [];
            var count = 0;
            var money_cn = 0;
            var money_vn = 0;
            var rate = 0;
            var total_count = 0;
            var total_money_cn = 0;
            var total_money_vn = 0;
            var money_cn_string = null;
            var money_vn_string = null;
            var is_check = false;
            var is_wood = false;
            var shop_note = '';
            for (var i = shop.length - 1; i >= 0; i--) {
               for (var j = pro.length - 1; j >= 0; j--) {
                  if(i==(shop.length - 1)){pro[j].item =  pro.length - 1 - j;}
                  if(shop[i].shop_link == pro[j].shop_link){
                     data.push(pro[j]);
                     count += pro[j].amount;
                     money_cn += pro[j].money_cn; 
                     money_vn += pro[j].money_vn; 
                     rate = pro[j].rate;
                     if(pro[j].is_check){
                        is_check = true;
                     }
                     if(pro[j].is_wood){
                        is_wood = true;
                     }
                     shop_note = pro[j].shop_note;
                  }
               };
               shop[i].comment = [];
               shop[i].selected = true;
               shop[i].money_cn = money_cn;
               shop[i].money_vn = money_vn;
               shop[i].money_cn_string = formatCurrency(money_cn);
               shop[i].money_vn_string = formatCurrency(money_vn);
               shop[i].product = data;
               shop[i].rate = rate;
               shop[i].total = count;
               shop[i].is_check = is_check;
               shop[i].is_wood = is_wood;
               shop[i].shop_note = shop_note;
               total_count += count;
               total_money_cn += money_cn;
               total_money_vn += money_vn;
               count = 0;
               data = [];
               money_cn = 0;
               money_vn = 0;
               rate = 0;
               is_check = false;
               is_wood = false;
            };
            shopdata = {shop:shop,total_shop:shop.length,total:total_count,money_cn:total_money_cn,money_vn:total_money_vn,money_cn_string:formatCurrency(total_money_cn),money_percent_string:formatCurrency(total_money_vn*0.9),money_vn_string:formatCurrency(total_money_vn)};
            return shopdata;
         }        
         //
      }
   }]);
   app1.controller('cartcontroller', ['$scope', '$timeout', '$rootScope', '$cookies','$cookieStore','myFactory1','$window', function($scope, $timeout, $rootScope, $cookies, $cookieStore, myFactory1, $window){
      //
      var old_cart = localStorage.getItem("old_8b54d81e2e464b085b44ef1b7a8e191a");
      if(typeof old_cart === 'undefined' || old_cart === null || old_cart.length === 0){
         var cart = myFactory1.getlocalData('cart_8b54d81e2e464b085b44ef1b7a8e191a');
         if(cart){
            // myFactory1.setlocalData('old_8b54d81e2e464b085b44ef1b7a8e191a',cart );
            localStorage.setItem('old_8b54d81e2e464b085b44ef1b7a8e191a', cart);
         }
      }
      // 
      $scope.cartquantity = 0;//set default
      myFactory1.syncDataShop();
      $timeout(function(){
         var dataShop = myFactory1.getDataShop('cart_8b54d81e2e464b085b44ef1b7a8e191a');
         if(dataShop){
            shop = JSON.parse(dataShop);
            tmp = myFactory1.isProductInShop(myFactory1.getProduct(shop), myFactory1.getShop(shop));
            // $scope.cartquantity = tmp.total;
            $scope.cartquantity = tmp.total_shop;
            $scope.cart = tmp;
         }
      },1500);

      $timeout(function(){
         localStorage.removeItem('cart_8b54d81e2e464b085b44ef1b7a8e191a');
      },3000);
   }]);
   
function updatewarehouse(id){
   // console.log(id);
      $.ajax({
         url: '/thanh-vien/tai-khoan/doi-kho',
         method: 'POST',
         data: {
            "warehouse_id": id,
            "_token": $('body').data("token")                  
         },
         dataType: 'json',
         beforeSend: function () {
         },
         success: function(data) {
            $.gritter.add({
                text: data.message,
                class_name: 'gritter-title'
            });
            return false;
         },
         error: function(xhr, textStatus, error){
            $.gritter.add({
                text: 'Lỗi hệ thống',
                class_name: 'gritter-title'
            });
            return false;
        }
      });
}


function noteshop(el){
   var note = $(el).val();
   var parent = $(el).parent('.note').parent('.col-sm-7').parent('.row_block').parent('.panel-heading');
   // console.log('oppa',parent);
   var nick = $(parent).children('.shop-nick').val();
   var link =$(parent).children('.shop-link').val();
   var item = {'shop_nick':nick,'shop_link':link,'shop_note':note};
   // console.log('noteshop',item);
   updateDataShop(JSON.stringify(item),2,2);//update shop
}

function myupdatenote(el){
   var note = $(el).val();
   var parent = $(el).parent('.text-quantity').parent('.col-sm-8').parent('.row').parent('.col-sm-7').parent('.row');
   var name = $(parent).children('.pro-name').val();
   var image = $(parent).children('.pro-image').val();
   var link = $(parent).children('.pro-link').val();
   var size = $(parent).children('.pro-size').val();
   var color = $(parent).children('.pro-color').val();
   var price = $(parent).children('.pro-price').val();
   var amount = $(parent).children('.pro-amount').val();
   var item = {'pro_link':link,'name':name,'image':image,'size':size,'color':color,'note':note};
   // console.log('myupdatenote',item);
   updateDataShop(JSON.stringify(item),2,2);//update product
}
function myupdate(el){
   var amount = $(el).val();
   var parent = $(el).parent('.text-quantity').parent('.col-sm-4').parent('.row').parent('.col-sm-5').parent('.row');
   var name = $(parent).children('.pro-name').val();
   var image = $(parent).children('.pro-image').val();
   var link = $(parent).children('.pro-link').val();
   var size = $(parent).children('.pro-size').val();
   var color = $(parent).children('.pro-color').val();
   var price = $(parent).children('.pro-price').val();
   var amount = $(parent).children('.pro-amount').val();
   if(amount>0){
      var item = {'pro_link':link,'name':name,'image':image,'size':size,'color':color,'price':price,'amount':amount};
      console.log('myupdate',item);
      updateDataShop(JSON.stringify(item),2,2);//update product
   }
}
function updateDataShop(item, type,update){
   var tmp = '';
   $.ajax({
      url: '/san-pham/sync-shopcart',
      method: 'POST',
      crossDomain:true,
      // async: false,
      data: {
         "type":type,//update shop and product
         "cart_update": update, //update or delete
         "cart_item": item, 
         "_token": $('body').data("token")
         
      },
      dataType: 'json',
      beforeSend: function () {
         $('#loading').show();
      },
      success: function(data) {
         tmp = data;
         if (data.error == 0) {
            // setlocalData('cart_8b54d81e2e464b085b44ef1b7a8e191a',data.data);
            // tmp = data.data;
         } else {
            
         }
         $('#loading').hide();
      },
      error: function(xhr, textStatus, error){
        
     }
   });
   return tmp;
};
// 
function showhide(el){
   var parent = $(el).parent('div').parent('div').parent('div').parent('th').parent('tr').parent('thead').parent('table').parent('.container-repeat1');
   var block = $(parent).find('.shop-block');
   if ( $(block).css('display') == 'none' ){
       // console.log(block);
       $(block).show();
   }else{
      $(block).hide();
   }
}