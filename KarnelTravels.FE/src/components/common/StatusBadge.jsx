import { cn } from '@/lib/utils';

/**
 * StatusBadge - Badge component for displaying status
 * Use for showing status labels (active, inactive, featured, etc.)
 */
const StatusBadge = ({ 
  status, 
  variant = 'default',
  className 
}) => {
  const variants = {
    default: 'bg-gray-100 text-gray-700',
    primary: 'bg-primary/10 text-primary',
    success: 'bg-green-100 text-green-700',
    warning: 'bg-amber-100 text-amber-700',
    danger: 'bg-red-100 text-red-700',
    info: 'bg-blue-100 text-blue-700',
  };

  const size = {
    sm: 'px-2 py-0.5 text-xs',
    md: 'px-2.5 py-1 text-sm',
    lg: 'px-3 py-1.5 text-base',
  };

  const statusLabels = {
    active: 'Hoạt động',
    inactive: 'Không hoạt động',
    featured: 'Nổi bật',
    new: 'Mới',
    popular: 'Phổ biến',
    recommended: 'Đề xuất',
    expired: 'Hết hạn',
    available: 'Còn trống',
    booked: 'Đã đặt',
    pending: 'Chờ xử lý',
    confirmed: 'Đã xác nhận',
    cancelled: 'Đã hủy',
  };

  return (
    <span className={cn(
      "inline-flex items-center rounded-full font-medium",
      variants[variant] || variants.default,
      size.md,
      className
    )}>
      {statusLabels[status] || status}
    </span>
  );
};

export default StatusBadge;
