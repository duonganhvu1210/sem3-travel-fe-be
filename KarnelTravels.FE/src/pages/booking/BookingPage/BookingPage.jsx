import { useState, useEffect } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import { 
  Calendar, Users, Bed, CreditCard, CheckCircle, Loader2,
  ArrowLeft, ArrowRight, Gift, Info, AlertCircle
} from 'lucide-react';
import { useBooking } from '@/context/BookingContext';
import api from '@/services/api';

// Service type labels
const serviceLabels = {
  tour: 'Tour du lịch',
  hotel: 'Khách sạn',
  resort: 'Resort',
  transport: 'Vận chuyển',
  restaurant: 'Nhà hàng'
};

// Step Components
const StepIndicator = ({ currentStep, steps }) => (
  <div className="flex items-center justify-center mb-8">
    {steps.map((step, index) => (
      <div key={index} className="flex items-center">
        <div className={`flex items-center justify-center w-10 h-10 rounded-full font-medium ${
          currentStep > index + 1 
            ? 'bg-green-500 text-white' 
            : currentStep === index + 1 
              ? 'bg-primary text-white' 
              : 'bg-gray-200 text-gray-500'
        }`}>
          {currentStep > index + 1 ? <CheckCircle className="w-5 h-5" /> : index + 1}
        </div>
        <span className={`ml-2 text-sm hidden sm:inline ${
          currentStep === index + 1 ? 'text-primary font-medium' : 'text-gray-500'
        }`}>{step}</span>
        {index < steps.length - 1 && (
          <div className={`w-12 h-0.5 mx-2 ${currentStep > index + 1 ? 'bg-green-500' : 'bg-gray-200'}`} />
        )}
      </div>
    ))}
  </div>
);

// Step 1: Service Configuration
const ServiceConfigStep = ({ serviceInfo, setServiceInfo, onNext }) => {
  const [error, setError] = useState('');

  const handleSubmit = () => {
    if (!serviceInfo.serviceDate) {
      setError('Vui lòng chọn ngày');
      return;
    }
    if (serviceInfo.quantity < 1) {
      setError('Số lượng phải lớn hơn 0');
      return;
    }
    setError('');
    onNext();
  };

  return (
    <div className="space-y-6">
      <h2 className="text-xl font-bold">Cấu hình dịch vụ</h2>
      
      <div className="bg-gray-50 p-4 rounded-xl">
        <p className="font-medium">{serviceLabels[serviceInfo.serviceType] || 'Dịch vụ'}</p>
        <p className="text-gray-600">{serviceInfo.serviceName}</p>
        {serviceInfo.price && (
          <p className="text-primary font-bold mt-2">{serviceInfo.price.toLocaleString()}đ</p>
        )}
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          <Calendar className="w-4 h-4 inline mr-2" />
          Ngày {serviceInfo.serviceType === 'hotel' || serviceInfo.serviceType === 'resort' ? 'nhận phòng' : 'sử dụng'}
        </label>
        <input
          type="date"
          value={serviceInfo.serviceDate || ''}
          onChange={(e) => setServiceInfo({ ...serviceInfo, serviceDate: e.target.value })}
          min={new Date().toISOString().split('T')[0]}
          className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary"
        />
      </div>

      {serviceInfo.serviceType === 'hotel' || serviceInfo.serviceType === 'resort' ? (
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            <Calendar className="w-4 h-4 inline mr-2" />
            Ngày trả phòng
          </label>
          <input
            type="date"
            value={serviceInfo.endDate || ''}
            onChange={(e) => setServiceInfo({ ...serviceInfo, endDate: e.target.value })}
            min={serviceInfo.serviceDate || new Date().toISOString().split('T')[0]}
            className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary"
          />
        </div>
      ) : null}

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          <Users className="w-4 h-4 inline mr-2" />
          Số lượng {serviceInfo.serviceType === 'hotel' || serviceInfo.serviceType === 'resort' ? 'phòng' : 'người'}
        </label>
        <input
          type="number"
          value={serviceInfo.quantity || 1}
          onChange={(e) => setServiceInfo({ ...serviceInfo, quantity: parseInt(e.target.value) || 1 })}
          min="1"
          max={serviceInfo.maxQuantity || 10}
          className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary"
        />
      </div>

      {serviceInfo.serviceType === 'hotel' && (
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            <Bed className="w-4 h-4 inline mr-2" />
            Loại phòng
          </label>
          <select
            value={serviceInfo.roomType || ''}
            onChange={(e) => setServiceInfo({ ...serviceInfo, roomType: e.target.value })}
            className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary"
          >
            <option value="">Chọn loại phòng</option>
            <option value="standard">Phòng Standard</option>
            <option value="deluxe">Phòng Deluxe</option>
            <option value="suite">Phòng Suite</option>
            <option value="family">Phòng Family</option>
          </select>
        </div>
      )}

      {error && (
        <div className="flex items-center gap-2 text-red-500">
          <AlertCircle className="w-5 h-5" />
          {error}
        </div>
      )}

      <button
        onClick={handleSubmit}
        className="w-full py-4 bg-primary text-white font-bold rounded-xl hover:bg-teal-700 flex items-center justify-center gap-2"
      >
        Tiếp tục <ArrowRight className="w-5 h-5" />
      </button>
    </div>
  );
};

// Step 2: Review & Promotion
const ReviewStep = ({ serviceInfo, pricing, setPricing, onNext, onPrev }) => {
  const [couponCode, setCouponCode] = useState('');
  const [couponLoading, setCouponLoading] = useState(false);
  const [couponError, setCouponError] = useState('');
  const [couponApplied, setCouponApplied] = useState(false);

  useEffect(() => {
    calculatePrice();
  }, [serviceInfo]);

  const calculatePrice = async () => {
    try {
      const nights = serviceInfo.endDate && serviceInfo.serviceDate
        ? Math.ceil((new Date(serviceInfo.endDate) - new Date(serviceInfo.serviceDate)) / (1000 * 60 * 60 * 24))
        : 1;

      const response = await api.post('/bookings/calculate-price', {
        serviceType: serviceInfo.serviceType,
        serviceId: serviceInfo.serviceId,
        quantity: serviceInfo.quantity,
        serviceDate: serviceInfo.serviceDate,
        endDate: serviceInfo.endDate,
        couponCode: pricing.couponCode
      });

      if (response.data.success) {
        setPricing(prev => ({
          ...prev,
          totalAmount: response.data.data.totalAmount,
          discountAmount: response.data.data.discountAmount,
          finalAmount: response.data.data.finalAmount
        }));
      }
    } catch (error) {
      console.error('Price calculation error:', error);
    }
  };

  const applyCoupon = async () => {
    if (!couponCode.trim()) return;
    
    setCouponLoading(true);
    setCouponError('');

    try {
      const response = await api.get(`/bookings/validate-coupon?code=${couponCode}&orderAmount=${pricing.totalAmount}`);
      
      if (response.data.success) {
        setPricing(prev => ({
          ...prev,
          couponCode: couponCode,
          couponData: response.data.data
        }));
        setCouponApplied(true);
        calculatePrice();
      } else {
        setCouponError(response.data.message || 'Mã giảm giá không hợp lệ');
      }
    } catch (error) {
      setCouponError('Không thể xác thực mã giảm giá');
    } finally {
      setCouponLoading(false);
    }
  };

  const nights = serviceInfo.endDate && serviceInfo.serviceDate
    ? Math.ceil((new Date(serviceInfo.endDate) - new Date(serviceInfo.serviceDate)) / (1000 * 60 * 60 * 24))
    : 1;

  return (
    <div className="space-y-6">
      <h2 className="text-xl font-bold">Xác nhận thông tin</h2>

      {/* Service Info Summary */}
      <div className="bg-gray-50 p-4 rounded-xl space-y-2">
        <div className="flex justify-between">
          <span className="text-gray-600">Dịch vụ</span>
          <span className="font-medium">{serviceLabels[serviceInfo.serviceType]}</span>
        </div>
        <div className="flex justify-between">
          <span className="text-gray-600">Tên</span>
          <span className="font-medium">{serviceInfo.serviceName}</span>
        </div>
        <div className="flex justify-between">
          <span className="text-gray-600">Ngày</span>
          <span className="font-medium">
            {serviceInfo.serviceDate && new Date(serviceInfo.serviceDate).toLocaleDateString('vi-VN')}
            {serviceInfo.endDate && ` - ${new Date(serviceInfo.endDate).toLocaleDateString('vi-VN')}`}
            {nights > 1 && ` (${nights} đêm)`}
          </span>
        </div>
        <div className="flex justify-between">
          <span className="text-gray-600">Số lượng</span>
          <span className="font-medium">{serviceInfo.quantity}</span>
        </div>
      </div>

      {/* Coupon */}
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          <Gift className="w-4 h-4 inline mr-2" />
          Mã giảm giá
        </label>
        <div className="flex gap-2">
          <input
            type="text"
            value={couponCode}
            onChange={(e) => {
              setCouponCode(e.target.value);
              setCouponApplied(false);
            }}
            placeholder="Nhập mã giảm giá"
            disabled={couponApplied}
            className="flex-1 px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary"
          />
          <button
            onClick={applyCoupon}
            disabled={couponLoading || couponApplied}
            className="px-4 py-3 bg-gray-800 text-white rounded-xl hover:bg-gray-900 disabled:bg-gray-400"
          >
            {couponLoading ? <Loader2 className="w-5 h-5 animate-spin" /> : 'Áp dụng'}
          </button>
        </div>
        {couponError && <p className="text-red-500 text-sm mt-1">{couponError}</p>}
        {couponApplied && pricing.couponData && (
          <p className="text-green-500 text-sm mt-1 flex items-center gap-1">
            <CheckCircle className="w-4 h-4" />
            Đã áp dụng: {pricing.couponData.title}
          </p>
        )}
      </div>

      {/* Pricing Summary */}
      <div className="bg-gray-50 p-4 rounded-xl space-y-2">
        <div className="flex justify-between">
          <span className="text-gray-600">Tổng tiền</span>
          <span>{pricing.totalAmount.toLocaleString()}đ</span>
        </div>
        {pricing.discountAmount > 0 && (
          <div className="flex justify-between text-green-500">
            <span>Giảm giá</span>
            <span>-{pricing.discountAmount.toLocaleString()}đ</span>
          </div>
        )}
        <div className="flex justify-between font-bold text-lg border-t pt-2">
          <span>Thành tiền</span>
          <span className="text-primary">{pricing.finalAmount.toLocaleString()}đ</span>
        </div>
      </div>

      <div className="flex gap-4">
        <button
          onClick={onPrev}
          className="flex-1 py-4 border-2 border-gray-300 rounded-xl font-medium hover:border-gray-400 flex items-center justify-center gap-2"
        >
          <ArrowLeft className="w-5 h-5" /> Quay lại
        </button>
        <button
          onClick={onNext}
          className="flex-1 py-4 bg-primary text-white font-bold rounded-xl hover:bg-teal-700 flex items-center justify-center gap-2"
        >
          Tiếp tục <ArrowRight className="w-5 h-5" />
        </button>
      </div>
    </div>
  );
};

// Step 3: Payment
const PaymentStep = ({ pricing, contactInfo, setContactInfo, onNext, onPrev, onSubmit, isSubmitting }) => {
  const [paymentMethod, setPaymentMethod] = useState('online');
  const [error, setError] = useState('');

  const handleSubmit = () => {
    if (!contactInfo.name.trim()) {
      setError('Vui lòng nhập họ tên');
      return;
    }
    if (!contactInfo.email.trim() || !contactInfo.email.includes('@')) {
      setError('Vui lòng nhập email hợp lệ');
      return;
    }
    if (!contactInfo.phone.trim() || contactInfo.phone.length < 10) {
      setError('Vui lòng nhập số điện thoại');
      return;
    }
    setError('');
    onSubmit();
  };

  return (
    <div className="space-y-6">
      <h2 className="text-xl font-bold">Thanh toán</h2>

      {/* Contact Info */}
      <div className="space-y-4">
        <h3 className="font-medium">Thông tin liên hệ</h3>
        <input
          type="text"
          placeholder="Họ tên *"
          value={contactInfo.name}
          onChange={(e) => setContactInfo({ ...contactInfo, name: e.target.value })}
          className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary"
        />
        <input
          type="email"
          placeholder="Email *"
          value={contactInfo.email}
          onChange={(e) => setContactInfo({ ...contactInfo, email: e.target.value })}
          className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary"
        />
        <input
          type="tel"
          placeholder="Số điện thoại *"
          value={contactInfo.phone}
          onChange={(e) => setContactInfo({ ...contactInfo, phone: e.target.value })}
          className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary"
        />
        <textarea
          placeholder="Ghi chú (tùy chọn)"
          value={contactInfo.notes}
          onChange={(e) => setContactInfo({ ...contactInfo, notes: e.target.value })}
          rows={3}
          className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary resize-none"
        />
      </div>

      {/* Payment Method */}
      <div className="space-y-4">
        <h3 className="font-medium">Phương thức thanh toán</h3>
        
        <label className={`flex items-center p-4 border-2 rounded-xl cursor-pointer transition-all ${
          paymentMethod === 'online' ? 'border-primary bg-primary/5' : 'border-gray-200'
        }`}>
          <input
            type="radio"
            name="paymentMethod"
            value="online"
            checked={paymentMethod === 'online'}
            onChange={(e) => setPaymentMethod(e.target.value)}
            className="w-5 h-5 text-primary"
          />
          <div className="ml-3">
            <p className="font-medium">Thanh toán online</p>
            <p className="text-sm text-gray-500">VNPay, MoMo, ATM, Visa</p>
          </div>
          <CreditCard className="w-6 h-6 ml-auto text-primary" />
        </label>

        <label className={`flex items-center p-4 border-2 rounded-xl cursor-pointer transition-all ${
          paymentMethod === 'cash' ? 'border-primary bg-primary/5' : 'border-gray-200'
        }`}>
          <input
            type="radio"
            name="paymentMethod"
            value="cash"
            checked={paymentMethod === 'cash'}
            onChange={(e) => setPaymentMethod(e.target.value)}
            className="w-5 h-5 text-primary"
          />
          <div className="ml-3">
            <p className="font-medium">Thanh toán khi nhận dịch vụ</p>
            <p className="text-sm text-gray-500">Tiền mặt tại quầy</p>
          </div>
        </label>
      </div>

      {/* Total */}
      <div className="bg-primary/10 p-4 rounded-xl">
        <div className="flex justify-between items-center">
          <span className="font-medium">Tổng thanh toán</span>
          <span className="text-2xl font-bold text-primary">{pricing.finalAmount.toLocaleString()}đ</span>
        </div>
      </div>

      {error && (
        <div className="flex items-center gap-2 text-red-500">
          <AlertCircle className="w-5 h-5" />
          {error}
        </div>
      )}

      <div className="flex gap-4">
        <button
          onClick={onPrev}
          disabled={isSubmitting}
          className="flex-1 py-4 border-2 border-gray-300 rounded-xl font-medium hover:border-gray-400 flex items-center justify-center gap-2 disabled:opacity-50"
        >
          <ArrowLeft className="w-5 h-5" /> Quay lại
        </button>
        <button
          onClick={handleSubmit}
          disabled={isSubmitting}
          className="flex-1 py-4 bg-primary text-white font-bold rounded-xl hover:bg-teal-700 flex items-center justify-center gap-2 disabled:opacity-50"
        >
          {isSubmitting ? (
            <> <Loader2 className="w-5 h-5 animate-spin" /> Đang xử lý...</>
          ) : paymentMethod === 'online' ? (
            <>Thanh toán ngay <ArrowRight className="w-5 h-5" /></>
          ) : (
            <>Xác nhận đặt <ArrowRight className="w-5 h-5" /></>
          )}
        </button>
      </div>
    </div>
  );
};

// Step 4: Confirmation
const ConfirmationStep = ({ booking }) => (
  <div className="text-center py-8">
    <div className="w-20 h-20 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-6">
      <CheckCircle className="w-10 h-10 text-green-500" />
    </div>
    <h2 className="text-2xl font-bold mb-4">Đặt dịch vụ thành công!</h2>
    <p className="text-gray-600 mb-2">Mã đơn: <span className="font-bold text-primary">{booking?.bookingCode}</span></p>
    <p className="text-gray-600 mb-6">Chúng tôi đã gửi email xác nhận đến {booking?.contactEmail}</p>
    
    <div className="bg-gray-50 p-4 rounded-xl text-left max-w-md mx-auto mb-6">
      <h3 className="font-medium mb-2">Thông tin đơn</h3>
      <div className="space-y-1 text-sm">
        <p><span className="text-gray-500">Tổng tiền:</span> {booking?.finalAmount?.toLocaleString()}đ</p>
        <p><span className="text-gray-500">Phương thức:</span> {booking?.paymentMethod === 'online' ? 'Thanh toán online' : 'Thanh toán khi nhận dịch vụ'}</p>
        <p><span className="text-gray-500">Trạng thái:</span> {booking?.status === 'Confirmed' ? 'Đã xác nhận' : 'Chờ thanh toán'}</p>
      </div>
    </div>

    <a
      href="/user/bookings"
      className="inline-block px-6 py-3 bg-primary text-white rounded-full font-medium hover:bg-teal-700"
    >
      Xem đơn đặt
    </a>
  </div>
);

// Main Booking Page
const BookingPage = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const { nextStep, prevStep, setBookingDetails } = useBooking();

  const [currentStep, setCurrentStep] = useState(1);
  const [loading, setLoading] = useState(false);
  const [booking, setBooking] = useState(null);

  // Get service info from URL params - support multiple sources
  const tourId = searchParams.get('tourId');
  const hotelId = searchParams.get('hotelId');
  const roomId = searchParams.get('roomId');
  const transportId = searchParams.get('transportId');
  
  // Determine service type and ID based on URL params
  let serviceType = 'tour';
  let serviceId = '';
  let serviceName = searchParams.get('name') || 'Dịch vụ';
  let price = parseFloat(searchParams.get('price') || '0');
  
  if (tourId) {
    serviceType = 'tour';
    serviceId = tourId;
  } else if (hotelId && roomId) {
    serviceType = 'hotel';
    serviceId = roomId;
  } else if (hotelId) {
    serviceType = 'hotel';
    serviceId = hotelId;
  } else if (transportId) {
    serviceType = 'transport';
    serviceId = transportId;
  }

  // Fetch service details from API
  useEffect(() => {
    const fetchServiceDetails = async () => {
      if (!serviceId) return;
      
      try {
        let endpoint = '';
        if (serviceType === 'tour') endpoint = `/tours/${serviceId}`;
        else if (serviceType === 'hotel') endpoint = `/hotels/${serviceId}`;
        else if (serviceType === 'transport') endpoint = `/transports/${serviceId}`;
        
        if (endpoint) {
          const response = await api.get(endpoint);
          if (response.data.success && response.data.data) {
            const data = response.data.data;
            let name = 'Dịch vụ';
            let servicePrice = 0;
            
            if (serviceType === 'tour') {
              name = data.name || data.tourName || 'Tour du lịch';
              servicePrice = data.basePrice || data.price || 0;
            } else if (serviceType === 'hotel') {
              name = data.name || 'Khách sạn';
              servicePrice = data.minPrice || data.price || 0;
            } else if (serviceType === 'transport') {
              name = data.name || data.vehicleType || 'Vận chuyển';
              servicePrice = data.price || 0;
            }
            
            setServiceInfo(prev => ({
              ...prev,
              serviceName: name,
              price: servicePrice
            }));
            setPricing(prev => ({
              ...prev,
              totalAmount: servicePrice,
              finalAmount: servicePrice
            }));
          }
        }
      } catch (error) {
        console.error('Error fetching service details:', error);
      }
    };
    
    if (serviceId) {
      fetchServiceDetails();
    }
  }, [serviceId, serviceType]);

  const [serviceInfo, setServiceInfo] = useState({
    serviceType,
    serviceId,
    serviceName,
    price,
    serviceDate: '',
    endDate: '',
    quantity: 1,
    roomType: '',
    maxQuantity: 10
  });

  const [pricing, setPricing] = useState({
    totalAmount: price || 0,
    discountAmount: 0,
    finalAmount: price || 0,
    couponCode: '',
    couponData: null
  });

  const [contactInfo, setContactInfo] = useState({
    name: '',
    email: '',
    phone: '',
    notes: ''
  });

  const steps = ['Cấu hình', 'Xác nhận', 'Thanh toán', 'Hoàn tất'];

  const handleSubmitBooking = async () => {
    setLoading(true);

    try {
      // Create booking
      const createResponse = await api.post('/bookings', {
        serviceType: serviceInfo.serviceType,
        serviceId: serviceInfo.serviceId,
        serviceDate: serviceInfo.serviceDate,
        endDate: serviceInfo.endDate,
        quantity: serviceInfo.quantity,
        couponCode: pricing.couponCode || null,
        contactName: contactInfo.name,
        contactEmail: contactInfo.email,
        contactPhone: contactInfo.phone,
        notes: contactInfo.notes || null,
        userId: '00000000-0000-0000-0000-000000000001' // Demo user
      });

      if (!createResponse.data.success) {
        alert(createResponse.data.message || 'Có lỗi xảy ra');
        setLoading(false);
        return;
      }

      const newBooking = createResponse.data.data;

      // Process payment with VNPAY
      const paymentResponse = await api.post('/payment/vnpay', {
        orderId: newBooking.bookingId,
        amount: pricing.finalAmount,
        orderDescription: `Thanh toán đơn hàng ${newBooking.bookingId} - ${serviceInfo.serviceName}`
      });

      if (paymentResponse.data.success && paymentResponse.data.data.paymentUrl) {
        // Redirect to VNPAY payment page
        window.location.href = paymentResponse.data.data.paymentUrl;
      } else {
        // If payment fails but booking created, show pending
        setBooking(newBooking);
        setCurrentStep(4);
      }
    } catch (error) {
      console.error('Booking error:', error);
      alert('Có lỗi xảy ra. Vui lòng thử lại.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="container mx-auto px-4">
        <div className="max-w-2xl mx-auto">
          {/* Header */}
          <div className="text-center mb-8">
            <h1 className="text-3xl font-bold text-gray-800">Đặt dịch vụ</h1>
            <p className="text-gray-600 mt-2">{serviceName}</p>
          </div>

          {/* Step Indicator */}
          <StepIndicator currentStep={currentStep} steps={steps} />

          {/* Step Content */}
          <div className="bg-white rounded-2xl shadow-lg p-6">
            {currentStep === 1 && (
              <ServiceConfigStep
                serviceInfo={serviceInfo}
                setServiceInfo={setServiceInfo}
                onNext={() => setCurrentStep(2)}
              />
            )}
            
            {currentStep === 2 && (
              <ReviewStep
                serviceInfo={serviceInfo}
                pricing={pricing}
                setPricing={setPricing}
                onNext={() => setCurrentStep(3)}
                onPrev={() => setCurrentStep(1)}
              />
            )}
            
            {currentStep === 3 && (
              <PaymentStep
                pricing={pricing}
                contactInfo={contactInfo}
                setContactInfo={setContactInfo}
                onPrev={() => setCurrentStep(2)}
                onSubmit={handleSubmitBooking}
                isSubmitting={loading}
              />
            )}
            
            {currentStep === 4 && (
              <ConfirmationStep booking={booking} />
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default BookingPage;
