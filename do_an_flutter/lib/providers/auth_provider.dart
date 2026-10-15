import 'package:flutter/foundation.dart';
import '../core/network/token_storage.dart';
import '../data/models/dashboard_model.dart';
import '../data/models/user_model.dart';
import '../data/repositories/data_repository.dart';

class AuthProvider with ChangeNotifier {
  final DataRepository _repository = DataRepository();

  UserModel? _currentUser;
  bool _isLoading = false;
  String? _errorMessage;
  List<BranchModel> _branches = [];
  BranchModel? _selectedBranch;

  UserModel? get currentUser => _currentUser;
  bool get isLoading => _isLoading;
  String? get errorMessage => _errorMessage;
  bool get isAuthenticated => _currentUser != null;
  List<BranchModel> get branches => _branches;
  BranchModel? get selectedBranch => _selectedBranch;
  int? get effectiveBranchId => (_selectedBranch != null && _selectedBranch!.branchId > 0) ? _selectedBranch!.branchId : _currentUser?.branchId;

  /// Check if user has saved token/session
  Future<void> checkAuthStatus() async {
    _isLoading = true;
    notifyListeners();

    final token = await TokenStorage.getToken();
    if (token != null && token.isNotEmpty) {
      _currentUser = await TokenStorage.getUser();
      if (_currentUser == null) {
        _currentUser = await _repository.getCurrentUser();
        if (_currentUser != null) {
          await TokenStorage.saveUser(_currentUser!);
        }
      }
      if (_currentUser != null) {
        await loadBranches();
      }
    }

    _isLoading = false;
    notifyListeners();
  }

  /// Login with username/password
  Future<bool> login(String username, String password) async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    try {
      final res = await _repository.login(username, password);
      if (res.success) {
        _currentUser = res.user ?? await _repository.getCurrentUser();
        await loadBranches();
        _isLoading = false;
        notifyListeners();
        return true;
      } else {
        _errorMessage = res.message ?? 'Đăng nhập không thành công.';
        _isLoading = false;
        notifyListeners();
        return false;
      }
    } catch (e) {
      _errorMessage = e.toString().replaceFirst('Exception: ', '');
      _isLoading = false;
      notifyListeners();
      return false;
    }
  }

  /// Login with Google Email via EMarket API
  Future<bool> loginWithGoogle({required String email, String? fullName}) async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    try {
      final res = await _repository.loginWithGoogle(email: email, fullName: fullName);
      if (res.success) {
        _currentUser = res.user ?? await _repository.getCurrentUser();
        await loadBranches();
        _isLoading = false;
        notifyListeners();
        return true;
      } else {
        _errorMessage = res.message ?? 'Đăng nhập Google không thành công.';
        _isLoading = false;
        notifyListeners();
        return false;
      }
    } catch (e) {
      _errorMessage = e.toString().replaceFirst('Exception: ', '');
      _isLoading = false;
      notifyListeners();
      return false;
    }
  }

  static final BranchModel allBranchesOption = BranchModel(
    branchId: 0,
    branchName: 'Toàn hệ thống (Tất cả chi nhánh)',
    address: 'Áp dụng trên toàn bộ chuỗi siêu thị EMarket',
  );

  bool get canSwitchBranch => _currentUser?.isAdmin == true;

  /// Load Branches
  Future<void> loadBranches() async {
    final rawBranches = await _repository.getBranches();
    final savedBranchId = await TokenStorage.getBranchId();

    if (_currentUser?.isAdmin == true) {
      _branches = [allBranchesOption, ...rawBranches];
      if (savedBranchId != null && savedBranchId > 0) {
        _selectedBranch = _branches.firstWhere(
          (b) => b.branchId == savedBranchId,
          orElse: () => allBranchesOption,
        );
      } else {
        _selectedBranch = allBranchesOption;
      }
    } else {
      _branches = rawBranches;
      final assignedId = _currentUser?.branchId ?? savedBranchId;
      if (assignedId != null && assignedId > 0 && _branches.isNotEmpty) {
        _selectedBranch = _branches.firstWhere(
          (b) => b.branchId == assignedId,
          orElse: () => _branches.first,
        );
      } else if (_branches.isNotEmpty) {
        _selectedBranch = _branches.first;
      }
    }
    notifyListeners();
  }

  void selectBranch(BranchModel branch) {
    if (!canSwitchBranch && branch.branchId != (_currentUser?.branchId ?? 0)) {
      return; // Nhân viên không được phép đổi chi nhánh khác quyền phân bổ
    }
    _selectedBranch = branch;
    TokenStorage.saveBranchId(branch.branchId);
    notifyListeners();
  }

  /// Sign out
  Future<void> logout() async {
    _isLoading = true;
    notifyListeners();

    await _repository.signOut();
    _currentUser = null;
    _selectedBranch = null;
    _errorMessage = null;

    _isLoading = false;
    notifyListeners();
  }
}
