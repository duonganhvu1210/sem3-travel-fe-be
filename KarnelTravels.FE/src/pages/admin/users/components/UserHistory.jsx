import { useState, useEffect } from 'react';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { History, Search } from 'lucide-react';
import userService from '@/services/userService';

const statusColors = {
  Pending: 'bg-yellow-100 text-yellow-800 border-yellow-200',
  Confirmed: 'bg-blue-100 text-blue-800 border-blue-200',
  Completed: 'bg-green-100 text-green-800 border-green-200',
  Cancelled: 'bg-red-100 text-red-800 border-red-200',
  Refunded: 'bg-gray-100 text-gray-800 border-gray-200',
};

const paymentStatusColors = {
  Pending: 'bg-yellow-100 text-yellow-800',
  Paid: 'bg-green-100 text-green-800',
  Failed: 'bg-red-100 text-red-800',
  Refunded: 'bg-gray-100 text-gray-800',
};

const typeLabels = {
  Tour: 'Tour du lịch',
  Hotel: 'Khách sạn',
  Resort: 'Resort',
  Transport: 'Vận chuyển',
  Restaurant: 'Nhà hàng',
};

export default function UserHistory({ open, onOpenChange, user }) {
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');

  useEffect(() => {
    if (open && user?.userId) {
      fetchBookings();
    }
  }, [open, user]);

  const fetchBookings = async () => {
    if (!user?.userId) return;
    setLoading(true);
    try {
      const response = await userService.getUserBookings(user.userId);
      if (response.success) {
        setBookings(response.data);
      }
    } catch (error) {
      console.error('Error fetching bookings:', error);
    } finally {
      setLoading(false);
    }
  };

  const filteredBookings = bookings.filter((booking) =>
    booking.bookingCode.toLowerCase().includes(searchTerm.toLowerCase()) ||
    (booking.itemName && booking.itemName.toLowerCase().includes(searchTerm.toLowerCase()))
  );

  const formatDate = (date) => {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('vi-VN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
    });
  };

  const formatCurrency = (amount) => {
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND',
    }).format(amount);
  };

  const getStatusBadge = (status) => {
    const colorClass = statusColors[status] || statusColors.Pending;
    return <Badge className={`${colorClass} border`}>{status}</Badge>;
  };

  const getPaymentStatusBadge = (status) => {
    const colorClass = paymentStatusColors[status] || paymentStatusColors.Pending;
    return <Badge className={colorClass}>{status}</Badge>;
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[900px] max-h-[80vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <History className="w-5 h-5" />
            Lịch sử đơn đặt
          </DialogTitle>
          <DialogDescription>
            Danh sách các đơn đặt của: <strong>{user?.fullName}</strong>
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-4">
          {/* Search */}
          <div className="flex items-center gap-2">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
              <Input
                placeholder="Tìm kiếm theo mã đơn..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="pl-9"
              />
            </div>
          </div>

          {loading ? (
            <div className="flex items-center justify-center h-40">
              <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
            </div>
          ) : filteredBookings.length === 0 ? (
            <div className="flex flex-col items-center justify-center h-40 text-muted-foreground">
              <History className="w-10 h-10 mb-2 opacity-50" />
              <p>Không có đơn đặt nào</p>
            </div>
          ) : (
            <div className="rounded-md border">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Mã đơn</TableHead>
                    <TableHead>Loại</TableHead>
                    <TableHead>Dịch vụ</TableHead>
                    <TableHead>Ngày</TableHead>
                    <TableHead>Số lượng</TableHead>
                    <TableHead>Tổng tiền</TableHead>
                    <TableHead>Thanh toán</TableHead>
                    <TableHead>Trạng thái</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredBookings.map((booking) => (
                    <TableRow key={booking.bookingId}>
                      <TableCell className="font-medium">{booking.bookingCode}</TableCell>
                      <TableCell>{typeLabels[booking.typeName] || booking.typeName}</TableCell>
                      <TableCell className="max-w-[150px] truncate">
                        {booking.itemName || '-'}
                      </TableCell>
                      <TableCell>{formatDate(booking.serviceDate)}</TableCell>
                      <TableCell>{booking.quantity}</TableCell>
                      <TableCell>{formatCurrency(booking.finalAmount)}</TableCell>
                      <TableCell>{getPaymentStatusBadge(booking.paymentStatusName)}</TableCell>
                      <TableCell>{getStatusBadge(booking.statusName)}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          )}

          {/* Summary */}
          {!loading && bookings.length > 0 && (
            <div className="flex items-center justify-between bg-muted p-3 rounded-lg">
              <div className="flex items-center gap-4">
                <span className="text-sm text-muted-foreground">
                  Tổng đơn đặt: <strong>{bookings.length}</strong>
                </span>
                <span className="text-sm text-muted-foreground">
                  Tổng chi tiêu:{' '}
                  <strong className="text-green-600">
                    {formatCurrency(bookings.reduce((sum, b) => sum + b.finalAmount, 0))}
                  </strong>
                </span>
              </div>
            </div>
          )}
        </div>
      </DialogContent>
    </Dialog>
  );
}
