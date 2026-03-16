import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { 
  MapPin, Star, Phone, Mail, Calendar, Users, Loader2, 
  ChevronLeft, ChevronRight, Wifi, Car, Coffee, Utensils,
  Waves, Dumbbell, Check, X, Heart, Share2
} from 'lucide-react';
import api from '@/services/api';

// Image Gallery Component
const ImageGallery = ({ images }) => {
  const [currentIndex, setCurrentIndex] = useState(0);
  const [lightboxOpen, setLightboxOpen] = useState(false);

  const allImages = images?.length > 0 
    ? images 
    : ['https://images.unsplash.com/photo-1566073771259-6a8506099945?w=1200'];

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
          alt="Hotel"
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

// Amenities Icon Mapping
const amenityIcons = {
  'Wifi': Wifi,
  'WiFi': Wifi,
  'Parking': Car,
  'Car': Car,
  'Restaurant': Utensils,
  'Coffee': Coffee,
  'Pool': Waves,
  'Swimming': Waves,
  'Gym': Dumbbell,
  'Fitness': Dumbbell,
  'Spa': Dumbbell,
};

// Room Type Card
const RoomCard = ({ room, onBook }) => (
  <div className="border border-gray-200 rounded-xl p-4 hover:shadow-lg transition-shadow">
    <div className="flex justify-between items-start mb-2">
      <div>
        <h4 className="font-bold text-lg">{room.roomType}</h4>
        <div className="flex items-center gap-2 text-gray-500 text-sm">
          <Users className="w-4 h-4" />
          Tối đa {room.maxOccupancy} người
        </div>
      </div>
      <div className="text-right">
        <span className="text-2xl font-bold text-primary">{room.pricePerNight?.toLocaleString() || 'Liên hệ'}₫</span>
        <span className="text-gray-500 text-sm">/đêm</span>
      </div>
    </div>
    <p className="text-gray-600 text-sm mb-4">{room.description}</p>
    <button onClick={() => onBook(room)} className="w-full py-2 bg-primary text-white rounded-lg hover:bg-teal-700 transition-colors">
      Đặt phòng
    </button>
  </div>
);

// Main Hotel Detail Component
const HotelDetailPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  
  const [hotel, setHotel] = useState(null);
  const [rooms, setRooms] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [activeTab, setActiveTab] = useState('rooms');

  useEffect(() => {
    const fetchHotel = async () => {
      try {
        setLoading(true);
        const response = await api.get(`/hotels/${id}`);
        if (response.data.success) {
          setHotel(response.data.data);
          
          // Fetch rooms if available
          try {
            const roomsRes = await api.get(`/hotels/${id}/rooms`);
            if (roomsRes.data.success) {
              setRooms(roomsRes.data.data || []);
            }
          } catch (e) {
            console.log('No rooms available');
          }
        } else {
          setError(response.data.message || 'Hotel not found');
        }
      } catch (err) {
        setError('Failed to load hotel details');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    if (id) fetchHotel();
  }, [id]);

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
        <button onClick={() => navigate('/info/hotels')} className="px-6 py-2 bg-primary text-white rounded-full">
          Quay lại
        </button>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="bg-gradient-to-r from-teal-600 to-cyan-700 text-white py-8">
        <div className="container mx-auto px-4">
          <button onClick={() => navigate(-1)} className="flex items-center gap-2 text-white/80 hover:text-white mb-4">
            <ChevronLeft className="w-5 h-5" /> Quay lại
          </button>
          
          <div className="flex flex-wrap gap-2 mb-3">
            {[...Array(hotel.starRating || 3)].map((_, i) => (
              <Star key={i} className="w-5 h-5 fill-yellow-400 text-yellow-400" />
            ))}
            <span className="ml-2 text-white/80">{hotel.starRating} sao</span>
          </div>

          <h1 className="text-3xl md:text-4xl font-bold mb-4">{hotel.name}</h1>
          
          <div className="flex flex-wrap gap-4 text-white/90">
            <span className="flex items-center gap-2"><MapPin className="w-5 h-5" />{hotel.city}</span>
            <span className="flex items-center gap-2"><Star className="w-5 h-5 fill-yellow-400 text-yellow-400" />{hotel.rating?.toFixed(1)} ({hotel.reviewCount} reviews)</span>
          </div>
        </div>
      </div>

      <div className="container mx-auto px-4 py-8">
        <div className="grid lg:grid-cols-3 gap-8">
          <div className="lg:col-span-2 space-y-8">
            <ImageGallery images={hotel.images} />

            <div className="bg-white rounded-2xl shadow-md p-6">
              <h2 className="text-xl font-bold mb-4">Giới thiệu</h2>
              <p className="text-gray-600 leading-relaxed">{hotel.description || 'No description available'}</p>
            </div>

            <div className="bg-white rounded-2xl shadow-md p-6">
              <h2 className="text-xl font-bold mb-4">Tiện nghi</h2>
              <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
                {hotel.amenities?.map((amenity, idx) => {
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

            {/* Rooms Tab */}
            <div className="bg-white rounded-2xl shadow-md overflow-hidden">
              <div className="flex border-b">
                <button onClick={() => setActiveTab('rooms')} className={`flex-1 px-6 py-4 font-medium ${activeTab === 'rooms' ? 'text-primary border-b-2 border-primary' : 'text-gray-500'}`}>
                  Phòng ({rooms.length})
                </button>
                <button onClick={() => setActiveTab('reviews')} className={`flex-1 px-6 py-4 font-medium ${activeTab === 'reviews' ? 'text-primary border-b-2 border-primary' : 'text-gray-500'}`}>
                  Đánh giá ({hotel.reviewCount})
                </button>
              </div>

              <div className="p-6">
                {activeTab === 'rooms' && (
                  rooms.length > 0 ? (
                    <div className="space-y-4">
                      {rooms.map((room, idx) => (
                        <RoomCard key={idx} room={room} onBook={() => navigate(`/checkout?hotelId=${hotel.hotelId}&roomId=${room.roomId}`)} />
                      ))}
                    </div>
                  ) : (
                    <div className="text-center py-8 text-gray-500">No room information available</div>
                  )
                )}
                {activeTab === 'reviews' && (
                  <div className="text-center py-8 text-gray-500">Review feature under development</div>
                )}
              </div>
            </div>
          </div>

          <div className="lg:col-span-1">
            <div className="bg-white rounded-2xl shadow-lg p-6 sticky top-4">
              <div className="mb-4">
                <span className="text-3xl font-bold text-primary">{hotel.minPrice?.toLocaleString() || 'Liên hệ'}₫</span>
                {hotel.maxPrice && hotel.minPrice !== hotel.maxPrice && (
                  <span className="text-gray-400 ml-2">- {hotel.maxPrice.toLocaleString()}₫</span>
                )}
                <span className="text-gray-500 text-sm">/đêm</span>
              </div>

              <button onClick={() => navigate(`/checkout?hotelId=${hotel.hotelId}`)} className="w-full py-4 bg-primary text-white font-bold rounded-xl hover:bg-teal-700 text-lg mb-3">
                Đặt phòng ngay
              </button>

              <div className="flex gap-2 mb-4">
                <button className="flex-1 py-3 bg-gray-100 rounded-xl flex items-center justify-center gap-2 hover:bg-gray-200">
                  <Heart className="w-5 h-5" /> Yêu thích
                </button>
                <button className="flex-1 py-3 bg-gray-100 rounded-xl flex items-center justify-center gap-2 hover:bg-gray-200">
                  <Share2 className="w-5 h-5" /> Chia sẻ
                </button>
              </div>

              <div className="pt-4 border-t space-y-3">
                <h4 className="font-medium">Liên hệ</h4>
                {hotel.contactPhone && (
                  <div className="flex items-center gap-2 text-gray-600">
                    <Phone className="w-4 h-4" /> {hotel.contactPhone}
                  </div>
                )}
                {hotel.contactEmail && (
                  <div className="flex items-center gap-2 text-gray-600">
                    <Mail className="w-4 h-4" /> {hotel.contactEmail}
                  </div>
                )}
                {hotel.address && (
                  <div className="flex items-center gap-2 text-gray-600">
                    <MapPin className="w-4 h-4" /> {hotel.address}, {hotel.city}
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default HotelDetailPage;
