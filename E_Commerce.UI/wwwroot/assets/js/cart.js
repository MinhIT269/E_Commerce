(function ($) {
    const baseUrl = window.location.origin;

    // 1) Load mini-cart
    function loadMiniCart() {
        $.ajax({
            url: `${baseUrl}/User/Cart/GetCartItems`,
            method: 'GET',
            dataType: 'json'
        })
            .done(function (cart) {
                $('.cart-count.badge-circle').text(cart.cartItems.length);

                let html = '';
                if (!cart.cartItems.length) {
                    html = '<p class="text-center">Giỏ hàng trống.</p>';
                } else {
                    cart.cartItems.forEach(item => {
                        const priceFormatted = new Intl.NumberFormat('vi-VN', {
                            style: 'currency',
                            currency: 'VND'
                        }).format(item.price);

                        html += `
                        <div class="product" id="cart-item-${item.cartItemId}">
                            <div class="product-details">
                                <h4 class="product-title">${item.productName}</h4>
                                <span class="cart-product-info">
                                    <span class="cart-product-qty">${item.quantity}</span> × ${priceFormatted}
                                </span>
                            </div>
                            <figure class="product-image-container">
                                <img src="${item.imageUrl}" width="80" height="80" alt="${item.productName}">
                                <a href="#" class="btn-remove" data-cartitem-id="${item.cartItemId}">
                                    <span>×</span>
                                </a>
                            </figure>
                        </div>`;
                    });
                }

                $('.dropdown-cart-products').html(html);
                $('.cart-total-price').text(
                    new Intl.NumberFormat('vi-VN', {
                        style: 'currency',
                        currency: 'VND'
                    }).format(cart.totalPrice)
                );
            })
            .fail(function (xhr) {
                if (xhr.status === 401) {
                    $('.dropdown-cart-products').html('<p class="text-center text-danger">Bạn chưa đăng nhập.</p>');
                    $('.cart-count.badge-circle').text('0');
                } else {
                    console.error('Không tải được giỏ hàng.');
                    $('.cart-count.badge-circle').text('0');
                }
            });
    }

    // 2) Load full cart page if present
    function loadCartPage() {
        const container = $('#cart-items-container');
        if (!container.length) return;

        $.ajax({
            url: `${baseUrl}/User/Cart/GetCartItems`,
            method: 'GET',
            dataType: 'json',
            success: function (cart) {
                container.empty();

                if (!cart.cartItems || cart.cartItems.length === 0) {
                    container.html('<tr><td colspan="5" class="text-center">Giỏ hàng trống.</td></tr>');
                    $('.cart-total-price').text('0 ₫');
                    return;
                }

                let html = '';
                cart.cartItems.forEach(item => {
                    const itemTotal = item.price * item.quantity;
                    html += `
                        <tr class="product-row" id="cart-item-${item.cartItemId}">
                            <td>
                                <figure class="product-image-container">
                                    <a href="/product/${item.productId}" class="product-image">
                                        <img src="${item.imageUrl}" alt="${item.productName}">
                                    </a>
                                    <a href="#" class="btn-remove icon-cancel" title="Xóa sản phẩm" data-cartitem-id="${item.cartItemId}"></a>
                                </figure>
                            </td>
                            <td class="product-col">
                                <h5 class="product-title">
                                    <a href="/product/${item.productId}">${item.productName}</a>
                                </h5>
                            </td>
                            <td>${item.price.toLocaleString('vi-VN')} ₫</td>
                            <td>
                                <div class="input-group product-single-qty">
                                    <div class="input-group bootstrap-touchspin bootstrap-touchspin-injected">
                                        <button class="btn btn-outline-primary btn-decrement" type="button" aria-label="Giảm số lượng">−</button>
                                        <input type="number"
                                               class="form-control text-center horizontal-quantity"
                                               min="1"
                                               value="${item.quantity}"
                                               data-cartitem-id="${item.cartItemId}"
                                               data-product-id="${item.productId}"
                                               placeholder="1"
                                               style="background-color: white"
                                               aria-label="Số lượng sản phẩm" readonly>
                                        <button class="btn btn-outline-primary btn-increment" type="button" aria-label="Tăng số lượng">+</button>
                                    </div>
                                </div>
                            </td>
                            <td class="text-right">
                                <span class="subtotal-price">${itemTotal.toLocaleString('vi-VN')} ₫</span>
                            </td>
                        </tr>`;
                });

                container.html(html);
                $('.cart-total-price').text(cart.totalPrice.toLocaleString('vi-VN') + ' ₫');
            },
            error: function (xhr, status, error) {
                console.error('Lỗi khi tải giỏ hàng:', error);
                container.html('<tr><td colspan="5" class="text-center text-danger">Không thể tải giỏ hàng. Vui lòng thử lại sau.</td></tr>');
                $('.cart-total-price').text('0 ₫');
            }
        });
    }

    // 3) Add product to cart
    $(document).on('click', '.add-cart', function (e) {
        e.preventDefault();
        const productId = $(this).data('product-id');

        $.ajax({
            url: `${baseUrl}/User/Cart/AddToCart`,
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ ProductId: productId, Quantity: 1 })
        })
            .done(function (res) {
                alert(res.message || 'Thêm vào giỏ thành công!');
                loadMiniCart();
                loadCartPage();
            })
            .fail(function (xhr) {
                if (xhr.status === 401) {
                    alert('Bạn phải đăng nhập để thêm sản phẩm.');
                } else {
                    alert('Lỗi thêm giỏ hàng: ' + xhr.responseText);
                }
            });
    });

    // 4) Remove product from cart
    $(document).on('click', '.btn-remove', function (e) {
        e.preventDefault();
        const cartItemId = $(this).data('cartitem-id');
        if (!confirm('Bạn chắc chắn muốn xóa?')) return;

        $.ajax({
            url: `${baseUrl}/User/Cart/Remove`,
            method: 'DELETE',
            contentType: 'application/json',
            data: JSON.stringify({ CartItemId: cartItemId })
        })
            .done(function (res) {
                alert(res.message || 'Xóa thành công!');
                loadMiniCart();
                loadCartPage();
                loadCheckoutSummary();
            })
            .fail(function (xhr) {
                if (xhr.status === 401) {
                    alert('Bạn phải đăng nhập để xóa sản phẩm.');
                } else {
                    alert('Lỗi xóa giỏ hàng: ' + xhr.responseText);
                }
            });
    });

    // 5) Tăng/Giảm số lượng và cập nhật tổng tiền (client-side)
    $(document).on('click', '.btn-increment', function () {
        const input = $(this).siblings('input');
        const current = parseInt(input.val()) || 1;
        input.val(current + 1).trigger('change');
    });

    $(document).on('click', '.btn-decrement', function () {
        const input = $(this).siblings('input');
        const current = parseInt(input.val()) || 1;
        if (current > 1) {
            input.val(current - 1).trigger('change');
        }
    });

    // 6) Cập nhật tổng phụ và tổng tiền khi thay đổi số lượng
    $(document).on('change', '.horizontal-quantity', function () {
        const input = $(this);
        const quantity = parseInt(input.val()) || 1;
        const row = input.closest('.product-row');

        const priceText = row.find('td').eq(2).text().replace(/[^\d]/g, '');
        const price = parseInt(priceText) || 0;
        const subtotal = quantity * price;

        row.find('.subtotal-price').text(subtotal.toLocaleString('vi-VN') + ' ₫');

        let total = 0;
        $('.subtotal-price').each(function () {
            const value = parseInt($(this).text().replace(/[^\d]/g, '')) || 0;
            total += value;
        });
        $('.price-temp').text(total.toLocaleString('vi-VN') + ' ₫');
    });

    $(document).on('click', '.btn-update-cart', function (e) {
        e.preventDefault();

        const cartItems = [];
        $('.product-row').each(function () {
            const row = $(this);
            const productId = row.find('.horizontal-quantity').data('product-id');  // Lấy productId
            const quantity = parseInt(row.find('.horizontal-quantity').val()) || 1;

            cartItems.push({
                productId: productId,
                quantity: quantity
            });
        });

        const requestData = {
            Items: cartItems
        };

        // Gửi yêu cầu PUT tới API để cập nhật giỏ hàng
        $.ajax({
            url: `${baseUrl}/User/Cart/UpdateCartItems`,
            method: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(requestData),
            success: function (res) {
                alert(res.message || 'Cập nhật giỏ hàng thành công!');
                loadMiniCart();  // Cập nhật mini-cart
                loadCartPage();  // Cập nhật trang giỏ hàng
            },
            error: function (xhr) {
                if (xhr.status === 401) {
                    alert('Bạn phải đăng nhập để cập nhật giỏ hàng.');
                } else {
                    alert('Lỗi cập nhật giỏ hàng: ' + xhr.responseText);
                }
            }
        });
    });
    // Cuối cart.js, sau tất cả hàm xử lý khác

    $('#checkout-form').on('submit', function (e) {
        e.preventDefault();

        const FirstName = $('input[name="FirstName"]').val();
        const LastName = $('input[name="LastName"]').val();
        const Address = $('input[name="Address"]').val();
        const PhoneNumber = $('input[name="PhoneNumber"]').val();
        const PromotionCode = $('#promotion-code-input').val().trim();

        $.ajax({
            url: `${baseUrl}/User/Cart/GetCartItems`,
            method: 'GET',
            dataType: 'json',
            success: function (cart) {
                if (!cart.cartItems || cart.cartItems.length === 0) {
                    alert('Giỏ hàng trống, không thể đặt hàng!');
                    return;
                }

                const orderItems = cart.cartItems.map(item => ({
                    productId: item.productId,
                    quantity: item.quantity
                }));

                const OrderRequestDto = {
                    Description: "Đơn hàng mới",
                    CreatedDate: new Date().toISOString(),
                    PromotionCode: PromotionCode || null,
                    Items: orderItems,
                    UserInfo: {
                        FirstName,
                        LastName,
                        Address,
                        PhoneNumber
                    }
                };

                $.ajax({
                    url: '/User/Checkout/CreateOrder',
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify(OrderRequestDto),
                    success: function (res) {
                        if (res && res.redirect) {
                            window.location.href = res.redirect;
                        } else {
                            window.location.href = res;
                        }
                    },
                    error: function (xhr) {
                        alert('Đặt hàng thất bại. Vui lòng thử lại!');
                        console.error(xhr.responseText);
                    }
                });
            },
            error: function () {
                alert('Không thể lấy giỏ hàng. Vui lòng thử lại!');
            }
        });
    });

    function loadCheckoutSummary() {
        const tbody = $('#checkout-cart-items');
        const totalEl = $('#checkout-total');

        $.ajax({
            url: `${baseUrl}/User/Cart/GetCartItems`,
            method: 'GET',
            dataType: 'json',
            success: function (cart) {
                tbody.empty();

                if (!cart.cartItems || cart.cartItems.length === 0) {
                    tbody.html('<tr><td colspan="2" class="text-center">Không có sản phẩm trong giỏ hàng.</td></tr>');
                    totalEl.text('0 ₫');
                    return;
                }

                let html = '';
                let subtotal = 0;

                cart.cartItems.forEach(item => {
                    const itemTotal = item.price * item.quantity;
                    subtotal += itemTotal;

                    const priceFormatted = new Intl.NumberFormat('vi-VN', {
                        style: 'currency',
                        currency: 'VND'
                    }).format(itemTotal);

                    html += `
                    <tr>
                        <td class="product-col">
                            <h3 class="product-title">${item.productName}<span class="product-qty" style="font-weight: bold"> x ${item.quantity}</span></h3>
                        </td>
                        <td class="price-col">
                            <span>${priceFormatted}</span>
                        </td>
                    </tr>`;
                });

                const subtotalFormatted = new Intl.NumberFormat('vi-VN', {
                    style: 'currency',
                    currency: 'VND'
                }).format(subtotal);

                tbody.html(html);
                totalEl.text(subtotalFormatted); // nếu không có phí ship hoặc khuyến mãi
                $('#checkout-subtotal').text(subtotalFormatted);
            },
            error: function () {
                totalEl.text('0 ₫');
            }
        });
    }

    // 6) Init on document ready
    $(function () {
        loadMiniCart();
        loadCartPage();
        loadCheckoutSummary();
    });

})(jQuery);
