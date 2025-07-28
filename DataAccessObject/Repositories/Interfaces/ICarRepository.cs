using BusinessObjects;

namespace DataAccessObjects.Repositories.Interfaces
{
    public interface ICarRepository
    {
        IEnumerable<Car> GetAll();
        Car? GetById(int id);
        Car? GetByLicensePlate(string licensePlate);
        bool Add(Car car);
        bool Update(Car car);
        bool Delete(int id);
        IEnumerable<Car> GetByStatus(string status);
        IEnumerable<Car> GetByBrand(int brandId);
        IEnumerable<Car> Search(string searchTerm);
    }
} 