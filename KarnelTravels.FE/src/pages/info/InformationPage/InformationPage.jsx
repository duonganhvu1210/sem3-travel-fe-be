import { useState, useEffect } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import api from '@/services/api';
import {
  Plane,
  Search,
  MapPin,
  Palmtree,
  Building2,
  Utensils,
  Bus,
  Star,
  ArrowRight,
  Calendar,
  Users,
  Heart,
  Clock,
  Tag,
  Gift,
  TrendingUp,
  ChevronRight,
  Menu,
  X,
  BookOpen,
  FileText,
  Newspaper,
  Phone,
  ExternalLink
} from 'lucide-react';

const InformationPage = () => {
  const location = useLocation();
  const [promotions, setPromotions] = useState([]);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState('promotions');
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

  useEffect(() => {
    fetchPromotions();
  }, []);

  const fetchPromotions = async () => {
    try {
      const response = await api.get('/promotions');
      if (response.data.success) {
        setPromotions(response.data.data);
      }
    } catch (error) {
      console.error('Error fetching promotions:', error);
    } finally {
      setLoading(false);
    }
  };

  // Menu items for Information page
  const infoMenuItems = [
    { id: 'promotions', name: 'Khuyến mãi', icon: Gift, path: '/info', count: promotions.length },
    { id: 'handbook', name: 'Cẩm nang du lịch', icon: BookOpen, path: '/info/handbook', count: 0 },
    { id: 'policy', name: 'Chính sách', icon: FileText, path: '/info/policy', count: 0 },
    { id: 'guide', name: 'Hướng dẫn', icon: FileText, path: '/info/guide', count: 0 },
    { id: 'news', name: 'Tin tức', icon: Newspaper, path: '/info/news', count: 0 },
    { id: 'contact', name: 'Liên hệ', icon: Phone, path: '/contact', count: 0 },
  ];

  // Calculate time remaining for promotion
  const getTimeRemaining = (endDate) => {
    const end = new Date(endDate);
    const now = new Date();
    const diff = end - now;

    if (diff <= 0) return { expired: true };

    const days = Math.floor(diff / (1000 * 60 * 60 * 24));
    const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
    const seconds = Math.floor((diff % (1000 * 60)) / 1000);

    return { days, hours, minutes, seconds, expired: false };
  };

  // Countdown Timer Component
  const CountdownTimer = ({ endDate }) => {
    const [timeLeft, setTimeLeft] = useState(getTimeRemaining(endDate));

    useEffect(() => {
      const timer = setInterval(() => {
        setTimeLeft(getTimeRemaining(endDate));
      }, 1000);

      return () => clearInterval(timer);
    }, [endDate]);

    if (timeLeft.expired) {
      return (
        <span className="text-red-500 text-sm font-medium">Đã hết hạn</span>
      );
    }

    return (
      <div className="flex items-center gap-2">
        <div className="flex items-center gap-1 bg-red-500 text-white px-2 py-1 rounded-lg">
          <Clock className="w-4 h-4" />
          <span className="font-bold">{timeLeft.days}</span>
          <span className="text-xs">ngày</span>
        </div>
        <div className="flex items-center gap-1 bg-red-500/80 text-white px-2 py-1 rounded-lg">
          <span className="font-bold">{timeLeft.hours.toString().padStart(2, '0')}</span>
          <span className="text-xs">:</span>
          <span className="font-bold">{timeLeft.minutes.toString().padStart(2, '0')}</span>
          <span className="text-xs">:</span>
          <span className="font-bold">{timeLeft.seconds.toString().padStart(2, '0')}</span>
        </div>
      </div>
    );
  };

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Hero Banner */}
      <div className="bg-gradient-to-r from-teal-600 to-cyan-700 text-white py-16">
        <div className="container mx-auto px-4">
          <h1 className="text-4xl font-bold mb-4">Thông tin & Khuyến mãi</h1>
          <p className="text-xl text-white/80 max-w-2xl">
            Cập nhật những khuyến mãi hấp dẫn và thông tin hữu ích cho chuyến du lịch của bạn
          </p>
        </div>
      </div>

      <div className="container mx-auto px-4 py-8">
        <div className="flex flex-col lg:flex-row gap-8">
          {/* Sidebar Menu */}
          <aside className="lg:w-1/4">
            {/* Mobile Menu Toggle */}
            <button
              className="lg:hidden w-full flex items-center justify-between px-4 py-3 bg-white rounded-xl shadow-sm mb-4"
              onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
            >
              <span className="font-medium">Menu</span>
              {mobileMenuOpen ? <X className="w-5 h-5" /> : <Menu className="w-5 h-5" />}
            </button>

            <div className={`lg:block ${mobileMenuOpen ? 'block' : 'hidden'}`}>
              <Card className="border-0 shadow-lg">
                <CardContent className="p-2">
                  {infoMenuItems.map((item) => (
                    <Link
                      key={item.id}
                      to={item.path}
                      className={`flex items-center gap-3 px-4 py-3 rounded-xl transition-all ${
                        activeTab === item.id
                          ? 'bg-teal-50 text-teal-600'
                          : 'text-gray-600 hover:bg-gray-50'
                      }`}
                      onClick={() => setActiveTab(item.id)}
                    >
                      <item.icon className="w-5 h-5" />
                      <span className="font-medium">{item.name}</span>
                      {item.count > 0 && (
                        <span className="ml-auto bg-teal-500 text-white text-xs px-2 py-0.5 rounded-full">
                          {item.count}
                        </span>
                      )}
                      <ChevronRight className="w-4 h-4 ml-auto opacity-50" />
                    </Link>
                  ))}
                </CardContent>
              </Card>
            </div>
          </aside>

          {/* Main Content */}
          <main className="lg:w-3/4">
            {/* Promotions Section */}
            <div className="mb-8">
              <div className="flex items-center gap-3 mb-6">
                <div className="w-12 h-12 bg-gradient-to-br from-red-500 to-orange-500 rounded-xl flex items-center justify-center">
                  <Gift className="w-6 h-6 text-white" />
                </div>
                <div>
                  <h2 className="text-2xl font-bold text-gray-800">Khuyến mãi hot</h2>
                  <p className="text-gray-500">Những ưu đãi hấp dẫn đang chờ bạn</p>
                </div>
              </div>

              {loading ? (
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                  {Array.from({ length: 4 }).map((_, index) => (
                    <Card key={index} className="overflow-hidden">
                      <Skeleton className="h-40 w-full" />
                      <CardContent className="p-4">
                        <Skeleton className="h-6 w-3/4 mb-2" />
                        <Skeleton className="h-4 w-full mb-2" />
                        <Skeleton className="h-4 w-1/2" />
                      </CardContent>
                    </Card>
                  ))}
                </div>
              ) : promotions.length > 0 ? (
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                  {promotions.map((promo) => (
                    <Card key={promo.promoId} className="overflow-hidden border-0 shadow-lg hover:shadow-xl transition-shadow">
                      <div className="relative h-40 bg-gradient-to-br from-teal-500 to-cyan-600 flex items-center justify-center">
                        <div className="absolute top-0 right-0 bg-red-500 text-white px-4 py-2 rounded-bl-xl font-bold">
                          {promo.discountType === 'Percentage' ? `${promo.discountValue}%` : `${promo.discountValue.toLocaleString()}₫`}
                        </div>
                        <Tag className="w-16 h-16 text-white/30" />
                      </div>
                      <CardContent className="p-4">
                        <div className="flex items-center gap-2 mb-2">
                          <span className="bg-teal-100 text-teal-700 px-2 py-1 rounded text-xs font-medium">
                            {promo.code}
                          </span>
                          <CountdownTimer endDate={promo.endDate} />
                        </div>
                        <h3 className="font-bold text-lg text-gray-800 mb-2">{promo.title}</h3>
                        <p className="text-gray-500 text-sm mb-4 line-clamp-2">{promo.description}</p>
                        <div className="flex items-center justify-between">
                          <span className="text-xs text-gray-400">
                            Hết hạn: {new Date(promo.endDate).toLocaleDateString('vi-VN')}
                          </span>
                          <Button size="sm" asChild>
                            <Link to="/search">
                              Sử dụng ngay
                              <ArrowRight className="w-4 h-4 ml-1" />
                            </Link>
                          </Button>
                        </div>
                      </CardContent>
                    </Card>
                  ))}
                </div>
              ) : (
                <Card className="border-0 shadow-lg">
                  <CardContent className="p-12 text-center">
                    <Gift className="w-16 h-16 text-gray-300 mx-auto mb-4" />
                    <h3 className="text-xl font-semibold text-gray-600 mb-2">Không có khuyến mãi nào</h3>
                    <p className="text-gray-400">Hiện tại không có khuyến mãi nào đang hoạt động</p>
                  </CardContent>
                </Card>
              )}
            </div>

            {/* Quick Links Section */}
            <div>
              <div className="flex items-center gap-3 mb-6">
                <div className="w-12 h-12 bg-gradient-to-br from-blue-500 to-indigo-600 rounded-xl flex items-center justify-center">
                  <TrendingUp className="w-6 h-6 text-white" />
                </div>
                <div>
                  <h2 className="text-2xl font-bold text-gray-800">Liên kết nhanh</h2>
                  <p className="text-gray-500">Truy cập nhanh các trang thông tin</p>
                </div>
              </div>

              <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
                <Link to="/info/destinations">
                  <Card className="border-0 shadow-md hover:shadow-lg transition-all group cursor-pointer">
                    <CardContent className="p-6 text-center">
                      <div className="w-14 h-14 bg-teal-100 rounded-xl flex items-center justify-center mx-auto mb-3 group-hover:scale-110 transition-transform">
                        <MapPin className="w-7 h-7 text-teal-600" />
                      </div>
                      <h3 className="font-semibold text-gray-800">Điểm du lịch</h3>
                      <p className="text-sm text-gray-500 mt-1">Khám phá ngay</p>
                    </CardContent>
                  </Card>
                </Link>

                <Link to="/info/tours">
                  <Card className="border-0 shadow-md hover:shadow-lg transition-all group cursor-pointer">
                    <CardContent className="p-6 text-center">
                      <div className="w-14 h-14 bg-cyan-100 rounded-xl flex items-center justify-center mx-auto mb-3 group-hover:scale-110 transition-transform">
                        <Palmtree className="w-7 h-7 text-cyan-600" />
                      </div>
                      <h3 className="font-semibold text-gray-800">Tour du lịch</h3>
                      <p className="text-sm text-gray-500 mt-1">Xem chi tiết</p>
                    </CardContent>
                  </Card>
                </Link>

                <Link to="/info/hotels">
                  <Card className="border-0 shadow-md hover:shadow-lg transition-all group cursor-pointer">
                    <CardContent className="p-6 text-center">
                      <div className="w-14 h-14 bg-indigo-100 rounded-xl flex items-center justify-center mx-auto mb-3 group-hover:scale-110 transition-transform">
                        <Building2 className="w-7 h-7 text-indigo-600" />
                      </div>
                      <h3 className="font-semibold text-gray-800">Khách sạn</h3>
                      <p className="text-sm text-gray-500 mt-1">Đặt ngay</p>
                    </CardContent>
                  </Card>
                </Link>

                <Link to="/info/restaurants">
                  <Card className="border-0 shadow-md hover:shadow-lg transition-all group cursor-pointer">
                    <CardContent className="p-6 text-center">
                      <div className="w-14 h-14 bg-amber-100 rounded-xl flex items-center justify-center mx-auto mb-3 group-hover:scale-110 transition-transform">
                        <Utensils className="w-7 h-7 text-amber-600" />
                      </div>
                      <h3 className="font-semibold text-gray-800">Nhà hàng</h3>
                      <p className="text-sm text-gray-500 mt-1">Thưởng thức</p>
                    </CardContent>
                  </Card>
                </Link>

                <Link to="/info/resorts">
                  <Card className="border-0 shadow-md hover:shadow-lg transition-all group cursor-pointer">
                    <CardContent className="p-6 text-center">
                      <div className="w-14 h-14 bg-pink-100 rounded-xl flex items-center justify-center mx-auto mb-3 group-hover:scale-110 transition-transform">
                        <Plane className="w-7 h-7 text-pink-600" />
                      </div>
                      <h3 className="font-semibold text-gray-800">Resort</h3>
                      <p className="text-sm text-gray-500 mt-1">Nghỉ dưỡng</p>
                    </CardContent>
                  </Card>
                </Link>

                <Link to="/info/transports">
                  <Card className="border-0 shadow-md hover:shadow-lg transition-all group cursor-pointer">
                    <CardContent className="p-6 text-center">
                      <div className="w-14 h-14 bg-purple-100 rounded-xl flex items-center justify-center mx-auto mb-3 group-hover:scale-110 transition-transform">
                        <Bus className="w-7 h-7 text-purple-600" />
                      </div>
                      <h3 className="font-semibold text-gray-800">Vận chuyển</h3>
                      <p className="text-sm text-gray-500 mt-1">Di chuyển</p>
                    </CardContent>
                  </Card>
                </Link>
              </div>
            </div>
          </main>
        </div>
      </div>
    </div>
  );
};

export default InformationPage;
