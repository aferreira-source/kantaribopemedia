namespace app.plataforma.Interfaces;

public interface ICurrentUser
{
    //Task<Cart> GetCartAsync();
    Task<int> GetItemCountAsync();
    Task AddItemAsync(int productId, int quantity);
    Task UpdateItemAsync(int cartItemId, int quantity);
    Task DeleteItemAsync(int cartItemId);
    string? Cookie { get; set; }
}
