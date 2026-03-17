import { useState, useEffect, useCallback } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { 
  Search, MapPin, Filter, Loader2, Heart, Clock
} from 'lucide-react';
import api from '@/services/api';

// Transport Card Component
const TransportCard = ({ transport }) => {
  const navigate = useNavigate();

  return (
    <div 
      className="bg-white rounded-2xl shadow-md hover:shadow-xl transition-all duration-300 overflow-hidden group cursor-pointer"
      onClick={() => navigate(`/info/transports/${transport.transportId}`)}
    >
      <div className="relative h-48 overflow-hidden bg-gradient-to-br from-teal-50 to-cyan-50">
        <div className="absolute inset-0 flex items-center justify-center">
          <span className="text-6xl font-bold text-teal-200">
            {transport.type === 'Flight' ? '✈️' : transport.type === 'Bus' ? '🚌' : transport.type === 'Train' ? '🚂' : '🚗'}
          </span>
        </div>
        <div className="absolute top-3 left-3">
          <span className="px-3 py-1 bg-primary text-white text-xs font-bold rounded-full">
            {transport.type}
          </span>
        </div>
        <div className="absolute top-3 right-3">
          <button 
            onClick={(e) => e.stopPropagation()}
            className="w-9 h-9 bg-white/90 rounded-full flex items-center justify-center text-gray-400 hover:text-red-500"
          >
            <Heart className="w-4 h-4" />
          </button>
        </div>
      </div>

      <div className="p-5">
        <div className="flex items-center gap-2 text-sm text-gray-500 mb-2">
          <span className="font-medium">{transport.provider}</span>
        </div>

        <h3 className="font-bold text-lg text-gray-800 mb-3 group-hover:text-primary">
          {transport.route || `${transport.fromCity} → ${transport.toCity}`}
        </h3>

        <div className="flex items-center justify-between text-sm mb-4">
          <div className="flex items-center gap-4">
            <div className="flex flex-col">
              <span className="font-medium text-gray-700">{transport.fromCity}</span>
              <span className="text-gray-500 text-xs">{transport.departureTime}</span>
            </div>
            <div className="flex items-center gap-1 text-gray-400">
              <span className="text-xs">→</span>
            </div>
            <div className="flex flex-col">
              <span className="font-medium text-gray-700">{transport.toCity}</span>
              <span className="text-gray-500 text-xs">{transport.arrivalTime}</span>
            </div>
          </div>
        </div>

        {transport.durationMinutes > 0 && (
          <div className="flex items-center gap-1 text-gray-500 text-sm mb-4">
            <Clock className="w-4 h-4" />
            {Math.floor(transport.durationMinutes / 60)}h {transport.durationMinutes % 60}p
          </div>
        )}

        <div className="flex items-baseline justify-between">
          <span className="text-primary font-bold text-xl">
            {transport.price?.toLocaleString() || 'Liên hệ'}₫
          </span>
          <span className="text-gray-500 text-sm">/người</span>
        </div>
      </div>
    </div>
  );
};

// Main TransportsPage Component
const TransportsPage = () => {
  const navigate = useNavigate();
  const [searchParams, setSearchParams] = useSearchParams();
  
  const [transports, setTransports] = useState([]);
  const [cities, setCities] = useState([]);
  const [loading, setLoading] = useState(true);

  const [filters, setFilters] = useState({
    type: searchParams.get('type') || '',
    fromCity: searchParams.get('fromCity') || '',
    toCity: searchParams.get('toCity') || '',
    minPrice: Number(searchParams.get('minPrice')) || 0,
    maxPrice: Number(searchParams.get('maxPrice')) || 50000000,
    sortBy: searchParams.get('sortBy') || 'price-asc',
    pageIndex: Number(searchParams.get('pageIndex')) || 1,
  });

  const fetchData = useCallback(async () => {
    setLoading(true);
    try {
      const params = new URLSearchParams();
      if (filters.type) params.append('type', filters.type);
      if (filters.fromCity) params.append('fromCity', filters.fromCity);
      if (filters.toCity) params.append('toCity', filters.toCity);
      if (filters.minPrice > 0) params.append('minPrice', filters.minPrice);
      if (filters.maxPrice < 50000000) params.append('maxPrice', filters.maxPrice);
      params.append('sortBy', filters.sortBy);
      params.append('pageIndex', filters.pageIndex);
      params.append('pageSize', 12);

      const response = await api.get(`/transports?${params.toString()}`);
      if (response.data.success) {
        setTransports(response.data.data || []);
        
        // Extract unique cities
        const allCities = new Set();
        (response.data.data || []).forEach(t => {
          if (t.fromCity) allCities.add(t.fromCity);
          if (t.toCity) allCities.add(t.toCity);
        });
        setCities([...allCities]);
      }
    } catch (error) {
      console.error('Error fetching transports:', error);
    } finally {
      setLoading(false);
    }
  }, [filters]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  useEffect(() => {
    const params = new URLSearchParams();
    if (filters.type) params.set('type', filters.type);
    if (filters.fromCity) params.set('fromCity', filters.fromCity);
    if (filters.toCity) params.set('toCity', filters.toCity);
    if (filters.minPrice > 0) params.set('minPrice', filters.minPrice.toString());
    if (filters.maxPrice < 50000000) params.set('maxPrice', filters.maxPrice.toString());
    if (filters.sortBy !== 'price-asc') params.set('sortBy', filters.sortBy);
    if (filters.pageIndex > 1) params.set('pageIndex', filters.pageIndex.toString());
    setSearchParams(params, { replace: true });
  }, [filters, setSearchParams]);

  const handleFilterChange = (key, value) => {
    setFilters(prev => ({ ...prev, [key]: value, pageIndex: 1 }));
  };

  const clearFilters = () => {
    setFilters({
      type: '',
      fromCity: '',
      toCity: '',
      minPrice: 0,
      maxPrice: 50000000,
      sortBy: 'price-asc',
      pageIndex: 1,
    });
  };

  const hasActiveFilters = filters.type || filters.fromCity || filters.toCity || filters.minPrice > 0 || filters.maxPrice < 50000000;

  const transportTypes = ['Flight', 'Bus', 'Train', 'Car', 'Limousine'];

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="bg-gradient-to-r from-teal-600 to-cyan-700 text-white py-12">
        <div className="container mx-auto px-4">
          <h1 className="text-4xl font-bold mb-4">Transportation</h1>
          <p className="text-lg text-white/80">Find the perfect transportation for your trip</p>
        </div>
      </div>

      <div className="container mx-auto px-4 -mt-6">
        <div className="bg-white rounded-2xl shadow-lg p-4">
          <div className="flex flex-wrap gap-4">
            <select
              value={filters.type}
              onChange={(e) => handleFilterChange('type', e.target.value)}
              className="px-4 py-3 border border-gray-200 rounded-xl focus:outline-none focus:border-primary min-w-[150px]"
            >
              <option value="">All Types</option>
              {transportTypes.map(type => (
                <option key={type} value={type}>{type === 'Flight' ? 'Flight' : type === 'Bus' ? 'Bus' : type === 'Train' ? 'Train' : type === 'Car' ? 'Car' : 'Limousine'}</option>
              ))}
            </select>

            <select
              value={filters.fromCity}
              onChange={(e) => handleFilterChange('fromCity', e.target.value)}
              className="px-4 py-3 border border-gray-200 rounded-xl focus:outline-none focus:border-primary min-w-[150px]"
            >
              <option value="">Điểm đi</option>
              {cities.map(city => (
                <option key={city} value={city}>{city}</option>
              ))}
            </select>

            <select
              value={filters.toCity}
              onChange={(e) => handleFilterChange('toCity', e.target.value)}
              className="px-4 py-3 border border-gray-200 rounded-xl focus:outline-none focus:border-primary min-w-[150px]"
            >
              <option value="">Điểm đến</option>
              {cities.map(city => (
                <option key={city} value={city}>{city}</option>
              ))}
            </select>

            <select
              value={filters.sortBy}
              onChange={(e) => handleFilterChange('sortBy', e.target.value)}
              className="px-4 py-3 border border-gray-200 rounded-xl focus:outline-none focus:border-primary min-w-[150px]"
            >
              <option value="price-asc">Giá thấp → cao</option>
              <option value="price-desc">Giá cao → thấp</option>
            </select>

            {hasActiveFilters && (
              <button onClick={clearFilters} className="px-4 py-3 text-red-500 hover:underline">
                Xóa lọc
              </button>
            )}
          </div>
        </div>
      </div>

      <div className="container mx-auto px-4 py-8">
        {loading ? (
          <div className="flex items-center justify-center py-20">
            <Loader2 className="w-10 h-10 text-primary animate-spin" />
          </div>
        ) : transports.length === 0 ? (
          <div className="text-center py-12">
            <Search className="w-16 h-16 text-gray-300 mx-auto mb-4" />
            <h3 className="text-xl text-gray-600">Không tìm thấy phương tiện</h3>
            <p className="text-gray-500">Thử điều chỉnh bộ lọc</p>
          </div>
        ) : (
          <>
            <h2 className="text-2xl font-bold mb-6">{transports.length} phương tiện</h2>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {transports.map(t => (
                <TransportCard key={t.transportId} transport={t} />
              ))}
            </div>
          </>
        )}
      </div>
    </div>
  );
};

export default TransportsPage;
