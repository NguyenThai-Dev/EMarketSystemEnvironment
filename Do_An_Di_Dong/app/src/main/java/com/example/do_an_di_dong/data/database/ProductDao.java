package com.example.do_an_di_dong.data.database;

import androidx.room.Dao;
import androidx.room.Insert;
import androidx.room.OnConflictStrategy;
import androidx.room.Query;

import java.util.List;

@Dao
public interface ProductDao {
    @Query("SELECT * FROM products")
    List<ProductEntity> getAllProducts();

    @Query("SELECT * FROM products WHERE sku = :sku LIMIT 1")
    ProductEntity getProductBySku(String sku);

    @Query("SELECT * FROM products WHERE name LIKE '%' || :keyword || '%' OR sku LIKE '%' || :keyword || '%'")
    List<ProductEntity> searchProductsOffline(String keyword);

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    void insertProducts(List<ProductEntity> products);

    @Query("DELETE FROM products")
    void deleteAllProducts();
}
