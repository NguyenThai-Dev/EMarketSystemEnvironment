package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class ProductImage {
    @SerializedName(value = "imageId", alternate = {"ImageId", "id", "Id"})
    private int imageId;

    @SerializedName(value = "productId", alternate = {"ProductId"})
    private int productId;

    @SerializedName(value = "imageUrl", alternate = {"ImageUrl", "image", "Image", "url", "Url"})
    private String imageUrl;

    @SerializedName(value = "isMain", alternate = {"IsMain"})
    private boolean isMain;

    public ProductImage() {}

    public ProductImage(int productId, String imageUrl) {
        this.productId = productId;
        this.imageUrl = imageUrl;
    }

    public int getImageId() { return imageId; }
    public void setImageId(int imageId) { this.imageId = imageId; }

    public int getProductId() { return productId; }
    public void setProductId(int productId) { this.productId = productId; }

    public String getImageUrl() { return imageUrl; }
    public void setImageUrl(String imageUrl) { this.imageUrl = imageUrl; }

    public boolean isMain() { return isMain; }
    public void setMain(boolean main) { isMain = main; }

    public String getFullImageUrl() {
        return com.example.do_an_di_dong.data.api.ApiClient.getFullImageUrl(imageUrl);
    }
}
