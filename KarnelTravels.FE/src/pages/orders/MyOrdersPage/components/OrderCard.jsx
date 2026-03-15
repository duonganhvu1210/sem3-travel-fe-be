import React from 'react';

// Status badge configuration
const STATUS_CONFIG = {
  Pending: { label: 'Chờ xác nhận', color: 'bg-yellow-100 text-yellow-800', icon: '⏳' },
  Confirmed: { label: 'Đã xác nhận', color: 'bg-blue-100 text-blue-800', icon: '✓' },
  Completed: { label: 'Đã hoàn thành', color: 'bg-green-100 text-green-800', icon: '★' },
  Cancelled: { label: 'Đã hủy', color: 'bg-red-100 text-red-800', icon: '✕' },
  Refunded: { label: 'Đã hoàn tiền', color: 'bg-gray-100 text-gray-800', icon: '↩' }
};

const PAYMENT_STATUS_CONFIG = {
  Pending: { label: 'Chờ thanh toán', color: 'bg-yellow-100 text-yellow-800' },
  Paid: { label: 'Đã thanh toán', color: 'bg-green-100 text-green-800' },
  Failed: { label: 'Thất bại', color: 'bg-red-100 text-red-800' },
  Refunded: { label: 'Đã hoàn tiền', color: 'bg-gray-100 text-gray-800' }
};

const SERVICE_TYPE_CONFIG = {
  Tour: { label: 'Tour', icon: '🧭' },
  Hotel: { label: 'Khách sạn', icon: '🏨' },
  Resort: { label: 'Resort', icon: '🏝️' },
  Transport: { label: 'Xe', icon: '🚌' },
  Restaurant: { label: 'Nhà hàng', icon: '🍽️' }
};

// Format currency
const formatCurrency = (amount) => {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(amount);
};

// Format date
const formatDate = (dateString) => {
  if (!dateString) return '-';
  const date = new Date(dateString);
  return new Intl.DateTimeFormat('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  }).format(date);
};

const OrderCard = ({ order, onClick, onCancel, onChangeDate }) => {
  const statusConfig = STATUS_CONFIG[order.status] || STATUS_CONFIG.Pending;
  const paymentConfig = PAYMENT_STATUS_CONFIG[order.paymentStatus] || PAYMENT_STATUS_CONFIG.Pending;
  const serviceConfig = SERVICE_TYPE_CONFIG[order.serviceType] || { label: order.serviceType, icon: '📦' };

  // Check if order can be cancelled or date changed (48h before service)
  const canModify = order.status === 'Pending' || order.status === 'Confirmed';
  const serviceDate = new Date(order.serviceDate);
  const now = new Date();
  const hoursUntilService = (serviceDate - now) / (1000 * 60 * 60);
  const canCancelOrChange = canModify && hoursUntilService > 48;

  return (
    <div 
      className="bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow duration-300 overflow-hidden cursor-pointer"
      onClick={() => onClick?.(order)}
    >
      {/* Header */}
      <div className="bg-gradient-to-r from-blue-600 to-blue-700 px-4 py-3">
        <div className="flex justify-between items-center">
          <span className="text-white font-bold">{order.orderCode}</span>
          <span className="text-white/90 text-sm">
            {serviceConfig.icon} {serviceConfig.label}
          </span>
        </div>
      </div>

      {/* Content */}
      <div className="p-4">
        {/* Service Name */}
        <h3 className="font-semibold text-lg text-gray-800 mb-2 line-clamp-2">
          {order.serviceName}
        </h3>

        {/* Dates */}
        <div className="flex items-center gap-2 text-sm text-gray-600 mb-3">
          <span className="flex items-center gap-1">
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
            </svg>
            {formatDate(order.serviceDate)}
          </span>
          {order.endDate && (
            <>
              <span>→</span>
              <span>{formatDate(order.endDate)}</span>
            </>
          )}
          <span className="text-gray-400">|</span>
          <span className="text-gray-500">{order.quantity} người</span>
        </div>

        {/* Status & Payment */}
        <div className="flex flex-wrap gap-2 mb-4">
          <span className={`px-3 py-1 rounded-full text-xs font-medium ${statusConfig.color}`}>
            {statusConfig.icon} {statusConfig.label}
          </span>
          <span className={`px-3 py-1 rounded-full text-xs font-medium ${paymentConfig.color}`}>
            {paymentConfig.label}
          </span>
        </div>

        {/* Price */}
        <div className="flex justify-between items-center pt-3 border-t border-gray-100">
          <div>
            <span className="text-xs text-gray-500">Tổng tiền</span>
            <p className="text-xl font-bold text-blue-600">
              {formatCurrency(order.totalPrice)}
            </p>
          </div>
          
          {/* Actions */}
          {canModify && (
            <div className="flex gap-2">
              {canCancelOrChange && (
                <>
                  <button
                    onClick={(e) => {
                      e.stopPropagation();
                      onChangeDate?.(order);
                    }}
                    className="px-3 py-1.5 text-sm border border-blue-500 text-blue-500 rounded-lg hover:bg-blue-50 transition-colors"
                  >
                    Đổi ngày
                  </button>
                  <button
                    onClick={(e) => {
                      e.stopPropagation();
                      onCancel?.(order);
                    }}
                    className="px-3 py-1.5 text-sm border border-red-500 text-red-500 rounded-lg hover:bg-red-50 transition-colors"
                  >
                    Hủy
                  </button>
                </>
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default OrderCard;
