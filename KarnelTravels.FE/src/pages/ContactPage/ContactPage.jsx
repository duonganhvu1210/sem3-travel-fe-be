import {
  MapPin,
  Phone,
  Mail,
  Clock,
  Facebook,
  Youtube,
  MessageCircle,
  Instagram
} from 'lucide-react';

// Contact Page Component - Display contact information only (form submission is hidden)
const ContactPage = () => {
  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header Banner */}
      <div className="bg-gradient-to-r from-teal-600 to-cyan-700 text-white py-16">
        <div className="container mx-auto px-4 text-center">
          <h1 className="text-4xl font-bold mb-4">Contact Us</h1>
          <p className="text-xl text-white/80">We are always ready to support you 24/7</p>
        </div>
      </div>

      <div className="container mx-auto px-4 py-12">
        <div className="max-w-4xl mx-auto space-y-6">
          {/* Contact Details */}
          <div className="bg-white rounded-2xl shadow-lg p-8">
            <h3 className="text-xl font-bold mb-6">Contact Information</h3>

            <div className="grid md:grid-cols-2 gap-6">
              <div className="flex items-start gap-4">
                <div className="w-10 h-10 bg-primary/10 rounded-full flex items-center justify-center flex-shrink-0">
                  <MapPin className="w-5 h-5 text-primary" />
                </div>
                <div>
                  <p className="font-medium text-gray-800">Address</p>
                  <p className="text-gray-600 text-sm">
                    123 Nguyen Trai Street, District 1, Ho Chi Minh City
                  </p>
                </div>
              </div>

              <div className="flex items-start gap-4">
                <div className="w-10 h-10 bg-primary/10 rounded-full flex items-center justify-center flex-shrink-0">
                  <Phone className="w-5 h-5 text-primary" />
                </div>
                <div>
                  <p className="font-medium text-gray-800">Hotline</p>
                  <p className="text-gray-600 text-sm">1900 6677</p>
                  <p className="text-gray-500 text-xs">
                    From 8:00 AM - 8:00 PM (Free of charge)
                  </p>
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
                  <p className="font-medium text-gray-800">Working Hours</p>
                  <p className="text-gray-600 text-sm">Monday - Sunday: 8:00 AM - 8:00 PM</p>
                </div>
              </div>
            </div>
          </div>

          {/* Map */}
          <div className="bg-white rounded-2xl shadow-lg p-4">
            <h3 className="text-lg font-bold mb-4">Map</h3>
            <div className="h-80 rounded-xl overflow-hidden bg-gray-100">
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
            <h3 className="text-lg font-bold mb-4">Connect With Us</h3>
            <div className="flex gap-4">
              <a
                href="https://facebook.com"
                target="_blank"
                rel="noopener noreferrer"
                className="w-12 h-12 bg-blue-600 text-white rounded-full flex items-center justify-center hover:bg-blue-700 transition-colors"
              >
                <Facebook className="w-5 h-5" />
              </a>
              <a
                href="https://zalo.me"
                target="_blank"
                rel="noopener noreferrer"
                className="w-12 h-12 bg-blue-500 text-white rounded-full flex items-center justify-center hover:bg-blue-600 transition-colors"
              >
                <MessageCircle className="w-5 h-5" />
              </a>
              <a
                href="https://youtube.com"
                target="_blank"
                rel="noopener noreferrer"
                className="w-12 h-12 bg-red-600 text-white rounded-full flex items-center justify-center hover:bg-red-700 transition-colors"
              >
                <Youtube className="w-5 h-5" />
              </a>
              <a
                href="https://instagram.com"
                target="_blank"
                rel="noopener noreferrer"
                className="w-12 h-12 bg-gradient-to-tr from-purple-600 via-pink-500 to-orange-500 text-white rounded-full flex items-center justify-center hover:opacity-80 transition-opacity"
              >
                <Instagram className="w-5 h-5" />
              </a>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

/*
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { useSearchParams, Link } from 'react-router-dom';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { CheckCircle, Loader2 } from 'lucide-react';
import api from '@/services/api';

// Validation Schema
const schema = yup.object().shape({
  fullName: yup.string().required('Full name is required'),
  email: yup.string().required('Email is required').email('Invalid email address'),
  phoneNumber: yup.string(),
  address: yup.string(),
  subject: yup.string(),
  requestType: yup.string().required('Please select a request type'),
  serviceType: yup.string(),
  expectedDate: yup.date().nullable(),
  participantCount: yup.number().positive().nullable(),
  messageContent: yup.string().required('Message content is required'),
  rating: yup.number().min(0).max(5)
});

// Request Type Options
const requestTypeOptions = [
  { value: 'General', label: 'General' },
  { value: 'Booking', label: 'Booking' },
  { value: 'Consulting', label: 'Consulting' },
  { value: 'Feedback', label: 'Feedback' },
  { value: 'Callback', label: 'Callback Request' }
];

// Service Type Options
const serviceTypeOptions = [
  { value: '', label: 'Select a service' },
  { value: 'Tour', label: 'Tour' },
  { value: 'Hotel', label: 'Hotel' },
  { value: 'Resort', label: 'Resort' },
  { value: 'Transport', label: 'Transport' },
  { value: 'Restaurant', label: 'Restaurant' }
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
          className={`text-2xl transition-colors ${
            readonly ? 'cursor-default' : 'cursor-pointer'
          } ${star <= (hoverValue || value) ? 'text-yellow-400' : 'text-gray-300'}`}
        >
          ★
        </button>
      ))}
    </div>
  );
};

// Contact Page Component with Form
const ContactPageWithForm = () => {
  const [searchParams] = useSearchParams();
  const [submitStatus, setSubmitStatus] = useState(null);
  const [submittedContactId, setSubmittedContactId] = useState(null);

  const itemType = searchParams.get('type');
  const itemName = searchParams.get('name');

  const getServiceTypeFromItemType = (type) => {
    if (!type) return '';
    const typeMap = {
      touristspot: 'Tour',
      tour: 'Tour',
      hotel: 'Hotel',
      resort: 'Resort',
      restaurant: 'Restaurant',
      transport: 'Transport'
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
      messageContent: itemName ? `I am interested in the service: ${itemName}` : ''
    }
  });

  const requestType = watch('requestType');
  const showRating = requestType === 'Feedback';
  const showSubject = requestType === 'Feedback';

  const onSubmit = async (data) => {
    console.log('Submitting contact data:', data);
    alert('Submitting form...');
    try {
      const requestData = {
        FullName: data.fullName,
        Email: data.email,
        PhoneNumber: data.phoneNumber || null,
        Address: data.address || null,
        Subject: data.subject || null,
        RequestType: data.requestType,
        ServiceType: data.serviceType || null,
        ExpectedDate: data.expectedDate || null,
        ParticipantCount: data.participantCount || null,
        MessageContent: data.messageContent,
        Rating: data.rating || null
      };

      console.log('Sending request:', requestData);
      alert('Sending to API...');
      const response = await api.post('/contacts', requestData);
      console.log('Response:', response);
      alert('Response received: ' + JSON.stringify(response.data));

      if (response.data.success && response.data.data) {
        const contactId = response.data.data.contactId || response.data.data.id;
        localStorage.setItem('lastContactId', contactId);
        setSubmittedContactId(contactId);
        setSubmitStatus('success');
      } else {
        setSubmitStatus('error');
      }
    } catch (error) {
      console.error('Contact submission error:', error);
      alert('Error: ' + error.message);
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
          <h2 className="text-2xl font-bold text-gray-800 mb-4">Request Sent Successfully!</h2>
          <p className="text-gray-600 mb-6">
            Thank you for contacting us. We will get back to you as soon as possible.
          </p>
          <div className="space-y-3">
            <Link
              to="/my-messages"
              className="block w-full px-6 py-3 bg-primary text-white rounded-full font-medium hover:bg-teal-700 transition-colors"
            >
              View Your Messages
            </Link>
            <button
              onClick={() => {
                setSubmitStatus(null);
                reset();
              }}
              className="block w-full px-6 py-3 bg-gray-100 text-gray-700 rounded-full font-medium hover:bg-gray-200 transition-colors"
            >
              Send Another Request
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="bg-gradient-to-r from-teal-600 to-cyan-700 text-white py-16">
        <div className="container mx-auto px-4 text-center">
          <h1 className="text-4xl font-bold mb-4">Contact Us</h1>
          <p className="text-xl text-white/80">We are always ready to support you 24/7</p>
        </div>
      </div>

      <div className="container mx-auto px-4 py-12">
        <div className="grid lg:grid-cols-3 gap-8">
          <div className="lg:col-span-2">
            <div className="bg-white rounded-2xl shadow-lg p-8">
              <h2 className="text-2xl font-bold mb-6">Send a Request</h2>
              <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Request Type <span className="text-red-500">*</span>
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

                {(requestType === 'Booking' || requestType === 'Consulting') && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Service of Interest
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

                <div className="grid md:grid-cols-2 gap-6">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Full Name <span className="text-red-500">*</span>
                    </label>
                    <input
                      {...register('fullName')}
                      type="text"
                      placeholder="Enter your full name"
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

                <div className="grid md:grid-cols-2 gap-6">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Phone Number
                    </label>
                    <input
                      {...register('phoneNumber')}
                      type="tel"
                      placeholder="0123 456 789"
                      className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent"
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Address
                    </label>
                    <input
                      {...register('address')}
                      type="text"
                      placeholder="Your address"
                      className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent"
                    />
                  </div>
                </div>

                {showSubject && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Subject <span className="text-red-500">*</span>
                    </label>
                    <input
                      {...register('subject')}
                      type="text"
                      placeholder="Enter feedback subject"
                      className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent"
                    />
                  </div>
                )}

                {(requestType === 'Booking' || requestType === 'Consulting') && (
                  <div className="grid md:grid-cols-2 gap-6">
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        Expected Date
                      </label>
                      <input
                        {...register('expectedDate')}
                        type="date"
                        className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent"
                      />
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        Number of Participants
                      </label>
                      <input
                        {...register('participantCount', { valueAsNumber: true })}
                        type="number"
                        min="1"
                        placeholder="Number of participants"
                        className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent"
                      />
                    </div>
                  </div>
                )}

                {showRating && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Your Rating <span className="text-red-500">*</span>
                    </label>
                    <StarRating
                      value={watch('rating') || 0}
                      onChange={(value) => setValue('rating', value)}
                    />
                  </div>
                )}

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Message <span className="text-red-500">*</span>
                  </label>
                  <textarea
                    {...register('messageContent')}
                    rows={6}
                    placeholder="Enter your request details..."
                    className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent resize-none"
                  />
                  {errors.messageContent && (
                    <p className="mt-1 text-sm text-red-500">{errors.messageContent.message}</p>
                  )}
                </div>

                <button
                  type="submit"
                  disabled={isSubmitting}
                  className="w-full py-4 bg-primary text-white font-bold rounded-xl hover:bg-teal-700 disabled:bg-gray-400 disabled:cursor-not-allowed flex items-center justify-center gap-2"
                >
                  {isSubmitting ? (
                    <>
                      <Loader2 className="w-5 h-5 animate-spin" />
                      Sending...
                    </>
                  ) : (
                    <>
                      <Send className="w-5 h-5" />
                      Send Request
                    </>
                  )}
                </button>

                {submitStatus === 'error' && (
                  <div className="p-4 bg-red-50 border border-red-200 rounded-xl text-red-600 text-center">
                    An error occurred. Please try again later.
                  </div>
                )}
              </form>
            </div>
          </div>

          <div className="space-y-6">
            <div className="bg-white rounded-2xl shadow-lg p-8">
              <h3 className="text-xl font-bold mb-6">Contact Information</h3>
              <div className="space-y-4">
                <div className="flex items-start gap-4">
                  <div className="w-10 h-10 bg-primary/10 rounded-full flex items-center justify-center flex-shrink-0">
                    <MapPin className="w-5 h-5 text-primary" />
                  </div>
                  <div>
                    <p className="font-medium text-gray-800">Address</p>
                    <p className="text-gray-600 text-sm">123 Nguyen Trai Street, District 1, Ho Chi Minh City</p>
                  </div>
                </div>

                <div className="flex items-start gap-4">
                  <div className="w-10 h-10 bg-primary/10 rounded-full flex items-center justify-center flex-shrink-0">
                    <Phone className="w-5 h-5 text-primary" />
                  </div>
                  <div>
                    <p className="font-medium text-gray-800">Hotline</p>
                    <p className="text-gray-600 text-sm">1900 6677</p>
                    <p className="text-gray-500 text-xs">From 8:00 AM - 8:00 PM (Free of charge)</p>
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
                    <p className="font-medium text-gray-800">Working Hours</p>
                    <p className="text-gray-600 text-sm">Monday - Sunday: 8:00 AM - 8:00 PM</p>
                  </div>
                </div>
              </div>
            </div>

            <div className="bg-white rounded-2xl shadow-lg p-4">
              <h3 className="text-lg font-bold mb-4">Map</h3>
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

            <div className="bg-white rounded-2xl shadow-lg p-8">
              <h3 className="text-lg font-bold mb-4">Connect With Us</h3>
              <div className="flex gap-4">
                <a href="https://facebook.com" target="_blank" rel="noopener noreferrer" className="w-12 h-12 bg-blue-600 text-white rounded-full flex items-center justify-center hover:bg-blue-700">
                  <Facebook className="w-5 h-5" />
                </a>
                <a href="https://zalo.me" target="_blank" rel="noopener noreferrer" className="w-12 h-12 bg-blue-500 text-white rounded-full flex items-center justify-center hover:bg-blue-600">
                  <MessageCircle className="w-5 h-5" />
                </a>
                <a href="https://youtube.com" target="_blank" rel="noopener noreferrer" className="w-12 h-12 bg-red-600 text-white rounded-full flex items-center justify-center hover:bg-red-700">
                  <Youtube className="w-5 h-5" />
                </a>
                <a href="https://instagram.com" target="_blank" rel="noopener noreferrer" className="w-12 h-12 bg-gradient-to-tr from-purple-600 via-pink-500 to-orange-500 text-white rounded-full flex items-center justify-center hover:opacity-80">
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
*/

export default ContactPage;