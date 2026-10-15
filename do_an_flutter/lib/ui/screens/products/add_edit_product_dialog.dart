import 'dart:io';
import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import '../../../core/constants/app_colors.dart';
import '../../../core/utils/image_helper.dart';
import '../../../data/models/product_model.dart';
import '../../../data/repositories/data_repository.dart';

class AddEditProductDialog extends StatefulWidget {
  final ProductModel? product;

  const AddEditProductDialog({super.key, this.product});

  static Future<bool?> show(BuildContext context, {ProductModel? product}) {
    return showDialog<bool>(
      context: context,
      barrierDismissible: false,
      builder: (context) => AddEditProductDialog(product: product),
    );
  }

  @override
  State<AddEditProductDialog> createState() => _AddEditProductDialogState();
}

class _AddEditProductDialogState extends State<AddEditProductDialog> {
  final _formKey = GlobalKey<FormState>();
  final _repository = DataRepository();

  late TextEditingController _nameController;
  late TextEditingController _skuController;
  late TextEditingController _priceController;
  late TextEditingController _costPriceController;
  late TextEditingController _unitController;
  late TextEditingController _minStockController;
  late TextEditingController _maxStockController;
  late TextEditingController _descriptionController;

  String? _imageUrl;
  XFile? _selectedLocalImage;
  bool _isUploadingImage = false;

  int? _selectedCategoryId;
  int? _selectedSupplierId;
  List<ProductCategoryModel> _categories = [];
  List<SupplierModel> _suppliers = [];

  bool _isLoading = false;
  bool _isLoadingData = true;
  String? _errorMessage;

  @override
  void initState() {
    super.initState();
    final p = widget.product;
    _nameController = TextEditingController(text: p?.name ?? '');
    _skuController = TextEditingController(text: p?.sku ?? '');
    _priceController = TextEditingController(text: p != null ? p.price.toStringAsFixed(0) : '');
    _costPriceController = TextEditingController(text: p != null ? p.costPrice.toStringAsFixed(0) : '');
    _unitController = TextEditingController(text: p?.unit ?? 'Món');
    _minStockController = TextEditingController(text: '5');
    _maxStockController = TextEditingController(text: '500');
    _descriptionController = TextEditingController(text: p?.description ?? '');
    _imageUrl = p?.imageUrl;
    _selectedCategoryId = p?.categoryId;

    _fetchMetadata();
  }

  @override
  void dispose() {
    _nameController.dispose();
    _skuController.dispose();
    _priceController.dispose();
    _costPriceController.dispose();
    _unitController.dispose();
    _minStockController.dispose();
    _maxStockController.dispose();
    _descriptionController.dispose();
    super.dispose();
  }

  Future<void> _fetchMetadata() async {
    try {
      final cats = await _repository.getCategories();
      final sups = await _repository.getSuppliers();
      if (!mounted) return;
      setState(() {
        _categories = cats;
        _suppliers = sups;
        _isLoadingData = false;
        if (_selectedCategoryId == null && _categories.isNotEmpty) {
          _selectedCategoryId = _categories.first.categoryId;
        }
      });
    } catch (_) {
      if (!mounted) return;
      setState(() => _isLoadingData = false);
    }
  }

  Future<void> _pickAndUploadImage() async {
    try {
      final picker = ImagePicker();
      final XFile? file = await picker.pickImage(
        source: ImageSource.gallery,
      );
      if (file == null) return;

      // 1. Kiểm tra dung lượng file (tối đa 20MB)
      final fileSize = await file.length();
      if (fileSize > 20 * 1024 * 1024) {
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              backgroundColor: AppColors.error,
              content: Text(
                'Ảnh quá lớn (${(fileSize / (1024 * 1024)).toStringAsFixed(1)} MB). Dung lượng tối đa là 20MB. Vui lòng chọn ảnh nhẹ hơn!',
              ),
              duration: const Duration(seconds: 4),
            ),
          );
        }
        return;
      }

      setState(() {
        _selectedLocalImage = file;
      });

      // Nếu đang sửa sản phẩm (đã có productId > 0), upload ngay lên server và cập nhật ảnh cho sản phẩm đó
      if (widget.product != null && widget.product!.productId > 0) {
        setState(() => _isUploadingImage = true);
        final uploadedUrl = await _repository.uploadProductImage(
          filePath: file.path,
          fileName: file.name,
          productId: widget.product!.productId,
        );
        if (mounted) {
          setState(() {
            if (uploadedUrl != null && uploadedUrl.isNotEmpty) {
              _imageUrl = uploadedUrl;
            }
            _isUploadingImage = false;
          });
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(
              backgroundColor: AppColors.success,
              content: Text('Cập nhật ảnh sản phẩm thành công!'),
              duration: Duration(seconds: 2),
            ),
          );
        }
      } else {
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(
              backgroundColor: AppColors.success,
              content: Text('Đã chọn ảnh. Ảnh sẽ được lưu cùng sản phẩm!'),
              duration: Duration(seconds: 2),
            ),
          );
        }
      }
    } catch (e) {
      if (mounted) {
        setState(() => _isUploadingImage = false);
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            backgroundColor: AppColors.error,
            content: Text('Lỗi tải ảnh: $e'),
          ),
        );
      }
    }
  }

  Future<void> _handleSubmit() async {
    if (!_formKey.currentState!.validate()) return;

    setState(() {
      _isLoading = true;
      _errorMessage = null;
    });

    final payload = {
      if (widget.product != null) 'ProductId': widget.product!.productId,
      'Name': _nameController.text.trim(),
      'Barcode': _skuController.text.trim(),
      // Chỉ gửi Image khi sửa sản phẩm và đã có URL ảnh hợp lệ
      if (widget.product != null && _imageUrl != null && _imageUrl!.isNotEmpty)
        'Image': _imageUrl,
      'Price': double.tryParse(_priceController.text.trim()) ?? 0.0,
      'CostPrice': double.tryParse(_costPriceController.text.trim()) ?? 0.0,
      'Unit': _unitController.text.trim(),
      'CategoryId': (_selectedCategoryId != null && _selectedCategoryId! > 0) ? _selectedCategoryId : null,
      'SupplierId': (_selectedSupplierId != null && _selectedSupplierId! > 0) ? _selectedSupplierId : null,
      'MinStock': int.tryParse(_minStockController.text.trim()) ?? 5,
      'MaxStock': int.tryParse(_maxStockController.text.trim()) ?? 500,
      'Description': _descriptionController.text.trim(),
    };

    try {
      if (widget.product != null) {
        // Cập nhật sản phẩm
        final success = await _repository.updateProduct(payload);
        if (!success) {
          setState(() => _errorMessage = 'Cập nhật không thành công.');
          return;
        }

        // Tải ảnh mới nếu người dùng chọn ảnh nhưng chưa được upload
        if (_selectedLocalImage != null && (_imageUrl == null || _imageUrl!.isEmpty)) {
          try {
            await _repository.uploadProductImage(
              filePath: _selectedLocalImage!.path,
              fileName: _selectedLocalImage!.name,
              productId: widget.product!.productId,
            );
          } catch (_) {}
        }
      } else {
        // Tạo mới sản phẩm
        final newProductId = await _repository.createProduct(payload);
        if (newProductId == null || newProductId <= 0) {
          setState(() => _errorMessage = 'Thao tác tạo sản phẩm không thành công.');
          return;
        }

        // Tải ảnh lên cho sản phẩm vừa tạo
        if (_selectedLocalImage != null) {
          try {
            await _repository.uploadProductImage(
              filePath: _selectedLocalImage!.path,
              fileName: _selectedLocalImage!.name,
              productId: newProductId,
            );
          } catch (uploadErr) {
            debugPrint('[Upload Product Image Error] $uploadErr');
          }
        }
      }

      if (!mounted) return;
      setState(() => _isLoading = false);
      Navigator.pop(context, true);
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _isLoading = false;
        _errorMessage = e.toString().replaceFirst('Exception: ', '');
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    final isEditing = widget.product != null;

    return Dialog(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
      insetPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 24),
      child: Container(
        constraints: const BoxConstraints(maxWidth: 480),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(20),
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            // Header
            Padding(
              padding: const EdgeInsets.fromLTRB(20, 16, 16, 12),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    isEditing ? 'Chỉnh sửa sản phẩm' : 'Thêm sản phẩm mới',
                    style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                  ),
                  IconButton(
                    icon: const Icon(Icons.close, size: 20),
                    padding: EdgeInsets.zero,
                    constraints: const BoxConstraints(),
                    onPressed: () => Navigator.pop(context),
                  ),
                ],
              ),
            ),
            const Divider(height: 1, color: AppColors.border),

            // Form Content
            Flexible(
              child: SingleChildScrollView(
                padding: const EdgeInsets.all(20),
                child: Form(
                  key: _formKey,
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      if (_errorMessage != null)
                        Container(
                          margin: const EdgeInsets.only(bottom: 16),
                          padding: const EdgeInsets.all(12),
                          decoration: BoxDecoration(
                            color: AppColors.error.withValues(alpha: 0.08),
                            borderRadius: BorderRadius.circular(8),
                            border: Border.all(color: AppColors.error.withValues(alpha: 0.3)),
                          ),
                          child: Text(_errorMessage!, style: const TextStyle(color: AppColors.error, fontSize: 12)),
                        ),

                      // Product Image Uploader Card
                      Container(
                        padding: const EdgeInsets.all(12),
                        decoration: BoxDecoration(
                          color: AppColors.surfaceVariant.withValues(alpha: 0.35),
                          borderRadius: BorderRadius.circular(14),
                          border: Border.all(color: AppColors.border),
                        ),
                        child: Row(
                          children: [
                            Stack(
                              alignment: Alignment.center,
                              children: [
                                ClipRRect(
                                  borderRadius: BorderRadius.circular(10),
                                  child: Container(
                                    width: 60,
                                    height: 60,
                                    color: AppColors.surfaceVariant,
                                    child: (_selectedLocalImage != null && !kIsWeb)
                                        ? Image.file(
                                            File(_selectedLocalImage!.path),
                                            fit: BoxFit.cover,
                                          )
                                        : (_imageUrl != null &&
                                                _imageUrl!.trim().isNotEmpty &&
                                                ImageHelper.getFullImageUrl(_imageUrl!) != null)
                                            ? CachedNetworkImage(
                                                imageUrl: ImageHelper.getFullImageUrl(_imageUrl!)!,
                                                fit: BoxFit.cover,
                                                errorWidget: (_, __, ___) => const Icon(Icons.broken_image, color: AppColors.textMuted),
                                              )
                                            : const Icon(Icons.image_outlined, color: AppColors.textMuted, size: 30),
                                  ),
                                ),
                                if (_isUploadingImage)
                                  Container(
                                    width: 60,
                                    height: 60,
                                    decoration: BoxDecoration(
                                      color: Colors.black45,
                                      borderRadius: BorderRadius.circular(10),
                                    ),
                                    child: const Center(
                                      child: SizedBox(
                                        width: 22,
                                        height: 22,
                                        child: CircularProgressIndicator(strokeWidth: 2.5, color: Colors.white),
                                      ),
                                    ),
                                  ),
                              ],
                            ),
                            const SizedBox(width: 14),
                            Expanded(
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  const Text(
                                    'Hình ảnh sản phẩm',
                                    style: TextStyle(fontSize: 13, fontWeight: FontWeight.w600, color: AppColors.textPrimary),
                                  ),
                                  const SizedBox(height: 3),
                                  Text(
                                    (_imageUrl != null && _imageUrl!.isNotEmpty)
                                        ? 'Đã tải lên ảnh'
                                        : 'Chưa có ảnh. Hỗ trợ PNG, JPG',
                                    style: TextStyle(
                                      fontSize: 11,
                                      color: (_imageUrl != null && _imageUrl!.isNotEmpty)
                                          ? AppColors.success
                                          : AppColors.textSecondary,
                                    ),
                                  ),
                                  const SizedBox(height: 8),
                                  Row(
                                    children: [
                                      OutlinedButton.icon(
                                        onPressed: _isUploadingImage ? null : _pickAndUploadImage,
                                        style: OutlinedButton.styleFrom(
                                          padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
                                          visualDensity: VisualDensity.compact,
                                          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
                                        ),
                                        icon: const Icon(Icons.cloud_upload_outlined, size: 16),
                                        label: const Text('Tải ảnh', style: TextStyle(fontSize: 12)),
                                      ),
                                      if (_imageUrl != null && _imageUrl!.isNotEmpty) ...[
                                        const SizedBox(width: 8),
                                        TextButton.icon(
                                          onPressed: _isUploadingImage ? null : () => setState(() => _imageUrl = null),
                                          style: TextButton.styleFrom(
                                            foregroundColor: AppColors.error,
                                            padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 6),
                                            visualDensity: VisualDensity.compact,
                                          ),
                                          icon: const Icon(Icons.delete_outline, size: 16),
                                          label: const Text('Gỡ', style: TextStyle(fontSize: 12)),
                                        ),
                                      ],
                                    ],
                                  ),
                                ],
                              ),
                            ),
                          ],
                        ),
                      ),
                      const SizedBox(height: 12),

                      // Product Name
                      TextFormField(
                        controller: _nameController,
                        decoration: InputDecoration(
                          labelText: 'Tên sản phẩm *',
                          hintText: 'Nhập tên hàng hóa...',
                          border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                          contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
                        ),
                        validator: (v) => (v == null || v.trim().isEmpty) ? 'Vui lòng nhập tên sản phẩm' : null,
                      ),
                      const SizedBox(height: 12),

                      // Barcode / SKU
                      TextFormField(
                        controller: _skuController,
                        decoration: InputDecoration(
                          labelText: 'Mã vạch / SKU *',
                          hintText: 'Nhập hoặc quét mã vạch...',
                          border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                          contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
                        ),
                        validator: (v) => (v == null || v.trim().isEmpty) ? 'Vui lòng nhập mã SKU' : null,
                      ),
                      const SizedBox(height: 12),

                      // Category Dropdown
                      if (!_isLoadingData && _categories.isNotEmpty)
                        DropdownButtonFormField<int>(
                          initialValue: _selectedCategoryId,
                          decoration: InputDecoration(
                            labelText: 'Danh mục sản phẩm',
                            border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                            contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
                          ),
                          items: _categories.map((c) {
                            return DropdownMenuItem<int>(
                              value: c.categoryId,
                              child: Text(c.categoryName, style: const TextStyle(fontSize: 13)),
                            );
                          }).toList(),
                          onChanged: (val) => setState(() => _selectedCategoryId = val),
                        ),
                      const SizedBox(height: 12),

                      // Supplier Dropdown
                      if (!_isLoadingData && _suppliers.isNotEmpty)
                        DropdownButtonFormField<int>(
                          initialValue: _selectedSupplierId,
                          decoration: InputDecoration(
                            labelText: 'Nhà cung cấp',
                            border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                            contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
                          ),
                          items: _suppliers.map((s) {
                            return DropdownMenuItem<int>(
                              value: s.supplierId,
                              child: Text(s.name, style: const TextStyle(fontSize: 13)),
                            );
                          }).toList(),
                          onChanged: (val) => setState(() => _selectedSupplierId = val),
                        ),
                      const SizedBox(height: 12),

                      // Price & CostPrice Row
                      Row(
                        children: [
                          Expanded(
                            child: TextFormField(
                              controller: _priceController,
                              keyboardType: TextInputType.number,
                              decoration: InputDecoration(
                                labelText: 'Giá bán (VND) *',
                                border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                                contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
                              ),
                              validator: (v) => (v == null || v.trim().isEmpty) ? 'Nhập giá bán' : null,
                            ),
                          ),
                          const SizedBox(width: 12),
                          Expanded(
                            child: TextFormField(
                              controller: _unitController,
                              decoration: InputDecoration(
                                labelText: 'Đơn vị tính',
                                hintText: 'Chai, Lon, Gói...',
                                border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                                contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
                              ),
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(height: 12),

                      // Min & Max Stock Row
                      Row(
                        children: [
                          Expanded(
                            child: TextFormField(
                              controller: _minStockController,
                              keyboardType: TextInputType.number,
                              decoration: InputDecoration(
                                labelText: 'Tồn tối thiểu',
                                border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                                contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
                              ),
                            ),
                          ),
                          const SizedBox(width: 12),
                          Expanded(
                            child: TextFormField(
                              controller: _maxStockController,
                              keyboardType: TextInputType.number,
                              decoration: InputDecoration(
                                labelText: 'Tồn tối đa',
                                border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                                contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
                              ),
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(height: 12),

                      // Description
                      TextFormField(
                        controller: _descriptionController,
                        maxLines: 2,
                        decoration: InputDecoration(
                          labelText: 'Mô tả chi tiết',
                          border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                          contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),

            // Actions
            Padding(
              padding: const EdgeInsets.all(16),
              child: SizedBox(
                width: double.infinity,
                height: 46,
                child: ElevatedButton(
                  style: ElevatedButton.styleFrom(
                    backgroundColor: AppColors.primary,
                    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
                  ),
                  onPressed: _isLoading ? null : _handleSubmit,
                  child: _isLoading
                      ? const SizedBox(width: 20, height: 20, child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white))
                      : Text(
                          isEditing ? 'Lưu thay đổi' : 'Thêm sản phẩm',
                          style: const TextStyle(fontWeight: FontWeight.bold, color: Colors.white),
                        ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
