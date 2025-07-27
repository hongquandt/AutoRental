using BusinessObjects;

namespace Service.Interfaces
{
    public interface IDiscountService
    {
        IEnumerable<Discount> GetAll();
        Discount? GetById(int discountId);
        bool Add(Discount discount);
        bool Update(Discount discount);
        bool Delete(int discountId);
    }
} 