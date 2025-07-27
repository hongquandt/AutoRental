using BusinessObjects;

namespace DataAccessObjects.Repositories.Interfaces
{
    public interface IDiscountRepository
    {
        IEnumerable<Discount> GetAll();
        Discount? GetById(int discountId);
        void Add(Discount discount);
        void Update(Discount discount);
        void Delete(int discountId);
        bool ExistsByName(string discountName, int? excludeId = null);
    }
} 