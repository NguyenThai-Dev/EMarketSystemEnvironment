import 'package:flutter/foundation.dart';
import '../data/models/dashboard_model.dart';
import '../data/repositories/data_repository.dart';

class DashboardProvider with ChangeNotifier {
  final DataRepository _repository = DataRepository();

  DashboardOverview _overview = DashboardOverview();
  bool _isLoading = false;
  String? _errorMessage;

  DashboardOverview get overview => _overview;
  bool get isLoading => _isLoading;
  String? get errorMessage => _errorMessage;

  Future<void> loadOverview({
    int? branchId,
    DateTime? fromDate,
    DateTime? toDate,
    String groupBy = 'day',
  }) async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    try {
      final actualBranchId = (branchId != null && branchId > 0) ? branchId : null;
      _overview = await _repository.getDashboardOverview(
        branchId: actualBranchId,
        fromDate: fromDate,
        toDate: toDate,
        groupBy: groupBy,
      );
    } catch (e) {
      _errorMessage = e.toString().replaceFirst('Exception: ', '');
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }
}
