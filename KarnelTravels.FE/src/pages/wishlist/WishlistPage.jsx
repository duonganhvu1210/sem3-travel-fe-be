import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Heart, MapPin, Star, Trash2, Loader2, Hotel, Mountain, Palmtree, Waves, Landmark, Car } from 'lucide-react';
import api from '@/services/api';
import favoriteService from '@/services/favoriteService';

const WishlistPage = () => {
  const [favorites, setFavorites] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    loadFavorites();
  }, []);

  const loadFavorites = async () => {
    try {
      setLoading(true);
      const response = await favoriteService.getFavorites();
      
      if (response.success) {
        setFavorites(response.data || []);
      } else {
        setError('Không thể tải danh sách yêu thích');
      }
    } catch (err) {
      console.error('Error loading favorites:', err);
      setError('Có lỗi xảy ra khi tải dữ liệu');
    } finally {
      setLoading(false);
    }
  };

  const handleRemoveFavorite = async (favoriteId) => {
    if (!window.confirm('Bạn có chắc muốn xóa khỏi danh sách yêu thích?')) return;

    try {
      const response = await favoriteService.removeFavorite(favoriteId);
      if (response.success) {
        setFavorites(prev => prev.filter(f => f.favoriteId !== favoriteId));
      }
    } catch (err) {
      alert('Không thể xóa khỏi danh sách yêu thích');
    }
  };

  const getItemIcon = (itemType) => {
    switch (itemType) {
      case 'Hotel': return <Hotel className="w-5 h-5" />;
      case 'TouristSpot': return <Mountain className="w-5 h-5" />;
      case 'Restaurant': return <Landmark className="w-5 h-5" />;
      case 'Resort': return <Palmtree className="w-5 h-5" />;
      case 'Tour': return <Waves className="w-5 h-5" />;
      case 'Transport': return <Car className="w-5 h-5" />;
      default: return <Heart className="w-5 h-5" />;
    }
  };

  const getItemLink = (item) => {
    switch (item.itemType) {
      case 'Hotel': return `/info/hotels/${item.itemId}`;
      case 'TouristSpot': return `/info/tourist-spots/${item.itemId}`;
      case 'Restaurant': return `/info/restaurants/${item.itemId}`;
      case 'Resort': return `/info/resorts/${item.itemId}`;
      case 'Tour': return `/info/tours/${item.itemId}`;
      case 'Transport': return `/info/transports/${item.itemId}`;
      default: return '#';
    }
  };

  const formatPrice = (price) => {
    if (!price) return 'Liên hệ';
    return `${price.toLocaleString('vi-VN')}₫`;
  };

  const getItemTypeLabel = (itemType) => {
    switch (itemType) {
      case 'Hotel': return 'Khách sạn';
      case 'TouristSpot': return 'Địa điểm';
      case 'Restaurant': return 'Nhà hàng';
      case 'Resort': return 'Resort';
      case 'Tour': return 'Tour';
      case 'Transport': return 'Phương tiện';
      default: return itemType;
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <Loader2 className="w-8 h-8 animate-spin text-primary" />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="container mx-auto px-4">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-800 mb-2">Danh sách yêu thích</h1>
          <p className="text-gray-600">Quản lý các địa điểm bạn đã lưu</p>
        </div>

        {/* Error */}
        {error && (
          <div className="bg-red-50 text-red-600 p-4 rounded-lg mb-4">
            {error}
          </div>
        )}

        {/* Empty State */}
        {!loading && favorites.length === 0 && (
          <div className="text-center py-16 bg-white rounded-xl shadow-sm">
            <Heart className="w-16 h-16 mx-auto text-gray-300 mb-4" />
            <h3 className="text-xl font-semibold text-gray-600 mb-2">Chưa có yêu thích</h3>
            <p className="text-gray-500 mb-6">Hãy lưu các địa điểm bạn quan tâm!</p>
            <Link
              to="/info/tourist-spots"
              className="inline-block px-6 py-3 bg-primary text-white rounded-lg hover:bg-teal-700"
            >
              Khám phá ngay
            </Link>
          </div>
        )}

        {/* Favorites List */}
        {favorites.length > 0 && (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {favorites.map((item) => (
              <div key={item.favoriteId} className="bg-white rounded-xl overflow-hidden shadow-md hover:shadow-xl transition-all">
                {/* Image */}
                <div className="relative h-48 overflow-hidden">
                  {item.itemImage ? (
                    <img
                      src={item.itemImage}
                      alt={item.itemName}
                      className="w-full h-full object-cover"
                    />
                  ) : (
                    <div className="w-full h-full bg-gray-200 flex items-center justify-center">
                      {getItemIcon(item.itemType)}
                    </div>
                  )}
                  
                  {/* Remove Button */}
                  <button
                    onClick={() => handleRemoveFavorite(item.favoriteId)}
                    className="absolute top-3 right-3 w-9 h-9 bg-white/90 rounded-full flex items-center justify-center text-red-500 hover:bg-red-50 transition-colors"
                  >
                    <Trash2 className="w-5 h-5" />
                  </button>

                  {/* Type Badge */}
                  <div className="absolute top-3 left-3 px-3 py-1 bg-white/90 rounded-full text-sm font-medium text-gray-700 flex items-center gap-1">
                    {getItemIcon(item.itemType)}
                    {getItemTypeLabel(item.itemType)}
                  </div>
                </div>

                {/* Content */}
                <div className="p-4">
                  <h3 className="font-semibold text-lg text-gray-800 mb-2 line-clamp-1">
                    {item.itemName || 'Đang tải...'}
                  </h3>

                  <div className="flex items-center justify-between mb-3">
                    <div className="flex items-center gap-1 text-sm text-gray-500">
                      <MapPin className="w-4 h-4" />
                      <span>{getItemTypeLabel(item.itemType)}</span>
                    </div>
                    {item.itemPrice && (
                      <span className="font-bold text-primary">
                        {formatPrice(item.itemPrice)}
                      </span>
                    )}
                  </div>

                  <Link
                    to={getItemLink(item)}
                    className="block w-full py-2 bg-gray-100 text-gray-700 text-center rounded-lg font-medium hover:bg-gray-200 transition-colors"
                  >
                    Xem chi tiết
                  </Link>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default WishlistPage;
