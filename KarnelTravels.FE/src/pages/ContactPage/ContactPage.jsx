import { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { useSearchParams, Link } from 'react-router-dom';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import {
  MapPin, Phone, Mail, Clock, Send, Star, CheckCircle,
  Loader2, Facebook, Youtube, MessageCircle, Instagram
} from 'lucide-react';
import api from '@/services/api';

// Validation Schema
const schema = yup.object().shape({
  fullName: yup.string().required('Họ tên là bắt buộc'),
  email: yup.string().required('Email là bắt buộc').email('Email không hợp lệ'),
  phoneNumber: yup.string()
    .matches(/^[0-9+\-\s()]*$/, 'Số điện thoại không hợp lệ')
    .min(10, 'Số điện thoại phải có ít nhất 10 chữ số'),
  address: yup.string(),
  subject: yup.string().when('requestType', {
    is: 'Feedback',
    then: (schema) => schema.required('Tiêu đề là bắt buộc cho phản hồi'),
    otherwise: (schema) => schema
  }),
  requestType: yup.string().required('Vui lòng chọn loại yêu cầu'),
  serviceType: yup.string(),
  expectedDate: yup.date().nullable(),
  participantCount: yup.number().min(1, 'Số người phải lớn hơn 0').positive(),
  messageContent: yup.string().required('Nội dung tin nhắn là bắt buộc').min(10, 'Nội dung phải có ít nhất 10 ký tự'),
  rating: yup.number().min(1, 'Vui lòng chọn đánh giá').max(5)
});

// Request Type Labels
const requestTypeOptions = [
  { value: 'General', label: 'Chung' },
  { value: 'Booking', label: 'Đặt tour' },
  { value: 'Consulting', label: 'Tư vấn' },
  { value: 'Feedback', label: 'Phản hồi' },
  { value: 'Callback', label: 'Yêu cầu gọi lại' }
];

// Service Type Options
const serviceTypeOptions = [
  { value: '', label: 'Chọn dịch vụ' },
  { value: 'Tour', label: 'Tour du lịch' },
  { value: 'Hotel', label: 'Khách sạn' },
  { value: 'Resort', label: 'Resort' },
  { value: 'Transport', label: 'Vận chuyển' },
  { value: 'Restaurant', label: 'Nhà hàng' }
];

// Star Rating Component
const StarRating = ({ value, onChange, readonly = false }) => {
  const [hoverValue, setHoverValue] = useState(0);

  return (
    <div className="flex gap-1">
      {[1, 2, 3, 4, 5].map((star) => (
        <button
          key={star}
          type="button"
          disabled={readonly}
          onClick={() => onChange(star)}
          onMouseEnter={() => !readonly && setHoverValue(star)}
          onMouseLeave={() => !readonly && setHoverValue(0)}
          className={`text-2xl transition-colors ${readonly ? 'cursor-default' : 'cursor-pointer'} ${
            star <= (hoverValue || value) ? 'text-yellow-400' : 'text-gray-300'
          }`}
        >
          ★
        </button>
      ))}
    </div>
  );
};

// Contact Page Component
const ContactPage = () => {
  const [searchParams] = useSearchParams();
  const [submitStatus, setSubmitStatus] = useState(null); // 'success' | 'error' | null
  const [submittedContactId, setSubmittedContactId] = useState(null);

  // Get pre-filled data from URL params
  const itemType = searchParams.get('type');
  const itemName = searchParams.get('name');

  // Map item type to service type
  const getServiceTypeFromItemType = (type) => {
    if (!type) return '';
    const typeMap = {
      'touristspot': 'Tour',
      'tour': 'Tour',
      'hotel': 'Hotel',
      'resort': 'Resort',
      'restaurant': 'Restaurant',
      'transport': 'Transport'
    };
    return typeMap[type.toLowerCase()] || '';
  };

  const {
    register,
    handleSubmit,
    watch,
    setValue,
    reset,
    formState: { errors, isSubmitting }
  } = useForm({
    resolver: yupResolver(schema),
    defaultValues: {
      requestType: 'Consulting',
      rating: 0,
      serviceType: getServiceTypeFromItemType(itemType),
      messageContent: itemName ? `Tôi quan tâm đến dịch vụ: ${itemName}` : ''
    }
  });

  const requestType = watch('requestType');
  const showRating = requestType === 'Feedback';
  const showSubject = requestType === 'Feedback';

  const onSubmit = async (data) => {
    try {
      const requestData = {
        fullName: data.fullName,
        email: data.email,
        phoneNumber: data.phoneNumber || null,
        address: data.address || null,
        subject: data.subject || null,
        requestType: data.requestType,
        serviceType: data.serviceType || null,
        expectedDate: data.expectedDate || null,
        participantCount: data.participantCount || null,
        messageContent: data.messageContent,
        rating: data.rating || null
      };

      const response = await api.post('/contacts', requestData);

      if (response.data.success && response.data.data) {
        // Save contact ID for viewing later
        const contactId = response.data.data.contactId || response.data.data.id;
        localStorage.setItem('lastContactId', contactId);
        setSubmittedContactId(contactId);
        setSubmitStatus('success');
      } else {
        setSubmitStatus('error');
      }
    } catch (error) {
      console.error('Contact submission error:', error);
      setSubmitStatus('error');
    }
  };

  if (submitStatus === 'success') {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center p-4">
        <div className="bg-white rounded-2xl shadow-lg p-8 max-w-md w-full text-center">
          <div className="w-20 h-20 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-6">
            <CheckCircle className="w-10 h-10 text-green-500" />
          </div>
          <h2 className="text-2xl font-bold text-gray-800 mb-4">Gửi yêu cầu thành công!</h2>
          <p className="text-gray-600 mb-6">
            Cảm ơn bạn đã liên hệ. Chúng tôi sẽ phản hồi trong thời gian sớm nhất.
          </p>
          <div className="space-y-3">
            <Link
              to="/my-messages"
              className="block w-full px-6 py-3 bg-primary text-white rounded-full font-medium hover:bg-teal-700 transition-colors"
            >
              Xem tin nhắn của bạn
            </Link>
            <button
              onClick={() => {
                setSubmitStatus(null);
                reset();
              }}
              className="block w-full px-6 py-3 bg-gray-100 text-gray-700 rounded-full font-medium hover:bg-gray-200 transition-colors"
            >
              Gửi yêu cầu khác
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header Banner */}
      <div className="bg-gradient-to-r from-teal-600 to-cyan-700 text-white py-16">
        <div className="container mx-auto px-4 text-center">
          <h1 className="text-4xl font-bold mb-4">Liên hệ với chúng tôi</h1>
          <p className="text-xl text-white/80">Chúng tôi luôn sẵn sàng hỗ trợ bạn 24/7</p>
        </div>
      </div>

      <div className="container mx-auto px-4 py-12">
        <div className="grid lg:grid-cols-3 gap-8">
          {/* Contact Form */}
          <div className="lg:col-span-2">
            <div className="bg-white rounded-2xl shadow-lg p-8">
              <h2 className="text-2xl font-bold mb-6">Gửi yêu cầu</h2>
              
              <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                {/* Request Type */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Loại yêu cầu <span className="text-red-500">*</span>
                  </label>
                  <select
                    {...register('requestType')}
                    className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent"
                  >
                    {requestTypeOptions.map((option) => (
                      <option key={option.value} value={option.value}>
                        {option.label}
                      </option>
                    ))}
                  </select>
                  {errors.requestType && (
                    <p className="mt-1 text-sm text-red-500">{errors.requestType.message}</p>
                  )}
                </div>

                {/* Service Type (show for Booking/Consulting) */}
                {(requestType === 'Booking' || requestType === 'Consulting') && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Dịch vụ quan tâm
                    </label>
                    <select
                      {...register('serviceType')}
                      className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent"
                    >
                      {serviceTypeOptions.map((option) => (
                        <option key={option.value} value={option.value}>
                          {option.label}
                        </option>
                      ))}
                    </select>
                  </div>
                )}

                {/* Name & Email Row */}
                <div className="grid md:grid-cols-2 gap-6">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Họ tên <span className="text-red-500">*</span>
                    </label>
                    <input
                      {...register('fullName')}
                      type="text"
                      placeholder="Nhập họ tên của bạn"
                      className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent"
                    />
                    {errors.fullName && (
                      <p className="mt-1 text-sm text-red-500">{errors.fullName.message}</p>
                    )}
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Email <span className="text-red-500">*</span>
                    </label>
                    <input
                      {...register('email')}
                      type="email"
                      placeholder="email@example.com"
                      className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent"
                    />
                    {errors.email && (
                      <p className="mt-1 text-sm text-red-500">{errors.email.message}</p>
                    )}
                  </div>
                </div>

                {/* Phone & Address Row */}
                <div className="grid md:grid-cols-2 gap-6">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Số điện thoại
                    </label>
                    <input
                      {...register('phoneNumber')}
                      type="tel"
                      placeholder="0123 456 789"
                      className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent"
                    />
                    {errors.phoneNumber && (
                      <p className="mt-1 text-sm text-red-500">{errors.phoneNumber.message}</p>
                    )}
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Địa chỉ
                    </label>
                    <input
                      {...register('address')}
                      type="text"
                      placeholder="Địa chỉ của bạn"
                      className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent"
                    />
                  </div>
                </div>

                {/* Subject (only for Feedback) */}
                {showSubject && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Tiêu đề <span className="text-red-500">*</span>
                    </label>
                    <input
                      {...register('subject')}
                      type="text"
                      placeholder="Nhập tiêu đề phản hồi"
                      className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent"
                    />
                    {errors.subject && (
                      <p className="mt-1 text-sm text-red-500">{errors.subject.message}</p>
                    )}
                  </div>
                )}

                {/* Date & Participants (for Booking) */}
                {(requestType === 'Booking' || requestType === 'Consulting') && (
                  <div className="grid md:grid-cols-2 gap-6">
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        Ngày dự kiến
                      </label>
                      <input
                        {...register('expectedDate')}
                        type="date"
                        className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent"
                      />
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        Số người tham gia
                      </label>
                      <input
                        {...register('participantCount', { valueAsNumber: true })}
                        type="number"
                        min="1"
                        placeholder="Số người"
                        className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent"
                      />
                      {errors.participantCount && (
                        <p className="mt-1 text-sm text-red-500">{errors.participantCount.message}</p>
                      )}
                    </div>
                  </div>
                )}

                {/* Rating (only for Feedback) */}
                {showRating && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Đánh giá của bạn <span className="text-red-500">*</span>
                    </label>
                    <StarRating
                      value={watch('rating') || 0}
                      onChange={(value) => setValue('rating', value)}
                    />
                    {errors.rating && (
                      <p className="mt-1 text-sm text-red-500">{errors.rating.message}</p>
                    )}
                  </div>
                )}

                {/* Message */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Nội dung <span className="text-red-500">*</span>
                  </label>
                  <textarea
                    {...register('messageContent')}
                    rows={6}
                    placeholder="Nhập nội dung yêu cầu của bạn..."
                    className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent resize-none"
                  />
                  {errors.messageContent && (
                    <p className="mt-1 text-sm text-red-500">{errors.messageContent.message}</p>
                  )}
                </div>

                {/* Submit Button */}
                <button
                  type="submit"
                  disabled={isSubmitting}
                  className="w-full py-4 bg-primary text-white font-bold rounded-xl hover:bg-teal-700 disabled:bg-gray-400 disabled:cursor-not-allowed flex items-center justify-center gap-2"
                >
                  {isSubmitting ? (
                    <>
                      <Loader2 className="w-5 h-5 animate-spin" />
                      Đang gửi...
                    </>
                  ) : (
                    <>
                      <Send className="w-5 h-5" />
                      Gửi yêu cầu
                    </>
                  )}
                </button>

                {submitStatus === 'error' && (
                  <div className="p-4 bg-red-50 border border-red-200 rounded-xl text-red-600 text-center">
                    Có lỗi xảy ra. Vui lòng thử lại sau.
                  </div>
                )}
              </form>
            </div>
          </div>

          {/* Contact Info Sidebar */}
          <div className="space-y-6">
            {/* Contact Details */}
            <div className="bg-white rounded-2xl shadow-lg p-8">
              <h3 className="text-xl font-bold mb-6">Thông tin liên hệ</h3>
              
              <div className="space-y-4">
                <div className="flex items-start gap-4">
                  <div className="w-10 h-10 bg-primary/10 rounded-full flex items-center justify-center flex-shrink-0">
                    <MapPin className="w-5 h-5 text-primary" />
                  </div>
                  <div>
                    <p className="font-medium text-gray-800">Địa chỉ</p>
                    <p className="text-gray-600 text-sm">123 Đường Nguyễn Trãi, Quận 1, TP. Hồ Chí Minh</p>
                  </div>
                </div>

                <div className="flex items-start gap-4">
                  <div className="w-10 h-10 bg-primary/10 rounded-full flex items-center justify-center flex-shrink-0">
                    <Phone className="w-5 h-5 text-primary" />
                  </div>
                  <div>
                    <p className="font-medium text-gray-800">Hotline</p>
                    <p className="text-gray-600 text-sm">1900 xxxx</p>
                    <p className="text-gray-500 text-xs">Từ 8:00 - 20:00 (Miễn phí)</p>
                  </div>
                </div>

                <div className="flex items-start gap-4">
                  <div className="w-10 h-10 bg-primary/10 rounded-full flex items-center justify-center flex-shrink-0">
                    <Mail className="w-5 h-5 text-primary" />
                  </div>
                  <div>
                    <p className="font-medium text-gray-800">Email</p>
                    <p className="text-gray-600 text-sm">contact@karneltravels.com</p>
                  </div>
                </div>

                <div className="flex items-start gap-4">
                  <div className="w-10 h-10 bg-primary/10 rounded-full flex items-center justify-center flex-shrink-0">
                    <Clock className="w-5 h-5 text-primary" />
                  </div>
                  <div>
                    <p className="font-medium text-gray-800">Giờ làm việc</p>
                    <p className="text-gray-600 text-sm">Thứ 2 - Chủ nhật: 8:00 - 20:00</p>
                  </div>
                </div>
              </div>
            </div>

            {/* Map */}
            <div className="bg-white rounded-2xl shadow-lg p-4">
              <h3 className="text-lg font-bold mb-4">Bản đồ</h3>
              <div className="h-64 rounded-xl overflow-hidden bg-gray-100">
                <iframe
                  title="Location Map"
                  src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3919.424358524794!2d106.69200431533418!3d10.776521992319566!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x31752f3a5a2d5a5b%3A0x5a5a5a5a5a5a5a5a!2zMTIzIMSQ4buLbmcgVGjDoWksIFBoxrDhu51uZyBOZ3V54buFbiwgUXXhuq1uIDEsIFRow6BuaCBwaOG7kSBI4buTIENow60gTWluaA!5e0!3m2!1svi!2s!4v1234567890"
                  width="100%"
                  height="100%"
                  style={{ border: 0 }}
                  allowFullScreen=""
                  loading="lazy"
                  referrerPolicy="no-referrer-when-downgrade"
                />
              </div>
            </div>

            {/* Social Media */}
            <div className="bg-white rounded-2xl shadow-lg p-8">
              <h3 className="text-lg font-bold mb-4">Kết nối với chúng tôi</h3>
              <div className="flex gap-4">
                <a href="https://facebook.com" target="_blank" rel="noopener noreferrer" className="w-12 h-12 bg-blue-600 text-white rounded-full flex items-center justify-center hover:bg-blue-700 transition-colors">
                  <Facebook className="w-5 h-5" />
                </a>
                <a href="https://zalo.me" target="_blank" rel="noopener noreferrer" className="w-12 h-12 bg-blue-500 text-white rounded-full flex items-center justify-center hover:bg-blue-600 transition-colors">
                  <MessageCircle className="w-5 h-5" />
                </a>
                <a href="https://youtube.com" target="_blank" rel="noopener noreferrer" className="w-12 h-12 bg-red-600 text-white rounded-full flex items-center justify-center hover:bg-red-700 transition-colors">
                  <Youtube className="w-5 h-5" />
                </a>
                <a href="https://instagram.com" target="_blank" rel="noopener noreferrer" className="w-12 h-12 bg-gradient-to-tr from-purple-600 via-pink-500 to-orange-500 text-white rounded-full flex items-center justify-center hover:opacity-80 transition-opacity">
                  <Instagram className="w-5 h-5" />
                </a>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ContactPage;
