using AsyncMessageSystem.Services;
namespace IOrderRepo.Test;
public class OrderRepo_Test
{    
    OrderRepositoryPql _orderRepo;
    AppDbContext _dbCtx;
    public OrderRepo_Test()
    {
       this._dbCtx = new AppDbContext();
       this._orderRepo = new OrderRepositoryPql(_dbCtx);

    }
    public void GetAllOrders_Should_ReturnAllOrders()
    {
        
    }
   
}