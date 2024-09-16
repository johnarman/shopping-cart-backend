using ShoppingCart.Service.Interface;
using Common.DTOs;
using Common.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controller
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItems()
        {
            try
            {
                var userId = JwtHelper.GetUserIdFromToken(User);
                var cartItems = await _shoppingCartService.GetCartItemsAsync(userId);
                return Ok(cartItems);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddItemToCart([FromBody] CartDTO cartItemDto)
        {
            try
            {
                var userId = JwtHelper.GetUserIdFromToken(User);

                if (cartItemDto.UserId > 0 && cartItemDto.UserId != userId)
                    return BadRequest("Failed to add item to cart.");

                cartItemDto.UserId = userId;

                var result = await _shoppingCartService.AddOrUpdateItemInCartAsync(cartItemDto);
                if (result)
                {
                    return Ok("Item added to cart.");
                }
                else
                {
                    return BadRequest("Failed to add item to cart.");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCartItem([FromBody] CartDTO cartItemDto)
        {
            try
            {
                var userId = JwtHelper.GetUserIdFromToken(User);

                if (cartItemDto.UserId > 0 && cartItemDto.UserId != userId)
                    return BadRequest("Failed to add item to cart.");

                cartItemDto.UserId = userId;

                var result = await _shoppingCartService.UpdateCartItemQuantityAsync(cartItemDto);

                if (result)
                {
                    return Ok("Cart item quantity updated.");
                }
                else
                {
                    return BadRequest("Failed to update cart item quantity.");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveItemFromCart(int productId)
        {
            try
            {
                var userId = JwtHelper.GetUserIdFromToken(User);
                var result = await _shoppingCartService.RemoveItemFromCartAsync(userId, productId);

                if (result)
                {
                    return Ok("Item removed from cart.");
                }
                else
                {
                    return BadRequest("Failed to remove item from cart.");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
