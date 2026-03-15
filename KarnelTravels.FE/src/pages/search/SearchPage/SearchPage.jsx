import { useState, useEffect, useMemo } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { 
  Search, 
  MapPin, 
  Calendar,
  Users,
  Bus,
  Building2,
  Utensils,
  Palmtree,
  Star,
  Grid,
  List,
  SlidersHorizontal,
  X,
  ChevronDown,
  Filter,
  Heart,
  Clock,
  Check,
  ArrowRight,
  ArrowLeftRight,
  Wallet,
  TrendingUp
} from 'lucide-react';
import api from '@/services/api';
import CommonButton from '@/components/common/CommonButton';
import CommonInput from '@/components/common/CommonInput';
import SpotCard from '@/components/common/SpotCard';

// Tab categories
const TABS = [
  { id: 'all', label: 'Tất cả', icon: Search },
  { id: 'spots', label: 'Điểm đến', icon: MapPin },
  { id: 'hotels', label: 'Khách sạn', icon: Building2 },
  { id: 'restaurants', label: 'Nhà hàng', icon: Utensils },
  { id: 'resorts', label: 'Resort', icon: Palmtree },
  { id: 'tours', label: 'Tour', icon: Palmtree },
  { id: 'transports', label: 'Vận chuyển', icon: Bus },
];

// Sort options
const SORT_OPTIONS = [
  { value: 'relevance', label: 'Liên quan' },
  { value: 'price-asc', label: 'Giá thấp đến cao' },
  { value: 'price-desc', label: 'Giá cao đến thấp' },
  { value: 'rating', label: 'Đánh giá cao nhất' },
  { value: 'name-asc', label: 'Tên A-Z' },
];

// Price ranges
const PRICE_RANGES = [
  { value: '', label: 'Tất cả' },
  { value: '0-500000', label: 'Dưới 500K' },
  { value: '500000-1000000', label: '500K - 1 triệu' },
  { value: '1000000-3000000', label: '1 - 3 triệu' },
  { value: '3000000-5000000', label: '3 - 5 triệu' },
  { value: '5000000-10000000', label: '5 - 10 triệu' },
  { value: '10000000-', label: 'Trên 10 triệu' },
];

// Star ratings
const STAR_RATINGS = [
  { value: '', label: 'Tất cả' },
  { value: '5', label: '5 sao' },
  { value: '4', label: '4 sao' },
  { value: '3', label: '3 sao' },
  { value: '2', label: '2 sao' },
  { value: '1', label: '1 sao' },
];

// Amenities options
const AMENITIES = [
  { value: 'wifi', label: 'Wifi' },
  { value: 'pool', label: 'Hồ bơi' },
  { value: 'parking', label: 'Đỗ xe' },
  { value: 'restaurant', label: 'Nhà hàng' },
  { value: 'spa', label: 'Spa' },
  { value: 'gym', label: 'Gym' },
  { value: 'ac', label: 'Điều hòa' },
  { value: 'beach', label: 'Gần biển' },
  { value: 'airport', label: 'Đưa đón sân bay' },
];

const SearchPage = () => {
  const [searchParams, setSearchParams] = useSearchParams();
  
  // State
  const [activeTab, setActiveTab] = useState(searchParams.get('tab') || 'all');
  const [searchQuery, setSearchQuery] = useState(searchParams.get('q') || '');
  const [results, setResults] = useState([]);
  const [loading, setLoading] = useState(true);
  const [viewMode, setViewMode] = useState('grid');
  const [showMobileFilters, setShowMobileFilters] = useState(false);
  
  // Filter states
  const [priceRange, setPriceRange] = useState('');
  const [starRating, setStarRating] = useState('');
  const [selectedAmenities, setSelectedAmenities] = useState([]);
  const [sortBy, setSortBy] = useState('relevance');
  const [showOnlyDiscount, setShowOnlyDiscount] = useState(false);
  
  // Pagination
  const [pageIndex, setPageIndex] = useState(1);
  const pageSize = 12;
  const [totalCount, setTotalCount] = useState(0);
  
  // Advanced features
  const [showCompare, setShowCompare] = useState(false);
  const [compareItems, setCompareItems] = useState([]);
  const [suggestions, setSuggestions] = useState([]);
  const [showSuggestions, setShowSuggestions] = useState(false);

  // Fetch results
  useEffect(() => {
    fetchResults();
  }, [activeTab, priceRange, starRating, selectedAmenities, sortBy, pageIndex, showOnlyDiscount]);

  // Get suggestions on search query change
  useEffect(() => {
    if (searchQuery.length >= 2) {
      getSuggestions();
    } else {
      setSuggestions([]);
    }
  }, [searchQuery]);

  const fetchResults = async () => {
    try {
      setLoading(true);
      
      const params = {
        search: searchQuery,
        pageIndex,
        pageSize,
        sortBy: sortBy === 'relevance' ? '' : sortBy,
      };

      // Add filters
      if (priceRange) {
        const [min, max] = priceRange.split('-');
        if (min) params.minPrice = min;
        if (max) params.maxPrice = max;
      }
      if (starRating) params.starRating = starRating;
      if (sortBy && sortBy !== 'relevance') params.sortBy = sortBy;

      let response;
      
      // Fetch based on active tab
      switch (activeTab) {
        case 'all':
          response = await api.get('/touristspots', { params: { ...params, limit: pageSize * 3 } });
          // Also fetch hotels, tours, etc. and combine
          const [spotsRes, hotelsRes] = await Promise.all([
            api.get('/touristspots', { params }),
            api.get('/hotels', { params: { ...params, limit: 6 } }),
          ]);
          const combined = [
            ...(spotsRes.data.data || []).map(s => ({ ...s, type: 'spot' })),
            ...(hotelsRes.data.data || []).map(h => ({ ...h, type: 'hotel' })),
          ];
          setResults(combined);
          setTotalCount((spotsRes.data.pagination?.totalCount || 0) + (hotelsRes.data.pagination?.totalCount || 0));
          setLoading(false);
          return;
        case 'spots':
          response = await api.get('/touristspots', { params });
          break;
        case 'hotels':
          response = await api.get('/hotels', { params });
          break;
        case 'restaurants':
          response = await api.get('/restaurants', { params });
          break;
        case 'resorts':
          response = await api.get('/resorts', { params });
          break;
        case 'tours':
          response = await api.get('/tours', { params });
          break;
        case 'transports':
          response = await api.get('/transports', { params });
          break;
        default:
          response = await api.get('/touristspots', { params });
      }

      if (response.data.success) {
        setResults(response.data.data || []);
        setTotalCount(response.data.pagination?.totalCount || 0);
      }
    } catch (error) {
      console.error('Search error:', error);
      setResults([]);
      setTotalCount(0);
    } finally {
      setLoading(false);
    }
  };

  const getSuggestions = async () => {
    try {
      const response = await api.get('/search/suggestions', { params: { q: searchQuery } });
      if (response.data.success && response.data.data) {
        setSuggestions(response.data.data.map(s => ({
          name: s.text,
          type: s.type,
          region: s.subtext
        })));
      }
    } catch (error) {
      console.error('Get suggestions error:', error);
      setSuggestions([]);
    }
  };

  const handleSearch = (e) => {
    e.preventDefault();
    setPageIndex(1);
    fetchResults();
    setSearchParams({ q: searchQuery, tab: activeTab });
  };

  const handleTabChange = (tabId) => {
    setActiveTab(tabId);
    setPageIndex(1);
    searchParams.set('tab', tabId);
    setSearchParams(searchParams);
  };

  const handleClearFilters = () => {
    setSearchQuery('');
    setPriceRange('');
    setStarRating('');
    setSelectedAmenities([]);
    setSortBy('relevance');
    setShowOnlyDiscount(false);
    setPageIndex(1);
  };

  const toggleAmenity = (amenity) => {
    setSelectedAmenities(prev => 
      prev.includes(amenity) 
        ? prev.filter(a => a !== amenity)
        : [...prev, amenity]
    );
    setPageIndex(1);
  };

  const toggleCompareItem = (item) => {
    const itemId = item.spotId || item.SpotId || item.hotelId || item.HotelId || item.tourId || item.TourId || item.id;
    setCompareItems(prev => {
      const exists = prev.find(i => 
        (i.spotId || i.SpotId || i.hotelId || i.HotelId || i.tourId || i.TourId || i.id) === itemId
      );
      if (exists) {
        return prev.filter(i => 
          (i.spotId || i.SpotId || i.hotelId || i.HotelId || i.tourId || i.TourId || i.id) !== itemId
        );
      }
      if (prev.length >= 3) return prev;
      return [...prev, item];
    });
  };

  const totalPages = Math.ceil(totalCount / pageSize);

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Search Header */}
      <div className="bg-gradient-to-r from-teal-600 to-cyan-700 text-white py-8">
        <div className="container mx-auto px-4">
          {/* Tabs */}
          <div className="flex overflow-x-auto gap-2 mb-6 pb-2 scrollbar-hide">
            {TABS.map((tab) => (
              <button
                key={tab.id}
                onClick={() => handleTabChange(tab.id)}
                className={`flex items-center gap-2 px-4 py-2 rounded-full whitespace-nowrap transition-all ${
                  activeTab === tab.id
                    ? 'bg-white text-teal-700 font-medium shadow-lg'
                    : 'bg-white/10 hover:bg-white/20 text-white/90'
                }`}
              >
                <tab.icon className="w-4 h-4" />
                {tab.label}
              </button>
            ))}
          </div>

          {/* Search Bar */}
          <form onSubmit={handleSearch} className="relative">
            <div className="bg-white rounded-2xl p-2 flex items-center shadow-2xl">
              <div className="flex-1 flex items-center px-3 border-r">
                <Search className="w-5 h-5 text-gray-400 mr-2" />
                <input
                  type="text"
                  placeholder="Tìm kiếm điểm đến, khách sạn, tour..."
                  value={searchQuery}
                  onChange={(e) => setSearchQuery(e.target.value)}
                  onFocus={() => setShowSuggestions(true)}
                  className="w-full py-3 text-gray-800 outline-none placeholder-gray-400"
                />
              </div>
              
              <CommonButton type="submit" className="ml-2 bg-teal-600 hover:bg-teal-700">
                <Search className="w-4 h-4 mr-2" />
                Tìm kiếm
              </CommonButton>
            </div>

            {/* Autocomplete Suggestions */}
            {showSuggestions && suggestions.length > 0 && (
              <div className="absolute top-full left-0 right-0 mt-2 bg-white rounded-xl shadow-2xl z-50 overflow-hidden">
                {suggestions.map((suggestion, index) => (
                  <button
                    key={index}
                    type="button"
                    onClick={() => {
                      setSearchQuery(suggestion.name);
                      setShowSuggestions(false);
                    }}
                    className="w-full flex items-center gap-3 px-4 py-3 hover:bg-gray-50 text-left"
                  >
                    <MapPin className="w-4 h-4 text-gray-400" />
                    <span className="text-gray-800">{suggestion.name}</span>
                    <span className="text-gray-400 text-sm">- {suggestion.region}</span>
                  </button>
                ))}
              </div>
            )}
          </form>

          {/* Active Filters Display */}
          {(priceRange || starRating || selectedAmenities.length > 0 || showOnlyDiscount) && (
            <div className="flex flex-wrap items-center gap-2 mt-4">
              <span className="text-white/80 text-sm">Lọc:</span>
              {priceRange && (
                <span className="px-3 py-1 bg-white/20 rounded-full text-sm flex items-center gap-1">
                  Giá: {PRICE_RANGES.find(p => p.value === priceRange)?.label}
                  <button onClick={() => setPriceRange('')}><X className="w-3 h-3" /></button>
                </span>
              )}
              {starRating && (
                <span className="px-3 py-1 bg-white/20 rounded-full text-sm flex items-center gap-1">
                  {starRating} sao
                  <button onClick={() => setStarRating('')}><X className="w-3 h-3" /></button>
                </span>
              )}
              {showOnlyDiscount && (
                <span className="px-3 py-1 bg-red-500 rounded-full text-sm flex items-center gap-1">
                  Giảm giá
                  <button onClick={() => setShowOnlyDiscount(false)}><X className="w-3 h-3" /></button>
                </span>
              )}
              <button onClick={handleClearFilters} className="text-white/80 text-sm underline">
                Xóa tất cả
              </button>
            </div>
          )}
        </div>
      </div>

      <div className="container mx-auto px-4 py-8">
        <div className="flex flex-col lg:flex-row gap-8">
          {/* Filter Sidebar */}
          <aside className="lg:w-1/4">
            {/* Mobile Filter Toggle */}
            <button
              onClick={() => setShowMobileFilters(!showMobileFilters)}
              className="lg:hidden w-full flex items-center justify-center gap-2 px-4 py-3 bg-white rounded-xl shadow-sm mb-4"
            >
              <SlidersHorizontal className="w-4 h-4" />
              Bộ lọc
            </button>

            <div className={`lg:block ${showMobileFilters ? 'block' : 'hidden'}`}>
              <div className="bg-white rounded-xl shadow-sm p-6 space-y-6">
                <div className="flex items-center justify-between">
                  <h3 className="font-semibold text-gray-800">Bộ lọc</h3>
                  <button onClick={handleClearFilters} className="text-sm text-primary hover:underline">
                    Xóa tất cả
                  </button>
                </div>

                {/* Price Range */}
                <div>
                  <h4 className="text-sm font-medium text-gray-700 mb-3">Khoảng giá</h4>
                  <div className="space-y-2">
                    {PRICE_RANGES.map((range) => (
                      <label key={range.value} className="flex items-center gap-2 cursor-pointer">
                        <input
                          type="radio"
                          name="price"
                          value={range.value}
                          checked={priceRange === range.value}
                          onChange={(e) => {
                            setPriceRange(e.target.value);
                            setPageIndex(1);
                          }}
                          className="w-4 h-4 text-primary"
                        />
                        <span className="text-sm text-gray-600">{range.label}</span>
                      </label>
                    ))}
                  </div>
                </div>

                {/* Star Rating */}
                <div>
                  <h4 className="text-sm font-medium text-gray-700 mb-3">Hạng sao</h4>
                  <div className="flex flex-wrap gap-2">
                    {STAR_RATINGS.map((star) => (
                      <button
                        key={star.value}
                        onClick={() => {
                          setStarRating(star.value);
                          setPageIndex(1);
                        }}
                        className={`px-3 py-1.5 rounded-lg text-sm transition-colors ${
                          starRating === star.value
                            ? 'bg-primary text-white'
                            : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
                        }`}
                      >
                        {star.label}
                      </button>
                    ))}
                  </div>
                </div>

                {/* Amenities */}
                <div>
                  <h4 className="text-sm font-medium text-gray-700 mb-3">Tiện nghi</h4>
                  <div className="space-y-2">
                    {AMENITIES.map((amenity) => (
                      <label key={amenity.value} className="flex items-center gap-2 cursor-pointer">
                        <input
                          type="checkbox"
                          checked={selectedAmenities.includes(amenity.value)}
                          onChange={() => toggleAmenity(amenity.value)}
                          className="w-4 h-4 rounded text-primary"
                        />
                        <span className="text-sm text-gray-600">{amenity.label}</span>
                      </label>
                    ))}
                  </div>
                </div>

                {/* Discount Filter */}
                <div>
                  <label className="flex items-center gap-2 cursor-pointer">
                    <input
                      type="checkbox"
                      checked={showOnlyDiscount}
                      onChange={(e) => {
                        setShowOnlyDiscount(e.target.checked);
                        setPageIndex(1);
                      }}
                      className="w-4 h-4 rounded text-primary"
                    />
                    <span className="text-sm font-medium text-gray-700">Chỉ hiển thị giảm giá</span>
                  </label>
                </div>
              </div>
            </div>
          </aside>

          {/* Main Content */}
          <main className="lg:w-3/4">
            {/* Toolbar */}
            <div className="bg-white rounded-xl shadow-sm p-4 mb-6">
              <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                <div className="text-gray-600">
                  <span className="font-semibold text-primary">{totalCount}</span> kết quả
                  {searchQuery && <span> cho "{searchQuery}"</span>}
                </div>

                <div className="flex items-center gap-4">
                  {/* Sort */}
                  <div className="relative">
                    <select
                      value={sortBy}
                      onChange={(e) => {
                        setSortBy(e.target.value);
                        setPageIndex(1);
                      }}
                      className="appearance-none px-4 py-2 pr-10 bg-gray-100 rounded-lg text-gray-700 focus:outline-none focus:ring-2 focus:ring-primary cursor-pointer"
                    >
                      {SORT_OPTIONS.map((option) => (
                        <option key={option.value} value={option.value}>
                          {option.label}
                        </option>
                      ))}
                    </select>
                    <ChevronDown className="absolute right-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-500 pointer-events-none" />
                  </div>

                  {/* Compare Toggle */}
                  <button
                    onClick={() => setShowCompare(!showCompare)}
                    className={`flex items-center gap-2 px-4 py-2 rounded-lg transition-colors ${
                      showCompare 
                        ? 'bg-primary text-white' 
                        : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                    }`}
                  >
                    <ArrowLeftRight className="w-4 h-4" />
                    So sánh
                    {compareItems.length > 0 && (
                      <span className="bg-white text-primary text-xs px-1.5 py-0.5 rounded-full">
                        {compareItems.length}
                      </span>
                    )}
                  </button>

                  {/* View Mode */}
                  <div className="hidden md:flex items-center gap-1 bg-gray-100 rounded-lg p-1">
                    <button
                      onClick={() => setViewMode('grid')}
                      className={`p-2 rounded-lg transition-colors ${
                        viewMode === 'grid' 
                          ? 'bg-white shadow text-primary' 
                          : 'text-gray-500 hover:text-gray-700'
                      }`}
                    >
                      <Grid className="w-4 h-4" />
                    </button>
                    <button
                      onClick={() => setViewMode('list')}
                      className={`p-2 rounded-lg transition-colors ${
                        viewMode === 'list' 
                          ? 'bg-white shadow text-primary' 
                          : 'text-gray-500 hover:text-gray-700'
                      }`}
                    >
                      <List className="w-4 h-4" />
                    </button>
                  </div>
                </div>
              </div>
            </div>

            {/* Compare Panel */}
            {showCompare && compareItems.length > 0 && (
              <div className="bg-white rounded-xl shadow-sm p-6 mb-6">
                <div className="flex items-center justify-between mb-4">
                  <h3 className="font-semibold text-gray-800">So sánh ({compareItems.length}/3)</h3>
                  <button onClick={() => setCompareItems([])} className="text-sm text-red-500">
                    Xóa tất cả
                  </button>
                </div>
                <div className="grid grid-cols-3 gap-4">
                  {compareItems.map((item, index) => (
                    <div key={index} className="relative p-4 bg-gray-50 rounded-xl">
                      <button
                        onClick={() => toggleCompareItem(item)}
                        className="absolute -top-2 -right-2 w-6 h-6 bg-red-500 text-white rounded-full flex items-center justify-center cursor-pointer hover:scale-110 z-10"
                      >
                        <X className="w-3 h-3" />
                      </button>
                      <img
                        src={item.images?.[0] || 'https://images.unsplash.com/photo-1506929562872-bb421503ef21?w=400'}
                        alt={item.name}
                        className="w-full h-32 object-cover rounded-lg mb-3"
                      />
                      <h4 className="font-medium text-gray-800 text-sm line-clamp-1">{item.name}</h4>
                      <div className="flex items-center gap-1 mt-1">
                        <Star className="w-3 h-3 fill-yellow-400 text-yellow-400" />
                        <span className="text-xs">{item.rating}</span>
                      </div>
                      <p className="text-primary font-bold mt-1">
                        {item.price?.toLocaleString() || item.ticketPrice?.toLocaleString()}₫
                      </p>
                    </div>
                  ))}
                  {compareItems.length < 3 && (
                    <div className="border-2 border-dashed border-gray-200 rounded-xl flex items-center justify-center p-4">
                      <p className="text-gray-400 text-sm text-center">Thêm item để so sánh</p>
                    </div>
                  )}
                </div>
              </div>
            )}

            {/* Results */}
            {loading ? (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {Array.from({ length: 6 }).map((_, index) => (
                  <div key={index} className="bg-white rounded-xl overflow-hidden shadow-md">
                    <div className="h-48 bg-gray-200 animate-pulse" />
                    <div className="p-4 space-y-3">
                      <div className="h-6 bg-gray-200 rounded animate-pulse" />
                      <div className="h-4 bg-gray-200 rounded animate-pulse w-3/4" />
                      <div className="h-4 bg-gray-200 rounded animate-pulse w-1/2" />
                    </div>
                  </div>
                ))}
              </div>
            ) : results.length === 0 ? (
              <div className="text-center py-12 bg-white rounded-xl">
                <Search className="w-16 h-16 text-gray-300 mx-auto mb-4" />
                <h3 className="text-xl font-semibold text-gray-600 mb-2">
                  Không tìm thấy kết quả
                </h3>
                <p className="text-gray-500 mb-4">
                  Thử thay đổi bộ lọc hoặc từ khóa tìm kiếm
                </p>
                <CommonButton variant="outline" onClick={handleClearFilters}>
                  Xóa bộ lọc
                </CommonButton>
              </div>
            ) : (
              <>
                <div className={`grid gap-6 ${
                  viewMode === 'grid' 
                    ? 'grid-cols-1 md:grid-cols-2 lg:grid-cols-3' 
                    : 'grid-cols-1'
                }`}>
                  {results.map((item) => {
                    const itemId = item.spotId || item.SpotId || item.hotelId || item.HotelId || item.tourId || item.TourId || item.id;
                    const isInCompare = compareItems.some(
                      i => (i.spotId || i.SpotId || i.hotelId || i.HotelId || i.tourId || i.TourId || i.id) === itemId
                    );

                    return (
                      <div key={itemId} className="bg-white rounded-xl overflow-hidden shadow-md hover:shadow-xl transition-all group">
                        {/* Image */}
                        <div className="relative h-48 overflow-hidden">
                          <img
                            src={item.images?.[0] || 'https://images.unsplash.com/photo-1506929562872-bb421503ef21?w=800'}
                            alt={item.name}
                            className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
                          />
                          
                          {/* Badges */}
                          <div className="absolute top-3 left-3 flex gap-2">
                            {item.stars && (
                              <span className="px-2 py-1 bg-amber-500 text-white text-xs font-medium rounded-lg flex items-center gap-1">
                                <Star className="w-3 h-3" />
                                {item.stars}
                              </span>
                            )}
                            {item.discount && (
                              <span className="px-2 py-1 bg-red-500 text-white text-xs font-medium rounded-lg">
                                Giảm {item.discount}%
                              </span>
                            )}
                          </div>

                          {/* Actions */}
                          <div className="absolute top-3 right-3 flex gap-2">
                              <button
                              onClick={(e) => {
                                e.stopPropagation();
                                toggleCompareItem(item);
                              }}
                              className={`w-9 h-9 bg-white/90 backdrop-blur-sm rounded-full flex items-center justify-center transition-colors cursor-pointer hover:scale-110 z-10 ${
                                isInCompare ? 'text-primary' : 'text-gray-400 hover:text-primary'
                              }`}
                            >
                              <ArrowLeftRight className="w-4 h-4" />
                            </button>
                            <button 
                              onClick={(e) => e.stopPropagation()}
                              className="w-9 h-9 bg-white/90 backdrop-blur-sm rounded-full flex items-center justify-center text-gray-400 hover:text-red-500 transition-colors cursor-pointer hover:scale-110 z-10"
                            >
                              <Heart className="w-4 h-4" />
                            </button>
                          </div>
                        </div>

                        {/* Content */}
                        <div className="p-4">
                          {/* Location */}
                          <div className="flex items-center gap-1 text-sm text-gray-500 mb-2">
                            <MapPin className="w-4 h-4" />
                            <span>{item.city || item.address || item.region}</span>
                          </div>

                          {/* Title */}
                          <h3 className="font-semibold text-lg text-gray-800 mb-2 line-clamp-1 group-hover:text-primary transition-colors">
                            {item.name}
                          </h3>

                          {/* Description */}
                          {item.description && (
                            <p className="text-gray-500 text-sm mb-3 line-clamp-2">
                              {item.description}
                            </p>
                          )}

                          {/* Info Row */}
                          <div className="flex items-center justify-between mb-3">
                            <div className="flex items-center gap-1">
                              <Star className="w-4 h-4 fill-yellow-400 text-yellow-400" />
                              <span className="font-medium text-sm">{item.rating?.toFixed(1)}</span>
                              <span className="text-gray-400 text-xs">({item.reviewCount || 0})</span>
                            </div>
                            <div className="text-right">
                              <span className="text-primary font-bold">
                                {item.price?.toLocaleString() || item.ticketPrice?.toLocaleString() || 'Liên hệ'}₫
                              </span>
                            </div>
                          </div>

                          {/* Duration for tours */}
                          {item.duration && (
                            <div className="flex items-center gap-1 text-sm text-gray-500 mb-3">
                              <Clock className="w-4 h-4" />
                              <span>{item.duration}</span>
                            </div>
                          )}

                          {/* Actions */}
                          <div className="flex gap-2">
                            <Link
                              to={item.type === 'hotel' ? `/info/hotels/${item.hotelId}` : `/info/destinations/${itemId}`}
                              className="flex-1 px-4 py-2 bg-gray-100 text-gray-700 text-center rounded-lg font-medium hover:bg-gray-200 transition-colors"
                            >
                              Xem chi tiết
                            </Link>
                            <Link
                              to={`/bookings?item=${itemId}&type=${item.type || activeTab}`}
                              className="flex-1 px-4 py-2 bg-primary text-white text-center rounded-lg font-medium hover:bg-primary/90 transition-colors"
                            >
                              Đặt ngay
                            </Link>
                          </div>
                        </div>
                      </div>
                    );
                  })}
                </div>

                {/* Pagination */}
                {totalPages > 1 && (
                  <div className="flex justify-center items-center gap-2 mt-8">
                    <CommonButton
                      variant="outline"
                      size="sm"
                      disabled={pageIndex === 1}
                      onClick={() => setPageIndex(pageIndex - 1)}
                    >
                      Trước
                    </CommonButton>
                    
                    {Array.from({ length: Math.min(5, totalPages) }, (_, i) => {
                      let page;
                      if (totalPages <= 5) {
                        page = i + 1;
                      } else if (pageIndex <= 3) {
                        page = i + 1;
                      } else if (pageIndex >= totalPages - 2) {
                        page = totalPages - 4 + i;
                      } else {
                        page = pageIndex - 2 + i;
                      }
                      
                      return (
                        <button
                          key={page}
                          onClick={() => setPageIndex(page)}
                          className={`w-10 h-10 rounded-lg font-medium transition-colors ${
                            pageIndex === page
                              ? 'bg-primary text-white'
                              : 'bg-white text-gray-600 hover:bg-gray-100'
                          }`}
                        >
                          {page}
                        </button>
                      );
                    })}

                    <CommonButton
                      variant="outline"
                      size="sm"
                      disabled={pageIndex === totalPages}
                      onClick={() => setPageIndex(pageIndex + 1)}
                    >
                      Sau
                    </CommonButton>
                  </div>
                )}
              </>
            )}
          </main>
        </div>
      </div>
    </div>
  );
};

export default SearchPage;
