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
            Link: link,
            Count: 1,
        };
        
        var p = COMPUTERBUILDING.ProductType[COMPUTERBUILDING.NowType];
        p.Select = pa;

        var html = '<p class="cssName"><a href = "' + link + '" target = "_blank" > ' + name + '</a ></p ><p class="cssSelect"><b>' + price + '</b><a href="javascript:COMPUTERBUILDING.ReSelect(' + COMPUTERBUILDING.NowType + ')">Chọn lại</a> - <a href="javascript:COMPUTERBUILDING.RepmoveSelect(' + COMPUTERBUILDING.NowType +')">Xóa bỏ</a></p>';

        $('#part_selected_' + COMPUTERBUILDING.NowType).html(html);

        COMPUTERBUILDING.ViewUpdate();
        COMPUTERBUILDING.NextProductType();
    },

    SelectProductNum: function (tt, i) {
        var p = COMPUTERBUILDING.ProductType[tt];
        if (p.Select != null) {
            p.Select.Count = i;
            COMPUTERBUILDING.ViewUpdate();
        }
    },

    RepmoveSelect: function(tt){
        var p = COMPUTERBUILDING.ProductType[tt];
        if(p.Select != null){
            p.Select = null;
            $('#part_selected_' + tt).html("");
            COMPUTERBUILDING.ViewUpdate();
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

    ViewUpdate: function(){
        var i = 0;
        var price = 0;
        var ithtml = "";

        while (true) {
            i++;

            var p = COMPUTERBUILDING.ProductType[i];
            if (p == null) {
                break;
            }

            if (p.Select != null) {
                var pr = p.Select;
                numPrice = pr.IntPrice*pr.Count;
                price += numPrice;

                numPricestr = numPrice.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1.");

                ithtml += "<tr id='pro" + i + "' class='itemPro'>";
                ithtml += "<td>"+ i +"</td>";
                ithtml += "<td>";
                ithtml += "    <p class='pcName'><a href='" + pr.Link + "' target='_blank'>" + pr.NAME + "</a></p>";
                ithtml += "    <p class='pcSummary'></p>";
                ithtml += "</td>";
                ithtml += "<td><input type='text' value='" + pr.Count + "' style='width:50px' onchange='COMPUTERBUILDING.SelectProductNum(" + i +",this.value)' class='cssCount'> </td>";
                ithtml += "    <td style='text-align:right; padding-right:5px;'>";
                ithtml += "        <span class='price'>" + pr.PRICE + " <span class='currencyVND'>VND</span></span>";

                ithtml += "    </td>";
                ithtml += "    <td style='text-align:right; padding-right:5px;'><span class='cssTotal" + i + "' relbefore='"+numPrice+"'>" + numPricestr+"</span></td>";
                ithtml += "    <td>36 Tháng</td>";
                ithtml += "    <td><a href='javascript: void (0); ' onclick='COMPUTERBUILDING.RepmoveSelect("+i+"); '><img src='" + ThemeUrl +"/Content/images/cart_del.png' alt='xóa'></a></td>";


                ithtml += "</tr>";
            }
        }
        $('#productData').html(ithtml);
        price = price.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1.");
        $('#TotalPrice').html("Tổng giá: <span>" + price + "</span>");
        $('#pc_part_total_price').html("Tổng giá: <span>" + price + " VND</span>");
    },

    ViewSelect: function() {
        COMPUTERBUILDING.ViewUpdate();
        $("#myModal").modal("show")
    },

    Commit: function () {
        var obj = new Object();
        obj.Products = [];

        var tt = 0;
        while(true){
            tt++;
            var p = COMPUTERBUILDING.ProductType[tt];
            if (p == null) {
                break;
            }
            if (p.Select != null) {
                var pr = p.Select;
                var pro = new Object();
                pro.id = pr.ID;
                pro.count = pr.Count;
                obj.Products[tt - 1] = pro;

                
            }
            
        }

        // Ajax call to post the view model to the controller
        var strung = JSON.stringify(obj);

        $.ajax({
            url: '/Cart/SetListProduct',
            type: 'POST',
            data: strung,
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                window.location = "/cart";
            },
            error: function (xhr, ajaxOptions, thrownError) {
                COMPUTERBUILDING.AjaxError(xhr, ajaxOptions, thrownError);
            }
        });
    },
}

function PrintElem(elem) {
    var mywindow = window.open('', 'PRINT', 'height=400,width=600');

    mywindow.document.write('<html><head><title>' + document.title + '</title>');
    mywindow.document.write('</head><body >');
    mywindow.document.write(document.getElementById(elem).innerHTML);
    mywindow.document.write('</body></html>');

    mywindow.document.close(); // necessary for IE >= 10
    mywindow.focus(); // necessary for IE >= 10*/

    mywindow.print();
    mywindow.close();

    return true;
}