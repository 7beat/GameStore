﻿using GameStore.Models;

namespace GameStore.DataAccess.Repository.IRepository
{
    public interface ICookieShoppingCartRepository
    {
        List<CartItem> GetAll();
        void Add(CartItem item);
        void Update(CartItem item);

        // Test
        void Add(ShoppingCart item);
        List<ShoppingCart> GetAll2();
    }
}
