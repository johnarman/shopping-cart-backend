using ShoppingCart.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class CartCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public CartCleanupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var shoppingCartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();

                    await CleanupAbandonedCarts(shoppingCartService);
                }
            }
        }
        catch(Exception ex)
        {

        }
    }

    public async Task CleanupAbandonedCarts(IShoppingCartService shoppingCartService)
    {
        var expirationThreshold = TimeSpan.FromMinutes(15);
        var abandonedCartItems = await shoppingCartService.GetAbandonedCartItemsAsync(expirationThreshold);

        foreach (var cartItem in abandonedCartItems)
        {
            await shoppingCartService.RemoveItemFromCartAsync(cartItem.UserId, cartItem.ProductId);
        }
    }
}
