import { useState, useEffect, useCallback } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { 
  Search, MapPin, Star, Filter, X, Loader2, Heart,
  ChevronDown, ArrowLeftRight
} from 'lucide-react';
import api from '@/services/api';
import favoriteService from '@/services/favoriteService';

// Price Range Slider Component
const PriceRangeSlider = ({ min, max, onChange }) => {
  const [values, setValues] = useState([min, max]);

  useEffect(() => {
    setValues([min, max]);
  }, [min, max]);

  const handleChange = (e, index) => {
    const newValues = [...values];
    newValues[index] = Number(e.target.value);
    setValues(newValues);
    onChange(newValues);
  };

  return (
    <div className="space-y-3">
      <div className="flex justify-between text-sm text-gray-600">
        <span>{values[0].toLocaleString()}₫</span>
        <span>{values[1].toLocaleString()}₫</span>
      </div>
      <div className="relative h-2 bg-gray-200 rounded-full">
        <input
          type="range"
          min={min}
          max={max}
          value={values[0]}
          onChange={(e) => handleChange(e, 0)}
          className="absolute w-full h-2 appearance-none bg-transparent pointer-events-none [&::-webkit-slider-thumb]:pointer-events-auto [&::-webkit-slider-thumb]:w-4 [&::-webkit-slider-thumb]:h-4 [&::-webkit-slider-thumb]:rounded-full [&::-webkit-slider-thumb]:bg-primary [&::-webkit-slider-thumb]:appearance-none"
        />
        <input
          type="range"
          min={min}
          max={max}
          value={values[1]}
          onChange={(e) => handleChange(e, 1)}
          className="absolute w-full h-2 appearance-none bg-transparent pointer-events-none [&::-webkit-slider-thumb]:pointer-events-auto [&::-webkit-slider-thumb]:w-4 [&::-webkit-slider-thumb]:h-4 [&::-webkit-slider-thumb]:rounded-full [&::-webkit-slider-thumb]:bg-primary [&::-webkit-slider-thumb]:appearance-none"
        />
        <div 
          className="absolute h-2 bg-primary rounded-full"
          style={{
            left: `${((values[0] - min) / (max - min)) * 100}%`,
            right: `${100 - ((values[1] - min) / (max - min)) * 100}%`
          }}
        />
      </div>
    </div>
  );
};

// Hotel Card Component
const HotelCard = ({ hotel }) => {
  const navigate = useNavigate();
  const [isFavorite, setIsFavorite] = useState(false);
  const [favoriteLoading, setFavoriteLoading] = useState(false);

  const handleFavoriteClick = async (e) => {
    e.stopPropagation();
    setFavoriteLoading(true);
    try {
      if (isFavorite) {
        await favoriteService.removeByItem('Hotel', hotel.hotelId);
      } else {
        await favoriteService.addFavorite('Hotel', hotel.hotelId);
      }
      setIsFavorite(!isFavorite);
    } catch (err) {
      console.error('Error toggling favorite:', err);
    } finally {
      setFavoriteLoading(false);
    }
  };

  return (
    <div 
      className="bg-white rounded-2xl shadow-md hover:shadow-xl transition-all duration-300 overflow-hidden group cursor-pointer"
      onClick={() => navigate(`/info/hotels/${hotel.hotelId}`)}
    >
      <div className="relative h-56 overflow-hidden">
        <img
          src={hotel.images?.[0] || 'https://images.unsplash.com/photo-1566073771259-6a8506099945?w=600'}
          alt={hotel.name}
          className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
        />
        <div className="absolute top-3 right-3">
          <button 
            onClick={handleFavoriteClick}
            disabled={favoriteLoading}
            className={`w-9 h-9 bg-white/90 backdrop-blur-sm rounded-full flex items-center justify-center transition-colors ${
              isFavorite ? 'text-red-500' : 'text-gray-400 hover:text-red-500'
            }`}
          >
            <Heart className={`w-4 h-4 ${isFavorite ? 'fill-current' : ''}`} />
          </button>
        </div>
        <div className="absolute bottom-3 left-3">
          <div className="flex">
            {[...Array(hotel.starRating || 3)].map((_, i) => (
              <Star key={i} className="w-4 h-4 fill-yellow-400 text-yellow-400" />
            ))}
          </div>
        </div>
      </div>

      <div className="p-5">
        <div className="flex items-center gap-2 text-sm text-gray-500 mb-2">
          <MapPin className="w-4 h-4 text-primary" />
          {hotel.city}
        </div>

        <h3 className="font-bold text-lg text-gray-800 mb-2 line-clamp-2 group-hover:text-primary transition-colors">
          {hotel.name}
        </h3>

        <div className="flex items-center gap-2 mb-3">
          <Star className="w-4 h-4 fill-yellow-400 text-yellow-400" />
          <span className="font-medium">{hotel.rating?.toFixed(1) || '0.0'}</span>
          <span className="text-gray-400 text-sm">({hotel.reviewCount || 0} reviews)</span>
        </div>

        {hotel.amenities && hotel.amenities.length > 0 && (
          <div className="flex flex-wrap gap-1 mb-3">
            {hotel.amenities.slice(0, 3).map((amenity, idx) => (
              <span key={idx} className="px-2 py-1 bg-gray-100 text-gray-600 text-xs rounded">
                {amenity}
              </span>
            ))}
          </div>
        )}

        <div className="flex items-baseline gap-2">
          <span className="text-primary font-bold text-xl">
            {hotel.minPrice?.toLocaleString() || 'Contact'}₫
          </span>
          <span className="text-gray-400 text-sm">/night</span>
        </div>
      </div>
    </div>
  );
};

// Main HotelsPage Component
const HotelsPage = () => {
  const navigate = useNavigate();
  const [searchParams, setSearchParams] = useSearchParams();
  
  const [hotels, setHotels] = useState([]);
  const [cities, setCities] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showFilters, setShowFilters] = useState(false);

  const [filters, setFilters] = useState({
    search: searchParams.get('search') || '',
    city: searchParams.get('city') || '',
    minPrice: Number(searchParams.get('minPrice')) || 0,
    maxPrice: Number(searchParams.get('maxPrice')) || 50000000,
    starRating: searchParams.get('starRating') || '',
    sortBy: searchParams.get('sortBy') || 'rating',
    pageIndex: Number(searchParams.get('pageIndex')) || 1,
  });

  const fetchData = useCallback(async () => {
    setLoading(true);
    try {
      const params = new URLSearchParams();
      if (filters.search) params.append('search', filters.search);
      if (filters.city) params.append('city', filters.city);
      if (filters.minPrice > 0) params.append('minPrice', filters.minPrice);
      if (filters.maxPrice < 50000000) params.append('maxPrice', filters.maxPrice);
      if (filters.starRating) params.append('starRating', filters.starRating);
      params.append('sortBy', filters.sortBy);
      params.append('pageIndex', filters.pageIndex);
      params.append('pageSize', 12);

      const response = await api.get(`/hotels?${params.toString()}`);
      if (response.data.success) {
        setHotels(response.data.data || []);
        
        // Extract unique cities from hotels if not already loaded
        const uniqueCities = [...new Set((response.data.data || []).map(h => h.city).filter(Boolean))];
        if (uniqueCities.length > 0 && cities.length === 0) {
          setCities(uniqueCities);
        }
      }
    } catch (error) {
      console.error('Error fetching hotels:', error);
    } finally {
      setLoading(false);
    }
  }, [filters, cities.length]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  // Update URL when filters change
  useEffect(() => {
    const params = new URLSearchParams();
    if (filters.search) params.set('search', filters.search);
    if (filters.city) params.set('city', filters.city);
    if (filters.minPrice > 0) params.set('minPrice', filters.minPrice.toString());
    if (filters.maxPrice < 50000000) params.set('maxPrice', filters.maxPrice.toString());
    if (filters.starRating) params.set('starRating', filters.starRating);
    if (filters.sortBy !== 'rating') params.set('sortBy', filters.sortBy);
    if (filters.pageIndex > 1) params.set('pageIndex', filters.pageIndex.toString());
    setSearchParams(params, { replace: true });
  }, [filters, setSearchParams]);

  const handleFilterChange = (key, value) => {
    setFilters(prev => ({ ...prev, [key]: value, pageIndex: 1 }));
  };

  const clearFilters = () => {
    setFilters({
      search: '',
      city: '',
      minPrice: 0,
      maxPrice: 50000000,
      starRating: '',
      sortBy: 'rating',
      pageIndex: 1,
    });
  };

  const hasActiveFilters = filters.search || filters.city || 
    filters.minPrice > 0 || filters.maxPrice < 50000000 || filters.starRating;

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <div className="bg-gradient-to-r from-teal-600 to-cyan-700 text-white py-12">
        <div className="container mx-auto px-4">
          <h1 className="text-4xl font-bold mb-4">Hotels & Resorts</h1>
          <p className="text-lg text-white/80">Find the perfect accommodation for your trip</p>
        </div>
      </div>

      {/* Search Bar */}
      <div className="container mx-auto px-4 -mt-6">
        <div className="bg-white rounded-2xl shadow-lg p-4 flex flex-wrap gap-4 items-center">
          <div className="flex-1 min-w-[200px] relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 w-5 h-5" />
            <input
              type="text"
              placeholder="Search hotels..."
              value={filters.search}
              onChange={(e) => handleFilterChange('search', e.target.value)}
              className="w-full pl-10 pr-4 py-3 border border-gray-200 rounded-xl focus:outline-none focus:border-primary"
            />
          </div>
          
          <select
            value={filters.city}
            onChange={(e) => handleFilterChange('city', e.target.value)}
            className="px-4 py-3 border border-gray-200 rounded-xl focus:outline-none focus:border-primary min-w-[180px]"
          >
            <option value="">All cities</option>
            {cities.map(city => (
              <option key={city} value={city}>{city}</option>
            ))}
          </select>

          <select
            value={filters.sortBy}
            onChange={(e) => handleFilterChange('sortBy', e.target.value)}
            className="px-4 py-3 border border-gray-200 rounded-xl focus:outline-none focus:border-primary min-w-[150px]"
          >
            <option value="rating">High rating</option>
            <option value="price-asc">Low price → high price</option>
            <option value="price-desc">High price → low price</option>
            <option value="name">Name A → Z</option>
          </select>

          <button
            onClick={() => setShowFilters(!showFilters)}
            className={`px-4 py-3 rounded-xl flex items-center gap-2 transition-colors ${
              showFilters || hasActiveFilters 
                ? 'bg-primary text-white' 
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            }`}
          >
            <Filter className="w-5 h-5" />
            Filter
          </button>
        </div>
      </div>

      {/* Filters Panel */}
      {showFilters && (
        <div className="container mx-auto px-4 mt-4">
          <div className="bg-white rounded-2xl shadow-lg p-6">
            <div className="flex items-center justify-between mb-6">
              <h3 className="font-bold text-lg">Advanced filters</h3>
              {hasActiveFilters && (
                <button
                  onClick={clearFilters}
                  className="text-primary hover:underline text-sm"
                >
                  Clear filters
                </button>
              )}
            </div>

            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              {/* City Filter */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  City
                </label>
                <select
                  value={filters.city}
                  onChange={(e) => handleFilterChange('city', e.target.value)}
                  className="w-full px-4 py-2 border border-gray-200 rounded-xl focus:outline-none focus:border-primary"
                >
                  <option value="">All cities</option>
                  {cities.map(city => (
                    <option key={city} value={city}>{city}</option>
                  ))}
                </select>
              </div>

              {/* Star Rating Filter */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Star rating
                </label>
                <div className="flex flex-wrap gap-2">
                  {[5, 4, 3].map(stars => (
                    <button
                      key={stars}
                      onClick={() => handleFilterChange('starRating', filters.starRating === stars ? '' : stars.toString())}
                      className={`px-4 py-2 rounded-full text-sm transition-colors flex items-center gap-1 ${
                        filters.starRating === stars
                          ? 'bg-primary text-white'
                          : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                      }`}
                    >
                      {stars} <Star className="w-3 h-3 fill-current" />
                    </button>
                  ))}
                </div>
              </div>

              {/* Price Range */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Price range
                </label>
                <PriceRangeSlider
                  min={0}
                  max={50000000}
                  onChange={([min, max]) => {
                    handleFilterChange('minPrice', min);
                    handleFilterChange('maxPrice', max);
                  }}
                />
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Main Content */}
      <div className="container mx-auto px-4 py-8">
        {loading ? (
          <div className="flex items-center justify-center py-20">
            <Loader2 className="w-10 h-10 text-primary animate-spin" />
          </div>
        ) : (
          <section>
            <div className="flex items-center justify-between mb-6">
              <h2 className="text-2xl font-bold">
                {filters.search || filters.city || hasActiveFilters 
                  ? `Search results (${hotels.length} hotels)` 
                  : 'All hotels'}
              </h2>
            </div>

            {hotels.length === 0 ? (
              <div className="text-center py-12">
                <Search className="w-16 h-16 text-gray-300 mx-auto mb-4" />
                  <h3 className="text-xl font-medium text-gray-600 mb-2">No hotels found</h3>
                <p className="text-gray-500">Try adjusting the filters to find suitable hotels</p>
              </div>
            ) : (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {hotels.map(hotel => (
                  <HotelCard key={hotel.hotelId} hotel={hotel} />
                ))}
              </div>
            )}
          </section>
        )}
      </div>
    </div>
  );
};

export default HotelsPage;
