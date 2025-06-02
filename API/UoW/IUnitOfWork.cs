using API.Repositories.Interfaces;

namespace API.UoW
{
    public interface IUnitOfWork
    {
        IOrderRepository OrderRepository { get; }
        IUserRepository UserRepository { get; }
        IRestaurantRepository RestaurantRepository { get; }
        IAddressRepository AddressRepository { get; }
        IDishRepository DishRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        ICartRepository CartRepository { get; }
        Task<int> CompleteAsync();
    }
}
