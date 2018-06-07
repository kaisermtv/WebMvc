COMPUTERBUILDING = {
    ProductType: [],
    NowType: 1,

    Init: function() {

    },

    OnLoad: function(page) {
        var p = COMPUTERBUILDING.ProductType[COMPUTERBUILDING.NowType];

        var obj = new Object();
        obj.Id = p.ID;
        obj.page = page;

        // Ajax call to post the view model to the controller
        var strung = JSON.stringify(obj);

        $.ajax({
            url: '/Product/AjaxProductForClass',
            type: 'POST',
            data: strung,
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                COMPUTERBUILDING.UpdateData(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                COMPUTERBUILDING.AjaxError(xhr, ajaxOptions, thrownError);
            }
        });
    },

    UpdateData: function(data) {
        $("#pc_part_process").html(data);

        //alert(data);
    },

    AjaxError: function (xhr, ajaxOptions, thrownError) {
        alert("Error: " + xhr.status + " " + thrownError);
    },

    AddProductType: function(id, tt){
        var p = {
            ID: id,
            TT: tt,
            Select:null,
        };

        COMPUTERBUILDING.ProductType[tt] = p;
    },

    SetProductType: function (tt) {
        $("#pc_part_process").html("");
        
        COMPUTERBUILDING.NowType = tt;
        
        COMPUTERBUILDING.OnLoad(1);
    },

    SetPaging: function (page) {
        //$("#pc_part_process").html("");
        COMPUTERBUILDING.OnLoad(page);
    },

    ReTotalPrice: function () {
        var i = 0;
        var price = 0;
        
        while (true){
            i++;
            
            var p = COMPUTERBUILDING.ProductType[i];
            if (p == null) {
                break;
            }

            if (p.Select != null){
                price += p.Select.IntPrice;
            }
        }
        price = price.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1.");
        $('#pc_part_total_price').html("Tổng giá: <span>" + price + " VND</span>");
    },

    SelectProduct: function(id,name,price,link,intprice) {
        var pa = {
            ID: id,
            NAME: name,
            PRICE: price,
            IntPrice: intprice,
        };
        
        var p = COMPUTERBUILDING.ProductType[COMPUTERBUILDING.NowType];
        p.Select = pa;

        var html = '<p class="cssName"><a href = "' + link + '" target = "_blank" > ' + name + '</a ></p ><p class="cssSelect"><b>' + price + '</b><a href="javascript:COMPUTERBUILDING.ReSelect(' + COMPUTERBUILDING.NowType + ')">Chọn lại</a> - <a href="javascript:COMPUTERBUILDING.RepmoveSelect(' + COMPUTERBUILDING.NowType +')">Xóa bỏ</a></p>';

        $('#part_selected_' + COMPUTERBUILDING.NowType).html(html);

        COMPUTERBUILDING.ReTotalPrice();
        COMPUTERBUILDING.NextProductType();
    },

    RepmoveSelect: function(tt){
        var p = COMPUTERBUILDING.ProductType[tt];
        if(p.Select != null){
            p.Select = null;
            $('#part_selected_' + tt).html("");
            COMPUTERBUILDING.ReTotalPrice();
        }
    },

    ReSelect: function(tt){
        this.SetProductType(tt);
    },

    NextProductType: function () {
        var tt = COMPUTERBUILDING.NowType;

        while (true){
            tt++;
            var p = COMPUTERBUILDING.ProductType[tt];
            if (p == null) {
                COMPUTERBUILDING.NextEnd();
                break;
            }

            if (p.Select == null){
                COMPUTERBUILDING.SetProductType(tt);
                break;
            }

        }
        
    },

    NextEnd:function(){
        $("#pc_part_process").html("Bạn đã xây dựng xong. Vui lòng <a href='javascript:COMPUTERBUILDING.ViewSelect()'>Click vào đây</a> để xem và in cấu hình");
    },

    ViewSelect: function() {
        
    },
}