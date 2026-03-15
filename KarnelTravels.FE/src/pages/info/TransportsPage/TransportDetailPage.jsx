import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { 
  MapPin, Clock, Phone, Mail, Loader2, ChevronLeft, ChevronRight,
  ArrowLeftRight, Wifi, Coffee, Tv, Car, Check, X, Heart, Share2
} from 'lucide-react';
import api from '@/services/api';

// Image Gallery Component
const ImageGallery = ({ images }) => {
  const [currentIndex, setCurrentIndex] = useState(0);
  const [lightboxOpen, setLightboxOpen] = useState(false);

  const allImages = images?.length > 0 
    ? images 
    : ['https://images.unsplash.com/photo-1449965408869-eaa3f722e40d?w=1200'];

  const nextImage = (e) => {
    e?.stopPropagation();
    setCurrentIndex(prev => (prev + 1) % allImages.length);
  };

  const prevImage = (e) => {
    e?.stopPropagation();
    setCurrentIndex(prev => (prev - 1 + allImages.length) % allImages.length);
  };

  return (
    <div className="space-y-4">
      <div className="relative h-[300px] md:h-[400px] rounded-2xl overflow-hidden cursor-pointer group">
        <img
          src={allImages[currentIndex]}
          alt="Transport"
          className="w-full h-full object-cover"
        />
        <button onClick={prevImage} className="absolute left-4 top-1/2 -translate-y-1/2 w-10 h-10 bg-white/90 rounded-full flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity">
          <ChevronLeft className="w-6 h-6" />
        </button>
        <button onClick={nextImage} className="absolute right-4 top-1/2 -translate-y-1/2 w-10 h-10 bg-white/90 rounded-full flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity">
          <ChevronRight className="w-6 h-6" />
        </button>
        <div className="absolute bottom-4 right-4 bg-black/50 text-white px-3 py-1 rounded-full text-sm">
          {currentIndex + 1} / {allImages.length}
        </div>
      </div>

      <div className="flex gap-2 overflow-x-auto pb-2">
        {allImages.map((img, idx) => (
          <button key={idx} onClick={() => setCurrentIndex(idx)} className={`flex-shrink-0 w-20 h-20 rounded-lg overflow-hidden ${idx === currentIndex ? 'ring-2 ring-primary' : 'opacity-70'}`}>
            <img src={img} alt="" className="w-full h-full object-cover" />
          </button>
        ))}
      </div>
    </div>
  );
};

// Amenity Icon Mapping
const amenityIcons = {
  'WiFi': Wifi,
  'Wifi': Wifi,
  'Wi-Fi': Wifi,
  'Meal': Coffee,
  'Food': Coffee,
  'Entertainment': Tv,
  'TV': Tv,
  'Parking': Car,
  'AC': Check,
  'Air Conditioning': Check,
};

// Transport Card for Schedule
const ScheduleCard = ({ transport, onSelect, isSelected }) => (
  <div 
    onClick={() => onSelect(transport)}
    className={`border-2 rounded-xl p-4 cursor-pointer transition-all ${
      isSelected 
        ? 'border-primary bg-primary/5' 
        : 'border-gray-200 hover:border-primary/50'
    }`}
  >
    <div className="flex items-center justify-between mb-3">
      <span className="font-bold text-lg">{transport.provider}</span>
      <span className="px-2 py-1 bg-primary/10 text-primary text-xs rounded-full">
        {transport.type}
      </span>
    </div>
    <div className="flex items-center justify-between text-sm">
      <div>
        <span className="font-medium">{transport.departureTime}</span>
        <span className="text-gray-400 mx-2">→</span>
        <span className="font-medium">{transport.arrivalTime}</span>
      </div>
      <span className="font-bold text-primary text-lg">
        {transport.price?.toLocaleString()}₫
      </span>
    </div>
  </div>
);

// Main Transport Detail Component
const TransportDetailPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  
  const [transport, setTransport] = useState(null);
  const [similarTransports, setSimilarTransports] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedSchedule, setSelectedSchedule] = useState(null);

  useEffect(() => {
    const fetchTransport = async () => {
      try {
        setLoading(true);
        const response = await api.get(`/transports/${id}`);
        if (response.data.success) {
          setTransport(response.data.data);
          
          // Fetch similar transports
          try {
            const similarRes = await api.get(`/transports?type=${response.data.data.type}&pageSize=4`);
            if (similarRes.data.success) {
              setSimilarTransports((similarRes.data.data || []).filter(t => t.transportId !== id));
            }
          } catch (e) {
            console.log('No similar transports');
          }
        } else {
          setError(response.data.message || 'Transport not found');
        }
      } catch (err) {
        setError('Failed to load transport details');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    if (id) fetchTransport();
  }, [id]);

  const handleBookNow = () => {
    if (selectedSchedule) {
      navigate(`/checkout?transportId=${selectedSchedule.transportId}`);
    } else if (transport) {
      navigate(`/checkout?transportId=${transport.transportId}`);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <Loader2 className="w-10 h-10 text-primary animate-spin" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen flex flex-col items-center justify-center">
        <h2 className="text-2xl font-bold text-red-500 mb-4">{error}</h2>
        <button onClick={() => navigate('/info/transports')} className="px-6 py-2 bg-primary text-white rounded-full">
          Quay lại
        </button>
      </div>
    );
  }

  const formatDuration = (minutes) => {
    const hours = Math.floor(minutes / 60);
    const mins = minutes % 60;
    return hours > 0 ? `${hours}h ${mins}p` : `${mins}p`;
  };

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <div className="bg-gradient-to-r from-teal-600 to-cyan-700 text-white py-8">
        <div className="container mx-auto px-4">
          <button onClick={() => navigate(-1)} className="flex items-center gap-2 text-white/80 hover:text-white mb-4">
            <ChevronLeft className="w-5 h-5" /> Quay lại
          </button>
          
          <div className="flex flex-wrap gap-3 mb-3">
            <span className="px-3 py-1 bg-white/20 text-white text-sm font-medium rounded-full">
              {transport.type === 'Flight' ? 'Máy bay' : 
               transport.type === 'Bus' ? 'Xe khách' : 
               transport.type === 'Train' ? 'Tàu hỏa' : 
               transport.type === 'Car' ? 'Ô tô' : 'Limousine'}
            </span>
            <span className="px-3 py-1 bg-white/20 text-white text-sm rounded-full">
              {transport.provider}
            </span>
          </div>

          <h1 className="text-3xl md:text-4xl font-bold mb-4">
            {transport.route || `${transport.fromCity} → ${transport.toCity}`}
          </h1>
          
          <div className="flex flex-wrap gap-4 text-white/90">
            <span className="flex items-center gap-2">
              <MapPin className="w-5 h-5" />
              {transport.fromCity} → {transport.toCity}
            </span>
            <span className="flex items-center gap-2">
              <Clock className="w-5 h-5" />
              {formatDuration(transport.durationMinutes)}
            </span>
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div className="container mx-auto px-4 py-8">
        <div className="grid lg:grid-cols-3 gap-8">
          {/* Left Column - Images & Info */}
          <div className="lg:col-span-2 space-y-6">
            <ImageGallery images={transport.images} />

            {/* Route Info */}
            <div className="bg-white rounded-2xl shadow-md p-6">
              <h2 className="text-xl font-bold mb-4">Thông tin chuyến đi</h2>
              
              <div className="flex items-center justify-between mb-6">
                <div className="text-center">
                  <div className="text-2xl font-bold text-gray-800">{transport.departureTime}</div>
                  <div className="text-gray-500">{transport.fromCity}</div>
                </div>
                
                <div className="flex-1 mx-4 relative">
                  <div className="border-t-2 border-dashed border-gray-300"></div>
                  <div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 bg-white px-3">
                    <ArrowLeftRight className="w-5 h-5 text-gray-400" />
                  </div>
                </div>
                
                <div className="text-center">
                  <div className="text-2xl font-bold text-gray-800">{transport.arrivalTime}</div>
                  <div className="text-gray-500">{transport.toCity}</div>
                </div>
              </div>

              <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                <div className="text-center p-3 bg-gray-50 rounded-xl">
                  <Clock className="w-5 h-5 text-primary mx-auto mb-1" />
                  <div className="text-sm text-gray-500">Thời gian</div>
                  <div className="font-medium">{formatDuration(transport.durationMinutes)}</div>
                </div>
                <div className="text-center p-3 bg-gray-50 rounded-xl">
                  <Car className="w-5 h-5 text-primary mx-auto mb-1" />
                  <div className="text-sm text-gray-500">Ghế trống</div>
                  <div className="font-medium">{transport.availableSeats} ghế</div>
                </div>
                <div className="text-center p-3 bg-gray-50 rounded-xl">
                  <MapPin className="w-5 h-5 text-primary mx-auto mb-1" />
                  <div className="text-sm text-gray-500">Tuyến đường</div>
                  <div className="font-medium">{transport.route || 'N/A'}</div>
                </div>
                <div className="text-center p-3 bg-gray-50 rounded-xl">
                  <span className="text-2xl">
                    {transport.type === 'Flight' ? '✈️' : 
                     transport.type === 'Bus' ? '🚌' : 
                     transport.type === 'Train' ? '🚂' : '🚗'}
                  </span>
                  <div className="text-sm text-gray-500">Loại xe</div>
                  <div className="font-medium">{transport.type}</div>
                </div>
              </div>
            </div>

            {/* Amenities */}
            {transport.amenities && transport.amenities.length > 0 && (
              <div className="bg-white rounded-2xl shadow-md p-6">
                <h2 className="text-xl font-bold mb-4">Tiện nghi</h2>
                <div className="grid grid-cols-2 md:grid-cols-3 gap-3">
                  {transport.amenities.map((amenity, idx) => {
                    const Icon = amenityIcons[amenity] || Check;
                    return (
                      <div key={idx} className="flex items-center gap-2 text-gray-700">
                        <Icon className="w-5 h-5 text-primary" />
                        <span>{amenity}</span>
                      </div>
                    );
                  })}
                </div>
              </div>
            )}

            {/* Similar Transports */}
            {similarTransports.length > 0 && (
              <div className="bg-white rounded-2xl shadow-md p-6">
                <h2 className="text-xl font-bold mb-4">Chuyến đi tương tự</h2>
                <div className="space-y-3">
                  {similarTransports.slice(0, 3).map(t => (
                    <div 
                      key={t.transportId}
                      onClick={() => navigate(`/info/transports/${t.transportId}`)}
                      className="flex items-center justify-between p-3 border border-gray-200 rounded-xl hover:border-primary cursor-pointer"
                    >
                      <div>
                        <div className="font-medium">{t.provider}</div>
                        <div className="text-sm text-gray-500">{t.fromCity} → {t.toCity}</div>
                      </div>
                      <div className="text-right">
                        <div className="font-bold text-primary">{t.price?.toLocaleString()}₫</div>
                        <div className="text-xs text-gray-500">{t.departureTime} → {t.arrivalTime}</div>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            )}
          </div>

          {/* Right Column - Booking */}
          <div className="lg:col-span-1">
            <div className="bg-white rounded-2xl shadow-lg p-6 sticky top-4">
              <div className="mb-4">
                <span className="text-3xl font-bold text-primary">{transport.price?.toLocaleString()}₫</span>
                <span className="text-gray-500 text-sm ml-2">/người</span>
              </div>

              <div className="mb-4 p-3 bg-gray-50 rounded-xl">
                <div className="flex justify-between text-sm mb-2">
                  <span className="text-gray-500">Ngày khởi hành</span>
                  <span className="font-medium">{new Date().toLocaleDateString('vi-VN')}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-gray-500">Giờ khởi hành</span>
                  <span className="font-medium">{transport.departureTime}</span>
                </div>
              </div>

              <button 
                onClick={handleBookNow}
                className="w-full py-4 bg-primary text-white font-bold rounded-xl hover:bg-teal-700 text-lg mb-3"
              >
                Đặt vé ngay
              </button>

              <div className="flex gap-2 mb-4">
                <button className="flex-1 py-3 bg-gray-100 rounded-xl flex items-center justify-center gap-2 hover:bg-gray-200">
                  <Heart className="w-5 h-5" /> Yêu thích
                </button>
                <button className="flex-1 py-3 bg-gray-100 rounded-xl flex items-center justify-center gap-2 hover:bg-gray-200">
                  <Share2 className="w-5 h-5" /> Chia sẻ
                </button>
              </div>

              <div className="pt-4 border-t">
                <h4 className="font-medium mb-3">Thông tin liên hệ</h4>
                <div className="space-y-2 text-sm text-gray-600">
                  <div className="flex items-center gap-2">
                    <MapPin className="w-4 h-4" /> 
                    Tuyến: {transport.fromCity} - {transport.toCity}
                  </div>
                  <div className="flex items-center gap-2">
                    <Clock className="w-4 h-4" /> 
                    Thời gian: {formatDuration(transport.durationMinutes)}
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default TransportDetailPage;
