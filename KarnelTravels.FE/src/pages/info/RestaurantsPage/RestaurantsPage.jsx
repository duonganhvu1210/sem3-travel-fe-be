import { useState, useEffect, useCallback } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { 
  Search, MapPin, Star, Filter, Loader2, Heart, Clock
} from 'lucide-react';
import api from '@/services/api';

// Restaurant Card Component
const RestaurantCard = ({ restaurant }) => {
  const navigate = useNavigate();

  return (
    <div 
      className="bg-white rounded-2xl shadow-md hover:shadow-xl transition-all duration-300 overflow-hidden group cursor-pointer"
      onClick={() => navigate(`/info/restaurants/${restaurant.restaurantId}`)}
    >
      <div className="relative h-56 overflow-hidden">
        <img
          src={restaurant.images?.[0] || 'https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=600'}
          alt={restaurant.name}
          className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
        />
        <div className="absolute top-3 right-3">
          <button 
            onClick={(e) => e.stopPropagation()}
            className="w-9 h-9 bg-white/90 rounded-full flex items-center justify-center text-gray-400 hover:text-red-500"
          >
            <Heart className="w-4 h-4" />
          </button>
        </div>
        {restaurant.cuisineType && (
          <div className="absolute bottom-3 left-3">
            <span className="px-3 py-1 bg-white/90 text-gray-700 text-xs font-medium rounded-full">
              {restaurant.cuisineType}
            </span>
          </div>
        )}
      </div>

      <div className="p-5">
        <div className="flex items-center gap-2 text-sm text-gray-500 mb-2">
          <MapPin className="w-4 h-4 text-primary" />
          {restaurant.city}
        </div>

        <h3 className="font-bold text-lg text-gray-800 mb-2 line-clamp-2 group-hover:text-primary">
          {restaurant.name}
        </h3>

        <div className="flex items-center gap-2 mb-3">
          <Star className="w-4 h-4 fill-yellow-400 text-yellow-400" />
          <span className="font-medium">{restaurant.rating?.toFixed(1) || '0.0'}</span>
          <span className="text-gray-400 text-sm">({restaurant.reviewCount || 0} đánh giá)</span>
        </div>

        <p className="text-gray-600 text-sm line-clamp-2">{restaurant.description}</p>
      </div>
    </div>
  );
};

// Main RestaurantsPage Component (List)
const RestaurantsPage = () => {
  const navigate = useNavigate();
  const [searchParams, setSearchParams] = useSearchParams();
  
  const [restaurants, setRestaurants] = useState([]);
  const [loading, setLoading] = useState(true);

  const [filters, setFilters] = useState({
    search: searchParams.get('search') || '',
    city: searchParams.get('city') || '',
    cuisineType: searchParams.get('cuisineType') || '',
    sortBy: searchParams.get('sortBy') || 'rating',
    pageIndex: Number(searchParams.get('pageIndex')) || 1,
  });

  const fetchData = useCallback(async () => {
    setLoading(true);
    try {
      const params = new URLSearchParams();
      if (filters.search) params.append('search', filters.search);
      if (filters.city) params.append('city', filters.city);
      if (filters.cuisineType) params.append('cuisineType', filters.cuisineType);
      params.append('sortBy', filters.sortBy);
      params.append('pageIndex', filters.pageIndex);
      params.append('pageSize', 12);

      const response = await api.get(`/restaurants?${params.toString()}`);
      if (response.data.success) {
        setRestaurants(response.data.data || []);
      }
    } catch (error) {
      console.error('Error fetching restaurants:', error);
    } finally {
      setLoading(false);
    }
  }, [filters]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  useEffect(() => {
    const params = new URLSearchParams();
    if (filters.search) params.set('search', filters.search);
    if (filters.city) params.set('city', filters.city);
    if (filters.cuisineType) params.set('cuisineType', filters.cuisineType);
    if (filters.sortBy !== 'rating') params.set('sortBy', filters.sortBy);
    if (filters.pageIndex > 1) params.set('pageIndex', filters.pageIndex.toString());
    setSearchParams(params, { replace: true });
  }, [filters, setSearchParams]);

  const handleFilterChange = (key, value) => {
    setFilters(prev => ({ ...prev, [key]: value, pageIndex: 1 }));
  };

  const clearFilters = () => {
    setFilters({ search: '', city: '', cuisineType: '', sortBy: 'rating', pageIndex: 1 });
  };

  const hasActiveFilters = filters.search || filters.city || filters.cuisineType;
  const cuisineTypes = ['Vietnamese', 'Seafood', 'BBQ', 'Chinese', 'Indian', 'Italian', 'Japanese', 'Korean'];

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="bg-gradient-to-r from-teal-600 to-cyan-700 text-white py-12">
        <div className="container mx-auto px-4">
          <h1 className="text-4xl font-bold mb-4">Nhà hàng</h1>
          <p className="text-lg text-white/80">Khám phá ẩm thực địa phương</p>
        </div>
      </div>

      <div className="container mx-auto px-4 -mt-6">
        <div className="bg-white rounded-2xl shadow-lg p-4 flex flex-wrap gap-4 items-center">
          <div className="flex-1 min-w-[200px] relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 w-5 h-5" />
            <input
              type="text"
              placeholder="Tìm kiếm nhà hàng..."
              value={filters.search}
              onChange={(e) => handleFilterChange('search', e.target.value)}
              className="w-full pl-10 pr-4 py-3 border border-gray-200 rounded-xl focus:outline-none focus:border-primary"
            />
          </div>

          <select
            value={filters.cuisineType}
            onChange={(e) => handleFilterChange('cuisineType', e.target.value)}
            className="px-4 py-3 border border-gray-200 rounded-xl focus:outline-none focus:border-primary"
          >
            <option value="">Tất cả ẩm thực</option>
            {cuisineTypes.map(type => (
              <option key={type} value={type}>{type}</option>
            ))}
          </select>

          <select
            value={filters.sortBy}
            onChange={(e) => handleFilterChange('sortBy', e.target.value)}
            className="px-4 py-3 border border-gray-200 rounded-xl focus:outline-none focus:border-primary"
          >
            <option value="rating">Đánh giá cao</option>
            <option value="name">Tên A → Z</option>
          </select>
        </div>
      </div>

      {hasActiveFilters && (
        <div className="container mx-auto px-4 mt-4">
          <button onClick={clearFilters} className="text-primary text-sm">Xóa bộ lọc</button>
        </div>
      )}

      <div className="container mx-auto px-4 py-8">
        {loading ? (
          <div className="flex justify-center py-20">
            <Loader2 className="w-10 h-10 text-primary animate-spin" />
          </div>
        ) : restaurants.length === 0 ? (
          <div className="text-center py-12">
            <Search className="w-16 h-16 text-gray-300 mx-auto mb-4" />
            <h3 className="text-xl text-gray-600">Không tìm thấy nhà hàng</h3>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {restaurants.map(r => (
              <RestaurantCard key={r.restaurantId} restaurant={r} />
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default RestaurantsPage;
