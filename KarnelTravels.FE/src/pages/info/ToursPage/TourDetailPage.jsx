import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { 
  MapPin, Clock, Star, Calendar, Users, Check, X,
  ChevronLeft, ChevronRight, Loader2, Flame, 
  ArrowLeftRight, Heart, Share2, Phone
} from 'lucide-react';
import api from '@/services/api';
import { useCompare } from '@/contexts/CompareContext';

// Image Gallery Component
const ImageGallery = ({ images }) => {
  const [currentIndex, setCurrentIndex] = useState(0);
  const [lightboxOpen, setLightboxOpen] = useState(false);

  const allImages = images?.length > 0 
    ? images 
    : ['https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=1200'];

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
      <div className="relative h-[400px] md:h-[500px] rounded-2xl overflow-hidden cursor-pointer group">
        <img
          src={allImages[currentIndex]}
          alt="Tour"
          className="w-full h-full object-cover"
        />
        <button
          onClick={prevImage}
          className="absolute left-4 top-1/2 -translate-y-1/2 w-10 h-10 bg-white/90 rounded-full flex items-center justify-center shadow-lg opacity-0 group-hover:opacity-100 transition-opacity hover:scale-110"
        >
          <ChevronLeft className="w-6 h-6" />
        </button>
        <button
          onClick={nextImage}
          className="absolute right-4 top-1/2 -translate-y-1/2 w-10 h-10 bg-white/90 rounded-full flex items-center justify-center shadow-lg opacity-0 group-hover:opacity-100 transition-opacity hover:scale-110"
        >
          <ChevronRight className="w-6 h-6" />
        </button>
        <div className="absolute bottom-4 right-4 bg-black/50 text-white px-3 py-1 rounded-full text-sm">
          {currentIndex + 1} / {allImages.length}
        </div>
      </div>

      <div className="flex gap-2 overflow-x-auto pb-2">
        {allImages.map((img, idx) => (
          <button
            key={idx}
            onClick={() => setCurrentIndex(idx)}
            className={`flex-shrink-0 w-20 h-20 rounded-lg overflow-hidden transition-all ${
              idx === currentIndex ? 'ring-2 ring-primary' : 'opacity-70 hover:opacity-100'
            }`}
          >
            <img src={img} alt="" className="w-full h-full object-cover" />
          </button>
        ))}
      </div>

      {lightboxOpen && (
        <div className="fixed inset-0 bg-black/90 z-50 flex items-center justify-center">
          <button
            onClick={(e) => { e.stopPropagation(); setLightboxOpen(false); }}
            className="absolute top-4 right-4 w-10 h-10 bg-white/20 rounded-full flex items-center justify-center text-white hover:bg-white/30"
          >
            <X className="w-6 h-6" />
          </button>
          <button onClick={prevImage} className="absolute left-4 w-12 h-12 bg-white/20 rounded-full flex items-center justify-center text-white hover:bg-white/30">
            <ChevronLeft className="w-8 h-8" />
          </button>
          <img
            src={allImages[currentIndex]}
            alt="Tour"
            className="max-w-[90vw] max-h-[90vh] object-contain"
            onClick={(e) => e.stopPropagation()}
          />
          <button onClick={nextImage} className="absolute right-4 w-12 h-12 bg-white/20 rounded-full flex items-center justify-center text-white hover:bg-white/30">
            <ChevronRight className="w-8 h-8" />
          </button>
          <div className="absolute bottom-4 left-1/2 -translate-x-1/2 flex gap-2">
            {allImages.map((_, idx) => (
              <button
                key={idx}
                onClick={() => setCurrentIndex(idx)}
                className={`w-2 h-2 rounded-full transition-colors ${
                  idx === currentIndex ? 'bg-white' : 'bg-white/50'
                }`}
              />
            ))}
          </div>
        </div>
      )}
    </div>
  );
};

// Timeline Itinerary Component
const ItineraryTimeline = ({ itineraries }) => {
  if (!itineraries?.length) {
    return <div className="text-center py-8 text-gray-500">Chưa có lịch trình chi tiết</div>;
  }

  return (
    <div className="space-y-0">
      {itineraries.map((day, index) => (
        <div key={day.itineraryId || index} className="relative">
          {index < itineraries.length - 1 && (
            <div className="absolute left-[19px] top-10 bottom-0 w-0.5 bg-gray-200" />
          )}
          <div className="flex gap-4 pb-8">
            <div className="flex-shrink-0">
              <div className="w-10 h-10 bg-primary text-white rounded-full flex items-center justify-center font-bold">
                {day.dayNumber || index + 1}
              </div>
            </div>
            <div className="flex-1 bg-white rounded-xl p-5 shadow-md hover:shadow-lg transition-shadow">
              <h4 className="font-bold text-lg text-gray-800 mb-2">
                {day.title || `Ngày ${day.dayNumber || index + 1}`}
              </h4>
              <p className="text-gray-600 mb-4">{day.content}</p>
              {day.activities?.length > 0 && (
                <div className="space-y-2">
                  {day.activities.map((activity, idx) => (
                    <div key={idx} className="flex items-start gap-2 text-sm text-gray-600">
                      <Check className="w-4 h-4 text-green-500 mt-0.5 flex-shrink-0" />
                      <span>{activity}</span>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
        </div>
      ))}
    </div>
  );
};

// Include/Exclude Table
const ServiceTable = ({ includes, excludes }) => {
  return (
    <div className="grid md:grid-cols-2 gap-6">
      <div className="bg-green-50 rounded-xl p-6">
        <h3 className="font-bold text-lg text-green-700 mb-4 flex items-center gap-2">
          <Check className="w-5 h-5" /> Bao gồm
        </h3>
        <ul className="space-y-3">
          {includes?.length > 0 ? (
            includes.map((item, idx) => (
              <li key={idx} className="flex items-start gap-2 text-gray-700">
                <Check className="w-4 h-4 text-green-500 mt-1 flex-shrink-0" />
                <span>{item}</span>
              </li>
            ))
          ) : (
            <li className="text-gray-500 italic">Không có thông tin</li>
          )}
        </ul>
      </div>

      <div className="bg-red-50 rounded-xl p-6">
        <h3 className="font-bold text-lg text-red-700 mb-4 flex items-center gap-2">
          <X className="w-5 h-5" /> Không bao gồm
        </h3>
        <ul className="space-y-3">
          {excludes?.length > 0 ? (
            excludes.map((item, idx) => (
              <li key={idx} className="flex items-start gap-2 text-gray-700">
                <X className="w-4 h-4 text-red-500 mt-1 flex-shrink-0" />
                <span>{item}</span>
              </li>
            ))
          ) : (
            <li className="text-gray-500 italic">Không có thông tin</li>
          )}
        </ul>
      </div>
    </div>
  );
};

// Main Tour Detail Component
const TourDetailPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { compareItems, addToCompare, removeFromCompare } = useCompare();
  
  const [tour, setTour] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [activeTab, setActiveTab] = useState('itinerary');

  useEffect(() => {
    const fetchTour = async () => {
      try {
        setLoading(true);
        const response = await api.get(`/tours/${id}`);
        if (response.data.success) {
          setTour(response.data.data);
        } else {
          setError(response.data.message || 'Tour not found');
        }
      } catch (err) {
        setError('Failed to load tour details');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    if (id) {
      fetchTour();
    }
  }, [id]);

  const isInCompare = tour ? compareItems.some(i => i.tourId === tour.tourId) : false;

  const handleCompare = () => {
    if (isInCompare) {
      removeFromCompare(tour);
    } else {
      addToCompare(tour);
    }
  };

  const discountPercent = tour?.discountPrice 
    ? Math.round((1 - tour.discountPrice / tour.price) * 100) 
    : 0;

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
        <button
          onClick={() => navigate('/info/tours')}
          className="px-6 py-2 bg-primary text-white rounded-full hover:bg-teal-700"
        >
          Quay lại danh sách tour
        </button>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="bg-gradient-to-r from-teal-600 to-cyan-700 text-white py-8">
        <div className="container mx-auto px-4">
          <button
            onClick={() => navigate(-1)}
            className="flex items-center gap-2 text-white/80 hover:text-white mb-4"
          >
            <ChevronLeft className="w-5 h-5" />
            Quay lại
          </button>
          
          <div className="flex flex-wrap gap-2 mb-3">
            {tour.isHotDeal && (
              <span className="px-3 py-1 bg-red-500 text-white text-sm font-bold rounded-full flex items-center gap-1">
                <Flame className="w-4 h-4" /> HOT DEAL
              </span>
            )}
            {discountPercent > 0 && (
              <span className="px-3 py-1 bg-orange-500 text-white text-sm font-bold rounded-full">
                Giảm {discountPercent}%
              </span>
            )}
            {tour.isNewArrival && (
              <span className="px-3 py-1 bg-green-500 text-white text-sm font-bold rounded-full">
                TOUR MỚI
              </span>
            )}
            {tour.isFeatured && (
              <span className="px-3 py-1 bg-yellow-500 text-white text-sm font-bold rounded-full">
                NỔI BẬT
              </span>
            )}
          </div>

          <h1 className="text-3xl md:text-4xl font-bold mb-4">{tour.name}</h1>
          
          <div className="flex flex-wrap gap-4 text-white/90">
            <span className="flex items-center gap-2">
              <MapPin className="w-5 h-5" />
              {tour.destination}
            </span>
            <span className="flex items-center gap-2">
              <Clock className="w-5 h-5" />
              {tour.durationDays} ngày
            </span>
            <span className="flex items-center gap-2">
              <Star className="w-5 h-5 fill-yellow-400 text-yellow-400" />
              {tour.rating?.toFixed(1)} ({tour.reviewCount} reviews)
            </span>
            <span className="flex items-center gap-2">
              <Users className="w-5 h-5" />
              {tour.availableSlots} chỗ còn trống
            </span>
          </div>
        </div>
      </div>

      <div className="container mx-auto px-4 py-8">
        <div className="grid lg:grid-cols-3 gap-8">
          <div className="lg:col-span-2 space-y-8">
            <ImageGallery images={tour.images} />

            <div className="bg-white rounded-2xl shadow-md overflow-hidden">
              <div className="flex border-b">
                <button
                  onClick={() => setActiveTab('itinerary')}
                  className={`flex-1 px-6 py-4 font-medium transition-colors ${
                    activeTab === 'itinerary'
                      ? 'text-primary border-b-2 border-primary'
                      : 'text-gray-500 hover:text-gray-700'
                  }`}
                >
                  Lịch trình
                </button>
                <button
                  onClick={() => setActiveTab('services')}
                  className={`flex-1 px-6 py-4 font-medium transition-colors ${
                    activeTab === 'services'
                      ? 'text-primary border-b-2 border-primary'
                      : 'text-gray-500 hover:text-gray-700'
                  }`}
                >
                  Dịch vụ
                </button>
                <button
                  onClick={() => setActiveTab('reviews')}
                  className={`flex-1 px-6 py-4 font-medium transition-colors ${
                    activeTab === 'reviews'
                      ? 'text-primary border-b-2 border-primary'
                      : 'text-gray-500 hover:text-gray-700'
                  }`}
                >
                  Đánh giá ({tour.reviewCount})
                </button>
              </div>

              <div className="p-6">
                {activeTab === 'itinerary' && (
                  <ItineraryTimeline itineraries={tour.itineraries} />
                )}
                {activeTab === 'services' && (
                  <ServiceTable includes={tour.includes} excludes={tour.excludes} />
                )}
                {activeTab === 'reviews' && (
                  <div className="text-center py-8 text-gray-500">
                    Review feature under development
                  </div>
                )}
              </div>
            </div>

            <div className="bg-white rounded-2xl shadow-md p-6">
              <h2 className="text-xl font-bold mb-4">Giới thiệu tour</h2>
              <p className="text-gray-600 leading-relaxed whitespace-pre-line">
                {tour.description || 'Chưa có mô tả chi tiết'}
              </p>
            </div>

            {tour.departureDates?.length > 0 && (
              <div className="bg-white rounded-2xl shadow-md p-6">
                <h2 className="text-xl font-bold mb-4 flex items-center gap-2">
                  <Calendar className="w-5 h-5 text-primary" />
                  Ngày khởi hành
                </h2>
                <div className="flex flex-wrap gap-2">
                  {tour.departureDates.map((date, idx) => (
                    <span
                      key={idx}
                      className="px-4 py-2 bg-gray-100 rounded-lg text-gray-700"
                    >
                      {date}
                    </span>
                  ))}
                </div>
              </div>
            )}
          </div>

          <div className="lg:col-span-1">
            <div className="bg-white rounded-2xl shadow-lg p-6 sticky top-4">
              <div className="mb-6">
                <div className="flex items-baseline gap-2">
                  {tour.discountPrice ? (
                    <>
                      <span className="text-3xl font-bold text-primary">
                        {tour.discountPrice.toLocaleString()}₫
                      </span>
                      <span className="text-lg text-gray-400 line-through">
                        {tour.price.toLocaleString()}₫
                      </span>
                    </>
                  ) : (
                    <span className="text-3xl font-bold text-primary">
                      {tour.price.toLocaleString()}₫
                    </span>
                  )}
                </div>

                {discountPercent > 0 && (
                  <div className="text-sm text-green-600 font-medium">
                    Tiết kiệm {discountPercent}% - Chỉ còn {tour.discountPrice?.toLocaleString()}₫
                  </div>
                )}
              </div>

              <div className="space-y-3">
                <button
                  className="w-full py-4 bg-primary text-white font-bold rounded-xl hover:bg-teal-700 transition-colors text-lg"
                  onClick={() => navigate(`/booking?tourId=${tour.tourId}&name=${encodeURIComponent(tour.tourName)}&price=${tour.basePrice || tour.price}&discountPrice=${tour.discountPrice || 0}`)}
                >
                  Đặt ngay
                </button>
                
                <div className="flex gap-2">
                  <button
                    onClick={handleCompare}
                    className={`flex-1 py-3 rounded-xl flex items-center justify-center gap-2 transition-colors ${
                      isInCompare
                        ? 'bg-primary text-white'
                        : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                    }`}
                  >
                    <ArrowLeftRight className="w-5 h-5" />
                    {isInCompare ? 'Đã thêm so sánh' : 'So sánh'}
                  </button>
                  
                  <button className="flex-1 py-3 bg-gray-100 text-gray-700 rounded-xl flex items-center justify-center gap-2 hover:bg-gray-200 transition-colors">
                    <Heart className="w-5 h-5" />
                    Yêu thích
                  </button>
                </div>

                <button className="w-full py-3 bg-gray-100 text-gray-700 rounded-xl flex items-center justify-center gap-2 hover:bg-gray-200 transition-colors">
                  <Share2 className="w-5 h-5" />
                  Chia sẻ
                </button>
              </div>

              <div className="mt-6 pt-6 border-t">
                <h4 className="font-medium text-gray-700 mb-3">Liên hệ tư vấn</h4>
                <div className="flex items-center gap-2 text-gray-600">
                  <Phone className="w-5 h-5 text-primary" />
                  <span>1900 xxxx</span>
                </div>
              </div>

              <div className="mt-6 pt-6 border-t space-y-3">
                <div className="flex justify-between text-sm">
                  <span className="text-gray-500">Mã tour</span>
                  <span className="font-medium">{tour.tourId?.slice(0, 8).toUpperCase()}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-gray-500">Thời gian</span>
                  <span className="font-medium">{tour.durationDays} ngày</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-gray-500">Phương tiện</span>
                  <span className="font-medium">Máy bay, xe du lịch</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-gray-500">Lượt xem</span>
                  <span className="font-medium">{tour.reviewCount || 0}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default TourDetailPage;
