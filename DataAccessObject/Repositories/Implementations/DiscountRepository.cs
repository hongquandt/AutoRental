using BusinessObjects;
using DataAccessObjects.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessObjects.Repositories.Implementations
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly AutoRentalPrnContext _context;

        public DiscountRepository()
        {
            _context = new AutoRentalPrnContext();
        }

        public IEnumerable<Discount> GetAll()
        {
            return _context.Discounts.OrderByDescending(d => d.DiscountId).ToList();
        }

        public Discount? GetById(int discountId)
        {
            return _context.Discounts.FirstOrDefault(d => d.DiscountId == discountId);
        }

        public void Add(Discount discount)
        {
            // Validate discount name uniqueness
            if (ExistsByName(discount.DiscountName))
            {
                throw new InvalidOperationException($"Voucher '{discount.DiscountName}' đã tồn tại!");
            }

            _context.Discounts.Add(discount);
            _context.SaveChanges();
        }

        public void Update(Discount discount)
        {
            var existingDiscount = _context.Discounts.FirstOrDefault(d => d.DiscountId == discount.DiscountId);
            if (existingDiscount == null)
            {
                throw new InvalidOperationException($"Không tìm thấy voucher với ID: {discount.DiscountId}");
            }

            // Validate discount name uniqueness (exclude current discount)
            if (ExistsByName(discount.DiscountName, discount.DiscountId))
            {
                throw new InvalidOperationException($"Voucher '{discount.DiscountName}' đã tồn tại!");
            }

            existingDiscount.DiscountName = discount.DiscountName;
            existingDiscount.DiscountValue = discount.DiscountValue;

            _context.SaveChanges();
        }

        public void Delete(int discountId)
        {
            var discount = _context.Discounts.FirstOrDefault(d => d.DiscountId == discountId);
            if (discount == null)
            {
                throw new InvalidOperationException($"Không tìm thấy voucher với ID: {discountId}");
            }

            // Check if discount is being used in any bookings
            var isUsed = _context.Bookings.Any(b => b.DiscountId == discountId);
            if (isUsed)
            {
                throw new InvalidOperationException($"Không thể xóa voucher '{discount.DiscountName}' vì đang được sử dụng trong đặt xe!");
            }

            _context.Discounts.Remove(discount);
            _context.SaveChanges();
        }

        public bool ExistsByName(string discountName, int? excludeId = null)
        {
            var query = _context.Discounts.Where(d => d.DiscountName == discountName);
            
            if (excludeId.HasValue)
            {
                query = query.Where(d => d.DiscountId != excludeId.Value);
            }

            return query.Any();
        }
    }
} 