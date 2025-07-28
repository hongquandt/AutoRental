using BusinessObjects;
using DataAccessObjects.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects.Repositories.Implementations
{
    public class CarRepository : ICarRepository
    {
        private readonly AutoRentalPrnContext _context;

        public CarRepository()
        {
            _context = new AutoRentalPrnContext();
        }

        public IEnumerable<Car> GetAll()
        {
            return _context.Cars
                .Include(c => c.Brand)
                .Include(c => c.CarImages)
                .OrderByDescending(c => c.CreatedDate)
                .ToList();
        }

        public Car? GetById(int id)
        {
            return _context.Cars
                .Include(c => c.Brand)
                .Include(c => c.CarImages)
                .FirstOrDefault(c => c.CarId == id);
        }

        public Car? GetByLicensePlate(string licensePlate)
        {
            return _context.Cars
                .Include(c => c.Brand)
                .Include(c => c.CarImages)
                .FirstOrDefault(c => c.LicensePlate == licensePlate);
        }

        public bool Add(Car car)
        {
            try
            {
                car.CreatedDate = DateTime.Now;
                _context.Cars.Add(car);
                return _context.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(Car car)
        {
            try
            {
                var existingCar = _context.Cars.Find(car.CarId);
                if (existingCar == null) return false;

                existingCar.BrandId = car.BrandId;
                existingCar.CarModel = car.CarModel;
                existingCar.LicensePlate = car.LicensePlate;
                existingCar.Seats = car.Seats;
                existingCar.PricePerDay = car.PricePerDay;
                existingCar.Status = car.Status;

                return _context.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                var car = _context.Cars.Find(id);
                if (car == null) return false;

                _context.Cars.Remove(car);
                return _context.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<Car> GetByStatus(string status)
        {
            return _context.Cars
                .Include(c => c.Brand)
                .Include(c => c.CarImages)
                .Where(c => c.Status == status)
                .OrderByDescending(c => c.CreatedDate)
                .ToList();
        }

        public IEnumerable<Car> GetByBrand(int brandId)
        {
            return _context.Cars
                .Include(c => c.Brand)
                .Include(c => c.CarImages)
                .Where(c => c.BrandId == brandId)
                .OrderByDescending(c => c.CreatedDate)
                .ToList();
        }

        public IEnumerable<Car> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            var term = searchTerm.ToLower();
            return _context.Cars
                .Include(c => c.Brand)
                .Include(c => c.CarImages)
                .Where(c => c.CarModel.ToLower().Contains(term) ||
                           c.LicensePlate.ToLower().Contains(term) ||
                           c.Brand.BrandName.ToLower().Contains(term) ||
                           c.Status.ToLower().Contains(term) ||
                           c.PricePerDay.ToString().Contains(term) ||
                           c.Seats.ToString().Contains(term))
                .OrderByDescending(c => c.CreatedDate)
                .ToList();
        }
    }
} 