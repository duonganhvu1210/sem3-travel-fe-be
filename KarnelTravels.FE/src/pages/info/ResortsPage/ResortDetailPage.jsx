import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { 
  MapPin, Star, Phone, Mail, Calendar, Users, Loader2, 
  ChevronLeft, ChevronRight, Wifi, Car, Coffee, Utensils,
  Waves, Dumbbell, Check, Heart, Share2, Palmtree
} from 'lucide-react';
import api from '@/services/api';

// Image Gallery Component
const ImageGallery = ({ images }) => {
  const [currentIndex, setCurrentIndex] = useState(0);
  const [lightboxOpen, setLightboxOpen] = useState(false);

  const allImages = images?.length > 0 
    ? images 
    : ['https://images.unsplash.com/photo-1582719508461-905c673771fd?w=1200'];

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
          alt="Resort"
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
  'Beach': Palmtree,
};

// Package Card
const PackageCard = ({ pkg, onBook }) => (
  <div className="border border-gray-200 rounded-xl p-4 hover:shadow-lg transition-shadow">
    <div className="flex justify-between items-start mb-2">
      <div>
        <h4 className="font-bold text-lg">{pkg.packageName}</h4>
        <div className="flex items-center gap-2 text-gray-500 text-sm">
          <Calendar className="w-4 h-4" />
          {pkg.durationDays} ngày
        </div>
      </div>
      <div className="text-right">
        <span className="text-2xl font-bold text-primary">{pkg.price?.toLocaleString() || 'Liên hệ'}₫</span>
      </div>
    </div>
    <p className="text-gray-600 text-sm mb-4">{pkg.description}</p>
    <button onClick={() => onBook(pkg)} className="w-full py-2 bg-primary text-white rounded-lg hover:bg-teal-700 transition-colors">
      Đặt ngay
    </button>
  </div>
);

// Main Resort Detail Component
const ResortDetailPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  
  const [resort, setResort] = useState(null);
  const [packages, setPackages] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [activeTab, setActiveTab] = useState('packages');

  useEffect(() => {
    const fetchResort = async () => {
      try {
        setLoading(true);
        const response = await api.get(`/resorts/${id}`);
        if (response.data.success) {
          setResort(response.data.data);
          
          // Fetch packages if available
          try {
            const packagesRes = await api.get(`/resorts/${id}/packages`);
            if (packagesRes.data.success) {
              setPackages(packagesRes.data.data || []);
            }
          } catch (e) {
            console.log('No packages available');
          }
        } else {
          setError(response.data.message || 'Resort not found');
        }
      } catch (err) {
        setError('Failed to load resort details');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    if (id) fetchResort();
  }, [id]);

  const handleBookPackage = (pkg) => {
    navigate(`/booking?item=${id}&type=resort&name=${encodeURIComponent(resort.name)}&price=${pkg.price}&discountPrice=${pkg.discountPrice || pkg.price}`);
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
        <button onClick={() => navigate('/info/resorts')} className="px-6 py-2 bg-primary text-white rounded-full">
          Quay lại
        </button>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="bg-gradient-to-r from-emerald-600 to-teal-700 text-white py-8">
        <div className="container mx-auto px-4">
          <button onClick={() => navigate(-1)} className="flex items-center gap-2 text-white/80 hover:text-white mb-4">
            <ChevronLeft className="w-5 h-5" /> Quay lại
          </button>
          
          <div className="flex flex-wrap gap-2 mb-3">
            {[...Array(resort.starRating || 4)].map((_, i) => (
              <Star key={i} className="w-5 h-5 fill-yellow-400 text-yellow-400" />
            ))}
            <span className="ml-2 text-white/80">{resort.starRating} sao</span>
          </div>

          <h1 className="text-3xl md:text-4xl font-bold mb-4">{resort.name}</h1>
          
          <div className="flex flex-wrap gap-4 text-white/90">
            <span className="flex items-center gap-2"><MapPin className="w-5 h-5" />{resort.city}</span>
            <span className="flex items-center gap-2"><Star className="w-5 h-5 fill-yellow-400 text-yellow-400" />{resort.rating?.toFixed(1)} ({resort.reviewCount} reviews)</span>
          </div>
        </div>
      </div>

      <div className="container mx-auto px-4 py-8">
        <div className="grid lg:grid-cols-3 gap-8">
          <div className="lg:col-span-2 space-y-8">
            <ImageGallery images={resort.images} />

            <div className="bg-white rounded-2xl shadow-md p-6">
              <h2 className="text-xl font-bold mb-4">Giới thiệu</h2>
              <p className="text-gray-600 leading-relaxed">{resort.description || 'Chưa có mô tả chi tiết'}</p>
            </div>

            <div className="bg-white rounded-2xl shadow-md p-6">
              <h2 className="text-xl font-bold mb-4">Tiện nghi</h2>
              <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
                {resort.amenities?.map((amenity, idx) => {
                  const Icon = amenityIcons[amenity] || Check;
                  return (
                    <div key={idx} className="flex items-center gap-2 text-gray-700">
                      <Icon className="w-5 h-5 text-primary" />
                      <span>{amenity}</span>
                    </div>
                  );
                })}
                {!resort.amenities?.length && (
                  <p className="text-gray-500">Chưa có thông tin tiện nghi</p>
                )}
              </div>
            </div>

            <div className="bg-white rounded-2xl shadow-md p-6">
              <h2 className="text-xl font-bold mb-4">Vị trí</h2>
              <div className="flex items-start gap-3 text-gray-700">
                <MapPin className="w-5 h-5 text-primary mt-1" />
                <div>
                  <p>{resort.address}</p>
                  <p>{resort.city}</p>
                </div>
              </div>
            </div>
          </div>

          <div className="space-y-6">
            <div className="bg-white rounded-2xl shadow-md p-6 sticky top-4">
              <h3 className="text-lg font-bold mb-4">Liên hệ đặt phòng</h3>
              
              <div className="space-y-3 mb-6">
                {resort.phone && (
                  <div className="flex items-center gap-3 text-gray-600">
                    <Phone className="w-5 h-5 text-primary" />
                    <span>{resort.phone}</span>
                  </div>
                )}
                {resort.email && (
                  <div className="flex items-center gap-3 text-gray-600">
                    <Mail className="w-5 h-5 text-primary" />
                    <span>{resort.email}</span>
                  </div>
                )}
              </div>

              <button 
                onClick={() => navigate(`/booking?item=${id}&type=resort&name=${encodeURIComponent(resort.name)}&price=${resort.pricePerNight || 0}&discountPrice=${resort.discountPrice || resort.pricePerNight || 0}`)}
                className="w-full py-3 bg-primary text-white font-bold rounded-xl hover:bg-teal-700 transition-colors"
              >
                Đặt ngay
              </button>
              
              <div className="flex gap-2 mt-3">
                <button className="flex-1 py-3 bg-gray-100 text-gray-700 rounded-xl flex items-center justify-center gap-2 hover:bg-gray-200 transition-colors">
                  <Heart className="w-5 h-5" />
                  Yêu thích
                </button>
                <button className="flex-1 py-3 bg-gray-100 text-gray-700 rounded-xl flex items-center justify-center gap-2 hover:bg-gray-200 transition-colors">
                  <Share2 className="w-5 h-5" />
                  Chia sẻ
                </button>
              </div>
            </div>

            {packages.length > 0 && (
              <div className="bg-white rounded-2xl shadow-md p-6">
                <h3 className="text-lg font-bold mb-4">Gói dịch vụ</h3>
                <div className="space-y-4">
                  {packages.slice(0, 3).map((pkg) => (
                    <PackageCard key={pkg.packageId || pkg.id} pkg={pkg} onBook={handleBookPackage} />
                  ))}
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default ResortDetailPage;
