using BusinessObjects;
using Service.Interfaces;
using DataAccessObjects.Repositories.Interfaces;
using DataAccessObjects.Repositories.Implementations;

namespace Service.Implementations
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;

        public DiscountService(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }

        public IEnumerable<Discount> GetAll()
        {
            try
            {
                return _discountRepository.GetAll();
            }
            catch (Exception)
            {
                // Log error here if needed
                throw;
            }
        }

        public Discount? GetById(int discountId)
        {
            try
            {
                return _discountRepository.GetById(discountId);
            }
            catch (Exception)
            {
                // Log error here if needed
                throw;
            }
        }

        public bool Add(Discount discount)
        {
            try
            {
                _discountRepository.Add(discount);
                return true;
            }
            catch (Exception)
            {
                // Log error here if needed
                throw;
            }
        }

        public bool Update(Discount discount)
        {
            try
            {
                _discountRepository.Update(discount);
                return true;
            }
            catch (Exception)
            {
                // Log error here if needed
                throw;
            }
        }

        public bool Delete(int discountId)
        {
            try
            {
                _discountRepository.Delete(discountId);
                return true;
            }
            catch (Exception)
            {
                // Log error here if needed
                throw;
            }
        }
    }
} 