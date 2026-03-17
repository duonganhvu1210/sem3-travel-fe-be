import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { 
  MapPin, 
  Plane, 
  Bus, 
  Building2, 
  Palmtree, 
  Star, 
  Award, 
  Users, 
  Calendar,
  ArrowRight,
  CheckCircle2,
  Truck,
  Ship,
  Loader2
} from 'lucide-react';
import api from '@/services/api';

const AboutPage = () => {
  const [tours, setTours] = useState([]);
  const [loading, setLoading] = useState(true);
  const [destinations, setDestinations] = useState([]);
  const [destinationsLoading, setDestinationsLoading] = useState(true);

  useEffect(() => {
    fetchTours();
    fetchDestinations();
  }, []);

  const fetchTours = async () => {
    try {
      const response = await api.get('/tours?pageSize=6&sortBy=rating');
      if (response.data.success) {
        setTours(response.data.data || []);
      }
    } catch (error) {
      console.error('Error fetching tours:', error);
    } finally {
      setLoading(false);
    }
  };

  const fetchDestinations = async () => {
    try {
      const response = await api.get('/tour-packages/destinations');
      if (response.data.success) {
        setDestinations(response.data.data || []);
      }
    } catch (error) {
      console.error('Error fetching destinations:', error);
    } finally {
      setDestinationsLoading(false);
    }
  };

  const services = [
    {
      icon: Bus,
      title: 'Luxury Buses',
      description: 'Comfortable air-conditioned coaches for group travel across Vietnam'
    },
    {
      icon: Truck,
      title: 'Limousine Services',
      description: 'Premium limousine fleets for VIP travel experience'
    },
    {
      icon: Plane,
      title: 'Flight Booking',
      description: 'Domestic and international flight reservation services'
    },
    {
      icon: Ship,
      title: 'Cruise Tours',
      description: 'Ha Long Bay and Mekong Delta cruise experiences'
    }
  ];

  const combinedServices = [
    {
      title: 'Transport + Hotel',
      description: 'Complete travel packages including comfortable transportation and quality accommodation'
    },
    {
      title: 'All-Inclusive Tours',
      description: 'Full-service packages covering meals, guides, and activities'
    }
  ];

  const awards = [
    { name: 'Best Travel Agency 2024', issuer: 'Vietnam Tourism Awards' },
    { name: 'Customer Excellence Award', issuer: 'Travel Weekly Asia' },
    { name: 'Sustainable Tourism Certification', issuer: 'Ministry of Culture, Sports and Tourism' }
  ];

  const gallery = [
    'https://images.unsplash.com/photo-1540541338287-41700207dee6?w=400',
    'https://images.unsplash.com/photo-1506929562872-bb421503ef21?w=400',
    'https://images.unsplash.com/photo-1528127269322-539801943592?w=400',
    'https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=400',
    'https://images.unsplash.com/photo-1559827260-dc66d52bef19?w=400',
    'https://images.unsplash.com/photo-1583417319070-4a69db38a482?w=400'
  ];

  const formatPrice = (price) => {
    if (!price) return 'Contact for price';
    return `${price.toLocaleString('vi-VN')}₫`;
  };

  const getDestinationImage = (destination) => {
    const images = {
      'Hanoi': 'https://images.unsplash.com/photo-1528127269322-539801943592?w=400',
      'Ho Chi Minh City': 'https://images.unsplash.com/photo-1583417319070-4a69db38a482?w=400',
      'Da Nang': 'https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=400',
      'Hoi An': 'https://images.unsplash.com/photo-1559827260-dc66d52bef19?w=400',
      'Ha Long': 'https://images.unsplash.com/photo-1540541338287-41700207dee6?w=400',
      'Phu Quoc': 'https://images.unsplash.com/photo-1506929562872-bb421503ef21?w=400',
      'Nha Trang': 'https://images.unsplash.com/photo-1512100356356-de1b84283e18?w=400',
      'Sapa': 'https://images.unsplash.com/photo-1528181304800-259b08848526?w=400',
    };
    return images[destination] || 'https://images.unsplash.com/photo-1506929562872-bb421503ef21?w=400';
  };

  return (
    <div className="min-h-screen bg-white">
      {/* Hero Section */}
      <div className="relative h-[60vh] bg-gradient-to-r from-indigo-900 via-purple-900 to-indigo-900 overflow-hidden">
        <div className="absolute inset-0 bg-black/30"></div>
        <div 
          className="absolute inset-0 bg-cover bg-center"
          style={{ backgroundImage: 'url(https://images.unsplash.com/photo-1506929562872-bb421503ef21?w=1920)' }}
        ></div>
        <div className="relative h-full flex items-center justify-center">
          <div className="text-center text-white px-4">
            <h1 className="text-5xl md:text-6xl font-bold mb-6">About Karnel Travels</h1>
            <p className="text-xl md:text-2xl text-white/90 max-w-3xl mx-auto">
              Your trusted partner for unforgettable journeys across Vietnam
            </p>
          </div>
        </div>
      </div>

      {/* Company History */}
      <section className="py-20 bg-gray-50">
        <div className="container mx-auto px-4">
          <div className="max-w-4xl mx-auto">
            <h2 className="text-4xl font-bold text-gray-900 mb-8 text-center">Our History</h2>
            <div className="bg-white rounded-2xl shadow-lg p-8">
              <p className="text-gray-600 leading-relaxed mb-6">
                Founded in 2015, <strong>Karnel Travels</strong> started with a simple vision: to make travel across Vietnam accessible, comfortable, and memorable for everyone. What began as a small local tour operator has grown into one of the most trusted travel agencies in Vietnam.
              </p>
              <p className="text-gray-600 leading-relaxed mb-6">
                Over the years, we have helped over <strong>500,000 travelers</strong> discover the beauty of Vietnam, from the misty mountains of Sapa to the pristine beaches of Phu Quoc. Our commitment to quality service and customer satisfaction has earned us numerous awards and the trust of millions of travelers.
              </p>
              <div className="flex flex-wrap gap-8 mt-8 pt-8 border-t border-gray-200">
                <div className="text-center flex-1 min-w-[150px]">
                  <div className="text-4xl font-bold text-primary mb-2">10+</div>
                  <div className="text-gray-500">Years Experience</div>
                </div>
                <div className="text-center flex-1 min-w-[150px]">
                  <div className="text-4xl font-bold text-primary mb-2">500K+</div>
                  <div className="text-gray-500">Happy Travelers</div>
                </div>
                <div className="text-center flex-1 min-w-[150px]">
                  <div className="text-4xl font-bold text-primary mb-2">50+</div>
                  <div className="text-gray-500">Destinations</div>
                </div>
                <div className="text-center flex-1 min-w-[150px]">
                  <div className="text-4xl font-bold text-primary mb-2">200+</div>
                  <div className="text-gray-500">Partner Hotels</div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Vision & Mission */}
      <section className="py-20">
        <div className="container mx-auto px-4">
          <div className="grid md:grid-cols-2 gap-12 max-w-5xl mx-auto">
            <div className="bg-gradient-to-br from-primary to-teal-600 rounded-2xl p-8 text-white">
              <div className="w-16 h-16 bg-white/20 rounded-xl flex items-center justify-center mb-6">
                <Plane className="w-8 h-8" />
              </div>
              <h3 className="text-2xl font-bold mb-4">Our Vision</h3>
              <p className="text-white/90 leading-relaxed">
                To become Vietnam's most innovative and trusted travel partner, creating memorable journeys that inspire travelers to explore the rich culture, natural beauty, and warm hospitality of Vietnam.
              </p>
            </div>
            <div className="bg-gradient-to-br from-purple-600 to-indigo-600 rounded-2xl p-8 text-white">
              <div className="w-16 h-16 bg-white/20 rounded-xl flex items-center justify-center mb-6">
                <Users className="w-8 h-8" />
              </div>
              <h3 className="text-2xl font-bold mb-4">Our Mission</h3>
              <ul className="space-y-3 text-white/90">
                <li className="flex items-start gap-3">
                  <CheckCircle2 className="w-5 h-5 mt-1 flex-shrink-0" />
                  <span>Provide exceptional travel experiences at competitive prices</span>
                </li>
                <li className="flex items-start gap-3">
                  <CheckCircle2 className="w-5 h-5 mt-1 flex-shrink-0" />
                  <span>Partner with local communities to promote sustainable tourism</span>
                </li>
                <li className="flex items-start gap-3">
                  <CheckCircle2 className="w-5 h-5 mt-1 flex-shrink-0" />
                  <span>Deliver personalized service with local expertise</span>
                </li>
                <li className="flex items-start gap-3">
                  <CheckCircle2 className="w-5 h-5 mt-1 flex-shrink-0" />
                  <span>Ensure safety and comfort in every journey</span>
                </li>
              </ul>
            </div>
          </div>
        </div>
      </section>

      {/* Transport Services */}
      <section className="py-20 bg-gray-50">
        <div className="container mx-auto px-4">
          <h2 className="text-4xl font-bold text-gray-900 mb-4 text-center">Transport Services</h2>
          <p className="text-gray-600 text-center mb-12 max-w-2xl mx-auto">
            We offer a comprehensive range of transportation options to suit every traveler's needs
          </p>
          <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-6">
            {services.map((service, index) => (
              <div key={index} className="bg-white rounded-xl p-6 shadow-md hover:shadow-xl transition-shadow">
                <div className="w-14 h-14 bg-primary/10 rounded-xl flex items-center justify-center mb-4">
                  <service.icon className="w-7 h-7 text-primary" />
                </div>
                <h3 className="text-xl font-bold text-gray-900 mb-2">{service.title}</h3>
                <p className="text-gray-600 text-sm">{service.description}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Tour Packages - Real Data */}
      <section className="py-20">
        <div className="container mx-auto px-4">
          <h2 className="text-4xl font-bold text-gray-900 mb-4 text-center">Tour Packages</h2>
          <p className="text-gray-600 text-center mb-12 max-w-2xl mx-auto">
            Carefully curated tours that showcase the best of Vietnam
          </p>
          
          {loading ? (
            <div className="flex justify-center py-12">
              <Loader2 className="w-10 h-10 text-primary animate-spin" />
            </div>
          ) : tours.length > 0 ? (
            <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8">
              {tours.map((tour) => (
                <div key={tour.tourId} className="bg-white rounded-xl overflow-hidden shadow-lg hover:shadow-2xl transition-shadow">
                  <div className="relative h-48 overflow-hidden">
                    <img 
                      src={tour.images?.[0] || 'https://images.unsplash.com/photo-1506929562872-bb421503ef21?w=600'} 
                      alt={tour.name}
                      className="w-full h-full object-cover"
                    />
                    {tour.discountPrice && tour.discountPrice < tour.price && (
                      <div className="absolute top-3 right-3 px-3 py-1 bg-red-500 text-white text-sm font-medium rounded-full">
                        Sale
                      </div>
                    )}
                  </div>
                  <div className="p-6">
                    <h3 className="text-xl font-bold text-gray-900 mb-2 line-clamp-1">{tour.name}</h3>
                    <div className="flex items-center gap-2 text-gray-500 text-sm mb-3">
                      <MapPin className="w-4 h-4" />
                      <span>{tour.destination}</span>
                    </div>
                    <div className="flex items-center gap-2 text-gray-500 text-sm mb-4">
                      <Calendar className="w-4 h-4" />
                      <span>{tour.durationDays} Days</span>
                    </div>
                    <div className="flex items-center gap-2 mb-4">
                      <Star className="w-4 h-4 fill-yellow-400 text-yellow-400" />
                      <span className="font-medium">{tour.rating?.toFixed(1) || '0.0'}</span>
                    </div>
                    <div className="flex items-center justify-between">
                      <div>
                        {tour.discountPrice ? (
                          <>
                            <span className="text-2xl font-bold text-primary">{formatPrice(tour.discountPrice)}</span>
                            <span className="ml-2 text-sm text-gray-400 line-through">{formatPrice(tour.price)}</span>
                          </>
                        ) : (
                          <span className="text-2xl font-bold text-primary">{formatPrice(tour.price)}</span>
                        )}
                      </div>
                      <Link 
                        to={`/info/tours/${tour.tourId}`}
                        className="px-4 py-2 bg-primary text-white rounded-lg hover:bg-teal-700 transition-colors"
                      >
                        View Details
                      </Link>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="text-center py-12 text-gray-500">
              <p>No tours available at the moment. Please check back later.</p>
            </div>
          )}
          
          <div className="text-center mt-12">
            <Link 
              to="/info/tours" 
              className="inline-flex items-center gap-2 px-8 py-3 bg-primary text-white rounded-full hover:bg-teal-700 transition-colors"
            >
              View All Tours <ArrowRight className="w-5 h-5" />
            </Link>
          </div>
        </div>
      </section>

      {/* Combined Services */}
      <section className="py-20 bg-gray-50">
        <div className="container mx-auto px-4">
          <h2 className="text-4xl font-bold text-gray-900 mb-4 text-center">Combined Services</h2>
          <p className="text-gray-600 text-center mb-12 max-w-2xl mx-auto">
            Seamless travel packages integrating transport and accommodation
          </p>
          <div className="grid md:grid-cols-2 gap-8 max-w-4xl mx-auto">
            {combinedServices.map((service, index) => (
              <div key={index} className="bg-white rounded-xl p-8 shadow-lg">
                <div className="flex items-start gap-4">
                  <div className="w-12 h-12 bg-primary/10 rounded-xl flex items-center justify-center flex-shrink-0">
                    <Building2 className="w-6 h-6 text-primary" />
                  </div>
                  <div>
                    <h3 className="text-xl font-bold text-gray-900 mb-2">{service.title}</h3>
                    <p className="text-gray-600">{service.description}</p>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Destinations - Real Data */}
      <section className="py-20">
        <div className="container mx-auto px-4">
          <h2 className="text-4xl font-bold text-gray-900 mb-4 text-center">Popular Destinations</h2>
          <p className="text-gray-600 text-center mb-12 max-w-2xl mx-auto">
            Discover the most beautiful destinations across Vietnam
          </p>
          
          {destinationsLoading ? (
            <div className="flex justify-center py-12">
              <Loader2 className="w-10 h-10 text-primary animate-spin" />
            </div>
          ) : destinations.length > 0 ? (
            <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
              {destinations.slice(0, 6).map((dest, index) => (
                <Link 
                  key={dest || index}
                  to={`/info/tours?destination=${encodeURIComponent(dest)}`}
                  className="group relative h-64 rounded-xl overflow-hidden"
                >
                  <img 
                    src={getDestinationImage(dest)} 
                    alt={dest}
                    className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
                  />
                  <div className="absolute inset-0 bg-gradient-to-t from-black/70 to-transparent"></div>
                  <div className="absolute bottom-0 left-0 right-0 p-6">
                    <h3 className="text-2xl font-bold text-white mb-1">{dest}</h3>
                    <span className="text-white/80 text-sm">Explore now</span>
                  </div>
                </Link>
              ))}
            </div>
          ) : (
            <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
              {['Hanoi', 'Ho Chi Minh City', 'Da Nang', 'Hoi An', 'Ha Long', 'Phu Quoc'].map((dest, index) => (
                <Link 
                  key={dest}
                  to={`/info/tours?destination=${encodeURIComponent(dest)}`}
                  className="group relative h-64 rounded-xl overflow-hidden"
                >
                  <img 
                    src={getDestinationImage(dest)} 
                    alt={dest}
                    className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
                  />
                  <div className="absolute inset-0 bg-gradient-to-t from-black/70 to-transparent"></div>
                  <div className="absolute bottom-0 left-0 right-0 p-6">
                    <h3 className="text-2xl font-bold text-white mb-1">{dest}</h3>
                    <span className="text-white/80 text-sm">Explore now</span>
                  </div>
                </Link>
              ))}
            </div>
          )}
          
          <div className="text-center mt-12">
            <Link 
              to="/info/destinations" 
              className="inline-flex items-center gap-2 px-8 py-3 bg-primary text-white rounded-full hover:bg-teal-700 transition-colors"
            >
              Explore All Destinations <ArrowRight className="w-5 h-5" />
            </Link>
          </div>
        </div>
      </section>

      {/* Gallery */}
      <section className="py-20 bg-gray-50">
        <div className="container mx-auto px-4">
          <h2 className="text-4xl font-bold text-gray-900 mb-4 text-center">Our Gallery</h2>
          <p className="text-gray-600 text-center mb-12 max-w-2xl mx-auto">
            Glimpses of unforgettable moments from our travelers' journeys
          </p>
          <div className="grid md:grid-cols-3 gap-4">
            {gallery.map((img, index) => (
              <div key={index} className={`rounded-xl overflow-hidden ${index === 0 || index === 3 ? 'md:row-span-2' : ''}`}>
                <img 
                  src={img} 
                  alt={`Gallery ${index + 1}`}
                  className="w-full h-full object-cover hover:scale-105 transition-transform duration-500"
                />
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Awards & Certifications */}
      <section className="py-20">
        <div className="container mx-auto px-4">
          <h2 className="text-4xl font-bold text-gray-900 mb-4 text-center">Awards & Certifications</h2>
          <p className="text-gray-600 text-center mb-12 max-w-2xl mx-auto">
            Recognition for our commitment to excellence in tourism
          </p>
          <div className="grid md:grid-cols-3 gap-6 max-w-4xl mx-auto">
            {awards.map((award, index) => (
              <div key={index} className="bg-white rounded-xl p-6 shadow-lg text-center border border-gray-100">
                <div className="w-16 h-16 bg-yellow-100 rounded-full flex items-center justify-center mx-auto mb-4">
                  <Award className="w-8 h-8 text-yellow-600" />
                </div>
                <h3 className="text-lg font-bold text-gray-900 mb-2">{award.name}</h3>
                <p className="text-gray-500 text-sm">{award.issuer}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-20 bg-gradient-to-r from-primary to-teal-600">
        <div className="container mx-auto px-4 text-center">
          <h2 className="text-4xl font-bold text-white mb-4">Ready to Explore Vietnam?</h2>
          <p className="text-white/90 text-lg mb-8 max-w-2xl mx-auto">
            Contact us today to plan your perfect trip. Our travel experts are ready to help you create unforgettable memories.
          </p>
          <div className="flex flex-wrap justify-center gap-4">
            <Link 
              to="/contact" 
              className="px-8 py-3 bg-white text-primary font-bold rounded-full hover:bg-gray-100 transition-colors"
            >
              Contact Us
            </Link>
            <Link 
              to="/info/tours" 
              className="px-8 py-3 bg-white/20 text-white font-bold rounded-full hover:bg-white/30 transition-colors"
            >
              Browse Tours
            </Link>
          </div>
        </div>
      </section>
    </div>
  );
};

export default AboutPage;
