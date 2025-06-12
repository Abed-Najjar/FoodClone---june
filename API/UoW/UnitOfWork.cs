using API.Data;
using API.Repositories.Interfaces;

namespace API.UoW
{
    public class UnitOfWork(
        AppDbContext context,
        ICategoryRepository categoryRepository,
        IDishRepository dishRepository,
        IRestaurantRepository restaurantRepository,
        IUserRepository userRepository,
        IOrderRepository orderRepository,
        IAddressRepository addressRepository,
        ICartRepository cartRepository,
        IOtpRepository otpRepository) : IUnitOfWork
    {
        private readonly AppDbContext _context = context;

        public ICategoryRepository CategoryRepository => categoryRepository;
        public IDishRepository DishRepository => dishRepository;
        public IRestaurantRepository RestaurantRepository => restaurantRepository;
        public IUserRepository UserRepository => userRepository;
        public IOrderRepository OrderRepository => orderRepository;
        public IAddressRepository AddressRepository => addressRepository;
        public ICartRepository CartRepository => cartRepository;
        public IOtpRepository OtpRepository => otpRepository;

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

    }
}
