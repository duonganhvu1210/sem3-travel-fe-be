import React, { useState, useEffect } from 'react';
import orderService from '../../../../services/orderService';

// Status configuration
const STATUS_CONFIG = {
  Pending: { label: 'Chờ xác nhận', color: 'bg-yellow-100 text-yellow-800', icon: '⏳' },
  Confirmed: { label: 'Confirmed', color: 'bg-blue-100 text-blue-800', icon: '✓' },
  Completed: { label: 'Completed', color: 'bg-green-100 text-green-800', icon: '★' },
  Cancelled: { label: 'Cancelled', color: 'bg-red-100 text-red-800', icon: '✕' },
  Refunded: { label: 'Đã hoàn tiền', color: 'bg-gray-100 text-gray-800', icon: '↩' }
};

const PAYMENT_STATUS_CONFIG = {
  Pending: { label: 'Chờ thanh toán', color: 'bg-yellow-100 text-yellow-800' },
  Paid: { label: 'Đã thanh toán', color: 'bg-green-100 text-green-800' },
  Failed: { label: 'Thất bại', color: 'bg-red-100 text-red-800' },
  Refunded: { label: 'Đã hoàn tiền', color: 'bg-gray-100 text-gray-800' }
};

// Format currency
const formatCurrency = (amount) => {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(amount || 0);
};

// Format date
const formatDate = (dateString) => {
  if (!dateString) return '-';
  const date = new Date(dateString);
  return new Intl.DateTimeFormat('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(date);
};

const OrderDetailModal = ({ orderId, isOpen, onClose, onReview, onDownloadInvoice }) => {
  const [order, setOrder] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (isOpen && orderId) {
      loadOrderDetail();
    }
  }, [isOpen, orderId]);

  const loadOrderDetail = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await orderService.getOrderDetail(orderId);
      setOrder(data);
    } catch (err) {
      setError('Không thể tải thông tin đơn hàng');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleDownloadInvoice = async () => {
    try {
      await orderService.downloadInvoice(orderId);
    } catch (err) {
      console.error('Failed to download invoice:', err);
    }
  };

  if (!isOpen) return null;

  const statusConfig = order ? (STATUS_CONFIG[order.status] || STATUS_CONFIG.Pending) : null;
  const paymentConfig = order ? (PAYMENT_STATUS_CONFIG[order.paymentStatus] || PAYMENT_STATUS_CONFIG.Pending) : null;

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto">
      {/* Backdrop */}
      <div 
        className="fixed inset-0 bg-black/50 transition-opacity"
        onClick={onClose}
      />

      {/* Modal */}
      <div className="flex min-h-full items-center justify-center p-4">
        <div className="relative bg-white rounded-xl shadow-2xl w-full max-w-2xl max-h-[90vh] overflow-hidden">
          {/* Header */}
          <div className="bg-gradient-to-r from-blue-600 to-blue-700 px-6 py-4 flex justify-between items-center">
            <div>
              <h2 className="text-xl font-bold text-white">
                Chi tiết đơn hàng
              </h2>
              <p className="text-white/80 text-sm">
                {order?.orderCode}
              </p>
            </div>
            <button
              onClick={onClose}
              className="text-white/80 hover:text-white transition-colors"
            >
              <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>

          {/* Content */}
          <div className="p-6 overflow-y-auto max-h-[calc(90vh-140px)]">
            {loading && (
              <div className="flex items-center justify-center py-12">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
              </div>
            )}

            {error && (
              <div className="bg-red-50 text-red-600 p-4 rounded-lg mb-4">
                {error}
              </div>
            )}

            {order && !loading && (
              <>
                {/* Status badges */}
                <div className="flex flex-wrap gap-2 mb-6">
                  <span className={`px-4 py-2 rounded-full text-sm font-medium ${statusConfig?.color}`}>
                    {statusConfig?.icon} {statusConfig?.label}
                  </span>
                  <span className={`px-4 py-2 rounded-full text-sm font-medium ${paymentConfig?.color}`}>
                    {paymentConfig?.label}
                  </span>
                </div>

                {/* Service Info */}
                <div className="bg-gray-50 rounded-lg p-4 mb-4">
                  <h3 className="font-semibold text-lg mb-3 flex items-center gap-2">
                    <span className="text-2xl">
                      {order.service?.serviceType === 'Hotel' ? '🏨' : 
                       order.service?.serviceType === 'Tour' ? '🧭' :
                       order.service?.serviceType === 'Resort' ? '🏝️' :
                       order.service?.serviceType === 'Transport' ? '🚌' : '🍽️'}
                    </span>
                    {order.service?.serviceName}
                  </h3>
                  
                  {order.service?.serviceAddress && (
                    <p className="text-gray-600 text-sm mb-2">
                      📍 {order.service?.serviceAddress}
                    </p>
                  )}
                  
                  {order.service?.roomType && (
                    <p className="text-gray-600 text-sm">
                      🛏️ Loại phòng: {order.service?.roomType} | 👤 Tối đa: {order.service?.maxOccupancy} người
                    </p>
                  )}
                  
                  {order.service?.transportRoute && (
                    <p className="text-gray-600 text-sm">
                      🚏 Tuyến: {order.service?.transportRoute}
                    </p>
                  )}
                  
                  {order.service?.tourDestination && (
                    <p className="text-gray-600 text-sm">
                      📍 Điểm đến: {order.service?.tourDestination} | ⏱️ {order.service?.durationDays} ngày
                    </p>
                  )}
                </div>

                {/* Dates */}
                <div className="grid grid-cols-2 gap-4 mb-4">
                  <div className="bg-blue-50 rounded-lg p-3">
                    <p className="text-xs text-gray-500">Ngày đặt</p>
                    <p className="font-medium">{formatDate(order.bookingDate)}</p>
                  </div>
                  <div className="bg-green-50 rounded-lg p-3">
                    <p className="text-xs text-gray-500">Ngày sử dụng</p>
                    <p className="font-medium">{formatDate(order.serviceDate)}</p>
                  </div>
                  {order.endDate && (
                    <div className="bg-orange-50 rounded-lg p-3">
                      <p className="text-xs text-gray-500">Ngày kết thúc</p>
                      <p className="font-medium">{formatDate(order.endDate)}</p>
                    </div>
                  )}
                  <div className="bg-purple-50 rounded-lg p-3">
                    <p className="text-xs text-gray-500">Số lượng</p>
                    <p className="font-medium">{order.quantity} người</p>
                  </div>
                </div>

                {/* Customer Info */}
                <div className="border-t pt-4 mb-4">
                  <h4 className="font-semibold mb-2">Thông tin liên hệ</h4>
                  <div className="grid grid-cols-2 gap-2 text-sm">
                    <div>
                      <span className="text-gray-500">Tên:</span>
                      <span className="ml-2">{order.customer?.contactName}</span>
                    </div>
                    <div>
                      <span className="text-gray-500">Email:</span>
                      <span className="ml-2">{order.customer?.contactEmail}</span>
                    </div>
                    <div>
                      <span className="text-gray-500">Điện thoại:</span>
                      <span className="ml-2">{order.customer?.contactPhone}</span>
                    </div>
                  </div>
                </div>

                {/* Payment Info */}
                <div className="border-t pt-4 mb-4">
                  <h4 className="font-semibold mb-3">Thông tin thanh toán</h4>
                  <div className="space-y-2 text-sm">
                    <div className="flex justify-between">
                      <span className="text-gray-500">Tổng tiền:</span>
                      <span>{formatCurrency(order.payment?.totalAmount)}</span>
                    </div>
                    {order.payment?.discountAmount > 0 && (
                      <div className="flex justify-between text-green-600">
                        <span>Giảm giá{order.payment?.promotionCode && ` (${order.payment.promotionCode}):`}</span>
                        <span>-{formatCurrency(order.payment?.discountAmount)}</span>
                      </div>
                    )}
                    <div className="flex justify-between font-bold text-lg text-blue-600 pt-2 border-t">
                      <span>Thành tiền:</span>
                      <span>{formatCurrency(order.payment?.finalAmount)}</span>
                    </div>
                  </div>
                </div>

                {/* Cancellation Policy */}
                {order.cancellationPolicy && (
                  <div className="border-t pt-4 mb-4">
                    <h4 className="font-semibold mb-2">Chính sách hủy/đổi</h4>
                    <div className="bg-yellow-50 rounded-lg p-3 text-sm">
                      {order.cancellationPolicy.canCancel ? (
                        <p className="text-green-700">
                          ✓ Có thể hủy/đổi trước {formatDate(order.cancellationPolicy.deadlineToCancel)}
                        </p>
                      ) : (
                        <p className="text-red-700">
                          ✕ Đã quá thời hạn hủy/đổi
                        </p>
                      )}
                      {order.cancellationPolicy.policyDescription && (
                        <p className="mt-1 text-gray-600">{order.cancellationPolicy.policyDescription}</p>
                      )}
                    </div>
                  </div>
                )}

                {/* Notes */}
                {order.customer?.notes && (
                  <div className="border-t pt-4 mb-4">
                    <h4 className="font-semibold mb-2">Ghi chú</h4>
                    <p className="text-sm text-gray-600 bg-gray-50 rounded-lg p-3">
                      {order.customer.notes}
                    </p>
                  </div>
                )}
              </>
            )}
          </div>

          {/* Footer */}
          {order && !loading && (
            <div className="px-6 py-4 bg-gray-50 border-t flex justify-between items-center">
              <div>
                {order.canReview && (
                  <button
                    onClick={() => onReview?.(order)}
                    className="px-4 py-2 bg-yellow-500 text-white rounded-lg hover:bg-yellow-600 transition-colors"
                  >
                    ⭐ Đánh giá ngay
                  </button>
                )}
              </div>
              <div className="flex gap-2">
                <button
                  onClick={handleDownloadInvoice}
                  className="px-4 py-2 border border-blue-500 text-blue-500 rounded-lg hover:bg-blue-50 transition-colors flex items-center gap-2"
                >
                  <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 10v6m0 0l-3-3m3 3l3-3m2 8H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                  </svg>
                  Tải hóa đơn
                </button>
                <button
                  onClick={onClose}
                  className="px-4 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 transition-colors"
                >
                  Đóng
                </button>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default OrderDetailModal;
