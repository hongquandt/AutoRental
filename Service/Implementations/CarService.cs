using BusinessObjects;
using Service.Interfaces;
using DataAccessObjects.Repositories.Interfaces;
using DataAccessObjects.Repositories.Implementations;

namespace Service.Implementations
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepository;

        public CarService(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        public IEnumerable<Car> GetAll()
        {
            return _carRepository.GetAll();
        }

        public Car? GetById(int id)
        {
            return _carRepository.GetById(id);
        }

        public Car? GetByLicensePlate(string licensePlate)
        {
            return _carRepository.GetByLicensePlate(licensePlate);
        }

        public bool Add(Car car)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(car.CarModel) ||
                string.IsNullOrWhiteSpace(car.LicensePlate) ||
                car.Seats <= 0 ||
                car.PricePerDay <= 0)
            {
                return false;
            }

            // Check if license plate already exists
            if (_carRepository.GetByLicensePlate(car.LicensePlate) != null)
            {
                return false;
            }

            return _carRepository.Add(car);
        }

        public bool Update(Car car)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(car.CarModel) ||
                string.IsNullOrWhiteSpace(car.LicensePlate) ||
                car.Seats <= 0 ||
                car.PricePerDay <= 0)
            {
                return false;
            }

            // Check if license plate already exists for other cars
            var existingCar = _carRepository.GetByLicensePlate(car.LicensePlate);
            if (existingCar != null && existingCar.CarId != car.CarId)
            {
                return false;
            }

            return _carRepository.Update(car);
        }

        public bool Delete(int id)
        {
            return _carRepository.Delete(id);
        }

        public IEnumerable<Car> GetByStatus(string status)
        {
            return _carRepository.GetByStatus(status);
        }

        public IEnumerable<Car> GetByBrand(int brandId)
        {
            return _carRepository.GetByBrand(brandId);
        }

        public IEnumerable<Car> Search(string searchTerm)
        {
            return _carRepository.Search(searchTerm);
        }
    }
} 