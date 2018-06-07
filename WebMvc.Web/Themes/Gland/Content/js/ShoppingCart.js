ShoppingCart = {
    ListProduct: [],
    Count: 0,

    Init: function() {

    },

    SetData: function (data) {
        ShoppingCart.Count = data.Count;
        ShoppingCart.ListProduct = data.Products;

        //$("#count_shopping_cart_stores,#count_shopping_cart_store_2").text(ShoppingCart.Count);
    },

    UpdateData: function (data) {
        if (data.State == 1) {
            ShoppingCart.Count = data.Count;
            ShoppingCart.ListProduct = data.Products;

            $("#count_shopping_cart_store_2,#count_shopping_cart_stores").text(ShoppingCart.Count);
            $("#cart-fixed").addClass("hover");
            setTimeout(function () { $("#cart-fixed").removeClass("hover"); }, 2000);
        }
    },

    AjaxError: function(xhr, ajaxOptions, thrownError){
        alert("Error: " + xhr.status + " " + thrownError);
    },

    AddProduct: function(id) {
        var CartAddViewModel = new Object();
        CartAddViewModel.Id = id;

        // Ajax call to post the view model to the controller
        var strung = JSON.stringify(CartAddViewModel);
        
        $.ajax({
            url: '/Cart/Addproduct',
            type: 'POST',
            data: strung,
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                ShoppingCart.UpdateData(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                ShoppingCart.AjaxError(xhr, ajaxOptions, thrownError);
            }
        });
    },

    addVariant: function(id,num) {
        var CartAddViewModel = new Object();
        CartAddViewModel.Id = id;
        CartAddViewModel.num = num;

        // Ajax call to post the view model to the controller
        var strung = JSON.stringify(CartAddViewModel);

        $.ajax({
            url: '/Cart/addVariant',
            type: 'POST',
            data: strung,
            contentType: 'application/json; charset=utf-8',
            success: function(data) {
                ShoppingCart.UpdateData(data);
            },
            error: function(xhr, ajaxOptions, thrownError) {
                ShoppingCart.AjaxError(xhr, ajaxOptions, thrownError);
            }
        });
    },

    removeItem: function (id){
        var CartAddViewModel = new Object();
        CartAddViewModel.Id = id;

        // Ajax call to post the view model to the controller
        var strung = JSON.stringify(CartAddViewModel);

        $.ajax({
            url: '/Cart/removeItem',
            type: 'POST',
            data: strung,
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                ShoppingCart.UpdateData(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                ShoppingCart.AjaxError(xhr, ajaxOptions, thrownError);
            }
        });
    },

    goToCartPage: function () {
        window.location = "/cart";
    },
}

ShoppingCart.Init();