import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import { getHomeData } from '@/services/homeService';
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
  CheckCircle,
  Mountain,
  Waves,
  Sun,
  Camera,
  Phone,
  Mail,
  Clock,
  Facebook,
  Instagram,
  Youtube,
  MessageCircle
} from 'lucide-react';

// Icon mapping
const iconMap = {
  MapPin,
  Palmtree,
  Building2,
  Utensils,
  Bus,
  Plane,
  CheckCircle,
  Users,
  Star,
  Heart,
  Sun,
  Mountain,
  Waves,
  Phone,
  Mail,
  Clock,
  Facebook,
  Instagram,
  Youtube,
  MessageCircle
};

const HomePage = () => {
  const [homeData, setHomeData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchHomeData = async () => {
      try {
        setLoading(true);
        const response = await getHomeData();
        if (response.success) {
          setHomeData(response.data);
        } else {
          setError(response.message);
        }
      } catch (err) {
        console.error('Error fetching home data:', err);
        setError('Không thể tải dữ liệu. Vui lòng thử lại sau.');
      } finally {
        setLoading(false);
      }
    };

    fetchHomeData();
  }, []);

  // Get icon component from name
  const getIconComponent = (iconName) => {
    const Icon = iconMap[iconName] || MapPin;
    return <Icon />;
  };

  return (
    <div className="min-h-screen bg-white">
      {/* Hero Section */}
      <section className="relative min-h-[600px] flex items-center justify-center overflow-hidden">
        {/* Background Image */}
        <div className="absolute inset-0">
          {loading ? (
            <Skeleton className="w-full h-full" />
          ) : (
            <img
              src={homeData?.banner?.imageUrl || "https://images.unsplash.com/photo-1506929562872-bb421503ef21?w=1920&q=80"}
              alt="Travel"
              className="w-full h-full object-cover"
            />
          )}
          <div className="absolute inset-0 bg-gradient-to-r from-black/70 via-black/50 to-black/30"></div>
        </div>

        {/* Content */}
        <div className="relative z-10 container mx-auto px-4 text-center text-white">
          <div className="max-w-4xl mx-auto">
            {loading ? (
              <>
                <Skeleton className="inline-flex items-center gap-2 px-4 py-2 rounded-full mb-6 w-64 h-10 mx-auto" />
                <Skeleton className="h-16 w-3/4 mx-auto mb-6" />
                <Skeleton className="h-6 w-2/3 mx-auto mb-10" />
              </>
            ) : (
              <>
                <div className="inline-flex items-center gap-2 px-4 py-2 bg-white/20 backdrop-blur-sm rounded-full mb-6">
                  <Plane className="w-4 h-4" />
                  <span className="text-sm font-medium">{homeData?.banner?.title || "Khám phá Việt Nam cùng KarnelTravels"}</span>
                </div>

                <h1 className="text-4xl md:text-6xl font-bold mb-6 leading-tight">
                  {homeData?.banner?.subtitle?.split(' - ')[0] || "Hành trình của bạn"}
                  <span className="block text-teal-400">{homeData?.banner?.subtitle?.split(' - ')[1] || "bắt đầu tại đây"}</span>
                </h1>

                <p className="text-lg md:text-xl text-white/80 mb-10 max-w-2xl mx-auto">
                  {homeData?.banner?.subtitle || "Khám phá những điểm đến tuyệt vời nhất Việt Nam và thế giới. Trải nghiệm dịch vụ chuyên nghiệp, giá cả hợp lý"}
                </p>
              </>
            )}

            {/* Search Box */}
            <div className="bg-white rounded-2xl p-2 md:p-3 shadow-2xl max-w-3xl mx-auto">
              <div className="grid grid-cols-1 md:grid-cols-4 gap-2">
                <div className="md:col-span-2 relative">
                  <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
                  <input
                    type="text"
                    placeholder="Bạn muốn đi đâu?"
                    className="w-full pl-10 pr-4 py-3 rounded-xl bg-gray-50 border-0 focus:outline-none focus:ring-2 focus:ring-primary"
                  />
                </div>
                <div className="relative">
                  <Calendar className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
                  <input
                    type="text"
                    placeholder="Ngày khởi hành"
                    className="w-full pl-10 pr-4 py-3 rounded-xl bg-gray-50 border-0 focus:outline-none focus:ring-2 focus:ring-primary"
                  />
                </div>
                <Button size="lg" className="w-full h-full min-h-[52px]">
                  <Search className="w-5 h-5 mr-2" />
                  Tìm kiếm
                </Button>
              </div>
            </div>
          </div>
        </div>

        {/* Wave Divider */}
        <div className="absolute bottom-0 left-0 right-0">
          <svg viewBox="0 0 1440 120" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M0 120L60 110C120 100 240 80 360 70C480 60 600 60 720 65C840 70 960 80 1080 85C1200 90 1320 90 1380 90L1440 90V120H1380C1320 120 1200 120 1080 120C960 120 840 120 720 120C600 120 480 120 360 120C240 120 120 120 60 120H0Z" fill="white"/>
          </svg>
        </div>
      </section>

      {/* Categories Section */}
      <section className="py-16 bg-white">
        <div className="container mx-auto px-4">
          <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-4">
            {loading ? (
              Array.from({ length: 6 }).map((_, index) => (
                <div key={index} className="p-6 rounded-2xl border border-gray-100">
                  <Skeleton className="w-14 h-14 rounded-xl mx-auto mb-4" />
                  <Skeleton className="h-5 w-20 mx-auto mb-1" />
                  <Skeleton className="h-4 w-16 mx-auto" />
                </div>
              ))
            ) : (
              homeData?.serviceCategories?.map((category, index) => (
                <Link
                  key={category.id || index}
                  to={category.link}
                  className="group p-6 rounded-2xl border border-gray-100 hover:border-teal-200 hover:bg-teal-50/50 transition-all duration-300 text-center"
                >
                  <div 
                    className="w-14 h-14 mx-auto mb-4 rounded-xl flex items-center justify-center group-hover:scale-110 transition-transform"
                    style={{ backgroundColor: `${category.color}20` }}
                  >
                    {getIconComponent(category.icon)}
                    <MapPin 
                      className="w-6 h-6" 
                      style={{ color: category.color }}
                    />
                  </div>
                  <h3 className="font-semibold text-gray-800 mb-1">{category.name}</h3>
                  <p className="text-sm text-muted-foreground">{category.itemCount} địa điểm</p>
                </Link>
              ))
            )}
          </div>
        </div>
      </section>

      {/* Popular Destinations - Featured Spots */}
      <section className="py-20 bg-gray-50">
        <div className="container mx-auto px-4">
          <div className="text-center mb-12">
            {loading ? (
              <>
                <Skeleton className="h-10 w-80 mx-auto mb-4" />
                <Skeleton className="h-6 w-96 mx-auto" />
              </>
            ) : (
              <>
                <h2 className="text-3xl md:text-4xl font-bold text-gray-800 mb-4">
                  Điểm đến phổ biến
                </h2>
                <p className="text-muted-foreground text-lg max-w-2xl mx-auto">
                  Khám phá những địa điểm du lịch hấp dẫn nhất được nhiều du khách lựa chọn
                </p>
              </>
            )}
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            {loading ? (
              Array.from({ length: 4 }).map((_, index) => (
                <Card key={index} className="overflow-hidden border-0 shadow-md">
                  <Skeleton className="h-56 w-full" />
                  <CardContent className="p-4">
                    <Skeleton className="h-6 w-3/4 mb-2" />
                    <Skeleton className="h-4 w-full mb-2" />
                    <Skeleton className="h-4 w-1/2" />
                  </CardContent>
                </Card>
              ))
            ) : (
              homeData?.featuredSpots?.slice(0, 4).map((spot) => (
                <Link key={spot.spotId} to={`/info/tourist-spots/${spot.spotId}`}>
                  <Card className="group overflow-hidden border-0 shadow-md hover:shadow-xl transition-all duration-300">
                    <div className="relative h-56 overflow-hidden">
                      <img
                        src={spot.imageUrl || "https://images.unsplash.com/photo-1506929562872-bb421503ef21?w=800&q=80"}
                        alt={spot.name}
                        className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
                      />
                      <div className="absolute top-3 right-3 px-2 py-1 bg-white/90 backdrop-blur-sm rounded-lg text-sm font-medium text-gray-800">
                        {spot.city || spot.region}
                      </div>
                    </div>
                    <CardContent className="p-4">
                      <h3 className="font-bold text-lg text-gray-800 mb-1">{spot.name}</h3>
                      <p className="text-muted-foreground text-sm mb-3 line-clamp-2">
                        {spot.description || `Địa điểm du lịch ${spot.type}`}
                      </p>
                      <div className="flex items-center gap-2">
                        <div className="flex items-center gap-1 text-sm text-muted-foreground">
                          <Star className="w-4 h-4 fill-yellow-400 text-yellow-400" />
                          <span>{spot.rating.toFixed(1)}</span>
                          <span>({spot.reviewCount})</span>
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                </Link>
              ))
            )}
          </div>

          <div className="text-center mt-10">
            <Button variant="outline" size="lg" asChild>
              <Link to="/info/destinations">
                Xem tất cả điểm đến
                <ArrowRight className="ml-2 w-4 h-4" />
              </Link>
            </Button>
          </div>
        </div>
      </section>

      {/* More Featured Spots - Row 2 */}
      {homeData?.featuredSpots?.length > 4 && (
        <section className="py-10 bg-gray-50">
          <div className="container mx-auto px-4">
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
              {homeData.featuredSpots.slice(4, 8).map((spot) => (
                <Link key={spot.spotId} to={`/info/tourist-spots/${spot.spotId}`}>
                  <Card className="group overflow-hidden border-0 shadow-md hover:shadow-xl transition-all duration-300">
                    <div className="relative h-56 overflow-hidden">
                      <img
                        src={spot.imageUrl || "https://images.unsplash.com/photo-1506929562872-bb421503ef21?w=800&q=80"}
                        alt={spot.name}
                        className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
                      />
                      <div className="absolute top-3 right-3 px-2 py-1 bg-white/90 backdrop-blur-sm rounded-lg text-sm font-medium text-gray-800">
                        {spot.city || spot.region}
                      </div>
                    </div>
                    <CardContent className="p-4">
                      <h3 className="font-bold text-lg text-gray-800 mb-1">{spot.name}</h3>
                      <p className="text-muted-foreground text-sm mb-3 line-clamp-2">
                        {spot.description || `Địa điểm du lịch ${spot.type}`}
                      </p>
                      <div className="flex items-center gap-2">
                        <div className="flex items-center gap-1 text-sm text-muted-foreground">
                          <Star className="w-4 h-4 fill-yellow-400 text-yellow-400" />
                          <span>{spot.rating.toFixed(1)}</span>
                          <span>({spot.reviewCount})</span>
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                </Link>
              ))}
            </div>
          </div>
        </section>
      )}

      {/* About Section */}
      <section className="py-20 bg-white">
        <div className="container mx-auto px-4">
          <div className="grid md:grid-cols-2 gap-12 items-center">
            {loading ? (
              <>
                <Skeleton className="h-96 w-full rounded-2xl" />
                <div>
                  <Skeleton className="h-10 w-3/4 mb-4" />
                  <Skeleton className="h-6 w-full mb-2" />
                  <Skeleton className="h-6 w-full mb-2" />
                  <Skeleton className="h-6 w-2/3 mb-6" />
                  <Skeleton className="h-12 w-40" />
                </div>
              </>
            ) : (
              <>
                <div className="relative">
                  <img
                    src={homeData?.companyInfo?.aboutImage || "https://images.unsplash.com/photo-1469474968028-56623f02e42e?w=800&q=80"}
                    alt="Về Karnel Travels"
                    className="rounded-2xl shadow-xl"
                  />
                  <div className="absolute -bottom-6 -right-6 bg-teal-600 text-white p-6 rounded-xl shadow-lg">
                    <div className="text-4xl font-bold">10+</div>
                    <div className="text-sm">Năm kinh nghiệm</div>
                  </div>
                </div>
                <div>
                  <h2 className="text-3xl md:text-4xl font-bold text-gray-800 mb-4">
                    {homeData?.companyInfo?.aboutTitle || "Về chúng tôi"}
                  </h2>
                  <p className="text-lg text-muted-foreground mb-6">
                    {homeData?.companyInfo?.description || "Karnel Travels tự hào là công ty du lịch và lữ hành hàng đầu Việt Nam."}
                  </p>
                  <ul className="space-y-3 mb-8">
                    {homeData?.companyInfo?.aboutPoints?.map((point, index) => (
                      <li key={index} className="flex items-start gap-3">
                        <CheckCircle className="w-5 h-5 text-teal-600 mt-0.5 flex-shrink-0" />
                        <span className="text-gray-600">{point}</span>
                      </li>
                    ))}
                  </ul>
                  <Button size="lg" asChild>
                    <Link to="/about">
                      Tìm hiểu thêm
                      <ArrowRight className="ml-2 w-4 h-4" />
                    </Link>
                  </Button>
                </div>
              </>
            )}
          </div>
        </div>
      </section>

      {/* Why Choose Us */}
      <section className="py-20 bg-gradient-to-br from-teal-600 to-cyan-700 text-white">
        <div className="container mx-auto px-4">
          <div className="text-center mb-12">
            {loading ? (
              <>
                <Skeleton className="h-10 w-80 mx-auto mb-4" />
                <Skeleton className="h-6 w-96 mx-auto" />
              </>
            ) : (
              <>
                <h2 className="text-3xl md:text-4xl font-bold mb-4">
                  Tại sao chọn KarnelTravels?
                </h2>
                <p className="text-white/80 text-lg max-w-2xl mx-auto">
                  Chúng tôi cam kết mang đến cho bạn trải nghiệm du lịch tốt nhất
                </p>
              </>
            )}
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            {loading ? (
              Array.from({ length: 4 }).map((_, index) => (
                <div key={index} className="text-center p-6">
                  <Skeleton className="w-16 h-16 rounded-2xl mx-auto mb-4" />
                  <Skeleton className="h-6 w-32 mx-auto mb-2" />
                  <Skeleton className="h-4 w-48 mx-auto" />
                </div>
              ))
            ) : (
              homeData?.companyInfo?.features?.map((feature, index) => (
                <div key={index} className="text-center p-6">
                  <div className="w-16 h-16 mx-auto mb-4 bg-white/20 rounded-2xl flex items-center justify-center">
                    {getIconComponent(feature.icon)}
                  </div>
                  <h3 className="font-bold text-xl mb-2">{feature.title}</h3>
                  <p className="text-white/70">{feature.description}</p>
                </div>
              ))
            )}
          </div>
        </div>
      </section>

      {/* Travel Types */}
      <section className="py-20 bg-gray-50">
        <div className="container mx-auto px-4">
          <div className="text-center mb-12">
            <h2 className="text-3xl md:text-4xl font-bold text-gray-800 mb-4">
              Trải nghiệm đa dạng
            </h2>
            <p className="text-muted-foreground text-lg">
              Khám phá nhiều loại hình du lịch phong phú
            </p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <Card className="group overflow-hidden border-0 shadow-md">
              <div className="relative h-64">
                <img
                  src="https://images.unsplash.com/photo-1469474968028-56623f02e42e?w=800&q=80"
                  alt="Biển đảo"
                  className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
                />
                <div className="absolute inset-0 bg-gradient-to-t from-black/70 to-transparent"></div>
                <div className="absolute bottom-4 left-4 right-4 text-white">
                  <Waves className="w-8 h-8 mb-2" />
                  <h3 className="font-bold text-xl">Du lịch biển đảo</h3>
                  <p className="text-white/80 text-sm">Khám phá những bờ biển đẹp nhất</p>
                </div>
              </div>
            </Card>

            <Card className="group overflow-hidden border-0 shadow-md">
              <div className="relative h-64">
                <img
                  src="https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=800&q=80"
                  alt="Núi rừng"
                  className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
                />
                <div className="absolute inset-0 bg-gradient-to-t from-black/70 to-transparent"></div>
                <div className="absolute bottom-4 left-4 right-4 text-white">
                  <Mountain className="w-8 h-8 mb-2" />
                  <h3 className="font-bold text-xl">Du lịch núi rừng</h3>
                  <p className="text-white/80 text-sm">Chinh phục đỉnh cao hùng vĩ</p>
                </div>
              </div>
            </Card>

            <Card className="group overflow-hidden border-0 shadow-md">
              <div className="relative h-64">
                <img
                  src="https://images.unsplash.com/photo-1501785888041-af3ef285b470?w=800&q=80"
                  alt="Văn hóa"
                  className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
                />
                <div className="absolute inset-0 bg-gradient-to-t from-black/70 to-transparent"></div>
                <div className="absolute bottom-4 left-4 right-4 text-white">
                  <Sun className="w-8 h-8 mb-2" />
                  <h3 className="font-bold text-xl">Du lịch văn hóa</h3>
                  <p className="text-white/80 text-sm">Trải nghiệm bản sắc văn hóa</p>
                </div>
              </div>
            </Card>
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-20 bg-white">
        <div className="container mx-auto px-4">
          <Card className="overflow-hidden border-0 shadow-2xl">
            <div className="grid md:grid-cols-2">
              <div className="relative h-64 md:h-auto">
                <img
                  src="https://images.unsplash.com/photo-1469854523086-cc02fe5d8800?w=800&q=80"
                  alt="Bắt đầu hành trình"
                  className="absolute inset-0 w-full h-full object-cover"
                />
              </div>
              <div className="p-8 md:p-12 flex flex-col justify-center">
                <h2 className="text-3xl font-bold text-gray-800 mb-4">
                  Sẵn sàng cho chuyến đi?
                </h2>
                <p className="text-muted-foreground mb-6">
                  Hãy để chúng tôi giúp bạn lên kế hoạch cho chuyến du lịch hoàn hảo.
                  Liên hệ ngay để được tư vấn miễn phí!
                </p>
                <div className="flex gap-4">
                  <Button size="lg" asChild>
                    <Link to="/contact">
                      Liên hệ ngay
                      <ArrowRight className="ml-2 w-4 h-4" />
                    </Link>
                  </Button>
                  <Button variant="outline" size="lg" asChild>
                    <Link to="/search">
                      Tìm kiếm tour
                    </Link>
                  </Button>
                </div>
              </div>
            </div>
          </Card>
        </div>
      </section>

      {/* Error Message */}
      {error && (
        <div className="fixed bottom-4 right-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg shadow-lg">
          <p>{error}</p>
        </div>
      )}
    </div>
  );
};

export default HomePage;
