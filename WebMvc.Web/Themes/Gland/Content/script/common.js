(function($){

    $.session = {

        _id: null,

        _cookieCache: undefined,

        _init: function()
        {
            if (!window.name) {
                window.name = Math.random();
            }
            this._id = window.name;
            this._initCache();

            // See if we've changed protcols

            var matches = (new RegExp(this._generatePrefix() + "=([^;]+);")).exec(document.cookie);
            if (matches && document.location.protocol !== matches[1]) {
               this._clearSession();
               for (var key in this._cookieCache) {
                   try {
                   window.sessionStorage.setItem(key, this._cookieCache[key]);
                   } catch (e) {};
               }
            }

            document.cookie = this._generatePrefix() + "=" + document.location.protocol + ';path=/;expires=' + (new Date((new Date).getTime() + 120000)).toUTCString();

        },

        _generatePrefix: function()
        {
            return '__session:' + this._id + ':';
        },

        _initCache: function()
        {
            var cookies = document.cookie.split(';');
            this._cookieCache = {};
            for (var i in cookies) {
                var kv = cookies[i].split('=');
                if ((new RegExp(this._generatePrefix() + '.+')).test(kv[0]) && kv[1]) {
                    this._cookieCache[kv[0].split(':', 3)[2]] = kv[1];
                }
            }
        },

        _setFallback: function(key, value, onceOnly)
        {
            var cookie = this._generatePrefix() + key + "=" + value + "; path=/";
            if (onceOnly) {
                cookie += "; expires=" + (new Date(Date.now() + 120000)).toUTCString();
            }
            document.cookie = cookie;
            this._cookieCache[key] = value;
            return this;
        },

        _getFallback: function(key)
        {
            if (!this._cookieCache) {
                this._initCache();
            }
            return this._cookieCache[key];
        },

        _clearFallback: function()
        {
            for (var i in this._cookieCache) {
                document.cookie = this._generatePrefix() + i + '=; path=/; expires=Thu, 01 Jan 1970 00:00:01 GMT;';
            }
            this._cookieCache = {};
        },

        _deleteFallback: function(key)
        {
            document.cookie = this._generatePrefix() + key + '=; path=/; expires=Thu, 01 Jan 1970 00:00:01 GMT;';
            delete this._cookieCache[key];
        },

        get: function(key)
        {
            return window.sessionStorage.getItem(key) || this._getFallback(key);
        },

        set: function(key, value, onceOnly)
        {
            try {
                window.sessionStorage.setItem(key, value);
            } catch (e) {}
            this._setFallback(key, value, onceOnly || false);
            return this;
        },
        
        'delete': function(key){
            return this.remove(key);
        },

        remove: function(key)
        {
            try {
            window.sessionStorage.removeItem(key);
            } catch (e) {};
            this._deleteFallback(key);
            return this;
        },

        _clearSession: function()
        {
          try {
                window.sessionStorage.clear();
            } catch (e) {
                for (var i in window.sessionStorage) {
                    window.sessionStorage.removeItem(i);
                }
            }
        },

        clear: function()
        {
            this._clearSession();
            this._clearFallback();
            return this;
        }

    };

    $.session._init();

})(jQuery);

function GetURLParameter(sParam)
{
    var sPageURL = window.location.search.substring(1);
    var sURLVariables = sPageURL.split('&');
    for (var i = 0; i < sURLVariables.length; i++)
    {
        var sParameterName = sURLVariables[i].split('=');
        if (sParameterName[0] == sParam)
        {
            return sParameterName[1];
        }
    }
}
//Popup hotline

$(document).ready(function(e) {
  if($("#content_popup_order").is(":visible")) $("#order_popup").width(720);
  $(".btn_popup").click(function(){
    $("#content_popup_order").slideToggle("fast");
    if($(".btn_popup.show").is(":visible")){
      $(".btn_popup.show").hide();
      $("#order_popup").width(720);
    }else{
      $(".btn_popup.show").show();
      $("#order_popup").width(320);
    }
    
    if($(".btn_popup.hide").is(":visible")){
      $(".btn_popup.hide").hide();
      $("#order_popup").width(320);
    }else{
      $(".btn_popup.hide").show();
      $("#order_popup").width(720);
    }
    
    return false;
  });
  
  $("#nav_vertical ul li").hover(function(){
    $(this).find(".sub_nav").css("left","207px");
  },function(){
    $(this).find(".sub_nav").css("left","-2209px");
  });
});


$(document).ready(function(){
    //SEARCH
    $("#text_search").width($("#search").width() - $("#ul_cat").width() - 65);
    $("#ul_cat").hover(function(){
        $(this).children("ul").show();
    },function(){
    	$(this).children("ul").hide();
    });
    $("#ul_cat li").click(function(){
        $("#ul_cat .selected span").text($(this).text());
        $("#sel_cat").val($(this).attr("title"));
        $("#text_search").width($("#search").width() - $("#ul_cat").width() - 65);
      	$("#ul_cat ul").hide();
    });
});


$(document).ready(function(){
    var arr_price;
    $(".img_price").each(function(){
        str_price = $(this).text().trim();

        if(str_price !='Liên hệ' && str_price !='Contact'){
            arr_price = str_price.split("");
            for(i=0;i< arr_price.length;i++){
                if(arr_price[i] == 0) arr_price[i] = "<span class='price-number-small-0'></span>";
                if(arr_price[i] == 1) arr_price[i] = "<span class='price-number-small-1'></span>";
                if(arr_price[i] == 2) arr_price[i] = "<span class='price-number-small-2'></span>";
                if(arr_price[i] == 3) arr_price[i] = "<span class='price-number-small-3'></span>";
                if(arr_price[i] == 4) arr_price[i] = "<span class='price-number-small-4'></span>";
                if(arr_price[i] == 5) arr_price[i] = "<span class='price-number-small-5'></span>";
                if(arr_price[i] == 6) arr_price[i] = "<span class='price-number-small-6'></span>";
                if(arr_price[i] == 7) arr_price[i] = "<span class='price-number-small-7'></span>";
                if(arr_price[i] == 8) arr_price[i] = "<span class='price-number-small-8'></span>";
                if(arr_price[i] == 9) arr_price[i] = "<span class='price-number-small-9'></span>";
                if(arr_price[i] == '.') arr_price[i] = "<span class='price-number-small-dot'></span>";
                if(i> arr_price.length - 4) arr_price[i]="";
            }

            $(this).html(arr_price);
        }
    });

    var arr_full_price;
    $(".img_price_full").each(function(){
        str_price = $(this).text().trim();

        if(str_price !='Liên hệ' && str_price !='Contact'){
            arr_price = str_price.split("");

            for(i=0;i< arr_price.length;i++){
                if(arr_price[i] == 0) arr_price[i] = "<span class='price-number-small-0'></span>";
                if(arr_price[i] == 1) arr_price[i] = "<span class='price-number-small-1'></span>";
                if(arr_price[i] == 2) arr_price[i] = "<span class='price-number-small-2'></span>";
                if(arr_price[i] == 3) arr_price[i] = "<span class='price-number-small-3'></span>";
                if(arr_price[i] == 4) arr_price[i] = "<span class='price-number-small-4'></span>";
                if(arr_price[i] == 5) arr_price[i] = "<span class='price-number-small-5'></span>";
                if(arr_price[i] == 6) arr_price[i] = "<span class='price-number-small-6'></span>";
                if(arr_price[i] == 7) arr_price[i] = "<span class='price-number-small-7'></span>";
                if(arr_price[i] == 8) arr_price[i] = "<span class='price-number-small-8'></span>";
                if(arr_price[i] == 9) arr_price[i] = "<span class='price-number-small-9'></span>";
                if(arr_price[i] == '.') arr_price[i] = "<span class='price-number-small-dot'></span>";
                if(arr_price[i] == "VNĐ") arr_price[i] = "";
            }
            $(this).html(arr_price);
        }
    });

});

//Compare Pro
$(document).ready(function(){
  		$("#product_compare_list").val("");
  		$("input.p_check").removeAttr("checked");
        $("input.p_check").click(function(){
            if($(this).is(":checked")){
                $(".compare_area").prepend("<a href='#' class='"+$(this).attr("id")+"'><img src='"+$(this).attr("name")+"' alt='' /><i class='bg icon_del_compare'></i></a>");
            }else{
				$("."+$(this).attr("id")).remove();
            }
  
  			$(".compare_area a").click(function(){
  			  $("#"+$(this).attr("class")).click();
              $("#"+$(this).attr("class")).removeAttr("checked");
  			  $(this).remove();
              return false;
  			});
  		    if($(".compare_area a").length >= 2) $(".btn_compare").show();
  			else $(".btn_compare").hide();
        });
  
    });

//Conver format price to price
function convertPrice(formatPrice){
	str = formatPrice.substring(0,formatPrice.length - 4);
  	str1 = str.replace('.','');
  	str2 = str1.replace('.','');
  	str3 = str2.replace('.','');
  	
  	return str3;
}

function formatCurrency(a) {
    var b = parseFloat(a).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1.").toString();
    var len = b.length;
    b = b.substring(0, len - 3);
    return b;
}


//POPUP
var urlLogin = "";

function confirmimage(url){
//$('select').hide();
    $('.popup').show();
    $('.overlay').show();
//window.location = url;
    urlLogin = url;
}
function closePop(){
//$('select').show();
    $('.popup').hide();
    $('.overlay').hide();
}
function confirmLogin(url){
//$('select').hide();
    $('.popup_login').show();
    $('.overlay').show();
//window.location = url;
    urlLogin = url;
}

function forwardLogin(){
    window.location = 'http://aha.vn';
}
$(document).keypress(function(e) {

    // ESCAPE key pressed
    if (escape(e.keyCode) == 27) {
        closePop();
        windown.close();
    }
});


/***************************/
//@Author: Adrian "yEnS" Mato Gondelle
//@website: www.yensdesign.com
//@email: yensamg@gmail.com
//@license: Feel free to use it, but keep this credits please!
/***************************/

//SETTING UP OUR POPUP
//0 means disabled; 1 means enabled;
var popupStatus = 0;

//loading popup with jQuery magic!
function loadPopup(){
    //loads popup only if it is disabled
    if(popupStatus==0){
        $("#backgroundPopup").css({
            "opacity": "0.7"
        });
        $("#backgroundPopup").fadeIn("slow");
        $("#popupContact").fadeIn("slow");
        popupStatus = 1;
    }
}

//disabling popup with jQuery magic!
function disablePopup(){
    //disables popup only if it is enabled
    if(popupStatus==1){
        $("#backgroundPopup").fadeOut("slow");
        $("#popupContact").fadeOut("slow");
        popupStatus = 0;
    }
}

//centering popup
function centerPopup(){
    //request data for centering
    var windowWidth = document.documentElement.clientWidth;
    var windowHeight = document.documentElement.clientHeight;
    var popupHeight = $("#popupContact").height();
    var popupWidth = $("#popupContact").width();
    //centering
    $("#popupContact").css({
        "position": "absolute",
        "top": windowHeight/2-popupHeight/2,
        "left": windowWidth/2-popupWidth/2
    });
    //only need force for IE6

    $("#backgroundPopup").css({
        "height": windowHeight
    });

}


//CONTROLLING EVENTS IN jQuery
$(document).ready(function(){

    //LOADING POPUP
    //centering with css
    centerPopup();
    //load popup
    loadPopup();
    //Click the button event!
    $("#button").click(function(){
        //centering with css
        centerPopup();
        //load popup
        loadPopup();
    });

    //CLOSING POPUP
    //Click the x event!
    $("#popupContactClose").click(function(){
        disablePopup();
    });
    //Click out event!
    $("#backgroundPopup").click(function(){
        disablePopup();
    });
    //Press Escape event!
    $(document).keypress(function(e){
        if(e.keyCode==27 && popupStatus==1){
            disablePopup();
        }
    });
  
  //Fixed category:
  var topfix = $(".ul_menu").offset().top + $(".ul_menu").height();
  $(window).load(function(){
  	$("#category-fixed").css("top",($(window).height() - $("#category-fixed").height())/2)
  });
  $(window).scroll(function(){
    if($(window).scrollTop() > topfix) $("#category-fixed").addClass("slideInLeft").fadeIn();
    else $("#category-fixed").removeClass("slideInLeft").fadeOut();
    
  });
});

//TOOLTIP
$(document).ready(function(e) {
		var w_tooltip = $(".tooltip").width();;
		var h_tooltip = 0;
		var pad = 10; 
		var x_mouse = 0; var y_mouse = 0;
		var wrap_left = 0;
		var wrap_right = 0;
		var wrap_top = 0;
		var wrap_bottom = 0;
	
    $(".product_list li .p_img").mousemove(function(e){
			wrap_left = $(this).parent().parent().parent().offset().left;
			wrap_top = $(this).parent().parent().parent().offset().top;
			wrap_bottom = $(this).parent().parent().parent().offset().top + $(this).parent().parent().parents(".product_list").height();
			x_mouse = e.pageX - $(this).offset().left;
			y_mouse = e.pageY - $(this).offset().top;
			h_tooltip = $(this).parent().parent().children(".tooltip").height();
			$(".tooltip").hide();
			
           
			//Move Horizontal
			if($(this).offset().left - pad - w_tooltip > wrap_left){
				$(this).parent().parent().children(".tooltip").css("left", 0-(w_tooltip + pad) + x_mouse);
			}else{
				$(this).parent().parent().children(".tooltip").css("left", pad + x_mouse + 20);
			}
			
			//Move Vertical		
			if(e.pageY + h_tooltip >= $(window).height() + $(window).scrollTop()){
				$(this).parent().parent().children(".tooltip").css("top", y_mouse - h_tooltip - pad)
			}else{
				$(this).parent().parent().children(".tooltip").css("top", pad+ y_mouse + 20);
				}
			//Show tooltip	
			$(this).parent().parent().children(".tooltip").show();
		});
			
		$(".product_list li .p_img").mouseout(function(){
			$(".tooltip").hide();
			});
  });


function compare_product(return_url) {
    var e = document.getElementById("product_compare_list");
    if (e == "undefined" || e == null) {
        alert("Cần có biến product_compare_list trong template");
        return false
    }
    var t = 0;
    if (e.value.length > 1) {
        current_list_id = e.value.split(",");
        t = current_list_id.length - 2
    }
    if (t > 1) {
        window.location = "/so-sanh?list=" + e.value + "&return_url=" + return_url;
    } else {
        alert("Bạn cần chọn tối thiểu 2 sản phẩm để so sánh");
        return false
    }
}


function sort_sub_nav(col_number, col_item, item_html_sort, parent_content){
        //Khoi tao
        var col_number = col_number; //so cot, = 0 neu muon sap xep tu dong theo theo so hang
        var col_item = col_item; //so hang trong 1 cot, = 0 neu muon sap xep tu dong theo so cot
        var item_html_sort = item_html_sort; //Phan tu muon sap xep
        var parent_content = parent_content;
        var item_per_col=0; //so phan tu 1 cot
        /*...............................*/
        var total_item = item_html_sort.length;
        if(col_item!=0) item_per_col = col_item;
        if(col_item==0 && col_number > 0) item_per_col = Math.ceil(total_item/col_number);

        var table_html = "";
        table_html+= "<table class='tbl_sub_nav'><tr><td valign='top'>";
        for(i=0;i<total_item; i++){
            table_html+= item_html_sort.parent().children(":eq("+i+")").html();
            if((i+1)%item_per_col==0) table_html+="</td><td valign='top'>";
        }
        table_html+="</tr></table></td>";
        parent_content.children().remove();
        parent_content.append(table_html);
    }



//show popup
function showPopup(a){
  if(a == 'popup-youtube'){
    var url = $("#url-video-popup").val();
  	$("#content-popup-youtube").html('<iframe width="495" height="315" src='+url+' frameborder="0" allowfullscreen></iframe>');
  }
  $("#bg-opacity").fadeIn();
  $("#"+a).fadeIn();
}

//close popup:
function closePopup(){
  	if($("#content-popup-youtube iframe").length > 0) $("#content-popup-youtube").empty();
	$(".popup-common").fadeOut();
  	$("#bg-opacity").fadeOut();
}

//gototop
$("#gotoTop").click(function(){
  	console.log("top");
	$('html,body').animate({scrollTop:0},800);
});
$(window).scroll(function(){
	if($(window).scrollTop() > 0) $("#gotoTop").fadeIn();
  	else $("#gotoTop").fadeOut();
});

(function (a) {
            (jQuery.browser = jQuery.browser || {}).mobile = /(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino/i.test(a) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0, 4))
        })(navigator.userAgent || navigator.vendor || window.opera);

$(function(){
  if(jQuery.browser.mobile==true){
    $(".tooltip").remove();
  	console.log(jQuery.browser.mobile);
  }	
  
});


