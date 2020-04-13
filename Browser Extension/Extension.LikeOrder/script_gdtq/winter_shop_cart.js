
window.addEventListener("message", function(request) {
   var received = [];
   // console.log('client_received',request);
   var check = merge(request.data);
   var data = initialShop();
   if(data && check){
      received = loadShopHtml(data);
      // console.log('shop_build',received);
   }
   
}, false);

// var GDTQ = 'https://gdtq.com';
var GDTQ = 'https://likeorder.com';

function loadShopHtml(data){
   return isProductInShop(getProduct(data),getShop(data));
};

function initialShop(){
   var dataClient = [];
   if (localStorage.getItem("cart_8b54d81e2e464b085b44ef1b7a8e191a") !== null) {
      dataClient = getlocalData('cart_8b54d81e2e464b085b44ef1b7a8e191a');
      dataClient = JSON.parse(dataClient);
      // console.log('2',dataClient);
   }
   if(dataClient.length){
      return dataClient;
   }else{
      return false;
   }
}
function merge(data){
   if(!isCookieExists('vgacartxyz')){
      // console.log('xoa ngay');
      localStorage.removeItem('cart_8b54d81e2e464b085b44ef1b7a8e191a');
   }
   var change = false;
   var shop_build = [];
   var data_cart = [];
   var shop = isArray(data);
   if(shop){
      change = true;
      // console.log('1a',shop);
      shop_build = getProduct(shop);
   }else{
      // console.log('1b',shop);
   }
   var dataClient = [];
   //
   if (localStorage.getItem("cart_8b54d81e2e464b085b44ef1b7a8e191a") !== null) {
      dataClient = getlocalData('cart_8b54d81e2e464b085b44ef1b7a8e191a');
      dataClient = JSON.parse(dataClient);
      // console.log('2',dataClient);
   }
   //
   if(shop_build.length){
      // console.log('3',shop_build);
      for (var i = shop_build.length - 1; i >= 0; i--) {
         data_cart = isInProduct(shop_build[i],dataClient);
      };
   }
   if(data_cart.length && change){
      $.cookie('vgacartxyz', generateUUID(), { expires: 30, path: '/' });
      // console.log('4',data_cart);
      // setlocalData('cart_build_8b54d81e2e464b085b44ef1b7a8e191a', JSON.stringify( isProductInShop(getProduct(data_cart),getShop(data_cart)) ));
      data_cart = check_duplicate(data_cart);
      setlocalData('cart_8b54d81e2e464b085b44ef1b7a8e191a', JSON.stringify(data_cart));
      //
      clearData();
      return true;
   }else{
      return false;
   }
};

function check_duplicate(data){
   var cart = [];
   var add = true;
   if(data.length){
      for (var i = 0; i < data.length; i++) {
         if(data[i] === 'object'){
            if(cart.length){
               for (var j = cart.length - 1; j >= 0; j--) {
                  if(data[i].pro_link == cart[j].pro_link && data[i].color == cart[j].color && data[i].size == cart[j].size && data[i].price == cart[j].price && data[i].shop_link == cart[j].shop_link){
                     add = false;
                  }
               }
            }
         }
         if(add){
            delete data[i].$$hashKey;
            cart.push(data[i]); 
         }else{
            add = true;
         }
      }
   }
   if(cart.length){
      return cart;
   }else{
      return data;
   }
};
function clientReceived(data){
   var shop_build = [];
   var shop = isArray(data);
   if(shop){
      // console.log('shop_origin',shop);
      shop_build = isProductInShop(getProduct(shop),getShop(shop));
      $("#loadShop").html(shop_build[0].shop_nick);
   }
   return shop_build;
   // if(typeof data.tbex_products !== 'undefined'){
   //    return isArray(data.tbex_products);
   // }
   
};

function getData(){
   // console.log(GDTQ);
   window.postMessage({type:"REQUEST_DATA"}, GDTQ);
};

function clearData(){
   // console.log(GDTQ);
   window.postMessage({type:"CLEAR_DATA"}, GDTQ);
};

function isArray(data){
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
};

function getProduct(shop){
   var data = [];
   for (var i = shop.length - 1; i >= 0; i--) {
      data = isInProduct(shop[i],data);
   };
   return data;
};

function getShop(shop){
   var data = [];
   for (var i = shop.length - 1; i >= 0; i--) {
      data = isInShop(shop[i],data);
   };
   return data;
}

function isInArray(value, array) {
   return array.indexOf(value) > -1;
}

function isInProduct(item, array){
   var add = true;
   if(array.length){
      for (var i = array.length - 1; i >= 0; i--) {
         if(array[i].pro_link == item.pro_link && array[i].color== item.color && array[i].size == item.size && array[i].price == item.price){
            array[i].shop_nick= decodeURI(item.shop_nick);
            array[i].amount += item.amount;
            if(array[i].note === '' && item.note !== ''){
               array[i].note += item.note;
            }else if(array[i].note !== item.note && array[i].note !== '' && item.note !== ''){
               array[i].note += ', '+ item.note;
            }
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
      //
      array.push(item);
   }
   return array;
};

function isInShop(item,array){
   var add = true;
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
      array.push({is_check:false,shop_link:item.shop_link,shop_nick:decodeURI(item.shop_nick)});
   }
   return array;
};

function isProductInShop(pro, shop){
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
   for (var i = shop.length - 1; i >= 0; i--) {
      for (var j = pro.length - 1; j >= 0; j--) {
         if(i==(shop.length - 1)){pro[j].item =  pro.length - 1 - j;}
         if(shop[i].shop_link == pro[j].shop_link){
            data.push(pro[j]);
            count += pro[j].amount;
            money_cn += pro[j].money_cn; 
            money_vn += pro[j].money_vn; 
            rate = pro[j].rate;
         }
      };
      shop[i].money_cn = money_cn;
      shop[i].money_vn = money_vn;
      shop[i].money_cn_string = formatCurrency(money_cn);
      shop[i].money_vn_string = formatCurrency(money_vn);
      shop[i].product = data;
      shop[i].rate = rate;
      shop[i].total = count;
      total_count += count;
      total_money_cn += money_cn;
      total_money_vn += money_vn;
      count = 0;
      data = [];
      money_cn = 0;
      money_vn = 0;
      rate = 0;
   };
   shopdata = {shop:shop,total:total_count,money_cn:total_money_cn,money_vn:total_money_vn,money_cn_string:formatCurrency(total_money_cn),money_vn_string:formatCurrency(total_money_vn)};
   return shopdata;
};

function formatCurrency(total) {
   var neg = false;
   if(total < 0) {
      neg = true;
      total = Math.abs(total);
   }
   return /*(neg ? "-$" : '$') + */parseFloat(total, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();
}

function getlocalData (key) {
   if(typeof(Storage) !== "undefined") {
      // console.log("getlocalData");
      return localStorage.getItem(key);
   } else {
      console.log('Sorry! No Web Storage support..');
      return false;
   }
   
};

function setlocalData (key,data) {
   if(typeof(Storage) !== "undefined") {
      // console.log("setlocalData");
      localStorage.setItem(key, data);
      return true;
   } else {
      console.log('Sorry! No Web Storage support..');
      return false;
   }
};

function saveLinkShop(val,key,type){
   var token = $('body').data("token") ;
   var result = null;
   $.ajax({
      url: '/san-pham/data-save-cart',
      method: 'POST',
      data: 'val='+val+'&&key='+ key+'&&_token='+token+'&&type='+type,
      dataType: 'json',
      async: false,
      success: function(data) {
         // console.log('saveLinkShop',data);
         if (data.error == 0) {
            result = data.data;
            if(data.uuid!==null){
               $.cookie('vgacartxyz', data.uuid ,{ expires: 30, path: '' });
            }
         } else {
            // result = data.data;
         }
      }
   });
   // console.log('result',result.responseText);
   return result;
};

function isCookieExists(cookiename) {
    return (typeof $.cookie(cookiename) !== "undefined");
};

function generateUUID(){
   var d = new Date().getTime();
   if(window.performance && typeof window.performance.now === "function"){
      d += performance.now();; //use high-precision timer if available
   }
   var uuid = 'xxxxxxxx_xxxx_4xxx_yxxx_xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
      var r = (d + Math.random()*16)%16 | 0;
      d = Math.floor(d/16);
      return (c=='x' ? r : (r&0x3|0x8)).toString(16);
   });
   uuid = uuid.replace(/\-/g,'');
   uuid = uuid.replace(/\_/g,'');
   return uuid;
}
/**/