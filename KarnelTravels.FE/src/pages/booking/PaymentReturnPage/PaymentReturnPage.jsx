import { useState, useEffect } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import { CheckCircle, XCircle, Loader2, Home, CreditCard } from 'lucide-react';
import api from '@/services/api';

const PaymentReturnPage = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const [status, setStatus] = useState('loading'); // loading, success, error
  const [paymentInfo, setPaymentInfo] = useState(null);

  useEffect(() => {
    const processPaymentResult = async () => {
      try {
        // Gọi API để xử lý kết quả thanh toán
        const response = await api.get('/payment/vnpay/return?' + searchParams.toString());

        if (response.data.success) {
          setStatus('success');
          setPaymentInfo(response.data.data);
        } else {
          setStatus('error');
          setPaymentInfo(response.data);
        }
      } catch (error) {
        console.error('Payment verification error:', error);
        setStatus('error');
      }
    };

    if (searchParams.has('vnp_ResponseCode')) {
      processPaymentResult();
    } else {
      setStatus('error');
    }
  }, [searchParams]);

  const handleGoHome = () => {
    navigate('/');
  };

  const handleGoBookings = () => {
    navigate('/bookings');
  };

  if (status === 'loading') {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <Loader2 className="w-16 h-16 text-primary animate-spin mx-auto mb-4" />
          <h2 className="text-xl font-semibold">Đang xử lý thanh toán...</h2>
          <p className="text-gray-500 mt-2">Vui lòng chờ trong giây lát</p>
        </div>
      </div>
    );
  }

  if (status === 'success') {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center p-4">
        <div className="bg-white rounded-2xl shadow-lg p-8 max-w-md w-full text-center">
          <div className="w-20 h-20 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-6">
            <CheckCircle className="w-10 h-10 text-green-500" />
          </div>
          <h2 className="text-2xl font-bold text-gray-800 mb-2">Thanh toán thành công!</h2>
          <p className="text-gray-600 mb-6">
            Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi.
          </p>

          {paymentInfo && (
            <div className="bg-gray-50 rounded-xl p-4 mb-6 text-left">
              <div className="flex justify-between mb-2">
                <span className="text-gray-500">Mã đơn hàng:</span>
                <span className="font-medium">{paymentInfo.orderId}</span>
              </div>
              <div className="flex justify-between mb-2">
                <span className="text-gray-500">Mã giao dịch:</span>
                <span className="font-medium">{paymentInfo.transactionNo}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-500">Số tiền:</span>
                <span className="font-medium text-primary">
                  {paymentInfo.amount?.toLocaleString()}đ
                </span>
              </div>
            </div>
          )}

          <div className="flex gap-4">
            <button
              onClick={handleGoHome}
              className="flex-1 py-3 border-2 border-gray-300 rounded-xl font-medium hover:border-gray-400 flex items-center justify-center gap-2"
            >
              <Home className="w-5 h-5" />
              Trang chủ
            </button>
            <button
              onClick={handleGoBookings}
              className="flex-1 py-3 bg-primary text-white rounded-xl font-medium hover:bg-teal-700 flex items-center justify-center gap-2"
            >
              <CreditCard className="w-5 h-5" />
              Xem đơn
            </button>
          </div>
        </div>
      </div>
    );
  }

  // Error status
  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center p-4">
      <div className="bg-white rounded-2xl shadow-lg p-8 max-w-md w-full text-center">
        <div className="w-20 h-20 bg-red-100 rounded-full flex items-center justify-center mx-auto mb-6">
          <XCircle className="w-10 h-10 text-red-500" />
        </div>
        <h2 className="text-2xl font-bold text-gray-800 mb-2">Thanh toán thất bại</h2>
        <p className="text-gray-600 mb-6">
          {paymentInfo?.message || 'Có lỗi xảy ra trong quá trình thanh toán. Vui lòng thử lại.'}
        </p>

        {paymentInfo?.responseCode && (
          <div className="bg-gray-50 rounded-xl p-4 mb-6 text-left">
            <div className="flex justify-between">
              <span className="text-gray-500">Mã lỗi:</span>
              <span className="font-medium text-red-500">{paymentInfo.responseCode}</span>
            </div>
          </div>
        )}

        <div className="flex gap-4">
          <button
            onClick={handleGoHome}
            className="flex-1 py-3 border-2 border-gray-300 rounded-xl font-medium hover:border-gray-400 flex items-center justify-center gap-2"
          >
            <Home className="w-5 h-5" />
            Trang chủ
          </button>
          <button
            onClick={() => navigate(-1)}
            className="flex-1 py-3 bg-primary text-white rounded-xl font-medium hover:bg-teal-700 flex items-center justify-center gap-2"
          >
            Thử lại
          </button>
        </div>
      </div>
    </div>
  );
};

export default PaymentReturnPage;
