import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { 
  Search, 
  Filter, 
  MapPin, 
  Grid, 
  List, 
  ChevronDown, 
  X,
  SlidersHorizontal,
  Mountain,
  Waves,
  Landmark,
  Palmtree,
  Sparkles
} from 'lucide-react';
import SpotCard from '@/components/common/SpotCard';
import CommonInput from '@/components/common/CommonInput';
import CommonButton from '@/components/common/CommonButton';
import api from '@/services/api';

// Region options
const regions = [
  { value: '', label: 'All Regions' },
  { value: 'North', label: 'Northern' },
  { value: 'Central', label: 'Central' },
  { value: 'South', label: 'Southern' },
];

// Spot type options
const spotTypes = [
  { value: '', label: 'All Types' },
  { value: 'Beach', label: 'Beach & Islands', icon: Waves },
  { value: 'Mountain', label: 'Mountains & Forests', icon: Mountain },
  { value: 'Historical', label: 'Historical Sites', icon: Landmark },
  { value: 'Waterfall', label: 'Waterfalls', icon: Palmtree },
  { value: 'Cultural', label: 'Cultural', icon: Sparkles },
];

// Sort options
const sortOptions = [
  { value: 'rating', label: 'Highest Rated' },
  { value: 'name', label: 'Name A-Z' },
  { value: 'name-desc', label: 'Name Z-A' },
  { value: 'price-asc', label: 'Price: Low to High' },
  { value: 'price-desc', label: 'Price: High to Low' },
];

const TouristSpotsPage = () => {
  const [spots, setSpots] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [viewMode, setViewMode] = useState('grid');
  const [showMobileFilters, setShowMobileFilters] = useState(false);

  // Filter states
  const [search, setSearch] = useState('');
  const [selectedRegion, setSelectedRegion] = useState('');
  const [selectedType, setSelectedType] = useState('');
  const [sortBy, setSortBy] = useState('rating');
  const [pageIndex, setPageIndex] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const pageSize = 12;

  // Fetch spots with filters
  useEffect(() => {
    fetchSpots();
  }, [selectedRegion, selectedType, sortBy, pageIndex]);

  const fetchSpots = async () => {
    try {
      setLoading(true);
      const params = new URLSearchParams();
      
      if (search) params.append('search', search);
      if (selectedRegion) params.append('region', selectedRegion);
      if (selectedType) params.append('type', selectedType);
      params.append('sortBy', sortBy === 'name-desc' ? 'name' : sortBy);
      params.append('sortOrder', sortBy === 'name-desc' ? 'DESC' : 'ASC');
      params.append('pageIndex', pageIndex.toString());
      params.append('pageSize', pageSize.toString());

      const response = await api.get(`/touristspots?${params.toString()}`);
      
      if (response.data.success) {
        setSpots(response.data.data);
        setTotalCount(response.data.pagination?.totalCount || 0);
      } else {
        setError(response.data.message);
      }
    } catch (err) {
      console.error('Error fetching spots:', err);
      setError('Không thể tải dữ liệu. Vui lòng thử lại.');
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = (e) => {
    e.preventDefault();
    setPageIndex(1);
    fetchSpots();
  };

  const handleClearFilters = () => {
    setSearch('');
    setSelectedRegion('');
    setSelectedType('');
    setSortBy('rating');
    setPageIndex(1);
    fetchSpots();
  };

  const totalPages = Math.ceil(totalCount / pageSize);

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Hero Banner */}
      <div className="bg-gradient-to-r from-teal-600 to-cyan-700 text-white py-16">
        <div className="container mx-auto px-4">
          <h1 className="text-4xl font-bold mb-4">Điểm Du Lịch</h1>
          <p className="text-xl text-white/80 max-w-2xl">
            Khám phá những địa điểm du lịch tuyệt vời nhất Việt Nam
          </p>
        </div>
      </div>

      <div className="container mx-auto px-4 py-8">
        <div className="flex flex-col lg:flex-row gap-8">
          {/* Filter Sidebar - Desktop */}
          <aside className="lg:w-1/4 hidden lg:block">
            <div className="bg-white rounded-xl shadow-lg p-6 sticky top-24">
              <div className="flex items-center justify-between mb-6">
                <h2 className="text-lg font-semibold text-gray-800">Bộ lọc</h2>
                <button 
                  onClick={handleClearFilters}
                  className="text-sm text-primary hover:underline"
                >
                  Xóa tất cả
                </button>
              </div>

              {/* Search */}
              <form onSubmit={handleSearch} className="mb-6">
                <CommonInput
                  placeholder="Tìm kiếm điểm du lịch..."
                  value={search}
                  onChange={(e) => setSearch(e.target.value)}
                  icon={<Search className="w-5 h-5 text-gray-400" />}
                />
              </form>

              {/* Region Filter */}
              <div className="mb-6">
                <h3 className="text-sm font-medium text-gray-700 mb-3">Vùng miền</h3>
                <div className="space-y-2">
                  {regions.map((region) => (
                    <label
                      key={region.value}
                      className={`flex items-center gap-3 p-2 rounded-lg cursor-pointer transition-colors ${
                        selectedRegion === region.value 
                          ? 'bg-primary/10 text-primary' 
                          : 'hover:bg-gray-100 text-gray-600'
                      }`}
                    >
                      <input
                        type="radio"
                        name="region"
                        value={region.value}
                        checked={selectedRegion === region.value}
                        onChange={(e) => {
                          setSelectedRegion(e.target.value);
                          setPageIndex(1);
                        }}
                        className="w-4 h-4 text-primary"
                      />
                      <span className="text-sm">{region.label}</span>
                    </label>
                  ))}
                </div>
              </div>

              {/* Type Filter */}
              <div className="mb-6">
                <h3 className="text-sm font-medium text-gray-700 mb-3">Loại hình</h3>
                <div className="space-y-2">
                  {spotTypes.map((type) => (
                    <label
                      key={type.value}
                      className={`flex items-center gap-3 p-2 rounded-lg cursor-pointer transition-colors ${
                        selectedType === type.value 
                          ? 'bg-primary/10 text-primary' 
                          : 'hover:bg-gray-100 text-gray-600'
                      }`}
                    >
                      <input
                        type="radio"
                        name="type"
                        value={type.value}
                        checked={selectedType === type.value}
                        onChange={(e) => {
                          setSelectedType(e.target.value);
                          setPageIndex(1);
                        }}
                        className="w-4 h-4 text-primary"
                      />
                      {type.icon && <type.icon className="w-4 h-4" />}
                      <span className="text-sm">{type.label}</span>
                    </label>
                  ))}
                </div>
              </div>
            </div>
          </aside>

          {/* Main Content */}
          <main className="lg:w-3/4">
            {/* Toolbar */}
            <div className="bg-white rounded-xl shadow-sm p-4 mb-6">
              <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                {/* Results Count */}
                <div className="text-gray-600">
                  <span className="font-semibold text-primary">{totalCount}</span> điểm du lịch
                  {selectedRegion && <span> - Lọc: {regions.find(r => r.value === selectedRegion)?.label}</span>}
                  {selectedType && <span> - {spotTypes.find(t => t.value === selectedType)?.label}</span>}
                </div>

                <div className="flex items-center gap-4">
                  {/* Mobile Filter Toggle */}
                  <button
                    onClick={() => setShowMobileFilters(!showMobileFilters)}
                    className="lg:hidden flex items-center gap-2 px-4 py-2 bg-gray-100 rounded-lg text-gray-700"
                  >
                    <SlidersHorizontal className="w-4 h-4" />
                    Lọc
                  </button>

                  {/* Sort Dropdown */}
                  <div className="relative">
                    <select
                      value={sortBy}
                      onChange={(e) => {
                        setSortBy(e.target.value);
                        setPageIndex(1);
                      }}
                      className="appearance-none px-4 py-2 pr-10 bg-gray-100 rounded-lg text-gray-700 focus:outline-none focus:ring-2 focus:ring-primary cursor-pointer"
                    >
                      {sortOptions.map((option) => (
                        <option key={option.value} value={option.value}>
                          {option.label}
                        </option>
                      ))}
                    </select>
                    <ChevronDown className="absolute right-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-500 pointer-events-none" />
                  </div>

                  {/* View Mode Toggle */}
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

            {/* Mobile Filters */}
            {showMobileFilters && (
              <div className="lg:hidden bg-white rounded-xl shadow-lg p-6 mb-6">
                <div className="flex items-center justify-between mb-6">
                  <h2 className="text-lg font-semibold text-gray-800">Bộ lọc</h2>
                  <button onClick={() => setShowMobileFilters(false)}>
                    <X className="w-5 h-5 text-gray-500" />
                  </button>
                </div>

                {/* Search */}
                <form onSubmit={handleSearch} className="mb-6">
                  <CommonInput
                    placeholder="Tìm kiếm..."
                    value={search}
                    onChange={(e) => setSearch(e.target.value)}
                    icon={<Search className="w-5 h-5 text-gray-400" />}
                  />
                </form>

                {/* Region */}
                <div className="mb-4">
                  <h3 className="text-sm font-medium text-gray-700 mb-2">Vùng miền</h3>
                  <div className="flex flex-wrap gap-2">
                    {regions.map((region) => (
                      <button
                        key={region.value}
                        onClick={() => {
                          setSelectedRegion(region.value);
                          setPageIndex(1);
                        }}
                        className={`px-3 py-1.5 rounded-full text-sm transition-colors ${
                          selectedRegion === region.value
                            ? 'bg-primary text-white'
                            : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
                        }`}
                      >
                        {region.label}
                      </button>
                    ))}
                  </div>
                </div>

                {/* Type */}
                <div className="mb-6">
                  <h3 className="text-sm font-medium text-gray-700 mb-2">Loại hình</h3>
                  <div className="flex flex-wrap gap-2">
                    {spotTypes.map((type) => (
                      <button
                        key={type.value}
                        onClick={() => {
                          setSelectedType(type.value);
                          setPageIndex(1);
                        }}
                        className={`px-3 py-1.5 rounded-full text-sm transition-colors ${
                          selectedType === type.value
                            ? 'bg-primary text-white'
                            : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
                        }`}
                      >
                        {type.label}
                      </button>
                    ))}
                  </div>
                </div>

                <CommonButton onClick={() => setShowMobileFilters(false)} className="w-full">
                  Áp dụng
                </CommonButton>
              </div>
            )}

            {/* Spots Grid */}
            {loading ? (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {Array.from({ length: 6 }).map((_, index) => (
                  <div key={index} className="bg-white rounded-xl overflow-hidden shadow-md">
                    <div className="h-56 bg-gray-200 animate-pulse" />
                    <div className="p-4 space-y-3">
                      <div className="h-6 bg-gray-200 rounded animate-pulse" />
                      <div className="h-4 bg-gray-200 rounded animate-pulse w-3/4" />
                      <div className="h-4 bg-gray-200 rounded animate-pulse w-1/2" />
                    </div>
                  </div>
                ))}
              </div>
            ) : error ? (
              <div className="text-center py-12">
                <p className="text-red-500 mb-4">{error}</p>
                <CommonButton onClick={fetchSpots}>Thử lại</CommonButton>
              </div>
            ) : spots.length === 0 ? (
              <div className="text-center py-12 bg-white rounded-xl">
                <MapPin className="w-16 h-16 text-gray-300 mx-auto mb-4" />
                <h3 className="text-xl font-semibold text-gray-600 mb-2">
                  Không tìm thấy điểm du lịch
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
                  {spots.map((spot) => (
                    <SpotCard key={spot.spotId} spot={spot} />
                  ))}
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

export default TouristSpotsPage;
