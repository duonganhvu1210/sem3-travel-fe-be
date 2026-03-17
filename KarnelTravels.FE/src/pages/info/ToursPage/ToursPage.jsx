import { useState, useEffect, useCallback } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { 
  Search, MapPin, Clock, Star, Calendar, 
  Filter, X, ArrowLeftRight, Heart, 
  ChevronDown, Loader2, Tag, Flame
} from 'lucide-react';
import api from '@/services/api';
import { useCompare } from '@/contexts/CompareContext';

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

// Tour Card Component
const TourCard = ({ tour, onCompare, isInCompare, onWishlist }) => {
  const navigate = useNavigate();
  const discountPercent = tour.discountPrice 
    ? Math.round((1 - tour.discountPrice / tour.price) * 100) 
    : 0;

  return (
    <div 
      className="bg-white rounded-2xl shadow-md hover:shadow-xl transition-all duration-300 overflow-hidden group cursor-pointer"
      onClick={() => navigate(`/info/tours/${tour.tourId}`)}
    >
      {/* Image */}
      <div className="relative h-56 overflow-hidden">
        <img
          src={tour.images?.[0] || 'https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=600'}
          alt={tour.name}
          className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
        />
        
        {/* Badges */}
        <div className="absolute top-3 left-3 flex flex-col gap-2">
          {tour.isHotDeal && (
            <span className="px-3 py-1 bg-red-500 text-white text-xs font-bold rounded-full flex items-center gap-1">
              <Flame className="w-3 h-3" /> HOT DEAL
            </span>
          )}
          {discountPercent > 0 && (
            <span className="px-3 py-1 bg-orange-500 text-white text-xs font-bold rounded-full">
              -{discountPercent}%
            </span>
          )}
          {tour.isNewArrival && (
            <span className="px-3 py-1 bg-green-500 text-white text-xs font-bold rounded-full">
              MỚI
            </span>
          )}
        </div>

        {/* Action Buttons */}
        <div className="absolute top-3 right-3 flex flex-col gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
          <button
            onClick={(e) => {
              e.stopPropagation();
              onCompare(tour);
            }}
            className={`w-9 h-9 bg-white/90 backdrop-blur-sm rounded-full flex items-center justify-center transition-colors hover:scale-110 z-10 ${
              isInCompare ? 'text-primary' : 'text-gray-400 hover:text-primary'
            }`}
          >
            <ArrowLeftRight className="w-4 h-4" />
          </button>
          <button 
            onClick={(e) => {
              e.stopPropagation();
              onWishlist(tour);
            }}
            className="w-9 h-9 bg-white/90 backdrop-blur-sm rounded-full flex items-center justify-center text-gray-400 hover:text-red-500 transition-colors hover:scale-110"
          >
            <Heart className="w-4 h-4" />
          </button>
        </div>
      </div>

      {/* Content */}
      <div className="p-5">
        {/* Destination & Duration */}
        <div className="flex items-center gap-3 text-sm text-gray-500 mb-2">
          <span className="flex items-center gap-1">
            <MapPin className="w-4 h-4 text-primary" />
            {tour.destination}
          </span>
          <span className="flex items-center gap-1">
            <Clock className="w-4 h-4 text-primary" />
            {tour.durationDays} ngày
          </span>
        </div>

        {/* Name */}
        <h3 className="font-bold text-lg text-gray-800 mb-2 line-clamp-2 group-hover:text-primary transition-colors">
          {tour.name}
        </h3>

        {/* Rating */}
        <div className="flex items-center gap-2 mb-3">
          <div className="flex items-center gap-1">
            <Star className="w-4 h-4 fill-yellow-400 text-yellow-400" />
            <span className="font-medium">{tour.rating?.toFixed(1) || '0.0'}</span>
          </div>
          <span className="text-gray-400 text-sm">({tour.reviewCount || 0} reviews)</span>
        </div>

        {/* Price */}
        <div className="flex items-baseline gap-2">
          {tour.discountPrice ? (
            <>
              <span className="text-primary font-bold text-xl">
                {tour.discountPrice.toLocaleString()}₫
              </span>
              <span className="text-gray-400 line-through text-sm">
                {tour.price.toLocaleString()}₫
              </span>
            </>
          ) : (
            <span className="text-primary font-bold text-xl">
              {tour.price.toLocaleString()}₫
            </span>
          )}
        </div>
      </div>
    </div>
  );
};

// Compare Bar Component
const CompareBar = ({ items, onRemove, onCompare }) => {
  if (items.length === 0) return null;

  return (
    <div className="fixed bottom-0 left-0 right-0 bg-white shadow-2xl border-t-2 border-primary z-50 p-4">
      <div className="container mx-auto">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-4 overflow-x-auto">
            <span className="font-medium text-gray-700">So sánh ({items.length}/3):</span>
            {items.map((item, index) => (
              <div key={index} className="relative flex-shrink-0">
                <button
                  onClick={() => onRemove(item)}
                  className="absolute -top-2 -right-2 w-5 h-5 bg-red-500 text-white rounded-full flex items-center justify-center text-xs hover:scale-110"
                >
                  <X className="w-3 h-3" />
                </button>
                <img
                  src={item.images?.[0] || 'https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=100'}
                  alt={item.name}
                  className="w-16 h-16 object-cover rounded-lg"
                />
                <p className="text-xs text-center mt-1 line-clamp-1 w-16">{item.name}</p>
              </div>
            ))}
            {items.length < 3 && (
              <div className="w-16 h-16 border-2 border-dashed border-gray-300 rounded-lg flex items-center justify-center text-gray-400">
                <span className="text-xs">+</span>
              </div>
            )}
          </div>
          <button
            onClick={onCompare}
            disabled={items.length < 2}
            className="px-6 py-2 bg-primary text-white rounded-full font-medium hover:bg-teal-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            So sánh ngay
          </button>
        </div>
      </div>
    </div>
  );
};

// Main ToursPage Component
const ToursPage = () => {
  const navigate = useNavigate();
  const [searchParams, setSearchParams] = useSearchParams();
  const { compareItems, addToCompare, removeFromCompare, clearCompare } = useCompare();
  
  // State
  const [tours, setTours] = useState([]);
  const [featuredTours, setFeaturedTours] = useState([]);
  const [newTours, setNewTours] = useState([]);
  const [dealTours, setDealTours] = useState([]);
  const [destinations, setDestinations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showFilters, setShowFilters] = useState(false);

  // Filter states from URL
  const [filters, setFilters] = useState({
    search: searchParams.get('search') || '',
    destination: searchParams.get('destination') || '',
    minPrice: Number(searchParams.get('minPrice')) || 0,
    maxPrice: Number(searchParams.get('maxPrice')) || 50000000,
    duration: searchParams.get('duration') || '',
    sortBy: searchParams.get('sortBy') || 'rating',
    pageIndex: Number(searchParams.get('pageIndex')) || 1,
  });

  const priceRanges = [0, 5000000, 10000000, 20000000, 50000000];

  // Fetch data
  const fetchData = useCallback(async () => {
    setLoading(true);
    try {
      const [toursRes, featuredRes, newRes, dealsRes, destRes] = await Promise.all([
        api.get('/tours', { params: { 
          search: filters.search || undefined,
          destination: filters.destination || undefined,
          minPrice: filters.minPrice > 0 ? filters.minPrice : undefined,
          maxPrice: filters.maxPrice < 50000000 ? filters.maxPrice : undefined,
          duration: filters.duration ? Number(filters.duration) : undefined,
          sortBy: filters.sortBy,
          pageIndex: filters.pageIndex,
          pageSize: 12
        }}),
        api.get('/tours/featured', { params: { pageSize: 6 } }),
        api.get('/tours/new', { params: { pageSize: 6 } }),
        api.get('/tours/deals', { params: { pageSize: 6 } }),
        api.get('/tours/destinations')
      ]);

      setTours(toursRes.data.data || []);
      setFeaturedTours(featuredRes.data.data || []);
      setNewTours(newRes.data.data || []);
      setDealTours(dealsRes.data.data || []);
      setDestinations(destRes.data.data || []);
    } catch (error) {
      console.error('Error fetching tours:', error);
    } finally {
      setLoading(false);
    }
  }, [filters]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  // Update URL when filters change
  useEffect(() => {
    const params = new URLSearchParams();
    if (filters.search) params.set('search', filters.search);
    if (filters.destination) params.set('destination', filters.destination);
    if (filters.minPrice > 0) params.set('minPrice', filters.minPrice.toString());
    if (filters.maxPrice < 50000000) params.set('maxPrice', filters.maxPrice.toString());
    if (filters.duration) params.set('duration', filters.duration);
    if (filters.sortBy !== 'rating') params.set('sortBy', filters.sortBy);
    if (filters.pageIndex > 1) params.set('pageIndex', filters.pageIndex.toString());
    setSearchParams(params, { replace: true });
  }, [filters, setSearchParams]);

  const handleFilterChange = (key, value) => {
    setFilters(prev => ({ ...prev, [key]: value, pageIndex: 1 }));
  };

  const handleCompare = (tour) => {
    const exists = compareItems.some(i => i.tourId === tour.tourId);
    if (exists) {
      removeFromCompare(tour);
    } else {
      addToCompare(tour);
    }
  };

  const isInCompare = (tourId) => compareItems.some(i => i.tourId === tourId);

  const clearFilters = () => {
    setFilters({
      search: '',
      destination: '',
      minPrice: 0,
      maxPrice: 50000000,
      duration: '',
      sortBy: 'rating',
      pageIndex: 1,
    });
  };

  const hasActiveFilters = filters.search || filters.destination || 
    filters.minPrice > 0 || filters.maxPrice < 50000000 || filters.duration;

  return (
    <div className="min-h-screen bg-gray-50 pb-24">
      {/* Header */}
      <div className="bg-gradient-to-r from-teal-600 to-cyan-700 text-white py-12">
        <div className="container mx-auto px-4">
          <h1 className="text-4xl font-bold mb-4">Explore Our Tours</h1>
          <p className="text-lg text-white/80">Experience unforgettable journeys with us</p>
        </div>
      </div>

      {/* Search Bar */}
      <div className="container mx-auto px-4 -mt-6">
        <div className="bg-white rounded-2xl shadow-lg p-4 flex flex-wrap gap-4 items-center">
          <div className="flex-1 min-w-[200px] relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 w-5 h-5" />
            <input
              type="text"
              placeholder="Search tours..."
              value={filters.search}
              onChange={(e) => handleFilterChange('search', e.target.value)}
              className="w-full pl-10 pr-4 py-3 border border-gray-200 rounded-xl focus:outline-none focus:border-primary"
            />
          </div>
          
          <select
            value={filters.destination}
            onChange={(e) => handleFilterChange('destination', e.target.value)}
            className="px-4 py-3 border border-gray-200 rounded-xl focus:outline-none focus:border-primary min-w-[180px]"
          >
            <option value="">All destinations</option>
            {destinations.map(dest => (
              <option key={dest} value={dest}>{dest}</option>
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
              {/* Destination Filter */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Destination
                </label>
                <select
                  value={filters.destination}
                  onChange={(e) => handleFilterChange('destination', e.target.value)}
                  className="w-full px-4 py-2 border border-gray-200 rounded-xl focus:outline-none focus:border-primary"
                >
                  <option value="">All destinations</option>
                  {destinations.map(dest => (
                    <option key={dest} value={dest}>{dest}</option>
                  ))}
                </select>
              </div>

              {/* Duration Filter */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Number of days
                </label>
                <div className="flex flex-wrap gap-2">
                  {[2, 3, 4, 5, 7].map(day => (
                    <button
                      key={day}
                      onClick={() => handleFilterChange('duration', filters.duration === day ? '' : day.toString())}
                      className={`px-4 py-2 rounded-full text-sm transition-colors ${
                        filters.duration === day
                          ? 'bg-primary text-white'
                          : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                      }`}
                    >
                      {day} days
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
          <>
            {/* Featured Tours Section */}
            {filters.pageIndex === 1 && !filters.search && (
              <section className="mb-12">
                <div className="flex items-center gap-2 mb-6">
                  <Flame className="w-6 h-6 text-orange-500" />
                  <h2 className="text-2xl font-bold">Featured tours</h2>
                </div>
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                  {featuredTours.slice(0, 6).map(tour => (
                    <TourCard
                      key={tour.tourId}
                      tour={tour}
                      onCompare={handleCompare}
                      isInCompare={isInCompare(tour.tourId)}
                      onWishlist={() => {}}
                    />
                  ))}
                </div>
              </section>
            )}

            {/* New Tours Section */}
            {filters.pageIndex === 1 && !filters.search && (
              <section className="mb-12">
                <div className="flex items-center gap-2 mb-6">
                  <Tag className="w-6 h-6 text-green-500" />
                  <h2 className="text-2xl font-bold">New tours</h2>
                </div>
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                  {newTours.slice(0, 6).map(tour => (
                    <TourCard
                      key={tour.tourId}
                      tour={tour}
                      onCompare={handleCompare}
                      isInCompare={isInCompare(tour.tourId)}
                      onWishlist={() => {}}
                    />
                  ))}
                </div>
              </section>
            )}

            {/* Hot Deals Section */}
            {filters.pageIndex === 1 && !filters.search && (
              <section className="mb-12">
                <div className="flex items-center gap-2 mb-6">
                  <Calendar className="w-6 h-6 text-red-500" />
                  <h2 className="text-2xl font-bold">Discount tours</h2>
                </div>
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                  {dealTours.slice(0, 6).map(tour => (
                    <TourCard
                      key={tour.tourId}
                      tour={tour}
                      onCompare={handleCompare}
                      isInCompare={isInCompare(tour.tourId)}
                      onWishlist={() => {}}
                    />
                  ))}
                </div>
              </section>
            )}

            {/* All Tours / Search Results */}
            <section>
              <div className="flex items-center justify-between mb-6">
                <h2 className="text-2xl font-bold">
                  {filters.search || filters.destination || hasActiveFilters 
                    ? `Search results (${tours.length} tour)` 
                    : 'All tours'}
                </h2>
              </div>

              {tours.length === 0 ? (
                <div className="text-center py-12">
                  <Search className="w-16 h-16 text-gray-300 mx-auto mb-4" />
                  <h3 className="text-xl font-medium text-gray-600 mb-2">No tours found</h3>
                  <p className="text-gray-500">Try adjusting the filters to find suitable tours</p>
                </div>
              ) : (
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                  {tours.map(tour => (
                    <TourCard
                      key={tour.tourId}
                      tour={tour}
                      onCompare={handleCompare}
                      isInCompare={isInCompare(tour.tourId)}
                      onWishlist={() => {}}
                    />
                  ))}
                </div>
              )}
            </section>
          </>
        )}
      </div>

      {/* Compare Bar */}
      <CompareBar
        items={compareItems}
        onRemove={handleCompare}
        onCompare={() => navigate('/search?tab=tours')}
      />
    </div>
  );
};

export default ToursPage;
