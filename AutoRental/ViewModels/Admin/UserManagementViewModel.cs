using BusinessObjects;
using Service.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AutoRental.ViewModels.Admin
{
    public class UserManagementViewModel : INotifyPropertyChanged
    {
        private readonly IUserService _userService;
        private ObservableCollection<User> _users;
        private User? _selectedUser;
        private string _searchText = "";
        private List<User> _allUsers; // Cache tất cả users để search nhanh hơn
        private string _statusMessage = "Sẵn sàng";
        private Role? _selectedRoleFilter = null;
        private ObservableCollection<Role> _availableRoles;

        // Properties
        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        public User? SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
                // Cập nhật trạng thái buttons khi chọn user
                OnPropertyChanged(nameof(CanEditUser));
                OnPropertyChanged(nameof(CanDeleteUser));
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                // Real-time search khi text thay đổi
                SearchUsers();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public Role? SelectedRoleFilter
        {
            get => _selectedRoleFilter;
            set
            {
                _selectedRoleFilter = value;
                OnPropertyChanged();
                // Apply filter when role selection changes
                ApplyFilters();
            }
        }

        public ObservableCollection<Role> AvailableRoles
        {
            get => _availableRoles;
            set
            {
                _availableRoles = value;
                OnPropertyChanged();
            }
        }

        // Computed Properties for Button States
        public bool CanEditUser => SelectedUser != null;
        public bool CanDeleteUser => SelectedUser != null;

        // Commands
        public ICommand LoadUsersCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand AddUserCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand ClearFiltersCommand { get; }

        public UserManagementViewModel(IUserService userService)
        {
            _userService = userService;
            _users = new ObservableCollection<User>();
            _allUsers = new List<User>();
            _availableRoles = new ObservableCollection<Role>();

            // Initialize Commands
            LoadUsersCommand = new RelayCommand(LoadUsers);
            SearchCommand = new RelayCommand(SearchUsers);
            RefreshCommand = new RelayCommand(RefreshUsers);
            AddUserCommand = new RelayCommand(AddUser);
            EditUserCommand = new RelayCommand(EditUser, () => CanEditUser);
            DeleteUserCommand = new RelayCommand(DeleteUser, () => CanDeleteUser);
            ClearFiltersCommand = new RelayCommand(ClearFilters);

            // Load users and roles when ViewModel is created
            LoadUsers();
            LoadAvailableRoles();
        }

        // Command Methods
        private void LoadUsers()
        {
            try
            {
                StatusMessage = "Đang tải...";
                _allUsers = _userService.GetAll().ToList(); // Cache tất cả users
                Users = new ObservableCollection<User>(_allUsers);
                
                // Load available roles after loading users
                LoadAvailableRoles();
                
                StatusMessage = "Sẵn sàng";
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi tải dữ liệu";
                System.Windows.MessageBox.Show($"Lỗi khi tải danh sách người dùng: {ex.Message}", 
                    "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void SearchUsers()
        {
            // Use ApplyFilters to handle both search and role filter
            ApplyFilters();
        }

        private void RefreshUsers()
        {
            SearchText = ""; // Clear search
            SelectedRoleFilter = null; // Clear role filter
            LoadUsers(); // Load lại từ database
        }

        private void LoadAvailableRoles()
        {
            try
            {
                // Load unique roles from users
                var roles = _allUsers
                    .Where(u => u.Role != null)
                    .Select(u => u.Role)
                    .Distinct()
                    .OrderBy(r => r.RoleName)
                    .ToList();

                // Add "Tất cả" option
                var allRoles = new List<Role> { new Role { RoleId = 0, RoleName = "Tất cả vai trò" } };
                allRoles.AddRange(roles);

                AvailableRoles = new ObservableCollection<Role>(allRoles);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi khi tải danh sách vai trò: {ex.Message}", 
                    "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                StatusMessage = "Đang lọc...";
                
                var filteredUsers = _allUsers.AsEnumerable();

                // Apply role filter
                if (SelectedRoleFilter != null && SelectedRoleFilter.RoleId != 0)
                {
                    filteredUsers = filteredUsers.Where(u => u.Role?.RoleId == SelectedRoleFilter.RoleId);
                }

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    var searchTerm = SearchText.Trim().ToLower();
                    filteredUsers = filteredUsers.Where(u => 
                        // 1. Search theo UserId
                        u.UserId.ToString().Contains(searchTerm) ||
                        // 2. Search theo Username
                        (u.Username?.ToLower().Contains(searchTerm) == true) ||
                        // 3. Search theo FullName
                        (u.FullName?.ToLower().Contains(searchTerm) == true) ||
                        // 4. Search theo Email  
                        (u.Email?.ToLower().Contains(searchTerm) == true) ||
                        // 5. Search theo PhoneNumber
                        (u.PhoneNumber?.ToLower().Contains(searchTerm) == true) ||
                        // 6. Search theo RoleName
                        (u.Role?.RoleName?.ToLower().Contains(searchTerm) == true) ||
                        // 7. Search theo CreatedDate
                        u.CreatedDate.ToString("dd/MM/yyyy").Contains(searchTerm) ||
                        u.CreatedDate.ToString("dd-MM-yyyy").Contains(searchTerm) ||
                        u.CreatedDate.ToString("yyyy-MM-dd").Contains(searchTerm) ||
                        u.CreatedDate.ToString("yyyy/MM/dd").Contains(searchTerm)
                    );
                }

                Users = new ObservableCollection<User>(filteredUsers.ToList());
                StatusMessage = "Sẵn sàng";
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi lọc dữ liệu";
                System.Windows.MessageBox.Show($"Lỗi khi lọc dữ liệu: {ex.Message}", 
                    "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ClearFilters()
        {
            SearchText = "";
            SelectedRoleFilter = null;
            ApplyFilters();
        }

        private void AddUser()
        {
            try
            {
                StatusMessage = "Đang mở form thêm user...";
                
                // Tạo ViewModel cho dialog thêm user
                var dialogViewModel = new UserDialogViewModel(_userService);
                var dialog = new View.Admin.UserDialog(dialogViewModel);
                
                // Hiển thị dialog modal
                bool? result = dialog.ShowDialog();
                
                if (result == true)
                {
                    // User được thêm thành công, refresh danh sách
                    StatusMessage = "Thêm user thành công";
                    LoadUsers();
                }
                else
                {
                    StatusMessage = "Sẵn sàng";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi thêm user";
                System.Windows.MessageBox.Show($"Lỗi khi thêm user: {ex.Message}", 
                    "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void EditUser()
        {
            try
            {
                if (SelectedUser == null)
                {
                    System.Windows.MessageBox.Show("Vui lòng chọn user cần sửa!", 
                        "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }

                StatusMessage = "Đang mở form sửa user...";
                
                // Tạo ViewModel cho dialog sửa user
                var dialogViewModel = new UserDialogViewModel(_userService, SelectedUser);
                var dialog = new View.Admin.UserDialog(dialogViewModel);
                
                // Hiển thị dialog modal
                bool? result = dialog.ShowDialog();
                
                if (result == true)
                {
                    // User được sửa thành công, refresh danh sách
                    StatusMessage = "Sửa user thành công";
                    LoadUsers();
                }
                else
                {
                    StatusMessage = "Sẵn sàng";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi sửa user";
                System.Windows.MessageBox.Show($"Lỗi khi sửa user: {ex.Message}", 
                    "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void DeleteUser()
        {
            try
            {
                if (SelectedUser == null)
                {
                    System.Windows.MessageBox.Show("Vui lòng chọn user cần xóa!", 
                        "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }

                // Lưu thông tin user trước khi xóa
                var userToDelete = SelectedUser;

                // Xác nhận xóa
                var result = System.Windows.MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa user '{userToDelete.Username}'?\n\n" +
                    $"Tên: {userToDelete.FullName}\n" +
                    $"Email: {userToDelete.Email}",
                    "Xác nhận xóa", 
                    System.Windows.MessageBoxButton.YesNo, 
                    System.Windows.MessageBoxImage.Question);

                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    StatusMessage = "Đang xóa user...";
                    
                    // Thực hiện xóa
                    bool success = _userService.Delete(userToDelete.UserId);
                    
                    if (success)
                    {
                        // Cập nhật UI
                        _allUsers.Remove(userToDelete);
                        Users.Remove(userToDelete);
                        SelectedUser = null;
                        
                        StatusMessage = "Xóa thành công";
                        
                        // Hiển thị MessageBox thành công
                        System.Windows.MessageBox.Show($"Xóa user '{userToDelete.Username}' thành công!", 
                            "Thành công", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    }
                    else
                    {
                        StatusMessage = "Lỗi xóa user";
                        
                        // Hiển thị MessageBox lỗi
                        System.Windows.MessageBox.Show("Không thể xóa user. Vui lòng kiểm tra lại và thử lại!", 
                            "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }
                }
                
                StatusMessage = "Sẵn sàng";
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi xóa user";
                
                // Hiển thị MessageBox exception
                System.Windows.MessageBox.Show($"Lỗi khi xóa user: {ex.Message}", 
                    "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
