import { useState, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { 
  ArrowLeft, 
  MapPin, 
  Star, 
  Calendar, 
  Ticket, 
  Phone, 
  Mail, 
  Clock,
  Heart,
  Share2,
  ChevronRight,
  Building2,
  Utensils,
  Palmtree,
  Bus,
  ExternalLink,
  MapPinned,
  Activity,
  Info
} from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import api from '@/services/api';
import CommonButton from '@/components/common/CommonButton';
import favoriteService from '@/services/favoriteService';

const TouristSpotDetailPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [spot, setSpot] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [activeImageIndex, setActiveImageIndex] = useState(0);
  const [isFavorite, setIsFavorite] = useState(false);
  const [favoriteLoading, setFavoriteLoading] = useState(false);

  useEffect(() => {
    if (id) {
      fetchSpotDetail();
      checkFavoriteStatus();
    }
  }, [id]);

  const checkFavoriteStatus = async () => {
    try {
      const response = await favoriteService.checkFavorites([id], 'TouristSpot');
      if (response.success && response.data) {
        setIsFavorite(response.data[id] || false);
      }
    } catch (err) {
      console.error('Error checking favorite status:', err);
    }
  };

  const handleToggleFavorite = async () => {
    setFavoriteLoading(true);
    try {
      if (isFavorite) {
        await favoriteService.removeByItem('TouristSpot', id);
        setIsFavorite(false);
      } else {
        await favoriteService.addFavorite('TouristSpot', id);
        setIsFavorite(true);
      }
    } catch (err) {
      console.error('Error toggling favorite:', err);
    } finally {
      setFavoriteLoading(false);
    }
  };

  const handleBooking = () => {
    // Pass both original price and discount price
    const originalPrice = spot?.ticketPrice || 0;
    const discountPrice = spot?.discountPrice || originalPrice;
    navigate(`/booking?item=${id}&type=touristspot&name=${encodeURIComponent(spot?.name || '')}&price=${originalPrice}&discountPrice=${discountPrice}`);
  };

  const handleConsult = () => {
    // Navigate to contact page with pre-filled info
    navigate(`/contact?item=${id}&type=touristspot&name=${encodeURIComponent(spot?.name || '')}`);
  };

  const fetchSpotDetail = async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await api.get(`/touristspots/${id}`);
      if (response.data.success) {
        setSpot(response.data.data);
      } else {
        setError(response.data.message || 'Không tìm thấy điểm du lịch');
      }
    } catch (err) {
      console.error('Error fetching spot detail:', err);
      setError('Không thể tải thông tin. Vui lòng thử lại.');
    } finally {
      setLoading(false);
    }
  };

  const images = spot?.images?.length > 0 
    ? spot.images 
    : ["https://images.unsplash.com/photo-1506929562872-bb421503ef21?w=1200&q=80"];

  const formatPrice = (price) => {
    if (!price || price === 0) return 'Miễn phí';
    return `${price.toLocaleString('vi-VN')}₫`;
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50">
        <div className="h-96 bg-gray-200 animate-pulse" />
        <div className="container mx-auto px-4 py-8">
          <Skeleton className="h-12 w-2/3 mb-4" />
          <Skeleton className="h-6 w-1/3 mb-8" />
          <div className="grid lg:grid-cols-3 gap-8">
            <div className="lg:col-span-2 space-y-4">
              <Skeleton className="h-48 w-full" />
              <Skeleton className="h-64 w-full" />
            </div>
            <Skeleton className="h-96" />
          </div>
        </div>
      </div>
    );
  }

  if (error || !spot) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <Info className="w-16 h-16 text-gray-300 mx-auto mb-4" />
          <h2 className="text-2xl font-bold text-gray-600 mb-2">Not Found</h2>
          <p className="text-gray-500 mb-4">{error || 'Destination does not exist'}</p>
          <CommonButton>
            <Link to="/info/tourist-spots">Go Back</Link>
          </CommonButton>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Breadcrumb */}
      <div className="bg-white border-b">
        <div className="container mx-auto px-4 py-3">
          <nav className="flex items-center gap-2 text-sm text-gray-500">
            <Link to="/" className="hover:text-primary">Home</Link>
            <ChevronRight className="w-4 h-4" />
            <Link to="/info/destinations" className="hover:text-primary">Destinations</Link>
            <ChevronRight className="w-4 h-4" />
            <span className="text-gray-800 font-medium">{spot.name}</span>
          </nav>
        </div>
      </div>

      {/* Hero Section with Images */}
      <div className="bg-white">
        <div className="container mx-auto px-4 py-6">
          <div className="grid lg:grid-cols-2 gap-6">
            {/* Main Image */}
            <div className="relative h-96 lg:h-[500px] rounded-2xl overflow-hidden">
              <img
                src={images[activeImageIndex]}
                alt={spot.name}
                className="w-full h-full object-cover"
              />
              {/* Image Navigation */}
              {images.length > 1 && (
                <div className="absolute bottom-4 left-1/2 -translate-x-1/2 flex gap-2">
                  {images.map((_, index) => (
                    <button
                      key={index}
                      onClick={() => setActiveImageIndex(index)}
                      className={`w-3 h-3 rounded-full transition-all ${
                        activeImageIndex === index 
                          ? 'bg-white scale-110' 
                          : 'bg-white/50 hover:bg-white/80'
                      }`}
                    />
                  ))}
                </div>
              )}
              {/* Actions */}
              <div className="absolute top-4 right-4 flex gap-2">
                <button 
                  onClick={handleToggleFavorite}
                  disabled={favoriteLoading}
                  className={`w-10 h-10 bg-white/90 backdrop-blur-sm rounded-full flex items-center justify-center transition-colors ${
                    isFavorite ? 'text-red-500 hover:text-red-600' : 'text-gray-600 hover:text-red-500'
                  }`}
                >
                  <Heart className={`w-5 h-5 ${isFavorite ? 'fill-current' : ''}`} />
                </button>
                <button className="w-10 h-10 bg-white/90 backdrop-blur-sm rounded-full flex items-center justify-center text-gray-600 hover:text-primary transition-colors">
                  <Share2 className="w-5 h-5" />
                </button>
              </div>
            </div>

            {/* Thumbnail Images */}
            <div className="grid grid-cols-2 gap-4">
              {images.slice(1, 5).map((img, index) => (
                <div 
                  key={index}
                  className="relative h-44 lg:h-60 rounded-xl overflow-hidden cursor-pointer"
                  onClick={() => setActiveImageIndex(index + 1)}
                >
                  <img
                    src={img}
                    alt={`${spot.name} ${index + 2}`}
                    className="w-full h-full object-cover hover:scale-110 transition-transform duration-300"
                  />
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>

      {/* Content Section */}
      <div className="container mx-auto px-4 py-8">
        <div className="grid lg:grid-cols-3 gap-8">
          {/* Main Content */}
          <div className="lg:col-span-2 space-y-6">
            {/* Header */}
            <div className="bg-white rounded-2xl p-6 shadow-sm">
              <div className="flex items-start justify-between gap-4 mb-4">
                <div>
                  <div className="flex items-center gap-2 mb-2">
                    <span className="px-3 py-1 bg-teal-100 text-teal-700 rounded-full text-sm font-medium">
                      {spot.region}
                    </span>
                    <span className="px-3 py-1 bg-blue-100 text-blue-700 rounded-full text-sm font-medium">
                      {spot.type}
                    </span>
                    {spot.isFeatured && (
                      <span className="px-3 py-1 bg-amber-100 text-amber-700 rounded-full text-sm font-medium">
                        Nổi bật
                      </span>
                    )}
                  </div>
                  <h1 className="text-3xl font-bold text-gray-800 mb-2">{spot.name}</h1>
                  <div className="flex items-center gap-4 text-gray-500">
                    <span className="flex items-center gap-1">
                      <MapPin className="w-4 h-4" />
                      {spot.city || spot.region}
                    </span>
                    <span className="flex items-center gap-1">
                      <Star className="w-4 h-4 fill-yellow-400 text-yellow-400" />
                      {spot.rating?.toFixed(1)} ({spot.reviewCount} reviews)
                    </span>
                  </div>
                </div>
              </div>
              
              {spot.address && (
                <div className="flex items-center gap-2 text-gray-600 mb-4">
                  <MapPinned className="w-4 h-4" />
                  <span>{spot.address}</span>
                </div>
              )}

              {/* Price & Time */}
              <div className="grid md:grid-cols-2 gap-4 pt-4 border-t">
                <div className="flex items-center gap-3">
                  <div className="w-12 h-12 bg-teal-100 rounded-xl flex items-center justify-center">
                    <Ticket className="w-6 h-6 text-teal-600" />
                  </div>
                  <div>
                    <p className="text-sm text-gray-500">Giá vé</p>
                    <p className="text-xl font-bold text-primary">{formatPrice(spot.ticketPrice)}</p>
                  </div>
                </div>
                <div className="flex items-center gap-3">
                  <div className="w-12 h-12 bg-amber-100 rounded-xl flex items-center justify-center">
                    <Calendar className="w-6 h-6 text-amber-600" />
                  </div>
                  <div>
                    <p className="text-sm text-gray-500">Thời điểm tốt nhất</p>
                    <p className="font-semibold text-gray-800">{spot.bestTime || 'Quanh năm'}</p>
                  </div>
                </div>
              </div>
            </div>

            {/* Description */}
            <div className="bg-white rounded-2xl p-6 shadow-sm">
              <h2 className="text-xl font-bold text-gray-800 mb-4">Giới thiệu</h2>
              <div className="prose max-w-none text-gray-600 leading-relaxed">
                {spot.description || 'Chưa có thông tin giới thiệu.'}
              </div>
            </div>

            {/* Activities */}
            <div className="bg-white rounded-2xl p-6 shadow-sm">
              <h2 className="text-xl font-bold text-gray-800 mb-4">Hoạt động</h2>
              <div className="grid md:grid-cols-2 gap-3">
                <div className="flex items-center gap-3 p-3 bg-gray-50 rounded-xl">
                  <Activity className="w-5 h-5 text-teal-600" />
                  <span className="text-gray-700">Khám phá thiên nhiên</span>
                </div>
                <div className="flex items-center gap-3 p-3 bg-gray-50 rounded-xl">
                  <Calendar className="w-5 h-5 text-teal-600" />
                  <span className="text-gray-700">Chụp ảnh cảnh quan</span>
                </div>
                <div className="flex items-center gap-3 p-3 bg-gray-50 rounded-xl">
                  <Palmtree className="w-5 h-5 text-teal-600" />
                  <span className="text-gray-700">Nghỉ dưỡng</span>
                </div>
                <div className="flex items-center gap-3 p-3 bg-gray-50 rounded-xl">
                  <Star className="w-5 h-5 text-teal-600" />
                  <span className="text-gray-700">Trải nghiệm văn hóa</span>
                </div>
              </div>
            </div>

            {/* Map Location */}
            <div className="bg-white rounded-2xl p-6 shadow-sm">
              <h2 className="text-xl font-bold text-gray-800 mb-4">Vị trí</h2>
              <div className="h-64 bg-gray-100 rounded-xl flex items-center justify-center">
                <div className="text-center">
                  <MapPinned className="w-12 h-12 text-gray-400 mx-auto mb-2" />
                  <p className="text-gray-500">Bản đồ sẽ được hiển thị tại đây</p>
                  <p className="text-sm text-gray-400">{spot.latitude}, {spot.longitude}</p>
                </div>
              </div>
            </div>
          </div>

          {/* Sidebar */}
          <div className="space-y-6">
            {/* Booking Card */}
            <Card className="border-0 shadow-lg sticky top-24">
              <CardContent className="p-6">
                <h3 className="text-lg font-bold text-gray-800 mb-4">Đặt tour ngay</h3>
                
                <div className="space-y-3 mb-6">
                  <div className="flex items-center gap-3 text-gray-600">
                    <Clock className="w-5 h-5 text-gray-400" />
                    <span className="text-sm">Thời gian: 1 ngày</span>
                  </div>
                  <div className="flex items-center gap-3 text-gray-600">
                    <Calendar className="w-5 h-5 text-gray-400" />
                    <span className="text-sm">Ngày linh hoạt</span>
                  </div>
                  <div className="flex items-center gap-3 text-gray-600">
                    <Bus className="w-5 h-5 text-gray-400" />
                    <span className="text-sm">Di chuyển: Xe khách</span>
                  </div>
                </div>

              <CommonButton className="w-full mb-3" onClick={handleBooking}>
                Đặt tour ngay
              </CommonButton>

              <CommonButton variant="outline" className="w-full" onClick={handleConsult}>
                Yêu cầu tư vấn
              </CommonButton>

                <p className="text-center text-xs text-gray-400 mt-4">
                  Liên hệ: 1900 xxxx để được hỗ trợ
                </p>
              </CardContent>
            </Card>

            {/* Nearby Services */}
            <Card className="border-0 shadow-lg">
              <CardContent className="p-6">
                <h3 className="text-lg font-bold text-gray-800 mb-4">Dịch vụ lân cận</h3>
                
                <div className="space-y-3">
                  <Link 
                    to={`/info/hotels?nearby=${spot.city}`}
                    className="flex items-center gap-3 p-3 rounded-xl hover:bg-gray-50 transition-colors"
                  >
                    <div className="w-10 h-10 bg-indigo-100 rounded-lg flex items-center justify-center">
                      <Building2 className="w-5 h-5 text-indigo-600" />
                    </div>
                    <div className="flex-1">
                      <p className="font-medium text-gray-800">Hotels</p>
                      <p className="text-xs text-gray-500">Gần điểm du lịch</p>
                    </div>
                    <ChevronRight className="w-4 h-4 text-gray-400" />
                  </Link>

                  <Link 
                    to={`/info/restaurants?nearby=${spot.city}`}
                    className="flex items-center gap-3 p-3 rounded-xl hover:bg-gray-50 transition-colors"
                  >
                    <div className="w-10 h-10 bg-amber-100 rounded-lg flex items-center justify-center">
                      <Utensils className="w-5 h-5 text-amber-600" />
                    </div>
                    <div className="flex-1">
                      <p className="font-medium text-gray-800">Restaurants</p>
                      <p className="text-xs text-gray-500">Ẩm thực địa phương</p>
                    </div>
                    <ChevronRight className="w-4 h-4 text-gray-400" />
                  </Link>

                  <Link 
                    to={`/info/resorts?nearby=${spot.city}`}
                    className="flex items-center gap-3 p-3 rounded-xl hover:bg-gray-50 transition-colors"
                  >
                    <div className="w-10 h-10 bg-pink-100 rounded-lg flex items-center justify-center">
                      <Palmtree className="w-5 h-5 text-pink-600" />
                    </div>
                    <div className="flex-1">
                      <p className="font-medium text-gray-800">Resorts</p>
                      <p className="text-xs text-gray-500">Nghỉ dưỡng cao cấp</p>
                    </div>
                    <ChevronRight className="w-4 h-4 text-gray-400" />
                  </Link>
                </div>
              </CardContent>
            </Card>

            {/* Contact */}
            <Card className="border-0 shadow-lg">
              <CardContent className="p-6">
                <h3 className="text-lg font-bold text-gray-800 mb-4">Liên hệ</h3>
                
                <div className="space-y-3">
                  <div className="flex items-center gap-3 text-gray-600">
                    <Phone className="w-5 h-5 text-gray-400" />
                    <span className="text-sm">1900 xxxx</span>
                  </div>
                  <div className="flex items-center gap-3 text-gray-600">
                    <Mail className="w-5 h-5 text-gray-400" />
                    <span className="text-sm">info@karneltravels.com</span>
                  </div>
                  <div className="flex items-center gap-3 text-gray-600">
                    <Clock className="w-5 h-5 text-gray-400" />
                    <span className="text-sm">08:00 - 20:00</span>
                  </div>
                </div>
              </CardContent>
            </Card>
          </div>
        </div>
      </div>
    </div>
  );
};

export default TouristSpotDetailPage;
